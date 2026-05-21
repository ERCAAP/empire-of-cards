using UnityEngine;
using TMPro;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.World
{
    public enum Card3DState
    {
        InHand,
        Dragging,
        OnBoard,
        Discarded
    }

    public class Card3D : MonoBehaviour
    {
        // ── Data ────────────────────────────────────────────────────
        CardData _data;
        Card3DState _state = Card3DState.InHand;

        // ── Visual refs ─────────────────────────────────────────────
        Renderer _bodyRenderer;
        Material _bodyMat;
        TextMeshPro _nameLabel;
        TextMeshPro _costLabel;
        TextMeshPro _descLabel;
        GameObject _glowOutline;

        // ── Card type colors ────────────────────────────────────────
        static readonly Color COL_STAFF      = new Color(0.20f, 0.70f, 0.30f); // green
        static readonly Color COL_OPERATION  = new Color(0.90f, 0.55f, 0.15f); // orange
        static readonly Color COL_MARKETING  = new Color(0.25f, 0.50f, 0.90f); // blue
        static readonly Color COL_SUPPLIER   = new Color(0.55f, 0.55f, 0.55f); // gray
        static readonly Color COL_RISK       = new Color(0.85f, 0.15f, 0.15f); // red
        static readonly Color COL_REACTION   = new Color(0.65f, 0.25f, 0.75f); // purple
        static readonly Color COL_CRISIS     = new Color(0.50f, 0.10f, 0.10f); // dark red

        // ── Properties ──────────────────────────────────────────────
        public CardData Data        => _data;
        public Card3DState State    => _state;

        // ── Build visuals ───────────────────────────────────────────

        public void BuildVisuals()
        {
            // Card body: thin flat cube
            transform.localScale = new Vector3(1.0f, 0.05f, 1.4f);

            _bodyRenderer = GetComponent<Renderer>();
            if (_bodyRenderer == null)
                _bodyRenderer = gameObject.AddComponent<MeshRenderer>();

            _bodyMat = new Material(Shader.Find("Standard"));
            _bodyMat.color = Color.white;
            _bodyRenderer.material = _bodyMat;

            // Name label (top of card face)
            _nameLabel = CreateLabel("NameLabel", new Vector3(0f, 0.6f, 0.35f), 0.08f);
            _nameLabel.alignment = TextAlignmentOptions.Center;
            _nameLabel.fontStyle = FontStyles.Bold;

            // Cost label (top-left corner)
            _costLabel = CreateLabel("CostLabel", new Vector3(-0.35f, 0.6f, 0.55f), 0.06f);
            _costLabel.alignment = TextAlignmentOptions.Left;
            _costLabel.color = Color.yellow;

            // Description label (center of card)
            _descLabel = CreateLabel("DescLabel", new Vector3(0f, 0.6f, -0.1f), 0.04f);
            _descLabel.alignment = TextAlignmentOptions.Center;
            _descLabel.enableWordWrapping = true;
            _descLabel.rectTransform.sizeDelta = new Vector2(0.9f, 0.6f);

            // Glow outline (slightly larger cube behind)
            _glowOutline = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _glowOutline.name = "GlowOutline";
            _glowOutline.transform.SetParent(transform);
            _glowOutline.transform.localPosition = Vector3.zero;
            _glowOutline.transform.localScale = new Vector3(1.08f, 1.2f, 1.05f);

            var glowMat = new Material(Shader.Find("Standard"));
            glowMat.EnableKeyword("_EMISSION");
            glowMat.SetColor("_EmissionColor", Color.yellow * 2f);
            glowMat.color = new Color(1f, 1f, 0.5f, 0.3f);
            _glowOutline.GetComponent<Renderer>().material = glowMat;

            // Destroy collider on glow so raycasts hit the card body
            var glowCollider = _glowOutline.GetComponent<Collider>();
            if (glowCollider != null) Object.Destroy(glowCollider);

            _glowOutline.SetActive(false);
        }

        // ── Set data ────────────────────────────────────────────────

        public void SetData(CardData data)
        {
            _data = data;
            if (data == null) return;

            // Update text
            if (_nameLabel != null) _nameLabel.text = data.cardName;
            if (_costLabel != null) _costLabel.text = data.buyCost > 0 ? $"${data.buyCost}" : "FREE";
            if (_descLabel != null) _descLabel.text = data.description ?? "";

            // Update body color by card type
            if (_bodyMat != null)
                _bodyMat.color = GetColorForType(data.cardType);

            gameObject.name = $"Card3D_{data.cardId}";
        }

        // ── State management ────────────────────────────────────────

        public void SetState(Card3DState newState)
        {
            _state = newState;
        }

        // ── Glow toggle ─────────────────────────────────────────────

        public void SetGlow(bool active)
        {
            if (_glowOutline != null)
                _glowOutline.SetActive(active);
        }

        // ── Color mapping ───────────────────────────────────────────

        static Color GetColorForType(CardType type)
        {
            switch (type)
            {
                case CardType.Staff:     return COL_STAFF;
                case CardType.Operation: return COL_OPERATION;
                case CardType.Marketing: return COL_MARKETING;
                case CardType.Supplier:  return COL_SUPPLIER;
                case CardType.Risk:      return COL_RISK;
                case CardType.Reaction:  return COL_REACTION;
                case CardType.Crisis:    return COL_CRISIS;
                default:                 return Color.white;
            }
        }

        // ── Label helper ────────────────────────────────────────────

        TextMeshPro CreateLabel(string labelName, Vector3 localPos, float fontSize)
        {
            var go = new GameObject(labelName);
            go.transform.SetParent(transform);
            go.transform.localPosition = localPos;
            go.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            go.transform.localScale = Vector3.one;

            var tmp = go.AddComponent<TextMeshPro>();
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            tmp.sortingOrder = 1;

            return tmp;
        }
    }
}
