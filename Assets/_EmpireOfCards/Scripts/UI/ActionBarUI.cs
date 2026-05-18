using UnityEngine;
using UnityEngine.UI;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Shows 3-5 action dots. Filled = available, empty = used.
    /// Subscribes to OnPhaseStarted (resets on PlayPhase) and polls
    /// GameManager.PlayerActions each frame during PlayPhase to keep
    /// the dot display in sync. All animations use Update() polling.
    /// </summary>
    public class ActionBarUI : MonoBehaviour
    {
        [Header("Dot References")]
        [SerializeField] private Image[] actionDots;          // up to MAX_ACTIONS (5)

        [Header("Colors")]
        [SerializeField] private Color filledColor = new Color(0.2f, 0.8f, 0.2f, 1f);
        [SerializeField] private Color emptyColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        [Header("Pulse Animation")]
        [SerializeField] private float pulseScale = 1.3f;
        [SerializeField] private float pulseSpeed = 8f;

        // Runtime
        private int lastKnownActions = -1;
        private bool isPlayPhase;
        private int pulsingDotIndex = -1;
        private float pulseTimer;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void OnEnable()
        {
            EventBus.OnPhaseStarted += OnPhaseStarted;
        }

        private void OnDisable()
        {
            EventBus.OnPhaseStarted -= OnPhaseStarted;
        }

        private void Update()
        {
            // Polling: during PlayPhase, check GameManager each frame
            if (isPlayPhase)
            {
                var gm = GameManager.Instance;
                if (gm != null)
                {
                    int current = gm.PlayerActions;
                    int max = gm.MaxActions;

                    if (current != lastKnownActions)
                    {
                        // An action was just used -- trigger pulse on the dot that flipped
                        if (lastKnownActions > current && current >= 0)
                        {
                            pulsingDotIndex = current; // the dot that just emptied
                            pulseTimer = 0f;
                        }

                        lastKnownActions = current;
                        ApplyDots(current, max);
                    }
                }
            }

            // Pulse animation (Update-driven, no coroutine)
            if (pulsingDotIndex >= 0 && pulsingDotIndex < actionDots.Length
                && actionDots[pulsingDotIndex] != null)
            {
                pulseTimer += Time.deltaTime * pulseSpeed;
                float scale = Mathf.Lerp(pulseScale, 1f, Mathf.Clamp01(pulseTimer));
                actionDots[pulsingDotIndex].transform.localScale = Vector3.one * scale;

                if (pulseTimer >= 1f)
                {
                    actionDots[pulsingDotIndex].transform.localScale = Vector3.one;
                    pulsingDotIndex = -1;
                }
            }
        }

        // ------------------------------------------------------------------
        // EventBus handler
        // ------------------------------------------------------------------

        private void OnPhaseStarted(TurnPhase phase)
        {
            isPlayPhase = phase == TurnPhase.PlayPhase;

            if (isPlayPhase)
            {
                // Force a fresh read on phase entry
                lastKnownActions = -1;
            }
        }

        // ------------------------------------------------------------------
        // Public
        // ------------------------------------------------------------------

        /// <summary>
        /// Directly sets the dot display. Called by UIManager.UpdateAllUI.
        /// </summary>
        public void UpdateActions(int current, int max)
        {
            lastKnownActions = current;
            ApplyDots(current, max);
        }

        /// <summary>
        /// Shows or hides the entire action bar.
        /// </summary>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        // ------------------------------------------------------------------
        // Internal
        // ------------------------------------------------------------------

        private void ApplyDots(int current, int max)
        {
            for (int i = 0; i < actionDots.Length; i++)
            {
                if (actionDots[i] == null)
                    continue;

                if (i < max)
                {
                    actionDots[i].gameObject.SetActive(true);
                    actionDots[i].color = i < current ? filledColor : emptyColor;
                }
                else
                {
                    actionDots[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
