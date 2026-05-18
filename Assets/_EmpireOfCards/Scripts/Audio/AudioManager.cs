using System;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Audio
{
    /// <summary>
    /// Data container for a named sound effect.
    /// </summary>
    [Serializable]
    public class SoundEffect
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    /// <summary>
    /// Singleton audio manager. Subscribes to EventBus events to play
    /// sounds automatically. Music crossfades between calm (early game)
    /// and intense (turn 10+) using Update() polling -- no coroutines.
    ///
    /// Expected Sound IDs:
    ///   card_place, card_draw, coin_ching, coin_cascade,
    ///   combo_trigger, negative_buzz, button_click, turn_bell,
    ///   fbi_siren, level_up
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSourceA;
        [SerializeField] private AudioSource musicSourceB;
        [SerializeField] private AudioSource sfxSource;

        [Header("Music Clips")]
        [SerializeField] private AudioClip calmMusic;
        [SerializeField] private AudioClip intenseMusic;

        [Header("Sound Effects")]
        [SerializeField] private SoundEffect[] soundEffects;

        [Header("Volume")]
        [Range(0f, 1f)][SerializeField] private float masterVolume = 1f;
        [Range(0f, 1f)][SerializeField] private float musicVolume = 0.7f;
        [Range(0f, 1f)][SerializeField] private float sfxVolume = 1f;

        [Header("Crossfade")]
        [SerializeField] private float crossfadeDuration = 1.5f;
        [SerializeField] private int intenseMusicTurnThreshold = 10;

        // Crossfade state (Update-driven, no coroutine)
        private bool isCrossfading;
        private float crossfadeTimer;
        private AudioSource fadeOutSource;
        private AudioSource fadeInSource;
        private float fadeOutStartVolume;
        private float fadeInTargetVolume;

        // Track which source is currently active
        private AudioSource activeSource;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService instead of RuntimeWiring.SetField().
        /// </summary>
        public void Init(AudioSource musicA, AudioSource musicB, AudioSource sfx)
        {
            this.musicSourceA = musicA;
            this.musicSourceB = musicB;
            this.sfxSource = sfx;
        }

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            activeSource = musicSourceA;
            ApplyVolumes();
        }

        private void OnEnable()
        {
            EventBus.OnCardPlayed += OnCardPlayed;
            EventBus.OnCardDrawn += OnCardDrawn;
            EventBus.OnIncomeReceived += OnIncomeReceived;
            EventBus.OnComboTriggered += OnComboTriggered;
            EventBus.OnFBIRaid += OnFBIRaid;
            EventBus.OnTurnStarted += OnTurnStarted;
            EventBus.OnBusinessClosed += OnBusinessClosed;
        }

        private void OnDisable()
        {
            EventBus.OnCardPlayed -= OnCardPlayed;
            EventBus.OnCardDrawn -= OnCardDrawn;
            EventBus.OnIncomeReceived -= OnIncomeReceived;
            EventBus.OnComboTriggered -= OnComboTriggered;
            EventBus.OnFBIRaid -= OnFBIRaid;
            EventBus.OnTurnStarted -= OnTurnStarted;
            EventBus.OnBusinessClosed -= OnBusinessClosed;
        }

        private void Update()
        {
            if (!isCrossfading)
                return;

            crossfadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(crossfadeTimer / crossfadeDuration);

            // Fade out old source
            if (fadeOutSource != null)
                fadeOutSource.volume = Mathf.Lerp(fadeOutStartVolume, 0f, t);

            // Fade in new source
            if (fadeInSource != null)
                fadeInSource.volume = Mathf.Lerp(0f, fadeInTargetVolume, t);

            if (t >= 1f)
            {
                // Crossfade complete
                if (fadeOutSource != null)
                {
                    fadeOutSource.Stop();
                    fadeOutSource.volume = 0f;
                }

                if (fadeInSource != null)
                    fadeInSource.volume = fadeInTargetVolume;

                activeSource = fadeInSource;
                isCrossfading = false;
            }
        }

        // ------------------------------------------------------------------
        // EventBus callbacks
        // ------------------------------------------------------------------

        private void OnCardPlayed(CardData card)
        {
            PlaySFX("card_place");
        }

        private void OnCardDrawn(CardData card)
        {
            PlaySFX("card_draw");
        }

        private void OnIncomeReceived(int amount)
        {
            PlaySFX("coin_ching");
        }

        private void OnComboTriggered(ComboData combo)
        {
            PlaySFX("combo_trigger");
        }

        private void OnFBIRaid(int penalty)
        {
            PlaySFX("fbi_siren");
        }

        private void OnTurnStarted(int turnNumber)
        {
            PlaySFX("turn_bell");

            // Switch to intense music at threshold
            if (turnNumber == intenseMusicTurnThreshold)
                CrossfadeMusic(true);
        }

        private void OnBusinessClosed(int businessIndex)
        {
            PlaySFX("negative_buzz");
        }

        // ------------------------------------------------------------------
        // Public API
        // ------------------------------------------------------------------

        /// <summary>
        /// Plays a one-shot sound effect by its string ID.
        /// </summary>
        public void PlaySFX(string soundId)
        {
            if (sfxSource == null || soundEffects == null)
                return;

            foreach (SoundEffect sfx in soundEffects)
            {
                if (sfx.id == soundId && sfx.clip != null)
                {
                    sfxSource.PlayOneShot(sfx.clip, sfx.volume * sfxVolume * masterVolume);
                    return;
                }
            }

            Debug.LogWarning($"[AudioManager] Sound ID not found: {soundId}");
        }

        /// <summary>
        /// Starts playing music. If intense is true, plays the intense track.
        /// </summary>
        public void PlayMusic(bool intense)
        {
            if (activeSource == null)
                return;

            AudioClip target = intense ? intenseMusic : calmMusic;
            if (target == null)
                return;

            activeSource.clip = target;
            activeSource.volume = musicVolume * masterVolume;
            activeSource.loop = true;
            activeSource.Play();
        }

        /// <summary>
        /// Smoothly crossfades between calm and intense music using Update() polling.
        /// </summary>
        public void CrossfadeMusic(bool toIntense)
        {
            AudioClip targetClip = toIntense ? intenseMusic : calmMusic;
            if (targetClip == null)
                return;

            // Determine which source pair to use
            fadeOutSource = activeSource;
            fadeInSource = (activeSource == musicSourceA) ? musicSourceB : musicSourceA;

            if (fadeInSource == null || fadeOutSource == null)
                return;

            fadeOutStartVolume = fadeOutSource.volume;
            fadeInTargetVolume = musicVolume * masterVolume;

            fadeInSource.clip = targetClip;
            fadeInSource.volume = 0f;
            fadeInSource.loop = true;
            fadeInSource.Play();

            crossfadeTimer = 0f;
            isCrossfading = true;
        }

        /// <summary>
        /// Stops music playback on both sources.
        /// </summary>
        public void StopMusic()
        {
            if (musicSourceA != null) musicSourceA.Stop();
            if (musicSourceB != null) musicSourceB.Stop();
            isCrossfading = false;
        }

        /// <summary>
        /// Sets the master volume (0-1) and applies it.
        /// </summary>
        public void SetMasterVolume(float v)
        {
            masterVolume = Mathf.Clamp01(v);
            ApplyVolumes();
        }

        /// <summary>
        /// Sets the music volume (0-1) and applies it.
        /// </summary>
        public void SetMusicVolume(float v)
        {
            musicVolume = Mathf.Clamp01(v);
            ApplyVolumes();
        }

        /// <summary>
        /// Sets the SFX volume (0-1) and applies it.
        /// </summary>
        public void SetSFXVolume(float v)
        {
            sfxVolume = Mathf.Clamp01(v);
            ApplyVolumes();
        }

        // ------------------------------------------------------------------
        // Internal
        // ------------------------------------------------------------------

        private void ApplyVolumes()
        {
            float effectiveMusicVol = musicVolume * masterVolume;

            if (musicSourceA != null && activeSource == musicSourceA)
                musicSourceA.volume = effectiveMusicVol;

            if (musicSourceB != null && activeSource == musicSourceB)
                musicSourceB.volume = effectiveMusicVol;

            if (sfxSource != null)
                sfxSource.volume = sfxVolume * masterVolume;
        }
    }
}
