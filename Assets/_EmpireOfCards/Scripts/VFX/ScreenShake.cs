using System.Collections;
using UnityEngine;

namespace EmpireOfCards.VFX
{
    /// <summary>
    /// Attach to the main camera to enable screen shake via coroutine.
    /// </summary>
    public class ScreenShake : MonoBehaviour
    {
        [SerializeField] private float shakeDuration;
        [SerializeField] private float shakeMagnitude;

        private Transform cameraTransform;
        private Vector3 originalPosition;
        private Coroutine shakeCoroutine;

        private void Awake()
        {
            cameraTransform = transform;
        }

        /// <summary>
        /// Shakes the camera for the given duration and magnitude.
        /// If a shake is already in progress it is replaced.
        /// </summary>
        public void Shake(float duration, float magnitude)
        {
            shakeDuration = duration;
            shakeMagnitude = magnitude;

            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
                cameraTransform.localPosition = originalPosition;
            }

            shakeCoroutine = StartCoroutine(ShakeCoroutine());
        }

        private IEnumerator ShakeCoroutine()
        {
            originalPosition = cameraTransform.localPosition;
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;

                // Dampen intensity over time so shake tapers off naturally
                float dampening = 1f - (elapsed / shakeDuration);
                float offsetX = Random.Range(-1f, 1f) * shakeMagnitude * dampening;
                float offsetY = Random.Range(-1f, 1f) * shakeMagnitude * dampening;

                cameraTransform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

                yield return null;
            }

            cameraTransform.localPosition = originalPosition;
            shakeCoroutine = null;
        }
    }
}
