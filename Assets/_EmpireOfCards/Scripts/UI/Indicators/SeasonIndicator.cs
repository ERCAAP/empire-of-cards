using UnityEngine;
using TMPro;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI.Indicators
{
    /// <summary>
    /// Displays the current season name and turn progress (X/25).
    /// Season name color changes per season for instant visual recognition.
    /// Subscribes to EventBus.OnSeasonChanged and EventBus.OnTurnStarted.
    /// </summary>
    public class SeasonIndicator : MonoBehaviour
    {
        private TMP_Text _seasonNameText;
        private TMP_Text _turnProgressText;

        private int _currentTurn = 1;
        private SeasonType _currentSeason = SeasonType.Spring;

        // One color per SeasonType enum value (same order as enum)
        private static readonly Color[] SeasonColors =
        {
            new Color(0.5f,  0.95f, 0.5f),  // Spring  — fresh green
            new Color(1f,    0.85f, 0.25f), // Summer  — warm yellow
            new Color(0.9f,  0.55f, 0.2f),  // Autumn  — orange
            new Color(0.55f, 0.8f,  1f),    // Winter  — cool blue
            new Color(0.95f, 0.7f,  0.25f)  // RamadanSeason — gold
        };

        private static readonly string[] SeasonDisplayNames =
        {
            "SPRING", "SUMMER", "AUTUMN", "WINTER", "RAMADAN"
        };

        public void Init(TMP_Text seasonNameText, TMP_Text turnProgressText)
        {
            _seasonNameText   = seasonNameText;
            _turnProgressText = turnProgressText;
            RefreshDisplay();
        }

        private void OnEnable()
        {
            EventBus.OnSeasonChanged += OnSeasonChanged;
            EventBus.OnTurnStarted   += OnTurnStarted;
        }

        private void OnDisable()
        {
            EventBus.OnSeasonChanged -= OnSeasonChanged;
            EventBus.OnTurnStarted   -= OnTurnStarted;
        }

        private void OnSeasonChanged(SeasonType season, int seasonIndex)
        {
            _currentSeason = season;
            RefreshDisplay();
        }

        private void OnTurnStarted(int turnNumber)
        {
            _currentTurn = turnNumber;
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            int idx = (int)_currentSeason;

            if (_seasonNameText != null)
            {
                _seasonNameText.text  = SeasonDisplayNames[idx];
                _seasonNameText.color = SeasonColors[idx];
            }

            if (_turnProgressText != null)
                _turnProgressText.text = $"{_currentTurn}/{Constants.MAX_TURNS}";
        }
    }
}
