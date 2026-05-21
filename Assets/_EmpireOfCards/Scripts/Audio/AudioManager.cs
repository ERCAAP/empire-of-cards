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
    /// Board ambient states, ordered by activity level.
    /// Used to drive ambient audio layers that respond to board fullness.
    /// </summary>
    public enum AmbientState
    {
        Empty,      // 0-1 businesses: quiet, ticking clock
        Growing,    // 2 businesses: light activity hum
        Busy        // 3+ businesses: bustling marketplace
    }

    /// <summary>
    /// Singleton audio manager. Subscribes to EventBus events to play
    /// sounds automatically. Music crossfades between calm (early game)
    /// and intense (turn 10+) using Update() polling -- no coroutines.
    ///
    /// Ambient audio layers respond to board fullness:
    ///   EMPTY -> GROWING -> BUSY -> THRIVING
    ///
    /// Expected Sound IDs:
    ///   card_place, card_draw, coin_ching, coin_cascade,
    ///   negative_buzz, button_click, turn_bell, level_up
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

        [Header("Ambient Layers (wire clips later)")]
        [SerializeField] private AudioClip ambientEmpty;
        [SerializeField] private AudioClip ambientGrowing;
        [SerializeField] private AudioClip ambientBusy;
        [SerializeField] private float ambientFadeDuration = 1f;

        // Crossfade state (Update-driven, no coroutine)
        private bool isCrossfading;
        private float crossfadeTimer;
        private AudioSource fadeOutSource;
        private AudioSource fadeInSource;
        private float fadeOutStartVolume;
        private float fadeInTargetVolume;

        // Track which source is currently active
        private AudioSource activeSource;

        // Ambient state tracking
        private AmbientState _currentAmbientState = AmbientState.Empty;
        private int _trackedBusinessCount;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService during bootstrap.
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
            EventBus.OnTurnStarted += OnTurnStarted;
            EventBus.OnBusinessClosed += OnBusinessClosed;

            // Ambient state tracking
            EventBus.OnBusinessPlaced += OnBusinessPlacedAmbient;
        }

        private void OnDisable()
        {
            EventBus.OnCardPlayed -= OnCardPlayed;
            EventBus.OnCardDrawn -= OnCardDrawn;
            EventBus.OnIncomeReceived -= OnIncomeReceived;
            EventBus.OnTurnStarted -= OnTurnStarted;
            EventBus.OnBusinessClosed -= OnBusinessClosed;

            // Ambient state tracking
            EventBus.OnBusinessPlaced -= OnBusinessPlacedAmbient;
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

            // A closed business reduces the count
            _trackedBusinessCount = Mathf.Max(0, _trackedBusinessCount - 1);
            RefreshAmbientState();
        }

        // ------------------------------------------------------------------
        // Ambient state callbacks
        // ------------------------------------------------------------------

        private void OnBusinessPlacedAmbient(CardData card, int slotIndex)
        {
            _trackedBusinessCount++;
            RefreshAmbientState();
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

        // ------------------------------------------------------------------
        // Ambient Board Audio
        // ------------------------------------------------------------------

        /// <summary>
        /// Evaluates the current board state and transitions the ambient layer
        /// if the state has changed. Called automatically from event callbacks.
        /// Can also be called externally to force a re-evaluation.
        /// </summary>
        public void UpdateAmbientState(int businessCount)
        {
            AmbientState newState;

            if (businessCount >= 3)
                newState = AmbientState.Busy;
            else if (businessCount >= 2)
                newState = AmbientState.Growing;
            else
                newState = AmbientState.Empty;

            if (newState == _currentAmbientState)
                return;

            AmbientState previous = _currentAmbientState;
            _currentAmbientState = newState;

            Debug.Log($"[AudioManager] Ambient state: {newState} ({businessCount} businesses)");

            // TODO: Wire actual audio clips here. When clips are assigned,
            // crossfade between ambient layers using ambientFadeDuration.
            // For now, the state machine is ready and logging transitions.
            OnAmbientStateChanged(previous, newState);
        }

        /// <summary>
        /// Returns the current ambient state. Useful for UI or debugging.
        /// </summary>
        public AmbientState CurrentAmbientState => _currentAmbientState;

        /// <summary>
        /// Internal refresh using tracked counters from event callbacks.
        /// </summary>
        private void RefreshAmbientState()
        {
            UpdateAmbientState(_trackedBusinessCount);
        }

        /// <summary>
        /// Placeholder for ambient audio transitions. When AudioClips are
        /// assigned to the ambient layer fields, this method will handle
        /// fading out the old layer and fading in the new one.
        /// </summary>
        private void OnAmbientStateChanged(AmbientState from, AmbientState to)
        {
            AudioClip targetClip = to switch
            {
                AmbientState.Empty    => ambientEmpty,
                AmbientState.Growing  => ambientGrowing,
                AmbientState.Busy     => ambientBusy,
                _                     => null
            };

            if (targetClip == null)
            {
                Debug.Log($"[AudioManager] No ambient clip assigned for state {to} -- skipping playback.");
                return;
            }

            // Future: Use a dedicated ambient AudioSource pair and fade
            // between them over ambientFadeDuration, similar to CrossfadeMusic.
            Debug.Log($"[AudioManager] Would crossfade ambient: {from} -> {to} over {ambientFadeDuration}s");
        }
    }
}
