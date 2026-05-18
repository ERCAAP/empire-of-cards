using UnityEngine;

namespace EmpireOfCards.Helpers
{
    /// <summary>
    /// Orthographic camera controller with screen shake support.
    /// Attach to the main camera GameObject.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Shake Settings")]
        [SerializeField] private float shakeIntensity;
        [SerializeField] private float shakeDuration;

        private Vector3 originalPosition;
        private float shakeTimer;

        private void Awake()
        {
            originalPosition = transform.localPosition;
        }

        private void Update()
        {
            if (shakeTimer > 0f)
            {
                // Apply random offset based on current intensity
                float currentIntensity = shakeIntensity * (shakeTimer / shakeDuration);
                Vector3 offset = new Vector3(
                    Random.Range(-currentIntensity, currentIntensity),
                    Random.Range(-currentIntensity, currentIntensity),
                    0f
                );

                transform.localPosition = originalPosition + offset;

                shakeTimer -= Time.deltaTime;

                // Snap back when shake is done
                if (shakeTimer <= 0f)
                {
                    transform.localPosition = originalPosition;
                }
            }
        }

        /// <summary>
        /// Triggers a screen shake effect with the given intensity and duration.
        /// </summary>
        /// <param name="intensity">Maximum offset in units.</param>
        /// <param name="duration">Duration in seconds.</param>
        public void Shake(float intensity, float duration)
        {
            shakeIntensity = intensity;
            shakeDuration = duration;
            shakeTimer = duration;
            originalPosition = transform.localPosition;
        }

        /// <summary>
        /// Immediately stops any active shake and snaps the camera back to its original position.
        /// </summary>
        public void ResetPosition()
        {
            shakeTimer = 0f;
            transform.localPosition = originalPosition;
        }
    }
}
