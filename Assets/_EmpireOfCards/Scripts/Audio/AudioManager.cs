using System;
using System.Collections;
using UnityEngine;

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
    /// Singleton audio manager. Handles music crossfading and one-shot SFX playback.
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
        [SerializeField] private AudioSource musicSource;
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

        private Coroutine crossfadeCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            ApplyVolumes();
        }

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
        /// Starts playing music. If intense is true, plays the intense track; otherwise calm.
        /// </summary>
        public void PlayMusic(bool intense)
        {
            if (musicSource == null)
                return;

            AudioClip target = intense ? intenseMusic : calmMusic;
            if (target == null)
                return;

            musicSource.clip = target;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.loop = true;
            musicSource.Play();
        }

        /// <summary>
        /// Smoothly crossfades between calm and intense music tracks.
        /// </summary>
        public void CrossfadeMusic(bool toIntense, float duration = 1f)
        {
            if (crossfadeCoroutine != null)
            {
                StopCoroutine(crossfadeCoroutine);
            }

            crossfadeCoroutine = StartCoroutine(CrossfadeMusicCoroutine(toIntense, duration));
        }

        /// <summary>
        /// Stops music playback.
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
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

        private void ApplyVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }

            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume * masterVolume;
            }
        }

        private IEnumerator CrossfadeMusicCoroutine(bool toIntense, float duration)
        {
            AudioClip target = toIntense ? intenseMusic : calmMusic;
            if (target == null || musicSource == null)
                yield break;

            float startVolume = musicSource.volume;
            float elapsed = 0f;

            // Fade out current track
            while (elapsed < duration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.5f);
                musicSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            // Switch track
            musicSource.clip = target;
            musicSource.Play();

            // Fade in new track
            elapsed = 0f;
            float targetVolume = musicVolume * masterVolume;

            while (elapsed < duration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.5f);
                musicSource.volume = Mathf.Lerp(0f, targetVolume, t);
                yield return null;
            }

            musicSource.volume = targetVolume;
            crossfadeCoroutine = null;
        }
    }
}
