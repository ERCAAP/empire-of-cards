using UnityEngine;
using TMPro;
using EmpireOfCards.Core;
using EmpireOfCards.Helpers;
using DG.Tweening;

namespace EmpireOfCards.World
{
    /// <summary>
    /// Master scene builder. Creates the entire 3D restaurant world programmatically.
    /// Called by GameSceneBootstrap after managers are created and wired.
    /// </summary>
    public class GameSceneBuilder : MonoBehaviour
    {
        // ── Zone colors ─────────────────────────────────────────────
        static readonly Color COL_GROUND    = new Color(0.12f, 0.12f, 0.14f);
        static readonly Color COL_KITCHEN   = new Color(0.85f, 0.45f, 0.15f);
        static readonly Color COL_SALON     = new Color(0.76f, 0.60f, 0.42f);
        static readonly Color COL_STORAGE   = new Color(0.55f, 0.55f, 0.55f);
        static readonly Color COL_MARKETING = new Color(0.25f, 0.50f, 0.85f);
        static readonly Color COL_EVENTS    = new Color(0.60f, 0.15f, 0.15f);
        static readonly Color COL_STREET    = new Color(0.80f, 0.80f, 0.78f);
        static readonly Color COL_RIVAL     = new Color(0.20f, 0.20f, 0.25f);
        static readonly Color COL_WALL      = new Color(0.30f, 0.25f, 0.22f);
        static readonly Color COL_GRID      = new Color(0.18f, 0.18f, 0.20f);

        // ── References ──────────────────────────────────────────────
        Transform _worldRoot;
        MarketShareBar3D _marketBar;
        RatingStars3D _ratingStars;

        public MarketShareBar3D MarketBar => _marketBar;
        public RatingStars3D RatingStars => _ratingStars;

        // ── Build entry point ───────────────────────────────────────

        public void Build()
        {
            _worldRoot = new GameObject("[SceneWorld]").transform;

            BuildGround();
            BuildKitchenZone(-4f, 3f);
            BuildSalonZone(0f, 3f);
            BuildStorageZone(4f, 3f);
            BuildMarketingZone(-2f, 0f);
            BuildEventsZone(2f, 0f);
            BuildStreetZone(12f);
            BuildRivalZone(10f);
            BuildMarketBar(7f);
            BuildRatingStars(7f);
            BuildWalls();
            BuildLighting();
            BuildGridLines();

            Debug.Log("[GameSceneBuilder] Full 3D scene built.");
        }

        // ── Ground ──────────────────────────────────────────────────

        void BuildGround()
        {
            var ground = CreateCube("Ground", Vector3.down * 0.05f, new Vector3(30f, 0.1f, 30f), COL_GROUND);
            ground.transform.SetParent(_worldRoot);
        }

        // ── Kitchen zone ────────────────────────────────────────────

        void BuildKitchenZone(float x, float z)
        {
            var floor = CreateCube("Zone_Kitchen", new Vector3(x, 0.025f, z), new Vector3(4f, 0.05f, 3f), COL_KITCHEN);
            floor.transform.SetParent(_worldRoot);

            CreateZoneLabel("MUTFAK", new Vector3(x, 0.3f, z + 1.8f), COL_KITCHEN);

            // Decorative kitchen items
            CreateCube("KitchenCounter_L", new Vector3(x - 1.5f, 0.2f, z + 0.8f), new Vector3(0.8f, 0.4f, 0.4f), new Color(0.4f, 0.3f, 0.2f));
            CreateCube("KitchenCounter_R", new Vector3(x + 1.5f, 0.2f, z + 0.8f), new Vector3(0.8f, 0.4f, 0.4f), new Color(0.4f, 0.3f, 0.2f));
            CreateCube("Stove", new Vector3(x, 0.15f, z + 1.0f), new Vector3(0.6f, 0.3f, 0.3f), new Color(0.2f, 0.2f, 0.2f));

            // Subtle elevation marker
            CreateCube("KitchenEdge", new Vector3(x, 0.06f, z - 1.5f), new Vector3(4f, 0.02f, 0.05f), new Color(1f, 0.6f, 0.2f));
        }

        // ── Salon zone ──────────────────────────────────────────────

        void BuildSalonZone(float x, float z)
        {
            var floor = CreateCube("Zone_Salon", new Vector3(x, 0f, z), new Vector3(5f, 0.05f, 3f), COL_SALON);
            floor.transform.SetParent(_worldRoot);

            CreateZoneLabel("SALON", new Vector3(x, 0.3f, z + 1.8f), COL_SALON);

            // Tables
            for (int i = 0; i < 3; i++)
            {
                float tx = x - 1.5f + i * 1.5f;
                CreateCube($"Table_{i}", new Vector3(tx, 0.15f, z - 0.5f), new Vector3(0.5f, 0.05f, 0.5f), new Color(0.5f, 0.35f, 0.2f));
                // Table legs
                for (int leg = 0; leg < 4; leg++)
                {
                    float lx = tx + (leg % 2 == 0 ? -0.2f : 0.2f);
                    float lz = z - 0.5f + (leg / 2 == 0 ? -0.2f : 0.2f);
                    CreateCube($"TableLeg_{i}_{leg}", new Vector3(lx, 0.06f, lz), new Vector3(0.04f, 0.12f, 0.04f), new Color(0.4f, 0.28f, 0.15f));
                }
            }
        }

        // ── Storage zone ────────────────────────────────────────────

        void BuildStorageZone(float x, float z)
        {
            var floor = CreateCube("Zone_Storage", new Vector3(x, 0.015f, z), new Vector3(3f, 0.03f, 3f), COL_STORAGE);
            floor.transform.SetParent(_worldRoot);

            CreateZoneLabel("DEPO", new Vector3(x, 0.3f, z + 1.8f), COL_STORAGE);

            // Shelf cubes
            CreateCube("Shelf_1", new Vector3(x - 0.8f, 0.25f, z + 0.8f), new Vector3(0.4f, 0.5f, 0.3f), new Color(0.45f, 0.4f, 0.35f));
            CreateCube("Shelf_2", new Vector3(x + 0.8f, 0.25f, z + 0.8f), new Vector3(0.4f, 0.5f, 0.3f), new Color(0.45f, 0.4f, 0.35f));
            CreateCube("Crate", new Vector3(x, 0.1f, z - 0.5f), new Vector3(0.35f, 0.2f, 0.35f), new Color(0.6f, 0.5f, 0.3f));
        }

        // ── Marketing zone ──────────────────────────────────────────

        void BuildMarketingZone(float x, float z)
        {
            var floor = CreateCube("Zone_Marketing", new Vector3(x, 0.025f, z), new Vector3(3.5f, 0.05f, 2.5f), COL_MARKETING);
            floor.transform.SetParent(_worldRoot);

            CreateZoneLabel("MARKETING", new Vector3(x, 0.3f, z + 1.5f), COL_MARKETING);

            // Signboard
            CreateCube("Signboard", new Vector3(x, 0.4f, z + 0.5f), new Vector3(1.2f, 0.6f, 0.05f), new Color(0.2f, 0.4f, 0.8f));
            CreateCube("SignPole", new Vector3(x, 0.15f, z + 0.5f), new Vector3(0.05f, 0.3f, 0.05f), new Color(0.3f, 0.3f, 0.3f));
        }

        // ── Events zone ─────────────────────────────────────────────

        void BuildEventsZone(float x, float z)
        {
            var floor = CreateCube("Zone_Events", new Vector3(x, 0.025f, z), new Vector3(3.5f, 0.05f, 2.5f), COL_EVENTS);
            floor.transform.SetParent(_worldRoot);

            CreateZoneLabel("OLAYLAR", new Vector3(x, 0.3f, z + 1.5f), COL_EVENTS);

            // Warning symbol (two crossed cubes)
            CreateCube("WarnCross_1", new Vector3(x, 0.2f, z), new Vector3(0.05f, 0.3f, 0.05f), new Color(0.9f, 0.2f, 0.1f));
            CreateCube("WarnCross_2", new Vector3(x, 0.2f, z), new Vector3(0.3f, 0.05f, 0.05f), new Color(0.9f, 0.2f, 0.1f));
        }

        // ── Street zone ─────────────────────────────────────────────

        void BuildStreetZone(float z)
        {
            var street = CreateCube("Zone_Street", new Vector3(0f, 0f, z), new Vector3(20f, 0.05f, 2f), COL_STREET);
            street.transform.SetParent(_worldRoot);

            CreateZoneLabel("SOKAK", new Vector3(0f, 0.3f, z + 1.3f), new Color(0.4f, 0.4f, 0.4f));

            // Lane markings (dashed line)
            for (int i = -8; i <= 8; i += 2)
            {
                CreateCube($"LaneMark_{i}", new Vector3(i, 0.06f, z), new Vector3(0.8f, 0.01f, 0.08f), new Color(0.95f, 0.95f, 0.5f));
            }

            // Customer spawn point markers (left and right)
            CreateCube("SpawnPoint_L", new Vector3(-10f, 0.1f, z), new Vector3(0.3f, 0.2f, 0.3f), new Color(0.3f, 0.8f, 0.3f));
            CreateCube("SpawnPoint_R", new Vector3(10f, 0.1f, z), new Vector3(0.3f, 0.2f, 0.3f), new Color(0.8f, 0.3f, 0.3f));

            // Sidewalk edges
            CreateCube("Sidewalk_N", new Vector3(0f, 0.04f, z + 1.1f), new Vector3(20f, 0.08f, 0.15f), new Color(0.5f, 0.5f, 0.48f));
            CreateCube("Sidewalk_S", new Vector3(0f, 0.04f, z - 1.1f), new Vector3(20f, 0.08f, 0.15f), new Color(0.5f, 0.5f, 0.48f));
        }

        // ── Rival zone ──────────────────────────────────────────────

        void BuildRivalZone(float z)
        {
            var rivalFloor = CreateCube("Zone_Rival", new Vector3(0f, 0f, z), new Vector3(8f, 0.05f, 1.5f), COL_RIVAL);
            rivalFloor.transform.SetParent(_worldRoot);

            CreateZoneLabel("RAKIP DUKKAN", new Vector3(0f, 0.3f, z + 1f), new Color(0.8f, 0.2f, 0.2f));

            // Rival building silhouette
            CreateCube("RivalBuilding_1", new Vector3(-2f, 0.4f, z), new Vector3(2f, 0.8f, 1f), new Color(0.15f, 0.15f, 0.18f));
            CreateCube("RivalBuilding_2", new Vector3(2f, 0.3f, z), new Vector3(2f, 0.6f, 1f), new Color(0.18f, 0.18f, 0.22f));

            // Rival indicator light (red glow)
            var rivalLight = CreateCube("RivalIndicator", new Vector3(0f, 0.6f, z), new Vector3(0.2f, 0.2f, 0.2f), Color.red);
            rivalLight.GetComponent<Renderer>().material = MaterialHelper.CreateEmissive(Color.red, Color.red, 1.5f);

            // Pulse the rival indicator
            rivalLight.transform.DOScale(Vector3.one * 0.25f, 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        // ── Market share bar ────────────────────────────────────────

        void BuildMarketBar(float z)
        {
            var barHost = new GameObject("MarketShareBar3D");
            barHost.transform.SetParent(_worldRoot);
            barHost.transform.position = new Vector3(0f, 0.2f, z);
            _marketBar = barHost.AddComponent<MarketShareBar3D>();
            _marketBar.Build();
        }

        // ── Rating stars ────────────────────────────────────────────

        void BuildRatingStars(float z)
        {
            var starsHost = new GameObject("RatingStars3D");
            starsHost.transform.SetParent(_worldRoot);
            starsHost.transform.position = new Vector3(5f, 0.2f, z);
            _ratingStars = starsHost.AddComponent<RatingStars3D>();
            _ratingStars.Build();
        }

        // ── Walls ───────────────────────────────────────────────────

        void BuildWalls()
        {
            // Restaurant boundary walls (thin tall cubes)
            float wallHeight = 0.6f;
            float wallThick = 0.1f;

            // Back wall (behind kitchen/salon/storage)
            CreateCube("Wall_Back", new Vector3(0f, wallHeight * 0.5f, 5f), new Vector3(14f, wallHeight, wallThick), COL_WALL)
                .transform.SetParent(_worldRoot);

            // Left wall
            CreateCube("Wall_Left", new Vector3(-7f, wallHeight * 0.5f, 1.5f), new Vector3(wallThick, wallHeight, 10f), COL_WALL)
                .transform.SetParent(_worldRoot);

            // Right wall
            CreateCube("Wall_Right", new Vector3(7f, wallHeight * 0.5f, 1.5f), new Vector3(wallThick, wallHeight, 10f), COL_WALL)
                .transform.SetParent(_worldRoot);

            // Front wall (with door gap)
            CreateCube("Wall_Front_L", new Vector3(-4.5f, wallHeight * 0.5f, -2f), new Vector3(5f, wallHeight, wallThick), COL_WALL)
                .transform.SetParent(_worldRoot);
            CreateCube("Wall_Front_R", new Vector3(4.5f, wallHeight * 0.5f, -2f), new Vector3(5f, wallHeight, wallThick), COL_WALL)
                .transform.SetParent(_worldRoot);

            // Door frame
            CreateCube("DoorFrame_L", new Vector3(-1.8f, wallHeight * 0.5f, -2f), new Vector3(0.15f, wallHeight, wallThick + 0.05f), new Color(0.5f, 0.35f, 0.2f))
                .transform.SetParent(_worldRoot);
            CreateCube("DoorFrame_R", new Vector3(1.8f, wallHeight * 0.5f, -2f), new Vector3(0.15f, wallHeight, wallThick + 0.05f), new Color(0.5f, 0.35f, 0.2f))
                .transform.SetParent(_worldRoot);
        }

        // ── Lighting ────────────────────────────────────────────────

        void BuildLighting()
        {
            // Warm directional light (sun)
            var sunGo = new GameObject("DirectionalLight_Sun");
            sunGo.transform.SetParent(_worldRoot);
            sunGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var sun = sunGo.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.95f, 0.85f);
            sun.intensity = 1.2f;
            sun.shadows = LightShadows.Soft;

            // Fill light (softer, from opposite direction)
            var fillGo = new GameObject("DirectionalLight_Fill");
            fillGo.transform.SetParent(_worldRoot);
            fillGo.transform.rotation = Quaternion.Euler(30f, 150f, 0f);
            var fill = fillGo.AddComponent<Light>();
            fill.type = LightType.Directional;
            fill.color = new Color(0.7f, 0.8f, 1f);
            fill.intensity = 0.3f;
            fill.shadows = LightShadows.None;

            // Kitchen spot light (warm orange)
            var kitchenLight = new GameObject("SpotLight_Kitchen");
            kitchenLight.transform.SetParent(_worldRoot);
            kitchenLight.transform.position = new Vector3(-4f, 3f, 3f);
            kitchenLight.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            var spotK = kitchenLight.AddComponent<Light>();
            spotK.type = LightType.Spot;
            spotK.color = new Color(1f, 0.7f, 0.3f);
            spotK.intensity = 1.5f;
            spotK.range = 6f;
            spotK.spotAngle = 60f;

            // Salon spot light (warm white)
            var salonLight = new GameObject("SpotLight_Salon");
            salonLight.transform.SetParent(_worldRoot);
            salonLight.transform.position = new Vector3(0f, 3f, 3f);
            salonLight.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            var spotS = salonLight.AddComponent<Light>();
            spotS.type = LightType.Spot;
            spotS.color = new Color(1f, 0.95f, 0.8f);
            spotS.intensity = 1f;
            spotS.range = 7f;
            spotS.spotAngle = 70f;

            // Set ambient
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.15f, 0.13f, 0.12f);
        }

        // ── Grid lines on ground ────────────────────────────────────

        void BuildGridLines()
        {
            float size = 24f;
            float step = 2f;

            for (float x = -size * 0.5f; x <= size * 0.5f; x += step)
            {
                CreateCube($"GridX_{x}", new Vector3(x, 0.001f, 0f), new Vector3(0.02f, 0.002f, size), COL_GRID)
                    .transform.SetParent(_worldRoot);
            }

            for (float z = -size * 0.5f; z <= size * 0.5f; z += step)
            {
                CreateCube($"GridZ_{z}", new Vector3(0f, 0.001f, z), new Vector3(size, 0.002f, 0.02f), COL_GRID)
                    .transform.SetParent(_worldRoot);
            }
        }

        // ── Cube factory ────────────────────────────────────────────

        GameObject CreateCube(string name, Vector3 position, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;

            var rend = go.GetComponent<Renderer>();
            rend.material = MaterialHelper.Create(color);

            return go;
        }

        // ── Zone label ──────────────────────────────────────────────

        void CreateZoneLabel(string text, Vector3 position, Color color)
        {
            var go = new GameObject($"Label_{text}");
            go.transform.SetParent(_worldRoot);
            go.transform.position = position;
            go.transform.rotation = Quaternion.Euler(45f, 0f, 0f);

            var tmp = go.AddComponent<TextMeshPro>();
            tmp.text = text;
            tmp.fontSize = 3f;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.sortingOrder = 5;

            // Subtle floating animation
            go.transform.DOMoveY(position.y + 0.08f, 2f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }
}
