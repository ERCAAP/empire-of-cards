using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Displays action point dots and animates them when used.
    /// </summary>
    public class ActionBarUI : MonoBehaviour
    {
        [Header("Dot References")]
        [SerializeField] private Image[] actionDots;

        [Header("Colors")]
        [SerializeField] private Color activeColor = new Color(0.2f, 0.8f, 0.2f, 1f);
        [SerializeField] private Color usedColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        [Header("Animation")]
        [SerializeField] private float pulseScale = 1.3f;
        [SerializeField] private float pulseDuration = 0.25f;

        /// <summary>
        /// Updates the action dots to show remaining vs used actions.
        /// </summary>
        public void UpdateActions(int remaining, int max)
        {
            for (int i = 0; i < actionDots.Length; i++)
            {
                if (i < max)
                {
                    actionDots[i].gameObject.SetActive(true);
                    actionDots[i].color = i < remaining ? activeColor : usedColor;
                }
                else
                {
                    actionDots[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Plays a pulse animation on the dot at the given index as it turns from active to used.
        /// </summary>
        public void AnimateUseAction(int index)
        {
            if (index < 0 || index >= actionDots.Length)
                return;

            StartCoroutine(PulseDotCoroutine(index));
        }

        private IEnumerator PulseDotCoroutine(int index)
        {
            Image dot = actionDots[index];
            RectTransform rt = dot.GetComponent<RectTransform>();
            Vector3 originalScale = rt.localScale;

            // Scale up
            float elapsed = 0f;
            float half = pulseDuration * 0.5f;

            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / half;
                rt.localScale = Vector3.Lerp(originalScale, originalScale * pulseScale, t);
                yield return null;
            }

            // Change color to used
            dot.color = usedColor;

            // Scale back down
            elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / half;
                rt.localScale = Vector3.Lerp(originalScale * pulseScale, originalScale, t);
                yield return null;
            }

            rt.localScale = originalScale;
        }
    }
}
