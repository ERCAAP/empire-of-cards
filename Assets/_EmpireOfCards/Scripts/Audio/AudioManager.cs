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
            EventBus.OnTurnStarted += HandleTurnStarted;
            EventBus.OnPhaseStarted += HandlePhaseStarted;
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
            EventBus.OnTurnStarted -= HandleTurnStarted;
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
        }

        // ── Event Handlers ─────────────────────────────────────────

        void HandleCardPlaced(CardData card, SlotType slot)
        {
            PlaySFX("card_place");
        }

        void HandleCrisisTriggered(CrisisType crisis)
        {
            PlaySFX("alarm");
            Debug.Log($"[AudioManager] Crisis alarm: {crisis}");
        }

        void HandleCustomersServed(int served, int waited, int left)
        {
            if (served > 0) PlaySFX("door_bell");
            if (left > 0) PlaySFX("disappointed");
        }

        void HandleMoneyChanged(int newAmount)
        {
            PlaySFX("cash_register");
        }

        void HandleRatingChanged(float newRating)
        {
            if (newRating > Constants.STARTING_RATING)
                PlaySFX("star_bling");
            else if (newRating < Constants.LOSE_RATING + 0.5f)
                PlaySFX("warning_chime");
        }

        void HandleStaffQuit(string staffName, string reason)
        {
            PlaySFX("sad_note");
            Debug.Log($"[AudioManager] Staff quit: {staffName} ({reason})");
        }

        void HandleGameOver(bool won, string reason)
        {
            if (won)
                PlaySFX("victory_fanfare");
            else
                PlaySFX("defeat_sting");

            Debug.Log($"[AudioManager] Game over. Won: {won}, Reason: {reason}");
        }

        void HandleEraChanged(Era newEra)
        {
            _currentEra = newEra;
            PlaySFX("era_transition");
            UpdateAmbient(newEra);
        }

        void HandleTurnStarted(int turnNumber)
        {
            PlaySFX("turn_start");
        }

        void HandlePhaseStarted(TurnPhase phase)
        {
            if (phase == TurnPhase.ResolvePhase)
                PlaySFX("resolve_whoosh");
        }

        // ── Era-Based Ambient ──────────────────────────────────────

        void UpdateAmbient(Era era)
        {
            string track;
            switch (era)
            {
                case Era.Garage:    track = "amb_garage_quiet"; break;
                case Era.Growth:    track = "amb_growth_bustle"; break;
                case Era.Scale:     track = "amb_scale_busy"; break;
                case Era.Dominance: track = "amb_dominance_peak"; break;
                default:            track = "amb_garage_quiet"; break;
            }
            Debug.Log($"[AudioManager] Ambient: {track} (Era: {era})");
        }

        // ── Playback ───────────────────────────────────────────────

        public void PlaySFX(string clipName)
        {
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
