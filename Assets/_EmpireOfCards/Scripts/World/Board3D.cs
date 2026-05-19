using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;
using EmpireOfCards.UI.Cards;

namespace EmpireOfCards.World
{
    public class Board3D : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManager;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService during bootstrap.
        /// </summary>
        public void Init(BoardManager board)
        {
            this.boardManager = board;
        }

        /// <summary>
        /// Shows or hides business slots based on max slot count.
        /// Called when player unlocks extra business slots.
        /// </summary>
        public void UpdateVisibleSlots(int maxSlots)
        {
            for (int i = 0; i < _businessSlots.Count; i++)
            {
                _businessSlots[i].gameObject.SetActive(i < maxSlots);
            }
        }

        // Created at runtime
        private TextMeshPro _tierLabel;
        private readonly List<SlotZone3D> _businessSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _employeeSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _upgradeSlots = new List<SlotZone3D>();
        private readonly List<GameObject> _territoryBlocks = new List<GameObject>();
        private readonly List<MeshRenderer> _territoryRenderers = new List<MeshRenderer>();
        private readonly List<MeshRenderer> _businessSlotRenderers = new List<MeshRenderer>();
        private SlotZone3D _sellZone;
        private SlotZone3D _actionZone;

        // Employee marker cubes — keyed by "bizIndex_empSlotIndex"
        private readonly Dictionary<string, GameObject> _employeeMarkers = new Dictionary<string, GameObject>();

        public IReadOnlyList<SlotZone3D> BusinessSlots => _businessSlots;

        public void BuildBoard()
        {
            // === TABLE (large flat cube) ===
            var table = GameObject.CreatePrimitive(PrimitiveType.Cube);
            table.name = "Table";
            table.transform.SetParent(transform);
            table.transform.localPosition = new Vector3(0, -0.25f, 0);
            table.transform.localScale = new Vector3(16, 0.5f, 12);
            table.GetComponent<MeshRenderer>().material.color = new Color(0.35f, 0.22f, 0.12f); // Wood
            table.layer = LayerMask.NameToLayer("Default");
            Destroy(table.GetComponent<Collider>()); // Table doesn't need raycast

            // === TERRITORY MAP (10 cubes, top of board) ===
            for (int i = 0; i < 10; i++)
            {
                var terr = GameObject.CreatePrimitive(PrimitiveType.Cube);
                terr.name = $"Territory_{i + 1:D2}";
                terr.transform.SetParent(transform);
                float x = (i - 4.5f) * 1.2f;
                terr.transform.localPosition = new Vector3(x, 0.1f, 4.5f);
                terr.transform.localScale = new Vector3(1.0f, 0.25f, 1.0f);
                terr.GetComponent<MeshRenderer>().material.color = Color.gray;
                Destroy(terr.GetComponent<Collider>());
                _territoryBlocks.Add(terr);
                _territoryRenderers.Add(terr.GetComponent<MeshRenderer>());
            }

            // === PLAYER BUSINESS SLOTS (5 cubes, center-bottom) ===
            for (int i = 0; i < 5; i++)
            {
                var slot = GameObject.CreatePrimitive(PrimitiveType.Cube);
                slot.name = $"BusinessSlot_{i + 1:D2}";
                slot.transform.SetParent(transform);
                float x = (i - 2) * 2.5f;
                slot.transform.localPosition = new Vector3(x, 0.05f, -1.5f);
                slot.transform.localScale = new Vector3(1.8f, 0.1f, 2.5f);
                slot.GetComponent<MeshRenderer>().material.color = new Color(0.25f, 0.25f, 0.3f);

                _businessSlotRenderers.Add(slot.GetComponent<MeshRenderer>());

                var zone = slot.AddComponent<SlotZone3D>();
                zone.RuntimeInit(DropZoneType.BusinessSlot, i);
                _businessSlots.Add(zone);

                // Employee sub-slots (3 per business)
                for (int e = 0; e < 3; e++)
                {
                    var empSlot = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    empSlot.name = $"EmpSlot_{e + 1:D2}";
                    empSlot.transform.SetParent(slot.transform);
                    empSlot.transform.localPosition = new Vector3((e - 1) * 0.38f, 1f, -0.45f);
                    empSlot.transform.localScale = new Vector3(0.45f, 0.5f, 0.3f);
                    empSlot.GetComponent<MeshRenderer>().material.color = new Color(0.2f, 0.35f, 0.2f);

                    var empZone = empSlot.AddComponent<SlotZone3D>();
                    empZone.RuntimeInit(DropZoneType.EmployeeSlot, e, i);
                    _employeeSlots.Add(empZone);
                }

                // Hide slots 4-5 initially
                if (i >= 3) slot.SetActive(false);
            }

            // === RIVAL AREA (3 slots, top) ===
            for (int i = 0; i < 3; i++)
            {
                var rivalSlot = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rivalSlot.name = $"RivalSlot_{i + 1:D2}";
                rivalSlot.transform.SetParent(transform);
                float x = (i - 1) * 2.8f;
                rivalSlot.transform.localPosition = new Vector3(x, 0.05f, 2.5f);
                rivalSlot.transform.localScale = new Vector3(1.5f, 0.1f, 1.2f);
                rivalSlot.GetComponent<MeshRenderer>().material.color = new Color(0.5f, 0.15f, 0.15f);
                Destroy(rivalSlot.GetComponent<Collider>()); // Read-only
            }

            // === UPGRADE AREA (left side) ===
            for (int i = 0; i < 4; i++)
            {
                var upSlot = GameObject.CreatePrimitive(PrimitiveType.Cube);
                upSlot.name = $"UpgradeSlot_{i + 1:D2}";
                upSlot.transform.SetParent(transform);
                upSlot.transform.localPosition = new Vector3(-6.5f, 0.05f, -2f + i * 1.3f);
                upSlot.transform.localScale = new Vector3(1.2f, 0.1f, 1f);
                upSlot.GetComponent<MeshRenderer>().material.color = new Color(0.35f, 0.2f, 0.45f);

                var upZone = upSlot.AddComponent<SlotZone3D>();
                upZone.RuntimeInit(DropZoneType.UpgradeSlot, i);
                _upgradeSlots.Add(upZone);
            }

            // === SELL ZONE (right side) ===
            var sell = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sell.name = "SellZone";
            sell.transform.SetParent(transform);
            sell.transform.localPosition = new Vector3(6.5f, 0.05f, -1.5f);
            sell.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f);
            sell.GetComponent<MeshRenderer>().material.color = new Color(0.6f, 0.4f, 0.1f);
            _sellZone = sell.AddComponent<SlotZone3D>();
            _sellZone.RuntimeInit(DropZoneType.SellZone, 0);

            // === ACTION ZONE (center, for action cards) ===
            var action = GameObject.CreatePrimitive(PrimitiveType.Cube);
            action.name = "ActionZone";
            action.transform.SetParent(transform);
            action.transform.localPosition = new Vector3(6.5f, 0.05f, 0.5f);
            action.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f);
            action.GetComponent<MeshRenderer>().material.color = new Color(0.6f, 0.15f, 0.15f);
            _actionZone = action.AddComponent<SlotZone3D>();
            _actionZone.RuntimeInit(DropZoneType.ActionZone, 0);

            // === EVENT DISPLAY (center-top) ===
            var eventArea = GameObject.CreatePrimitive(PrimitiveType.Cube);
            eventArea.name = "EventDisplay";
            eventArea.transform.SetParent(transform);
            eventArea.transform.localPosition = new Vector3(3.5f, 0.15f, 3.5f);
            eventArea.transform.localScale = new Vector3(1.2f, 0.02f, 1.7f);
            eventArea.GetComponent<MeshRenderer>().material.color = new Color(0.9f, 0.8f, 0.15f, 0.3f);
            Destroy(eventArea.GetComponent<Collider>());

            // === VISUAL DIVIDERS ===
            // Divider between player zone and territory map
            var div1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            div1.name = "Divider_PlayerTerritory";
            div1.transform.SetParent(transform);
            div1.transform.localPosition = new Vector3(0, 0.06f, 1.5f);
            div1.transform.localScale = new Vector3(14f, 0.02f, 0.05f);
            div1.GetComponent<MeshRenderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            Destroy(div1.GetComponent<Collider>());

            // Divider between territory map and rival zone
            var div2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            div2.name = "Divider_TerritoryRival";
            div2.transform.SetParent(transform);
            div2.transform.localPosition = new Vector3(0, 0.06f, 3.5f);
            div2.transform.localScale = new Vector3(14f, 0.02f, 0.05f);
            div2.GetComponent<MeshRenderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            Destroy(div2.GetComponent<Collider>());

            // === FLOATING BOARD LABELS ===
            CreateBoardLabels();
        }

        // Called by TerritoryManager to update visuals
        public void UpdateTerritoryVisuals(int playerCount, int rivalCount)
        {
            for (int i = 0; i < _territoryRenderers.Count; i++)
            {
                if (i < playerCount)
                    _territoryRenderers[i].material.color = new Color(0.2f, 0.5f, 1f); // Player blue
                else if (i < playerCount + rivalCount)
                    _territoryRenderers[i].material.color = new Color(0.9f, 0.2f, 0.2f); // Rival red
                else
                    _territoryRenderers[i].material.color = Color.gray; // Empty
            }
        }

        // ================================================================
        //  Board Labels -- floating 3D TextMeshPro above each area
        // ================================================================

        /// <summary>
        /// Creates floating world-space text labels above the key board zones
        /// so first-time players can identify what each area is.
        /// </summary>
        private void CreateBoardLabels()
        {
            // Label positions are relative to the board root.
            // Y is raised above the board surface; text faces the camera (flat on XZ, rotated to face -Z).
            float labelY = 0.6f;
            float fontSize = 4f;
            Quaternion labelRot = Quaternion.Euler(55f, 0f, 0f); // Match camera angle

            // COMPANY TIER -- top-left corner
            _tierLabel = CreateLabel("ESNAF", new Vector3(-6f, labelY, 4.2f), labelRot, fontSize * 0.9f,
                        new Color(0.9f, 0.75f, 0.3f));

            // YOUR BUSINESSES -- above player business slots (center-bottom)
            CreateLabel("YOUR BUSINESSES", new Vector3(0f, labelY, -0.2f), labelRot, fontSize,
                        new Color(0.5f, 0.7f, 1f));

            // RIVAL -- above rival area (center-top)
            CreateLabel("RIVAL", new Vector3(0f, labelY, 3.2f), labelRot, fontSize,
                        new Color(1f, 0.4f, 0.4f));

            // TERRITORIES -- above territory bar (very top)
            CreateLabel("TERRITORIES", new Vector3(0f, labelY, 5.2f), labelRot, fontSize,
                        new Color(0.9f, 0.9f, 0.9f));

            // UPGRADES -- above upgrade area (left side)
            CreateLabel("UPGRADES", new Vector3(-6.5f, labelY, 1f), labelRot, fontSize * 0.85f,
                        new Color(0.7f, 0.45f, 0.9f));

            // SELL -- above sell zone (right side)
            CreateLabel("SELL", new Vector3(6.5f, labelY, -1f), labelRot, fontSize * 0.85f,
                        new Color(0.9f, 0.7f, 0.2f));
        }

        /// <summary>
        /// Creates a single floating 3D TextMeshPro label as a child of the board.
        /// </summary>
        private TextMeshPro CreateLabel(string text, Vector3 localPos, Quaternion localRot, float fontSize, Color color)
        {
            var go = new GameObject($"Label_{text.Replace(" ", "")}");
            go.transform.SetParent(transform);
            go.transform.localPosition = localPos;
            go.transform.localRotation = localRot;
            go.transform.localScale = Vector3.one;

            var tmp = go.AddComponent<TextMeshPro>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = color;
            tmp.enableWordWrapping = false;

            // Make sure the label is visible but doesn't interfere with raycasts
            var rt = go.GetComponent<RectTransform>();
            if (rt != null)
                rt.sizeDelta = new Vector2(6f, 1.5f);

            return tmp;
        }

        // ================================================================
        //  Tier Change -- update the board label when company tier changes
        // ================================================================

        private static readonly string[] TierNames = { "ESNAF", "GİRİŞİMCİ", "ŞİRKET", "HOLDİNG" };
        private static readonly Color[] TierLabelColors = {
            new Color(0.9f, 0.75f, 0.3f),
            new Color(0.3f, 0.8f, 0.4f),
            new Color(0.3f, 0.6f, 1f),
            new Color(1f, 0.8f, 0.2f)
        };

        private void OnEnable()
        {
            EventBus.OnCompanyTierChanged += HandleTierChanged;
            EventBus.OnBusinessNeglected += HandleBusinessNeglected;
            EventBus.OnEmployeePlaced += HandleSlotRefresh;
            EventBus.OnUpgradePlaced += HandleSlotRefresh;
            EventBus.OnEmployeeLeft += HandleEmployeeLeft;
            EventBus.OnTerritoryChanged += HandleTerritoryChanged;
            EventBus.OnBusinessSlotsChanged += UpdateVisibleSlots;
        }

        private void OnDisable()
        {
            EventBus.OnCompanyTierChanged -= HandleTierChanged;
            EventBus.OnBusinessNeglected -= HandleBusinessNeglected;
            EventBus.OnEmployeePlaced -= HandleSlotRefresh;
            EventBus.OnUpgradePlaced -= HandleSlotRefresh;
            EventBus.OnEmployeeLeft -= HandleEmployeeLeft;
            EventBus.OnTerritoryChanged -= HandleTerritoryChanged;
            EventBus.OnBusinessSlotsChanged -= UpdateVisibleSlots;
        }

        private void HandleTerritoryChanged(int playerCount, int rivalCount)
        {
            UpdateTerritoryVisuals(playerCount, rivalCount);
        }

        private void HandleEmployeeLeft(CardData card, int businessIndex)
        {
            // Remove all employee markers for this business
            var keysToRemove = new List<string>();
            foreach (var kvp in _employeeMarkers)
            {
                if (kvp.Key.StartsWith($"{businessIndex}_"))
                {
                    if (kvp.Value != null) Destroy(kvp.Value);
                    keysToRemove.Add(kvp.Key);
                }
            }
            foreach (var key in keysToRemove)
                _employeeMarkers.Remove(key);
        }

        private void HandleTierChanged(CompanyTier newTier)
        {
            if (_tierLabel == null) return;
            int idx = (int)newTier;
            _tierLabel.text = TierNames[idx];
            _tierLabel.color = TierLabelColors[idx];
        }

        // ================================================================
        //  Business Neglect -- darken slot when business is neglected
        // ================================================================

        private void HandleBusinessNeglected(int businessIndex, int neglectTurns)
        {
            if (businessIndex < 0 || businessIndex >= _businessSlotRenderers.Count) return;

            var renderer = _businessSlotRenderers[businessIndex];
            if (renderer == null) return;

            // Darken the business slot based on neglect level
            if (neglectTurns >= 6) // Major neglect
                renderer.material.color = new Color(0.15f, 0.1f, 0.1f); // Dark red tint
            else if (neglectTurns >= 4) // Minor neglect
                renderer.material.color = new Color(0.2f, 0.18f, 0.15f); // Slightly darker
        }

        private void HandleSlotRefresh(CardData card, int businessIndex)
        {
            if (businessIndex < 0 || businessIndex >= _businessSlotRenderers.Count) return;
            var renderer = _businessSlotRenderers[businessIndex];
            if (renderer != null)
                renderer.material.color = new Color(0.25f, 0.25f, 0.3f); // Original color

            // Spawn a small marker cube for employee placements
            if (card != null && card.cardType == CardType.Employee)
                SpawnEmployeeMarker(card, businessIndex);
        }

        // ================================================================
        //  Employee Markers -- small cubes indicating workers on a slot
        // ================================================================

        /// <summary>
        /// Spawns a small colored cube above an employee sub-slot to indicate
        /// a worker is stationed there. Placeholder visual -- proper models later.
        /// </summary>
        private void SpawnEmployeeMarker(CardData card, int businessIndex)
        {
            if (businessIndex < 0 || businessIndex >= _businessSlots.Count) return;

            // Find the first unoccupied employee sub-slot index for this business
            int empIndex = -1;
            for (int i = 0; i < _employeeSlots.Count; i++)
            {
                if (_employeeSlots[i].ParentBusinessIndex == businessIndex && _employeeSlots[i].IsOccupied)
                {
                    // Use the most recently occupied one (the one we just placed)
                    empIndex = i;
                }
            }

            if (empIndex < 0) return;

            string key = $"{businessIndex}_{empIndex}";
            // Don't double-spawn
            if (_employeeMarkers.ContainsKey(key)) return;

            var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = $"EmpMarker_{key}";
            marker.transform.SetParent(_employeeSlots[empIndex].transform);
            marker.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            marker.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            // Use employee green tint, slightly varied by card
            Color markerColor = Card3D.GetCardTypeColor(CardType.Employee);
            markerColor *= 1.2f; // Brighter than the card
            markerColor.a = 1f;
            marker.GetComponent<MeshRenderer>().material.color = markerColor;

            // Remove collider -- marker is decorative only
            Destroy(marker.GetComponent<Collider>());

            _employeeMarkers[key] = marker;
        }
    }
}
