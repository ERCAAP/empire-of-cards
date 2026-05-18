using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Data;
using EmpireOfCards.Core;

namespace EmpireOfCards.World
{
    public class CardFactory : MonoBehaviour
    {
        [SerializeField] private int cardLayer = 6; // "Card" layer index

        public Card3D CreateCard(CardData data)
        {
            // Root
            var go = new GameObject($"Card_{data.cardId}");
            go.layer = cardLayer;
            go.transform.localScale = new Vector3(0.6f, 0.02f, 0.85f); // Card shape: wide, thin, tall

            // Mesh (cube graybox)
            var meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = CreateQuadMesh();
            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            // Collider for raycasting
            var col = go.AddComponent<BoxCollider>();
            col.size = Vector3.one;

            // World-space canvas for text
            var canvasGo = new GameObject("CardCanvas");
            canvasGo.transform.SetParent(go.transform);
            canvasGo.transform.localPosition = new Vector3(0, 1f, 0);
            canvasGo.transform.localRotation = Quaternion.Euler(90, 0, 0);
            canvasGo.transform.localScale = new Vector3(1.5f, 1f, 1.2f);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var rt = canvas.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(200, 280);

            canvasGo.AddComponent<GraphicRaycaster>();

            // Card name
            var nameText = CreateWorldText(canvasGo.transform, "NameText",
                new Vector2(0, 110), new Vector2(180, 35), 22, FontStyles.Bold);

            // Cost
            var costText = CreateWorldText(canvasGo.transform, "CostText",
                new Vector2(75, 120), new Vector2(40, 30), 18, FontStyles.Bold);

            // Description
            var descText = CreateWorldText(canvasGo.transform, "DescText",
                new Vector2(0, 0), new Vector2(170, 100), 13, FontStyles.Normal);

            // Stats
            var statsText = CreateWorldText(canvasGo.transform, "StatsText",
                new Vector2(0, -100), new Vector2(170, 40), 12, FontStyles.Italic);

            // Glow outline (child cube, slightly bigger, transparent)
            var glow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            glow.name = "GlowOutline";
            glow.transform.SetParent(go.transform);
            glow.transform.localPosition = Vector3.zero;
            glow.transform.localScale = Vector3.one * 1.08f;
            var glowRenderer = glow.GetComponent<MeshRenderer>();
            var glowMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            glowMat.color = new Color(1f, 1f, 0f, 0.3f);
            // Make transparent
            glowMat.SetFloat("_Surface", 1); // Transparent
            glowMat.SetFloat("_Blend", 0);
            glowMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            glowMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            glowMat.SetInt("_ZWrite", 0);
            glowMat.renderQueue = 3000;
            glowRenderer.material = glowMat;
            Destroy(glow.GetComponent<Collider>()); // Don't interfere with raycast
            glow.SetActive(false);

            // Card3D component
            var card3D = go.AddComponent<Card3D>();
            card3D.Initialize(data, meshRenderer, nameText, costText, descText, statsText, glow);

            return card3D;
        }

        private TMP_Text CreateWorldText(Transform parent, string name,
            Vector2 pos, Vector2 size, int fontSize, FontStyles style)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rectT = go.AddComponent<RectTransform>();
            rectT.anchoredPosition = pos;
            rectT.sizeDelta = size;
            var text = go.AddComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Truncate;
            return text;
        }

        private Mesh CreateQuadMesh()
        {
            // Just use Unity's built-in cube mesh
            var tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var mesh = tempCube.GetComponent<MeshFilter>().mesh;
            Destroy(tempCube);
            return mesh;
        }
    }
}
