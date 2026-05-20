using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Data;
using EmpireOfCards.Core;

namespace EmpireOfCards.World
{
    public class CardFactory : MonoBehaviour
    {
        [SerializeField] private CardData[] allCards;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService during bootstrap.
        /// </summary>
        public void Init(CardData[] cards)
        {
            this.allCards = cards;
        }

        // Card dimensions in world units
        private const float CARD_WIDTH = 0.6f;
        private const float CARD_HEIGHT = 0.85f;
        private const float CARD_THICKNESS = 0.03f;

        // Card text styling
        private static readonly Color GoldColor = new Color(1f, 0.85f, 0.2f);

        // Venture-type tint colors applied as a subtle overlay on the card body
        private static readonly Color FastFoodTint = new Color(0.85f, 0.35f, 0.15f);
        private static readonly Color CafeTint = new Color(0.55f, 0.35f, 0.2f);
        private static readonly Color TechAppTint = new Color(0.2f, 0.45f, 0.85f);
        private static readonly Color ClothingStoreTint = new Color(0.8f, 0.35f, 0.6f);
        private static readonly Color GroceryStoreTint = new Color(0.25f, 0.65f, 0.3f);
        private static readonly Color GeneralTint = new Color(0.5f, 0.5f, 0.5f);

        public Card3D CreateCard(CardData data)
        {
            // Root - UNIFORM scale (1,1,1) so children aren't distorted
            var go = new GameObject($"Card_{data.cardId}");
            // Use Default layer (0) -- raycasting uses component checks, not layers

            // Card body as child with non-uniform scale
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "CardBody";
            body.transform.SetParent(go.transform);
            body.transform.localPosition = Vector3.zero;
            body.transform.localScale = new Vector3(CARD_WIDTH, CARD_THICKNESS, CARD_HEIGHT);

            var meshRenderer = body.GetComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            // Collider on root for raycasting (box matching card size)
            var col = go.AddComponent<BoxCollider>();
            col.size = new Vector3(CARD_WIDTH, CARD_THICKNESS * 2f, CARD_HEIGHT);
            col.center = Vector3.zero;

            // Remove collider from body primitive (we use root collider)
            Destroy(body.GetComponent<Collider>());

            // World-space canvas - sits on TOP of the card facing UP
            var canvasGo = new GameObject("CardCanvas");
            canvasGo.transform.SetParent(go.transform); // parent to ROOT (uniform scale)
            canvasGo.transform.localPosition = new Vector3(0, CARD_THICKNESS + 0.001f, 0);
            canvasGo.transform.localRotation = Quaternion.Euler(90, 0, 0); // face up

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var canvasRT = canvas.GetComponent<RectTransform>();
            canvasRT.sizeDelta = new Vector2(200, 280);
            // Scale canvas so 200px = CARD_WIDTH world units
            float pixelToWorld = CARD_WIDTH / 200f; // 0.003
            canvasGo.transform.localScale = new Vector3(pixelToWorld, pixelToWorld, pixelToWorld);

            // Card name (top center) - auto-sized to prevent truncation
            var nameText = CreateText(canvasGo.transform, "NameText",
                new Vector2(0, 100), new Vector2(180, 45), 28, FontStyles.Bold, Color.white);
            nameText.enableAutoSizing = true;
            nameText.fontSizeMin = 14;
            nameText.fontSizeMax = 28;
            nameText.textWrappingMode = TextWrappingModes.Normal;

            // Cost (top right) - gold color for visibility
            var costText = CreateText(canvasGo.transform, "CostText",
                new Vector2(70, 120), new Vector2(55, 32), 24, FontStyles.Bold, GoldColor);

            // Description (center) - larger and more readable
            var descText = CreateText(canvasGo.transform, "DescText",
                new Vector2(0, -10), new Vector2(170, 120), 18, FontStyles.Normal, new Color(0.92f, 0.92f, 0.92f, 0.95f));
            descText.enableAutoSizing = true;
            descText.fontSizeMin = 12;
            descText.fontSizeMax = 18;

            // Stats (bottom) - player-friendly summary
            var statsText = CreateText(canvasGo.transform, "StatsText",
                new Vector2(0, -110), new Vector2(170, 40), 13, FontStyles.Normal, new Color(0.75f, 0.9f, 1f));
            statsText.enableAutoSizing = true;
            statsText.fontSizeMin = 10;
            statsText.fontSizeMax = 14;

            // Glow outline (slightly larger card body, transparent)
            var glow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            glow.name = "GlowOutline";
            glow.transform.SetParent(go.transform);
            glow.transform.localPosition = Vector3.zero;
            glow.transform.localScale = new Vector3(CARD_WIDTH * 1.1f, CARD_THICKNESS * 3f, CARD_HEIGHT * 1.1f);
            var glowRenderer = glow.GetComponent<MeshRenderer>();
            var glowMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            glowMat.color = new Color(1f, 1f, 0f, 0.3f);
            glowMat.SetFloat("_Surface", 1);
            glowMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            glowMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            glowMat.SetInt("_ZWrite", 0);
            glowMat.renderQueue = 3000;
            glowRenderer.material = glowMat;
            Destroy(glow.GetComponent<Collider>());
            glow.SetActive(false);

            // Card3D component on root
            var card3D = go.AddComponent<Card3D>();
            card3D.Initialize(data, meshRenderer, nameText, costText, descText, statsText, glow);

            // Apply venture-type tint as a subtle color blend on the card body
            ApplyVentureTint(meshRenderer, data);

            return card3D;
        }

        private static void ApplyVentureTint(MeshRenderer renderer, CardData data)
        {
            if (renderer == null || data == null) return;

            // Only tint cards that belong to a specific venture (not general cards)
            if (data.isGeneralCard) return;

            Color ventureTint = GetVentureTint(data.ventureType);
            // Blend: 70% original card-type color + 30% venture tint
            Color baseColor = renderer.material.color;
            renderer.material.color = Color.Lerp(baseColor, ventureTint, 0.3f);
        }

        private static Color GetVentureTint(VentureType venture)
        {
            return venture switch
            {
                VentureType.FastFood      => FastFoodTint,
                VentureType.Cafe          => CafeTint,
                VentureType.TechApp       => TechAppTint,
                VentureType.ClothingStore => ClothingStoreTint,
                VentureType.GroceryStore  => GroceryStoreTint,
                _                         => GeneralTint
            };
        }

        private TMP_Text CreateText(Transform parent, string name,
            Vector2 pos, Vector2 size, int fontSize, FontStyles style, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            var text = go.AddComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.color = color;
            text.alignment = TextAlignmentOptions.Center;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Ellipsis;
            return text;
        }
    }
}
