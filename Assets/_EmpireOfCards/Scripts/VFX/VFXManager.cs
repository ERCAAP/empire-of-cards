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
    ///
    /// Programmatic particle effects inspired by Epic Toon FX style.
    /// No external prefab dependencies -- all effects built at runtime.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("Prefabs (Legacy Pool)")]
        [SerializeField] private ParticleSystem coinRainPrefab;
        [SerializeField] private ParticleSystem comboGlowPrefab;

        [Header("Screen Flash")]
        [SerializeField] private UnityEngine.UI.Image screenFlashImage;

        [Header("Pool")]
        [SerializeField] private Transform poolParent;
        [SerializeField] private int initialPoolSize = 5;

        // Object pooling (legacy prefab-based effects)
        private readonly Dictionary<ParticleSystem, Queue<ParticleSystem>> pools = new();
        private readonly Dictionary<ParticleSystem, ParticleSystem> instanceToPrefab = new();
        private readonly List<ParticleSystem> activeEffects = new();

        // Programmatic effect tracking (auto-destroy via lifetime)
        private readonly List<GameObject> programmaticEffects = new();

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

        // Shared default particle material (cached)
        private Material _particleMaterial;

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
            // Board placement
            EventBus.OnBusinessPlaced += OnBusinessPlaced;
            EventBus.OnEmployeePlaced += OnEmployeePlaced;

            // Card play
            EventBus.OnCardPlayed += OnCardPlayed;

            // Economy
            EventBus.OnIncomeReceived += OnIncomeReceived;
        }

        private void OnDisable()
        {
            EventBus.OnBusinessPlaced -= OnBusinessPlaced;
            EventBus.OnEmployeePlaced -= OnEmployeePlaced;
            EventBus.OnCardPlayed -= OnCardPlayed;
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

            // Return finished particle systems to pool (legacy)
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                if (activeEffects[i] != null && !activeEffects[i].isPlaying)
                {
                    ReturnToPool(activeEffects[i]);
                    activeEffects.RemoveAt(i);
                }
            }

            // Clean up destroyed programmatic effects from tracking list
            for (int i = programmaticEffects.Count - 1; i >= 0; i--)
            {
                if (programmaticEffects[i] == null)
                    programmaticEffects.RemoveAt(i);
            }
        }

        // ------------------------------------------------------------------
        // EventBus Callbacks
        // ------------------------------------------------------------------

        private void OnBusinessPlaced(CardData card, int slotIndex)
        {
            Vector3 pos = GetBusinessSlotPosition(slotIndex);
            SpawnCardPlaceEffect(pos);
        }

        private void OnEmployeePlaced(CardData card, int businessIndex)
        {
            Vector3 pos = GetBusinessSlotPosition(businessIndex);
            // Offset slightly upward so employee sparkle sits above business
            pos.y += 0.4f;
            SpawnSmallSparkleEffect(pos);
        }

        private void OnCardPlayed(CardData card)
        {
            // Small poof at a default hand-play position (center-bottom of board)
            Vector3 pos = new Vector3(0f, 0.3f, -1.0f);
            SpawnCardPoofEffect(pos);
        }

        private void OnIncomeReceived(int amount)
        {
            // Spawn income particles; more particles for larger amounts
            Vector3 pos = new Vector3(0f, 1.5f, -1.5f);
            SpawnIncomeEffect(pos, amount);
        }

        // ------------------------------------------------------------------
        // Programmatic Particle Effects (Epic Toon FX style)
        // ------------------------------------------------------------------

        /// <summary>
        /// Business card placed on board: gold/yellow burst of sparkles.
        /// 20 particles, 0.5s lifetime, small sphere shape.
        /// </summary>
        public void SpawnCardPlaceEffect(Vector3 position)
        {
            GameObject go = CreateEffectObject("VFX_CardPlace", position);
            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 0.5f;
            main.loop = false;
            main.startLifetime = 0.5f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
            main.startColor = new Color(1f, 0.85f, 0.2f); // Gold/yellow
            main.gravityModifier = 0.5f;
            main.maxParticles = 20;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 20)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.3f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(1f, 0.85f, 0.2f), 0f),
                    new GradientColorKey(new Color(1f, 0.95f, 0.6f), 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = grad;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));

            ApplyParticleRenderer(go);
            ps.Play();

            programmaticEffects.Add(go);
            Destroy(go, 1.0f);
        }

        /// <summary>
        /// Small sparkle for employee placement.
        /// 10 particles, subtle and quick.
        /// </summary>
        public void SpawnSmallSparkleEffect(Vector3 position)
        {
            GameObject go = CreateEffectObject("VFX_SmallSparkle", position);
            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 0.4f;
            main.loop = false;
            main.startLifetime = 0.4f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2.5f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
            main.startColor = new Color(0.8f, 0.9f, 1f); // Light blue-white
            main.gravityModifier = 0.3f;
            main.maxParticles = 10;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 10)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.2f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(new Color(0.8f, 0.9f, 1f), 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = grad;

            ApplyParticleRenderer(go);
            ps.Play();

            programmaticEffects.Add(go);
            Destroy(go, 0.8f);
        }

        /// <summary>
        /// Small poof when any card is played from hand.
        /// 8 particles, very short lived.
        /// </summary>
        public void SpawnCardPoofEffect(Vector3 position)
        {
            GameObject go = CreateEffectObject("VFX_CardPoof", position);
            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 0.3f;
            main.loop = false;
            main.startLifetime = 0.3f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.2f);
            main.startColor = new Color(0.9f, 0.9f, 1f, 0.8f); // Faint white
            main.gravityModifier = 0.1f;
            main.maxParticles = 8;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 8)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.15f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0.8f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = grad;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0.5f, 1f, 1.5f));

            ApplyParticleRenderer(go);
            ps.Play();

            programmaticEffects.Add(go);
            Destroy(go, 0.6f);
        }

        /// <summary>
        /// Combo triggered: large radial burst with rainbow/gold sparkles.
        /// 50 particles, 1.0s lifetime, outward explosion.
        /// </summary>
        public void SpawnComboEffect(Vector3 position, Color baseColor)
        {
            GameObject go = CreateEffectObject("VFX_Combo", position);
            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 1.0f;
            main.loop = false;
            main.startLifetime = 1.0f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 8f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
            main.startColor = new ParticleSystem.MinMaxGradient(
                baseColor,
                new Color(1f, 0.95f, 0.6f) // Gold highlight
            );
            main.gravityModifier = 0.3f;
            main.maxParticles = 50;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 50)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.1f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(baseColor, 0f),
                    new GradientColorKey(Color.white, 0.5f),
                    new GradientColorKey(new Color(1f, 0.85f, 0.2f), 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.8f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = grad;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.2f));

            ApplyParticleRenderer(go);
            ps.Play();

            programmaticEffects.Add(go);
            Destroy(go, 1.5f);
        }

        /// <summary>
        /// Income received: green particles floating upward (money style).
        /// 15 particles, negative gravity to float up, 1.0s lifetime.
        /// Scales particle count slightly for larger income amounts.
        /// </summary>
        public void SpawnIncomeEffect(Vector3 position, int amount = 0)
        {
            GameObject go = CreateEffectObject("VFX_Income", position);
            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            // Scale particles: base 15, up to 30 for large incomes
            int particleCount = Mathf.Clamp(15 + (amount / 50), 15, 30);

            var main = ps.main;
            main.duration = 1.0f;
            main.loop = false;
            main.startLifetime = 1.0f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 3f);
            main.startSize = 0.15f;
            main.startColor = new Color(0.2f, 0.9f, 0.3f); // Green (money)
            main.gravityModifier = -0.5f; // Float upward
            main.maxParticles = particleCount;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, (short)particleCount)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 15f;
            shape.radius = 0.1f;
            // Cone points upward by default via transform rotation
            go.transform.rotation = Quaternion.identity;

            var velocityOverLifetime = ps.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            // All 3 axes must use the same curve mode to avoid "Velocity curves must all be in the same mode"
            velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.3f, 0.3f);
            velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(1f, 2f);
            velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.3f, 0.3f);

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(0.2f, 0.9f, 0.3f), 0f),
                    new GradientColorKey(new Color(0.5f, 1f, 0.5f), 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = grad;

            ApplyParticleRenderer(go);
            ps.Play();

            programmaticEffects.Add(go);
            Destroy(go, 1.5f);
        }

        /// <summary>
        /// Company tier promotion: massive gold/white nova burst.
        /// 80 particles, 1.5s lifetime, large radial explosion.
        /// </summary>
        public void SpawnTierUpEffect(Vector3 position)
        {
            GameObject go = CreateEffectObject("VFX_TierUp", position);
            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 1.5f;
            main.loop = false;
            main.startLifetime = 1.5f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(6f, 10f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.8f);
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(1f, 0.85f, 0.2f), // Gold center
                Color.white                   // White edges
            );
            main.gravityModifier = 0.2f;
            main.maxParticles = 80;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 80)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.15f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(1f, 0.85f, 0.2f), 0f),
                    new GradientColorKey(Color.white, 0.4f),
                    new GradientColorKey(new Color(1f, 0.95f, 0.5f), 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.9f, 0.3f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = grad;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.1f));

            ApplyParticleRenderer(go);
            ps.Play();

            programmaticEffects.Add(go);
            Destroy(go, 2.0f);
        }

        /// <summary>
        /// FBI raid: aggressive red/orange explosion.
        /// 60 particles, 1.0s lifetime, fast radial burst.
        /// </summary>
        public void SpawnFBIRaidEffect(Vector3 position)
        {
            GameObject go = CreateEffectObject("VFX_FBIRaid", position);
            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 1.0f;
            main.loop = false;
            main.startLifetime = 1.0f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(6f, 8f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.4f, 0.8f);
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(1f, 0.3f, 0f),  // Orange
                new Color(1f, 0f, 0f)      // Red
            );
            main.gravityModifier = 0.4f;
            main.maxParticles = 60;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 60)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.2f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(1f, 0.4f, 0f), 0f),
                    new GradientColorKey(new Color(1f, 0f, 0f), 0.5f),
                    new GradientColorKey(new Color(0.3f, 0f, 0f), 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.7f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = grad;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.3f));

            ApplyParticleRenderer(go);
            ps.Play();

            programmaticEffects.Add(go);
            Destroy(go, 2.0f);
        }

        // ------------------------------------------------------------------
        // Board Position Helper
        // ------------------------------------------------------------------

        /// <summary>
        /// Returns the world position for a business slot.
        /// Slots are spaced 2.5 units apart, centered on x=0.
        /// Y sits slightly above the board surface.
        /// </summary>
        private Vector3 GetBusinessSlotPosition(int slotIndex)
        {
            float x = (slotIndex - 2) * 2.5f;
            return new Vector3(x, 0.5f, -1.5f);
        }

        // ------------------------------------------------------------------
        // Effect Construction Helpers
        // ------------------------------------------------------------------

        /// <summary>
        /// Creates a fresh GameObject at the given world position, parented
        /// under poolParent to keep the hierarchy tidy.
        /// </summary>
        private GameObject CreateEffectObject(string name, Vector3 position)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(poolParent);
            go.transform.position = position;
            return go;
        }

        /// <summary>
        /// Ensures the ParticleSystemRenderer has a usable material so
        /// particles are visible. Uses the built-in default particle material.
        /// </summary>
        private void ApplyParticleRenderer(GameObject go)
        {
            ParticleSystemRenderer psr = go.GetComponent<ParticleSystemRenderer>();
            if (psr == null) return;

            if (_particleMaterial == null)
            {
                // Use Universal Render Pipeline particles shader (no texture needed for additive glow)
                var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
                if (shader == null) shader = Shader.Find("Particles/Standard Unlit");
                if (shader == null) shader = Shader.Find("Unlit/Color");
                _particleMaterial = new Material(shader);
                _particleMaterial.SetFloat("_Surface", 1f); // Transparent
                _particleMaterial.SetFloat("_Blend", 1f); // Additive
                _particleMaterial.renderQueue = 3000;
            }

            psr.material = _particleMaterial;
            psr.renderMode = ParticleSystemRenderMode.Billboard;
        }

        // ------------------------------------------------------------------
        // Public VFX Methods (Legacy + general-purpose)
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
            // Legacy pooled effects
            foreach (ParticleSystem ps in activeEffects)
            {
                if (ps != null)
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            activeEffects.Clear();

            // Programmatic effects
            foreach (GameObject go in programmaticEffects)
            {
                if (go != null)
                    Destroy(go);
            }
            programmaticEffects.Clear();

            isFlashing = false;

            if (tokenInstance != null)
            {
                Destroy(tokenInstance);
                tokenInstance = null;
            }

            isMovingToken = false;
        }

        // ------------------------------------------------------------------
        // Object Pooling (Legacy)
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
                instanceToPrefab[ps] = prefab;
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
                instanceToPrefab[ps] = prefab;
            }

            ps.gameObject.SetActive(true);
            return ps;
        }

        private void ReturnToPool(ParticleSystem ps)
        {
            if (ps == null) return;

            ps.gameObject.SetActive(false);

            if (instanceToPrefab.TryGetValue(ps, out ParticleSystem prefab) && pools.ContainsKey(prefab))
            {
                pools[prefab].Enqueue(ps);
            }
        }
    }
}
