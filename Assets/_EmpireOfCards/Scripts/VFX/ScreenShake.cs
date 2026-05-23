using UnityEngine;

namespace EmpireOfCards.VFX
{
    /// <summary>
    /// Camera shake component. Uses Update() polling (NOT coroutine).
    /// Dampening over time. Additive -- multiple shakes can overlap
    /// by re-invoking Shake() which increases remaining timer and intensity.
    /// </summary>
    public class ScreenShake : MonoBehaviour
    {
        [Header("Defaults")]
        [SerializeField] private float defaultIntensity = 0.3f;
        [SerializeField] private float defaultDuration = 0.3f;

        // Runtime
        private Vector3 originalPosition;
        private float shakeTimer;
        private float shakeDuration;
        private float shakeIntensity;
        private bool isShaking;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void Awake()
        {
            originalPosition = transform.localPosition;
        }

        private void Update()
        {
            if (!isShaking)
                return;

            shakeTimer += Time.deltaTime;

            if (shakeTimer >= shakeDuration)
            {
                // Shake finished -- snap back
                transform.localPosition = originalPosition;
                isShaking = false;
                return;
            }

            // Dampening: intensity tapers linearly toward zero
            float remaining = 1f - (shakeTimer / shakeDuration);
            float currentIntensity = shakeIntensity * remaining;

            float offsetX = Random.Range(-currentIntensity, currentIntensity);
            float offsetY = Random.Range(-currentIntensity, currentIntensity);

            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);
        }

        // ------------------------------------------------------------------
        // Public
        // ------------------------------------------------------------------

        /// <summary>
        /// Triggers a screen shake. Additive -- calling Shake() while
        /// already shaking extends the timer and takes the higher intensity.
        /// </summary>
        public void Shake(float intensity, float duration)
        {
            if (!isShaking)
            {
                // Fresh shake -- record the camera's rest position
                originalPosition = transform.localPosition;
            }

            // Additive: extend remaining time, use higher intensity
            float remainingTime = isShaking ? Mathf.Max(0f, shakeDuration - shakeTimer) : 0f;
            shakeDuration = remainingTime + duration;
            shakeIntensity = Mathf.Max(shakeIntensity, intensity);
            shakeTimer = 0f;
            isShaking = true;
        }

        /// <summary>
        /// Convenience overload using inspector defaults.
        /// </summary>
        public void Shake()
        {
            Shake(defaultIntensity, defaultDuration);
        }

        /// <summary>
        /// Immediately stops any active shake and snaps back.
        /// </summary>
        public void ResetPosition()
        {
            isShaking = false;
            shakeTimer = 0f;
            transform.localPosition = originalPosition;
        }
    }
}
