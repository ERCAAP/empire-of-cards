using System;
using UnityEngine;
using EmpireOfCards.Data;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Central UI controller. Holds references to all UI panels and orchestrates
    /// transitions between them.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Panel References")]
        [SerializeField] private TopBarUI topBar;
        [SerializeField] private ActionBarUI actionBar;
        [SerializeField] private ShopPanel shopPanel;
        [SerializeField] private HandUI handUI;
        [SerializeField] private ComboPopup comboPopup;
        [SerializeField] private EventPopup eventPopup;
        [SerializeField] private RivalPopup rivalPopup;
        [SerializeField] private ScoreScreen scoreScreen;
        [SerializeField] private GameOverScreen gameOverScreen;

        // --- Events ---
        public event Action OnEndTurnClicked;
        public event Action<bool> OnShopToggled;

        // --- Properties ---
        public TopBarUI TopBar => topBar;
        public ActionBarUI ActionBar => actionBar;
        public ShopPanel Shop => shopPanel;
        public HandUI Hand => handUI;

        /// <summary>
        /// Opens the shop panel and fires the toggle event.
        /// </summary>
        public void ShowShop()
        {
            if (shopPanel != null)
            {
                shopPanel.gameObject.SetActive(true);
            }

            OnShopToggled?.Invoke(true);
        }

        /// <summary>
        /// Closes the shop panel and fires the toggle event.
        /// </summary>
        public void HideShop()
        {
            if (shopPanel != null)
            {
                shopPanel.Close();
            }

            OnShopToggled?.Invoke(false);
        }

        /// <summary>
        /// Displays a combo popup with the given text and color.
        /// </summary>
        public void ShowComboPopup(string text, Color color)
        {
            if (comboPopup != null)
            {
                comboPopup.Show(text, color);
            }
        }

        /// <summary>
        /// Displays the event popup with the given event card.
        /// </summary>
        public void ShowEventPopup(CardData eventCard)
        {
            if (eventPopup != null)
            {
                eventPopup.Show(eventCard);
            }
        }

        /// <summary>
        /// Shows a rival action popup.
        /// </summary>
        public void ShowRivalAction(string action)
        {
            if (rivalPopup != null)
            {
                rivalPopup.Show(action, string.Empty, null);
            }
        }

        /// <summary>
        /// Opens the score screen with the final run data.
        /// </summary>
        public void ShowScoreScreen(ScoreData data)
        {
            if (scoreScreen != null)
            {
                scoreScreen.gameObject.SetActive(true);
                scoreScreen.Show(data);
            }
        }

        /// <summary>
        /// Shows the game over screen.
        /// </summary>
        public void ShowGameOver(bool won)
        {
            if (gameOverScreen != null)
            {
                gameOverScreen.gameObject.SetActive(true);
                gameOverScreen.Show(won);
            }
        }

        /// <summary>
        /// Refreshes every visible UI element to reflect current game state.
        /// </summary>
        public void UpdateAllUI()
        {
            var gm = Core.GameManager.Instance;
            if (gm == null)
                return;

            if (topBar != null)
            {
                topBar.UpdateMoney(gm.PlayerMoney);
                topBar.UpdateTurn(gm.CurrentTurn, gm.MaxTurns);
                topBar.UpdateFBIRisk(gm.FBIRisk);
            }

            if (actionBar != null)
            {
                actionBar.UpdateActions(gm.PlayerActions, gm.MaxActions);
            }
        }

        /// <summary>
        /// Called by the End Turn button in the scene.
        /// </summary>
        public void OnEndTurnButtonClicked()
        {
            OnEndTurnClicked?.Invoke();
        }
    }
}
