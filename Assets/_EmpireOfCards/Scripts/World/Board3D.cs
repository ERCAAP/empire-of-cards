using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.World
{
    public class Board3D : MonoBehaviour
    {
        // ── Zone parents ────────────────────────────────────────────
        Transform _kitchenZone;
        Transform _salonZone;
        Transform _storageZone;
        Transform _marketingZone;
        Transform _eventsZone;
        Transform _streetZone;
        Transform _rivalZone;
        Transform _handZone;

        // ── Slot markers per zone ───────────────────────────────────
        readonly Dictionary<SlotType, List<SlotZone3D>> _slotMarkers = new();

        // ── Zone colors ─────────────────────────────────────────────
        static readonly Color COL_KITCHEN   = new Color(0.85f, 0.45f, 0.15f); // warm orange
        static readonly Color COL_SALON     = new Color(0.76f, 0.60f, 0.42f); // light brown
        static readonly Color COL_STORAGE   = new Color(0.55f, 0.55f, 0.55f); // gray
        static readonly Color COL_MARKETING = new Color(0.25f, 0.50f, 0.85f); // blue
        static readonly Color COL_EVENTS    = new Color(0.60f, 0.15f, 0.15f); // dark red
        static readonly Color COL_STREET    = new Color(0.80f, 0.80f, 0.78f); // light gray
        static readonly Color COL_RIVAL     = new Color(0.20f, 0.20f, 0.25f, 0.6f); // dark semi-transparent
        static readonly Color COL_HAND      = new Color(0.30f, 0.30f, 0.35f); // dark gray

        // ── Slot layout per zone (x start, z position, max slots) ───
        struct ZoneLayout
        {
            public float xStart;
            public float z;
            public float width;
            public float depth;
            public int initialSlots;
        }

        // ── Public API ──────────────────────────────────────────────

        public Transform StreetZone => _streetZone;
        public Transform HandZone   => _handZone;

        public void BuildBoard()
        {
            ClearBoard();

            // Mutfak: z=0, left side
            _kitchenZone = CreateZoneFloor("Mutfak", new Vector3(-4f, 0f, 0f), 3.5f, 2.5f, COL_KITCHEN);
            CreateSlotMarkers(SlotType.Kitchen, _kitchenZone, new Vector3(-5.25f, 0.05f, 0f), 5, 1.1f);

            // Salon: z=0, center
            _salonZone = CreateZoneFloor("Salon", new Vector3(0.5f, 0f, 0f), 4.5f, 2.5f, COL_SALON);
            CreateSlotMarkers(SlotType.Salon, _salonZone, new Vector3(-1.75f, 0.05f, 0f), 7, 1.1f);

            // Depo: z=0, right side
            _storageZone = CreateZoneFloor("Depo", new Vector3(5f, 0f, 0f), 2.5f, 2.5f, COL_STORAGE);
            CreateSlotMarkers(SlotType.Storage, _storageZone, new Vector3(4f, 0.05f, 0f), 3, 1.1f);

            // Marketing: z=-2, left side
            _marketingZone = CreateZoneFloor("Marketing", new Vector3(-3f, 0f, -3f), 3f, 2f, COL_MARKETING);
            CreateSlotMarkers(SlotType.Marketing, _marketingZone, new Vector3(-4f, 0.05f, -3f), 3, 1.1f);

            // Olaylar (TempEffect): z=-2, right side
            _eventsZone = CreateZoneFloor("Olaylar", new Vector3(1.5f, 0f, -3f), 3f, 2f, COL_EVENTS);
            CreateSlotMarkers(SlotType.TempEffect, _eventsZone, new Vector3(0.5f, 0.05f, -3f), 3, 1.1f);

            // Sokak (street / customer walking area): z=6
            _streetZone = CreateZoneFloor("Sokak", new Vector3(0f, 0f, 4f), 14f, 1.5f, COL_STREET);

            // Rakip zone: z=4, behind the street
            _rivalZone = CreateZoneFloor("Rakip", new Vector3(0f, 0f, 6f), 10f, 1.5f, COL_RIVAL);

            // Hand zone: z=-5
            _handZone = CreateZoneFloor("El", new Vector3(0f, 0f, -6f), 10f, 2f, COL_HAND);

            Debug.Log("[Board3D] Board built with all zones and slot markers.");
        }

        // ── EventBus subscriptions ──────────────────────────────────

        void OnEnable()
        {
            EventBus.OnSlotUnlocked += HandleSlotUnlocked;
        }

        void OnDisable()
        {
            EventBus.OnSlotUnlocked -= HandleSlotUnlocked;
        }

        void HandleSlotUnlocked(SlotType slotType, int newMax)
        {
            if (!_slotMarkers.ContainsKey(slotType)) return;

            var existing = _slotMarkers[slotType];
            if (existing.Count >= newMax) return;

            int toAdd = newMax - existing.Count;
            for (int i = 0; i < toAdd; i++)
            {
                int idx = existing.Count;
                Vector3 pos = GetSlotPosition(slotType, idx);
                var marker = CreateSingleSlotMarker(slotType, idx, pos);
                existing.Add(marker);
            }

            Debug.Log($"[Board3D] Unlocked {toAdd} new slot(s) for {slotType}. Total: {existing.Count}");
        }

        // ── Zone floor creation ─────────────────────────────────────

        Transform CreateZoneFloor(string zoneName, Vector3 center, float width, float depth, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = $"Zone_{zoneName}";
            go.transform.SetParent(transform);
            go.transform.localPosition = center;
            go.transform.localScale = new Vector3(width, 0.05f, depth);

            var rend = go.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            if (color.a < 1f)
            {
                mat.SetFloat("_Mode", 3); // Transparent
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
            rend.material = mat;

            return go.transform;
        }

        // ── Slot marker creation ────────────────────────────────────

        void CreateSlotMarkers(SlotType slotType, Transform parent, Vector3 startPos, int maxSlots, float spacing)
        {
            if (!_slotMarkers.ContainsKey(slotType))
                _slotMarkers[slotType] = new List<SlotZone3D>();

            for (int i = 0; i < maxSlots; i++)
            {
                Vector3 pos = startPos + Vector3.right * (i * spacing);
                var marker = CreateSingleSlotMarker(slotType, i, pos);
                _slotMarkers[slotType].Add(marker);
            }
        }

        SlotZone3D CreateSingleSlotMarker(SlotType slotType, int index, Vector3 position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = $"Slot_{slotType}_{index}";
            go.transform.SetParent(transform);
            go.transform.localPosition = position;

            var slot = go.AddComponent<SlotZone3D>();
            slot.RuntimeInit(slotType, index);

            return slot;
        }

        Vector3 GetSlotPosition(SlotType slotType, int index)
        {
            float spacing = 1.1f;
            switch (slotType)
            {
                case SlotType.Kitchen:    return new Vector3(-5.25f + index * spacing, 0.05f, 0f);
                case SlotType.Salon:      return new Vector3(-1.75f + index * spacing, 0.05f, 0f);
                case SlotType.Storage:    return new Vector3(4f + index * spacing, 0.05f, 0f);
                case SlotType.Marketing:  return new Vector3(-4f + index * spacing, 0.05f, -3f);
                case SlotType.TempEffect: return new Vector3(0.5f + index * spacing, 0.05f, -3f);
                default:                  return Vector3.zero;
            }
        }

        // ── Queries ─────────────────────────────────────────────────

        public SlotZone3D GetSlotMarker(SlotType slotType, int index)
        {
            if (_slotMarkers.TryGetValue(slotType, out var list) && index < list.Count)
                return list[index];
            return null;
        }

        public SlotZone3D FindNearestEmptySlot(SlotType slotType, Vector3 worldPos)
        {
            if (!_slotMarkers.TryGetValue(slotType, out var list)) return null;

            SlotZone3D nearest = null;
            float bestDist = float.MaxValue;

            foreach (var slot in list)
            {
                if (slot.IsOccupied) continue;
                float dist = Vector3.Distance(slot.transform.position, worldPos);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest = slot;
                }
            }

            return nearest;
        }

        // ── Cleanup ─────────────────────────────────────────────────

        void ClearBoard()
        {
            foreach (var kvp in _slotMarkers)
                kvp.Value.Clear();
            _slotMarkers.Clear();

            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
        }
    }
}
