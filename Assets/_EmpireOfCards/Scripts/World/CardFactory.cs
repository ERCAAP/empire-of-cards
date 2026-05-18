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
        /// Called by WiringService instead of RuntimeWiring.SetField().
        /// </summary>
        public void Init(CardData[] cards)
        {
            this.allCards = cards;
        }

        // Card dimensions in world units
        private const float CARD_WIDTH = 0.6f;
        private const float CARD_HEIGHT = 0.85f;
        private const float CARD_THICKNESS = 0.03f;

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

            // Card name (top center)
            var nameText = CreateText(canvasGo.transform, "NameText",
                new Vector2(0, 100), new Vector2(180, 40), 28, FontStyles.Bold, Color.white);

            // Cost (top right)
            var costText = CreateText(canvasGo.transform, "CostText",
                new Vector2(70, 120), new Vector2(50, 30), 22, FontStyles.Bold, Color.yellow);

            // Description (center)
            var descText = CreateText(canvasGo.transform, "DescText",
                new Vector2(0, -10), new Vector2(170, 120), 16, FontStyles.Normal, new Color(0.9f, 0.9f, 0.9f));

            // Stats (bottom)
            var statsText = CreateText(canvasGo.transform, "StatsText",
                new Vector2(0, -110), new Vector2(170, 40), 14, FontStyles.Italic, new Color(0.7f, 0.85f, 1f));

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

            return card3D;
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
            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Ellipsis;
            return text;
        }
    }
}
