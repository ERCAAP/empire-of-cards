using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI.Indicators
{
    /// <summary>
    /// Displays the 100-customer market as a horizontal split bar.
    /// Left (blue) = player share, right (red) = rival share, gray = neutral.
    /// Subscribes to EventBus.OnMarketShareChanged.
    /// </summary>
    public class CustomerMarketIndicator : MonoBehaviour
    {
        private Image    _playerFill;
        private Image    _rivalFill;
        private TMP_Text _scoreText;
        private RectTransform _trackBg;

        private float _trackWidth;
        private float _displayedPlayerPct;
        private float _displayedRivalPct;
        private float _targetPlayerPct;
        private float _targetRivalPct;
        private int _playerCount;
        private int _rivalCount;

        public void Init(Image playerFill, Image rivalFill, TMP_Text scoreText, RectTransform trackBg)
        {
            _playerFill = playerFill;
            _rivalFill  = rivalFill;
            _scoreText  = scoreText;
            _trackBg    = trackBg;

            _trackWidth = trackBg != null ? trackBg.sizeDelta.x : 380f;

            Refresh(0, 0);
        }

        private void OnEnable()  => EventBus.OnMarketShareChanged += OnMarketShareChanged;
        private void OnDisable() => EventBus.OnMarketShareChanged -= OnMarketShareChanged;

        private void OnMarketShareChanged(int playerCount, int rivalCount)
            => Refresh(playerCount, rivalCount);

        private void Update()
        {
            _displayedPlayerPct = Mathf.Lerp(_displayedPlayerPct, _targetPlayerPct, Time.deltaTime * 8f);
            _displayedRivalPct = Mathf.Lerp(_displayedRivalPct, _targetRivalPct, Time.deltaTime * 8f);

            if (_playerFill != null)
                _playerFill.rectTransform.sizeDelta = new Vector2(_trackWidth * _displayedPlayerPct, _playerFill.rectTransform.sizeDelta.y);

            if (_rivalFill != null)
                _rivalFill.rectTransform.sizeDelta = new Vector2(_trackWidth * _displayedRivalPct, _rivalFill.rectTransform.sizeDelta.y);

            if (_scoreText != null)
                _scoreText.text = $"{_playerCount} / {_rivalCount}";
        }

        private void Refresh(int playerCount, int rivalCount)
        {
            int total = Constants.TOTAL_MARKET_CUSTOMERS;

            // Clamp to valid range
            playerCount = Mathf.Clamp(playerCount, 0, total);
            rivalCount  = Mathf.Clamp(rivalCount,  0, total - playerCount);

            _playerCount = playerCount;
            _rivalCount = rivalCount;
            _targetPlayerPct = playerCount / (float)total;
            _targetRivalPct = rivalCount / (float)total;
        }
    }
}
