using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmpireOfCards.VFX
{
    /// <summary>
    /// Manages visual effects such as coin rain, combo glows, screen flashes,
    /// and screen shake. Uses simple object pooling for particle systems.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("Prefabs")]
        [SerializeField] private ParticleSystem coinRainPrefab;
        [SerializeField] private ParticleSystem comboGlowPrefab;
        [SerializeField] private GameObject screenFlashPrefab;

        [Header("Pool")]
        [SerializeField] private Transform poolParent;
        [SerializeField] private int initialPoolSize = 5;

        private readonly Dictionary<ParticleSystem, Queue<ParticleSystem>> pools = new();
        private readonly List<ParticleSystem> activeEffects = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (poolParent == null)
            {
                poolParent = transform;
            }

            WarmPool(coinRainPrefab, initialPoolSize);
            WarmPool(comboGlowPrefab, initialPoolSize);
        }

        /// <summary>
        /// Spawns a coin rain particle effect at the given world position.
        /// </summary>
        public void PlayCoinRain(Vector3 position)
        {
            ParticleSystem ps = GetFromPool(coinRainPrefab);
            if (ps == null) return;

            ps.transform.position = position;
            ps.Play();
            activeEffects.Add(ps);
            StartCoroutine(ReturnToPoolWhenDone(ps, coinRainPrefab));
        }

        /// <summary>
        /// Spawns a colored glow effect at the given position (e.g. for combo triggers).
        /// </summary>
        public void PlayComboGlow(Vector3 position, Color color)
        {
            ParticleSystem ps = GetFromPool(comboGlowPrefab);
            if (ps == null) return;

            ps.transform.position = position;

            var main = ps.main;
            main.startColor = color;

            ps.Play();
            activeEffects.Add(ps);
            StartCoroutine(ReturnToPoolWhenDone(ps, comboGlowPrefab));
        }

        /// <summary>
        /// Flashes the screen with the given color for a brief duration.
        /// </summary>
        public void PlayScreenFlash(Color color, float duration = 0.3f)
        {
            if (screenFlashPrefab == null) return;

            GameObject flash = Instantiate(screenFlashPrefab, poolParent);
            UnityEngine.UI.Image img = flash.GetComponentInChildren<UnityEngine.UI.Image>();
            if (img != null)
            {
                img.color = color;
            }

            StartCoroutine(ScreenFlashCoroutine(flash, duration));
        }

        /// <summary>
        /// Shakes the main camera for a short duration.
        /// </summary>
        public void PlayScreenShake(float intensity = 0.3f, float duration = 0.3f)
        {
            ScreenShake shaker = Camera.main != null
                ? Camera.main.GetComponent<ScreenShake>()
                : null;

            if (shaker != null)
            {
                shaker.Shake(duration, intensity);
            }
            else if (Camera.main != null)
            {
                // Fallback: run inline shake coroutine
                StartCoroutine(InlineShakeCoroutine(Camera.main.transform, intensity, duration));
            }
        }

        /// <summary>
        /// Animates a token (e.g. coin icon) moving from one position to another.
        /// </summary>
        public void PlayTokenMove(Vector3 from, Vector3 to)
        {
            StartCoroutine(TokenMoveCoroutine(from, to));
        }

        /// <summary>
        /// Stops all active particle effects and clears tracking.
        /// </summary>
        public void StopAllEffects()
        {
            foreach (ParticleSystem ps in activeEffects)
            {
                if (ps != null)
                {
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }

            activeEffects.Clear();
        }

        // --- Object Pooling ---

        private void WarmPool(ParticleSystem prefab, int count)
        {
            if (prefab == null) return;

            if (!pools.ContainsKey(prefab))
                pools[prefab] = new Queue<ParticleSystem>();

            for (int i = 0; i < count; i++)
            {
                ParticleSystem ps = Instantiate(prefab, poolParent);
                ps.gameObject.SetActive(false);
                pools[prefab].Enqueue(ps);
            }
        }

        private ParticleSystem GetFromPool(ParticleSystem prefab)
        {
            if (prefab == null) return null;

            if (!pools.ContainsKey(prefab))
                pools[prefab] = new Queue<ParticleSystem>();

            ParticleSystem ps;

            if (pools[prefab].Count > 0)
            {
                ps = pools[prefab].Dequeue();
            }
            else
            {
                ps = Instantiate(prefab, poolParent);
            }

            ps.gameObject.SetActive(true);
            return ps;
        }

        private IEnumerator ReturnToPoolWhenDone(ParticleSystem ps, ParticleSystem prefab)
        {
            yield return new WaitUntil(() => !ps.isPlaying);

            ps.gameObject.SetActive(false);
            activeEffects.Remove(ps);

            if (pools.ContainsKey(prefab))
            {
                pools[prefab].Enqueue(ps);
            }
        }

        // --- Coroutines ---

        private IEnumerator ScreenFlashCoroutine(GameObject flash, float duration)
        {
            UnityEngine.UI.Image img = flash.GetComponentInChildren<UnityEngine.UI.Image>();
            Color startColor = img != null ? img.color : Color.white;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                if (img != null)
                {
                    Color c = startColor;
                    c.a = Mathf.Lerp(startColor.a, 0f, t);
                    img.color = c;
                }
                yield return null;
            }

            Destroy(flash);
        }

        private IEnumerator InlineShakeCoroutine(Transform camTransform, float magnitude, float duration)
        {
            Vector3 originalPos = camTransform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float dampening = 1f - (elapsed / duration);
                float offsetX = UnityEngine.Random.Range(-1f, 1f) * magnitude * dampening;
                float offsetY = UnityEngine.Random.Range(-1f, 1f) * magnitude * dampening;
                camTransform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);
                yield return null;
            }

            camTransform.localPosition = originalPos;
        }

        private IEnumerator TokenMoveCoroutine(Vector3 from, Vector3 to)
        {
            // Create a simple sprite or UI element to animate
            GameObject token = new GameObject("MovingToken");
            token.transform.SetParent(poolParent);
            token.transform.position = from;

            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                // Arc movement
                float arc = Mathf.Sin(t * Mathf.PI) * 0.5f;
                Vector3 pos = Vector3.Lerp(from, to, t);
                pos.y += arc;
                token.transform.position = pos;
                yield return null;
            }

            Destroy(token);
        }
    }
}
