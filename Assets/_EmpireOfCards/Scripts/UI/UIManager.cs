using System;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Central UI controller. Subscribes to EventBus events and routes
    /// updates to the appropriate panel components. No panel calls manager
    /// methods for data -- everything flows through events.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Panel References")]
        [SerializeField] private TopBarUI topBar;
        [SerializeField] private ActionBarUI actionBar;
        [SerializeField] private ShopPanel shopPanel;
        // HandUI removed -- Hand3D replaces it in 3D mode
        [SerializeField] private ComboPopup comboPopup;
        [SerializeField] private EventPopup eventPopup;
        [SerializeField] private RivalPopup rivalPopup;
        [SerializeField] private ScoreScreen scoreScreen;
        [SerializeField] private GameOverScreen gameOverScreen;

        // --- Events for external listeners (e.g. TurnManager) ---
        public event Action OnEndTurnClicked;
        public event Action<bool> OnShopToggled;

        // --- Properties ---
        public TopBarUI TopBar => topBar;
        public ActionBarUI ActionBar => actionBar;
        public ShopPanel Shop => shopPanel;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService instead of RuntimeWiring.SetField().
        /// </summary>
        public void Init(TopBarUI top, ActionBarUI action, ShopPanel shop,
            ComboPopup combo, EventPopup eventP, RivalPopup rival,
            ScoreScreen score, GameOverScreen gameOver)
        {
            this.topBar = top;
            this.actionBar = action;
            this.shopPanel = shop;
            this.comboPopup = combo;
            this.eventPopup = eventP;
            this.rivalPopup = rival;
            this.scoreScreen = score;
            this.gameOverScreen = gameOver;
        }

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void OnEnable()
        {
            EventBus.OnPhaseStarted += HandlePhaseStarted;
            EventBus.OnComboTriggered += HandleComboTriggered;
            EventBus.OnEventActivated += HandleEventActivated;
            EventBus.OnRivalAction += HandleRivalAction;
            EventBus.OnRivalTaunt += HandleRivalTaunt;
            EventBus.OnGameOver += HandleGameOver;
            EventBus.OnMoneyChanged += HandleMoneyChanged;
            EventBus.OnTerritoryChanged += HandleTerritoryChanged;
            EventBus.OnFBIRiskChanged += HandleFBIRiskChanged;
            EventBus.OnTurnStarted += HandleTurnStarted;
        }

        private void OnDisable()
        {
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
            EventBus.OnComboTriggered -= HandleComboTriggered;
            EventBus.OnEventActivated -= HandleEventActivated;
            EventBus.OnRivalAction -= HandleRivalAction;
            EventBus.OnRivalTaunt -= HandleRivalTaunt;
            EventBus.OnGameOver -= HandleGameOver;
            EventBus.OnMoneyChanged -= HandleMoneyChanged;
            EventBus.OnTerritoryChanged -= HandleTerritoryChanged;
            EventBus.OnFBIRiskChanged -= HandleFBIRiskChanged;
            EventBus.OnTurnStarted -= HandleTurnStarted;
        }

        // ------------------------------------------------------------------
        // EventBus handlers
        // ------------------------------------------------------------------

        private void HandlePhaseStarted(TurnPhase phase)
        {
            // During PlayPhase the player can interact; hide/show as needed
            bool isPlayPhase = phase == TurnPhase.PlayPhase;

            // Hand interactability now driven by Hand3D via EventBus

            if (actionBar != null)
                actionBar.SetVisible(isPlayPhase);
        }

        private void HandleComboTriggered(ComboData combo)
        {
            if (comboPopup != null)
                comboPopup.Show(combo.displayText, combo.glowColor);
        }

        private void HandleEventActivated(CardData eventCard)
        {
            if (eventPopup != null)
                eventPopup.Show(eventCard);
        }

        private void HandleRivalAction(string action)
        {
            if (rivalPopup != null)
                rivalPopup.Show(action, string.Empty);
        }

        private void HandleRivalTaunt(string taunt)
        {
            if (rivalPopup != null)
                rivalPopup.Show(string.Empty, taunt);
        }

        private void HandleGameOver(bool won)
        {
            if (won)
            {
                if (scoreScreen != null)
                {
                    scoreScreen.gameObject.SetActive(true);
                }
            }
            else
            {
                if (gameOverScreen != null)
                {
                    gameOverScreen.gameObject.SetActive(true);
                    gameOverScreen.Show(false);
                }
            }
        }

        private void HandleMoneyChanged(int newAmount)
        {
            if (topBar != null)
                topBar.SetTargetMoney(newAmount);
        }

        private void HandleTerritoryChanged(int player, int rival)
        {
            // TopBar or a dedicated territory widget can reflect this.
            // Kept intentionally thin -- panels subscribe themselves.
        }

        private void HandleFBIRiskChanged(float risk)
        {
            if (topBar != null)
                topBar.UpdateFBIRisk(risk);
        }

        private void HandleTurnStarted(int turnNumber)
        {
            if (topBar != null)
                topBar.UpdateTurn(turnNumber, Constants.MAX_TURNS);
        }

        // ------------------------------------------------------------------
        // Public helpers (called by buttons in the scene)
        // ------------------------------------------------------------------

        /// <summary>
        /// Opens the shop panel and fires the toggle event.
        /// </summary>
        public void ShowShop()
        {
            if (shopPanel != null)
                shopPanel.gameObject.SetActive(true);

            OnShopToggled?.Invoke(true);
        }

        /// <summary>
        /// Closes the shop panel and fires the toggle event.
        /// </summary>
        public void HideShop()
        {
            if (shopPanel != null)
                shopPanel.Close();

            OnShopToggled?.Invoke(false);
        }

        /// <summary>
        /// Refreshes every visible UI element by pulling current GameManager state.
        /// Called once on game start or when re-entering the playing state.
        /// </summary>
        public void UpdateAllUI()
        {
            var gm = GameManager.Instance;
            if (gm == null)
                return;

            if (topBar != null)
            {
                topBar.SetTargetMoney(gm.PlayerMoney);
                topBar.UpdateTurn(gm.CurrentTurn, gm.MaxTurns);
                topBar.UpdateFBIRisk(gm.FBIRisk);
            }

            if (actionBar != null)
                actionBar.UpdateActions(gm.PlayerActions, gm.MaxActions);
        }

        /// <summary>
        /// Called by the End Turn button in the scene.
        /// </summary>
        public void OnEndTurnButtonClicked()
        {
            OnEndTurnClicked?.Invoke();
        }

        // ------------------------------------------------------------------
        // Pause / GameOver menu helpers
        // ------------------------------------------------------------------

        public void ShowPauseMenu()
        {
            // TODO: Activate pause menu panel when created
        }

        public void HidePauseMenu()
        {
            // TODO: Deactivate pause menu panel when created
        }

        public void ShowGameOverScreen()
        {
            if (gameOverScreen != null)
            {
                gameOverScreen.gameObject.SetActive(true);
                gameOverScreen.Show(false);
            }
        }

        public void HideGameOverScreen()
        {
            if (gameOverScreen != null)
                gameOverScreen.gameObject.SetActive(false);
        }
    }
}
