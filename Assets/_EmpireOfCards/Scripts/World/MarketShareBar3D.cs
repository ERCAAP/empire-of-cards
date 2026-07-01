using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Helpers;
using DG.Tweening;

namespace EmpireOfCards.World
{
    /// <summary>
    /// 3D visual bar showing player vs rival market share.
    /// Two cubes side by side: green = player, red = rival.
    /// Width scales with share percentage, animated with DOTween.
    /// </summary>
    public class MarketShareBar3D : MonoBehaviour
    {
        // ── Visual refs ─────────────────────────────────────────────
        Transform _playerBar;
        Transform _rivalBar;
        Transform _separator;
        TMPro.TextMeshPro _playerLabel;
        TMPro.TextMeshPro _rivalLabel;

        // ── Config ──────────────────────────────────────────────────
        const float BAR_TOTAL_WIDTH = 8f;
        const float BAR_HEIGHT = 0.15f;
        const float BAR_DEPTH = 0.4f;

        static readonly Color COL_PLAYER = new Color(0.2f, 0.75f, 0.3f);
        static readonly Color COL_RIVAL  = new Color(0.85f, 0.2f, 0.2f);

        // ── State ───────────────────────────────────────────────────
        float _currentPlayerShare;
        float _currentRivalShare;

        // ── Build ───────────────────────────────────────────────────

        public void Build()
        {
            // Background bar (dark)
            var bgGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bgGo.name = "MarketBar_BG";
            bgGo.transform.SetParent(transform);
            bgGo.transform.localPosition = Vector3.zero;
            bgGo.transform.localScale = new Vector3(BAR_TOTAL_WIDTH + 0.2f, BAR_HEIGHT * 0.5f, BAR_DEPTH + 0.1f);
            SetColor(bgGo, new Color(0.1f, 0.1f, 0.12f));

            // Player bar (left side, green)
            var playerGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            playerGo.name = "MarketBar_Player";
            playerGo.transform.SetParent(transform);
            playerGo.transform.localPosition = new Vector3(-BAR_TOTAL_WIDTH * 0.25f, 0.01f, 0f);
            playerGo.transform.localScale = new Vector3(BAR_TOTAL_WIDTH * 0.5f, BAR_HEIGHT, BAR_DEPTH);
            SetColor(playerGo, COL_PLAYER);
            SetEmission(playerGo, COL_PLAYER * 0.3f);
            _playerBar = playerGo.transform;

            // Rival bar (right side, red)
            var rivalGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rivalGo.name = "MarketBar_Rival";
            rivalGo.transform.SetParent(transform);
            rivalGo.transform.localPosition = new Vector3(BAR_TOTAL_WIDTH * 0.25f, 0.01f, 0f);
            rivalGo.transform.localScale = new Vector3(BAR_TOTAL_WIDTH * 0.5f, BAR_HEIGHT, BAR_DEPTH);
            SetColor(rivalGo, COL_RIVAL);
            SetEmission(rivalGo, COL_RIVAL * 0.3f);
            _rivalBar = rivalGo.transform;

            // Center separator
            var sepGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sepGo.name = "MarketBar_Sep";
            sepGo.transform.SetParent(transform);
            sepGo.transform.localPosition = new Vector3(0f, 0.02f, 0f);
            sepGo.transform.localScale = new Vector3(0.04f, BAR_HEIGHT * 1.3f, BAR_DEPTH + 0.05f);
            SetColor(sepGo, Color.white);
            _separator = sepGo.transform;

            // Player percentage label
            var playerLabelGo = new GameObject("PlayerShareLabel");
            playerLabelGo.transform.SetParent(transform);
            playerLabelGo.transform.localPosition = new Vector3(-BAR_TOTAL_WIDTH * 0.25f, 0.3f, 0f);
            playerLabelGo.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            _playerLabel = playerLabelGo.AddComponent<TMPro.TextMeshPro>();
            _playerLabel.text = "15%";
            _playerLabel.fontSize = 3f;
            _playerLabel.color = COL_PLAYER;
            _playerLabel.alignment = TMPro.TextAlignmentOptions.Center;
            _playerLabel.fontStyle = TMPro.FontStyles.Bold;

            // Rival percentage label
            var rivalLabelGo = new GameObject("RivalShareLabel");
            rivalLabelGo.transform.SetParent(transform);
            rivalLabelGo.transform.localPosition = new Vector3(BAR_TOTAL_WIDTH * 0.25f, 0.3f, 0f);
            rivalLabelGo.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            _rivalLabel = rivalLabelGo.AddComponent<TMPro.TextMeshPro>();
            _rivalLabel.text = "15%";
            _rivalLabel.fontSize = 3f;
            _rivalLabel.color = COL_RIVAL;
            _rivalLabel.alignment = TMPro.TextAlignmentOptions.Center;
            _rivalLabel.fontStyle = TMPro.FontStyles.Bold;

            // Title label
            var titleGo = new GameObject("MarketBarTitle");
            titleGo.transform.SetParent(transform);
            titleGo.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            titleGo.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            var titleTmp = titleGo.AddComponent<TMPro.TextMeshPro>();
            titleTmp.text = "PAZAR PAYI";
            titleTmp.fontSize = 2.5f;
            titleTmp.color = new Color(0.8f, 0.8f, 0.8f);
            titleTmp.alignment = TMPro.TextAlignmentOptions.Center;
            titleTmp.fontStyle = TMPro.FontStyles.Bold;

            // Initial state
            UpdateBar(Constants.STARTING_MARKET_SHARE, Constants.STARTING_MARKET_SHARE);
        }

        // ── EventBus ────────────────────────────────────────────────

        void OnEnable()
        {
            EventBus.OnMarketShareChanged += HandleMarketShareChanged;
        }

        void OnDisable()
        {
            EventBus.OnMarketShareChanged -= HandleMarketShareChanged;
        }

        void HandleMarketShareChanged(float playerShare, float rivalShare)
        {
            UpdateBar(playerShare, rivalShare);
        }

        // ── Update ──────────────────────────────────────────────────

        void UpdateBar(float playerShare, float rivalShare)
        {
            _currentPlayerShare = playerShare;
            _currentRivalShare = rivalShare;

            DOTweenAnimations.MarketBarUpdate(_playerBar, _rivalBar, playerShare, rivalShare);

            // Update labels
            if (_playerLabel != null)
                _playerLabel.text = $"{playerShare:F0}%";
            if (_rivalLabel != null)
                _rivalLabel.text = $"{rivalShare:F0}%";

            // Move separator to division point
            if (_separator != null)
            {
                float sepX = -BAR_TOTAL_WIDTH * 0.5f + (playerShare / 100f) * BAR_TOTAL_WIDTH;
                _separator.DOLocalMoveX(sepX, 0.5f).SetEase(Ease.OutQuad);
            }
        }

        // ── Helpers ─────────────────────────────────────────────────

        void SetColor(GameObject go, Color color)
        {
            var rend = go.GetComponent<Renderer>();
            if (rend == null) return;
            rend.material = MaterialHelper.Create(color);
        }

        void SetEmission(GameObject go, Color emissionColor)
        {
            var rend = go.GetComponent<Renderer>();
            if (rend == null || rend.material == null) return;
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", emissionColor);
        }
    }
}
