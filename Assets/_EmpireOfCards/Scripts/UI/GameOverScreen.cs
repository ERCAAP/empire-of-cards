using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Simple game over overlay. Shows win or loss title and message.
    /// Play Again and Menu buttons.
    /// </summary>
    public class GameOverScreen : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button menuButton;

        [Header("Text Presets")]
        [SerializeField] private string winTitle = "Zafer!";
        [SerializeField] private string winMessage = "Pazara hakim bir imparatorluk kurdun!";
        [SerializeField] private string loseTitle = "Oyun Bitti";
        [SerializeField] private string loseMessage = "Is imparatorlugun coktu...";

        // Events
        public event Action OnPlayAgainClicked;
        public event Action OnMenuClicked;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void Awake()
        {
            if (playAgainButton != null)
                playAgainButton.onClick.AddListener(() => OnPlayAgainClicked?.Invoke());

            if (menuButton != null)
                menuButton.onClick.AddListener(() => OnMenuClicked?.Invoke());
        }

        // ------------------------------------------------------------------
        // Public
        // ------------------------------------------------------------------

        /// <summary>
        /// Displays the game over screen with win or loss text.
        /// </summary>
        public void Show(bool won)
        {
            if (titleText != null)
                titleText.text = won ? winTitle : loseTitle;

            if (messageText != null)
                messageText.text = won ? winMessage : loseMessage;
        }

        // ------------------------------------------------------------------
        // Cleanup
        // ------------------------------------------------------------------

        private void OnDestroy()
        {
            if (playAgainButton != null)
                playAgainButton.onClick.RemoveAllListeners();

            if (menuButton != null)
                menuButton.onClick.RemoveAllListeners();
        }
    }
}
