using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Simple game-over overlay. Shows win or loss state.
    /// </summary>
    public class GameOverScreen : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Text Presets")]
        [SerializeField] private string winTitle = "Victory!";
        [SerializeField] private string winMessage = "You built an empire that dominates the market!";
        [SerializeField] private string loseTitle = "Game Over";
        [SerializeField] private string loseMessage = "Your business empire has crumbled...";

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
    }
}
