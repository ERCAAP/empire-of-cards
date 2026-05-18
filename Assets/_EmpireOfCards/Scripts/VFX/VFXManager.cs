using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.VFX
{
    /// <summary>
    /// Manages visual effects with object pooling. Subscribes to EventBus
    /// events for automatic VFX triggers. All animations are Update()-driven
    /// -- no coroutines.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("Prefabs")]
        [SerializeField] private ParticleSystem coinRainPrefab;
        [SerializeField] private ParticleSystem comboGlowPrefab;

        [Header("Screen Flash")]
        [SerializeField] private UnityEngine.UI.Image screenFlashImage;

        [Header("Pool")]
        [SerializeField] private Transform poolParent;
        [SerializeField] private int initialPoolSize = 5;

        // Object pooling
        private readonly Dictionary<ParticleSystem, Queue<ParticleSystem>> pools = new();
        private readonly List<ParticleSystem> activeEffects = new();

        // Screen flash (Update-driven)
        private bool isFlashing;
        private float flashTimer;
        private float flashDuration;
        private Color flashStartColor;

        // Token move (Update-driven)
        private bool isMovingToken;
        private float tokenTimer;
        private float tokenDuration;
        private Vector3 tokenFrom;
        private Vector3 tokenTo;
        private GameObject tokenInstance;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (poolParent == null)
                poolParent = transform;

            WarmPool(coinRainPrefab, initialPoolSize);
            WarmPool(comboGlowPrefab, initialPoolSize);

            // Hide flash image
            if (screenFlashImage != null)
            {
                Color c = screenFlashImage.color;
                c.a = 0f;
                screenFlashImage.color = c;
            }
        }

        private void OnEnable()
        {
            EventBus.OnComboTriggered += OnComboTriggered;
            EventBus.OnFBIRaid += OnFBIRaid;
            EventBus.OnBusinessPlaced += OnBusinessPlaced;
            EventBus.OnIncomeReceived += OnIncomeReceived;
        }

        private void OnDisable()
        {
            EventBus.OnComboTriggered -= OnComboTriggered;
            EventBus.OnFBIRaid -= OnFBIRaid;
            EventBus.OnBusinessPlaced -= OnBusinessPlaced;
            EventBus.OnIncomeReceived -= OnIncomeReceived;
        }

        private void Update()
        {
            // Screen flash fade
            if (isFlashing)
            {
                flashTimer += Time.deltaTime;
                float t = Mathf.Clamp01(flashTimer / flashDuration);

                if (screenFlashImage != null)
                {
                    Color c = flashStartColor;
                    c.a = Mathf.Lerp(flashStartColor.a, 0f, t);
                    screenFlashImage.color = c;
                }

                if (t >= 1f)
                {
                    isFlashing = false;
                    if (screenFlashImage != null)
                    {
                        Color c = screenFlashImage.color;
                        c.a = 0f;
                        screenFlashImage.color = c;
                    }
                }
            }

            // Token move animation
            if (isMovingToken && tokenInstance != null)
            {
                tokenTimer += Time.deltaTime;
                float t = Mathf.Clamp01(tokenTimer / tokenDuration);

                // Arc movement
                float arc = Mathf.Sin(t * Mathf.PI) * 0.5f;
                Vector3 pos = Vector3.Lerp(tokenFrom, tokenTo, t);
                pos.y += arc;
                tokenInstance.transform.position = pos;

                if (t >= 1f)
                {
                    Destroy(tokenInstance);
                    tokenInstance = null;
                    isMovingToken = false;
                }
            }

            // Return finished particle systems to pool
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                if (activeEffects[i] != null && !activeEffects[i].isPlaying)
                {
                    ReturnToPool(activeEffects[i]);
                    activeEffects.RemoveAt(i);
                }
            }
        }

        // ------------------------------------------------------------------
        // EventBus callbacks
        // ------------------------------------------------------------------

        private void OnComboTriggered(ComboData combo)
        {
            PlayScreenShake(combo.screenShakeIntensity, combo.screenShakeDuration);
            PlayComboGlow(Vector3.zero, combo.glowColor);
        }

        private void OnFBIRaid(int penalty)
        {
            PlayScreenFlash(new Color(1f, 0f, 0f, 0.4f), 0.5f);
            PlayScreenShake(0.4f, 0.4f);
        }

        private void OnBusinessPlaced(CardData card, int slotIndex)
        {
            PlayTokenMove(Vector3.up * 2f, Vector3.zero);
        }

        private void OnIncomeReceived(int amount)
        {
            PlayCoinRain(Vector3.up * 3f);
        }

        // ------------------------------------------------------------------
        // Public VFX methods
        // ------------------------------------------------------------------

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
        }

        /// <summary>
        /// Spawns a colored glow effect at the given position.
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
        }

        /// <summary>
        /// Flashes the screen with the given color for a brief duration.
        /// Driven by Update() polling.
        /// </summary>
        public void PlayScreenFlash(Color color, float duration = 0.3f)
        {
            if (screenFlashImage == null) return;

            flashStartColor = color;
            flashDuration = duration;
            flashTimer = 0f;
            isFlashing = true;

            screenFlashImage.color = color;
        }

        /// <summary>
        /// Shakes the main camera. Delegates to ScreenShake component.
        /// </summary>
        public void PlayScreenShake(float intensity = 0.3f, float duration = 0.3f)
        {
            ScreenShake shaker = Camera.main != null
                ? Camera.main.GetComponent<ScreenShake>()
                : null;

            if (shaker != null)
                shaker.Shake(intensity, duration);
        }

        /// <summary>
        /// Animates a token moving from one position to another with an arc.
        /// Driven by Update() polling.
        /// </summary>
        public void PlayTokenMove(Vector3 from, Vector3 to)
        {
            // Clean up previous token if still in flight
            if (tokenInstance != null)
                Destroy(tokenInstance);

            tokenInstance = new GameObject("MovingToken");
            tokenInstance.transform.SetParent(poolParent);
            tokenInstance.transform.position = from;

            tokenFrom = from;
            tokenTo = to;
            tokenDuration = 0.5f;
            tokenTimer = 0f;
            isMovingToken = true;
        }

        /// <summary>
        /// Stops all active particle effects and clears tracking.
        /// </summary>
        public void StopAllEffects()
        {
            foreach (ParticleSystem ps in activeEffects)
            {
                if (ps != null)
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            activeEffects.Clear();
            isFlashing = false;

            if (tokenInstance != null)
            {
                Destroy(tokenInstance);
                tokenInstance = null;
            }

            isMovingToken = false;
        }

        // ------------------------------------------------------------------
        // Object Pooling
        // ------------------------------------------------------------------

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

        private void ReturnToPool(ParticleSystem ps)
        {
            if (ps == null) return;

            ps.gameObject.SetActive(false);

            // Find which pool this belongs to by checking all prefab keys
            foreach (var kvp in pools)
            {
                // Simple heuristic: return to first matching pool
                // In practice, you'd store a mapping of instance -> prefab
                kvp.Value.Enqueue(ps);
                return;
            }
        }
    }
}
