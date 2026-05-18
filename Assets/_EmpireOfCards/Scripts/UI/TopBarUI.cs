using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Displays the player's money, turn counter, and FBI risk meter in the top bar.
    /// </summary>
    public class TopBarUI : MonoBehaviour
    {
        [Header("Text Fields")]
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private TMP_Text fbiRiskText;

        [Header("Risk Bar")]
        [SerializeField] private Image fbiRiskBar;

        [Header("Animation")]
        [SerializeField] private float moneyAnimDuration = 0.5f;

        private Coroutine moneyAnimCoroutine;

        /// <summary>
        /// Sets the money display to the given amount immediately.
        /// </summary>
        public void UpdateMoney(int amount)
        {
            if (moneyText != null)
            {
                moneyText.text = $"${amount:N0}";
            }
        }

        /// <summary>
        /// Updates the turn counter display.
        /// </summary>
        public void UpdateTurn(int current, int max)
        {
            if (turnText != null)
            {
                turnText.text = $"Turn {current}/{max}";
            }
        }

        /// <summary>
        /// Updates the FBI risk bar and text. Percent should be 0-1.
        /// </summary>
        public void UpdateFBIRisk(float percent)
        {
            percent = Mathf.Clamp01(percent);

            if (fbiRiskBar != null)
            {
                fbiRiskBar.fillAmount = percent;
                fbiRiskBar.color = Color.Lerp(Color.yellow, Color.red, percent);
            }

            if (fbiRiskText != null)
            {
                fbiRiskText.text = $"FBI Risk: {Mathf.RoundToInt(percent * 100)}%";
            }
        }

        /// <summary>
        /// Smoothly animates the money display from one value to another.
        /// </summary>
        public void AnimateMoneyChange(int from, int to)
        {
            if (moneyAnimCoroutine != null)
            {
                StopCoroutine(moneyAnimCoroutine);
            }

            moneyAnimCoroutine = StartCoroutine(AnimateMoneyCoroutine(from, to));
        }

        private IEnumerator AnimateMoneyCoroutine(int from, int to)
        {
            float elapsed = 0f;

            while (elapsed < moneyAnimDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / moneyAnimDuration);
                // Ease-out curve for satisfying deceleration
                t = 1f - Mathf.Pow(1f - t, 3f);
                int current = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
                UpdateMoney(current);
                yield return null;
            }

            UpdateMoney(to);
            moneyAnimCoroutine = null;
        }
    }
}
