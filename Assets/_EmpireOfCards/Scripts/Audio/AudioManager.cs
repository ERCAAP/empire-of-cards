using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Audio
{
    public class AudioManager : MonoBehaviour
    {
        AudioSource _musicSource;
        AudioSource _sfxSource;

        float _musicVolume = 1f;
        float _sfxVolume = 1f;

        Era _currentEra;

        public void Init()
        {
            // Create two AudioSource components: one for music, one for SFX
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
            _musicSource.volume = _musicVolume;

            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.loop = false;
            _sfxSource.playOnAwake = false;
            _sfxSource.volume = _sfxVolume;

            _currentEra = Era.Garage;
            Debug.Log("[AudioManager] Initialized with 2 AudioSources.");
        }

        // ── EventBus Subscriptions ─────────────────────────────────

        void OnEnable()
        {
            EventBus.OnCardPlaced += HandleCardPlaced;
            EventBus.OnCrisisTriggered += HandleCrisisTriggered;
            EventBus.OnCustomersServed += HandleCustomersServed;
            EventBus.OnMoneyChanged += HandleMoneyChanged;
            EventBus.OnRatingChanged += HandleRatingChanged;
            EventBus.OnStaffQuit += HandleStaffQuit;
            EventBus.OnGameOver += HandleGameOver;
            EventBus.OnEraChanged += HandleEraChanged;
        }

        void OnDisable()
        {
            EventBus.OnCardPlaced -= HandleCardPlaced;
            EventBus.OnCrisisTriggered -= HandleCrisisTriggered;
            EventBus.OnCustomersServed -= HandleCustomersServed;
            EventBus.OnMoneyChanged -= HandleMoneyChanged;
            EventBus.OnRatingChanged -= HandleRatingChanged;
            EventBus.OnStaffQuit -= HandleStaffQuit;
            EventBus.OnGameOver -= HandleGameOver;
            EventBus.OnEraChanged -= HandleEraChanged;
        }

        // ── Event Handlers ─────────────────────────────────────────

        void HandleCardPlaced(CardData card, SlotType slot)
        {
            PlaySFX("card_place");
        }

        void HandleCrisisTriggered(CrisisType crisis)
        {
            PlaySFX("alarm");
        }

        void HandleCustomersServed(int served, int waited, int left)
        {
            if (served > 0)
                PlaySFX("door_bell");
        }

        void HandleMoneyChanged(int newAmount)
        {
            // Only play cash register on positive changes
            // (We cannot track delta here, so play on any change above starting)
            PlaySFX("cash_register");
        }

        void HandleRatingChanged(float newRating)
        {
            if (newRating > Constants.STARTING_RATING)
                PlaySFX("star_bling");
        }

        void HandleStaffQuit(string staffName, string reason)
        {
            PlaySFX("sad_note");
        }

        void HandleGameOver(bool won, string reason)
        {
            if (won)
                PlaySFX("victory");
            else
                PlaySFX("defeat");
        }

        void HandleEraChanged(Era newEra)
        {
            _currentEra = newEra;
            UpdateAmbient(newEra);
        }

        // ── Era-Based Ambient ──────────────────────────────────────

        void UpdateAmbient(Era era)
        {
            switch (era)
            {
                case Era.Garage:
                    Debug.Log("[AudioManager] Ambient: Quiet garage atmosphere.");
                    break;
                case Era.Growth:
                    Debug.Log("[AudioManager] Ambient: Moderate bustle.");
                    break;
                case Era.Scale:
                    Debug.Log("[AudioManager] Ambient: Busy restaurant.");
                    break;
                case Era.Dominance:
                    Debug.Log("[AudioManager] Ambient: Full restaurant, peak energy.");
                    break;
            }
        }

        // ── Playback ───────────────────────────────────────────────

        public void PlaySFX(string clipName)
        {
            // Placeholder: no actual AudioClip assets yet
            Debug.Log($"[AudioManager] SFX: {clipName}");
        }

        // ── Volume Control ─────────────────────────────────────────

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            if (_musicSource != null)
                _musicSource.volume = _musicVolume;
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            if (_sfxSource != null)
                _sfxSource.volume = _sfxVolume;
        }

        public float GetMusicVolume() => _musicVolume;
        public float GetSFXVolume() => _sfxVolume;
    }
}
