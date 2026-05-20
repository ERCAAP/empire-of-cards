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

        // Slot System v2 — player zone slots
        private readonly List<SlotZone3D> _operationSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _staffSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _marketingSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _supplierSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _tempEffectSlots = new List<SlotZone3D>();

        // Legacy lists (kept for backward-compat with UpdateVisibleSlots / employee markers)
        private readonly List<SlotZone3D> _businessSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _employeeSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _upgradeSlots = new List<SlotZone3D>();
        private readonly List<MeshRenderer> _businessSlotRenderers = new List<MeshRenderer>();

        // Market zone
        private readonly List<GameObject> _territoryBlocks = new List<GameObject>();
        private readonly List<MeshRenderer> _territoryRenderers = new List<MeshRenderer>();

        // Utility zones
        private SlotZone3D _sellZone;
        private SlotZone3D _actionZone;

        // Employee marker cubes — keyed by "bizIndex_empSlotIndex"
        private readonly Dictionary<string, GameObject> _employeeMarkers = new Dictionary<string, GameObject>();

        public IReadOnlyList<SlotZone3D> BusinessSlots => _operationSlots; // Operation = main business slots

        public void BuildBoard()
        {
            // ====================================================================
            // TABLE — compact size to fit within camera FOV 50, pos (0,14,-6) rot 55°
            // ====================================================================
            var table = GameObject.CreatePrimitive(PrimitiveType.Cube);
            table.name = "Table";
            table.transform.SetParent(transform);
            table.transform.localPosition = new Vector3(0, -0.25f, 1f);
            table.transform.localScale = new Vector3(14, 0.5f, 10);
            table.GetComponent<MeshRenderer>().material.color = new Color(0.35f, 0.22f, 0.12f);
            table.layer = LayerMask.NameToLayer("Default");
            Destroy(table.GetComponent<Collider>());

            // ====================================================================
            // ZONE 1 — PLAYER ZONE
            // Row A (Z = -2.0): Operation slots — business infrastructure
            // Row B (Z = -0.5): Staff slots — employees
            // Row C (Z = 1.0): Marketing (left) + Supplier (right)
            // Right column (X = 6.2): TempEffect slots + Sell + Action
            // ====================================================================

            // Row A: Operation Slots (4 starting) — spacing 2.2 → total 6.6 wide
            for (int i = 0; i < Constants.STARTING_OPERATION_SLOTS; i++)
            {
                var slot = CreateSlotCube($"OperationSlot_{i + 1:D2}",
                    new Vector3((i - 1.5f) * 2.2f, 0.05f, -2.0f),
                    new Vector3(2.0f, 0.1f, 1.5f),
                    new Color(0.2f, 0.3f, 0.5f));

                var zone = slot.AddComponent<SlotZone3D>();
                zone.RuntimeInit(DropZoneType.OperationSlot, i);
                _operationSlots.Add(zone);
                _businessSlots.Add(zone);
                _businessSlotRenderers.Add(slot.GetComponent<MeshRenderer>());
            }

            // Row B: Staff Slots (5 starting) — spacing 1.8 → total 7.2 wide
            for (int i = 0; i < Constants.STARTING_STAFF_SLOTS; i++)
            {
                var slot = CreateSlotCube($"StaffSlot_{i + 1:D2}",
                    new Vector3((i - 2f) * 1.8f, 0.05f, -0.5f),
                    new Vector3(1.5f, 0.1f, 1.2f),
                    new Color(0.2f, 0.45f, 0.25f));

                var zone = slot.AddComponent<SlotZone3D>();
                zone.RuntimeInit(DropZoneType.StaffSlot, i);
                _staffSlots.Add(zone);
                _employeeSlots.Add(zone);
            }

            // Row C-Left: Marketing Slots (3 starting) — spacing 1.8, left cluster
            for (int i = 0; i < Constants.STARTING_MARKETING_SLOTS; i++)
            {
                var slot = CreateSlotCube($"MarketingSlot_{i + 1:D2}",
                    new Vector3(-4.5f + i * 1.8f, 0.05f, 1.0f),
                    new Vector3(1.5f, 0.1f, 1.0f),
                    new Color(0.5f, 0.25f, 0.5f));

                var zone = slot.AddComponent<SlotZone3D>();
                zone.RuntimeInit(DropZoneType.MarketingSlot, i);
                _marketingSlots.Add(zone);
                _upgradeSlots.Add(zone);
            }

            // Row C-Right: Supplier Slots (2 starting) — spacing 1.8, right of center
            for (int i = 0; i < Constants.STARTING_SUPPLIER_SLOTS; i++)
            {
                var slot = CreateSlotCube($"SupplierSlot_{i + 1:D2}",
                    new Vector3(2.2f + i * 1.8f, 0.05f, 1.0f),
                    new Vector3(1.5f, 0.1f, 1.0f),
                    new Color(0.45f, 0.35f, 0.15f));

                var zone = slot.AddComponent<SlotZone3D>();
                zone.RuntimeInit(DropZoneType.SupplierSlot, i);
                _supplierSlots.Add(zone);
            }

            // Right column: TempEffect Slots (3 fixed) — vertical stack at far right
            for (int i = 0; i < Constants.STARTING_TEMP_EFFECT_SLOTS; i++)
            {
                var slot = CreateSlotCube($"TempEffectSlot_{i + 1:D2}",
                    new Vector3(6.2f, 0.05f, -2.0f + i * 1.3f),
                    new Vector3(1.2f, 0.1f, 1.0f),
                    new Color(0.6f, 0.35f, 0.1f));

                var zone = slot.AddComponent<SlotZone3D>();
                zone.RuntimeInit(DropZoneType.TempEffectSlot, i);
                _tempEffectSlots.Add(zone);
            }

            // Sell Zone — right column, below TempEffect stack
            var sell = CreateSlotCube("SellZone",
                new Vector3(6.2f, 0.05f, 1.0f),
                new Vector3(1.2f, 0.1f, 1.5f),
                new Color(0.6f, 0.4f, 0.1f));
            _sellZone = sell.AddComponent<SlotZone3D>();
            _sellZone.RuntimeInit(DropZoneType.SellZone, 0);

            // Action Zone — right column, above Sell
            var action = CreateSlotCube("ActionZone",
                new Vector3(6.2f, 0.05f, 2.2f),
                new Vector3(1.2f, 0.1f, 1.5f),
                new Color(0.7f, 0.15f, 0.15f));
            _actionZone = action.AddComponent<SlotZone3D>();
            _actionZone.RuntimeInit(DropZoneType.ActionZone, 0);

            // ====================================================================
            // ZONE 2 — CUSTOMER MARKET ZONE (Z = 3.8)
            // 10 blocks, spacing 1.2, each = 10 customers from shared pool of 100
            // ====================================================================
            CreateDivider("Divider_PlayerMarket", new Vector3(0, 0.07f, 3.1f), new Vector3(13f, 0.02f, 0.05f));

            int marketBlocks = 10;
            for (int i = 0; i < marketBlocks; i++)
            {
                var terr = GameObject.CreatePrimitive(PrimitiveType.Cube);
                terr.name = $"CustomerBlock_{i + 1:D2}";
                terr.transform.SetParent(transform);
                float x = (i - 4.5f) * 1.2f;
                terr.transform.localPosition = new Vector3(x, 0.1f, 3.8f);
                terr.transform.localScale = new Vector3(1.0f, 0.25f, 0.6f);
                terr.GetComponent<MeshRenderer>().material.color = Color.gray;
                Destroy(terr.GetComponent<Collider>());
                _territoryBlocks.Add(terr);
                _territoryRenderers.Add(terr.GetComponent<MeshRenderer>());
            }

            CreateDivider("Divider_MarketRival", new Vector3(0, 0.07f, 4.5f), new Vector3(13f, 0.02f, 0.05f));

            // ====================================================================
            // ZONE 3 — RIVAL ZONE (top, Z = 5 to 6.5) — visual only, no drop zones
            // ====================================================================

            // Rival operation slots (4 visual-only) — spacing 2.2
            for (int i = 0; i < 4; i++)
            {
                var rivalSlot = CreateSlotCube($"RivalOp_{i + 1:D2}",
                    new Vector3((i - 1.5f) * 2.2f, 0.05f, 6.0f),
                    new Vector3(2.0f, 0.1f, 1.2f),
                    new Color(0.5f, 0.15f, 0.15f));
                Destroy(rivalSlot.GetComponent<Collider>());
            }

            // Rival staff slots (3 visual-only) — spacing 1.8
            for (int i = 0; i < 3; i++)
            {
                var rivalStaff = CreateSlotCube($"RivalStaff_{i + 1:D2}",
                    new Vector3((i - 1f) * 1.8f, 0.05f, 4.9f),
                    new Vector3(1.5f, 0.1f, 0.8f),
                    new Color(0.45f, 0.12f, 0.12f));
                Destroy(rivalStaff.GetComponent<Collider>());
            }

            // Event display (rival side, top-right corner)
            var eventArea = CreateSlotCube("EventDisplay",
                new Vector3(5.5f, 0.05f, 5.8f),
                new Vector3(1.5f, 0.08f, 1.5f),
                new Color(0.9f, 0.8f, 0.15f, 0.3f));
            Destroy(eventArea.GetComponent<Collider>());

            // ====================================================================
            // BOARD LABELS
            // ====================================================================
            CreateBoardLabels();
        }

        /// <summary>
        /// Creates a named cube slot with given position, scale and color as a board child.
        /// </summary>
        private GameObject CreateSlotCube(string slotName, Vector3 localPos, Vector3 localScale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = slotName;
            go.transform.SetParent(transform);
            go.transform.localPosition = localPos;
            go.transform.localScale = localScale;
            go.GetComponent<MeshRenderer>().material.color = color;
            return go;
        }

        private void CreateDivider(string divName, Vector3 localPos, Vector3 localScale)
        {
            var div = GameObject.CreatePrimitive(PrimitiveType.Cube);
            div.name = divName;
            div.transform.SetParent(transform);
            div.transform.localPosition = localPos;
            div.transform.localScale = localScale;
            div.GetComponent<MeshRenderer>().material.color = new Color(0.55f, 0.55f, 0.55f, 0.5f);
            Destroy(div.GetComponent<Collider>());
        }

        /// <summary>
        /// Updates the customer market visualization. Each block = 10 customers.
        /// Player = blue (left), Rival = red (right), Unclaimed = gray.
        /// </summary>
        public void UpdateTerritoryVisuals(int playerCount, int rivalCount)
        {
            // Convert customer counts to blocks (10 customers per block)
            int playerBlocks = Mathf.CeilToInt(playerCount / 10f);
            int rivalBlocks = Mathf.CeilToInt(rivalCount / 10f);
            int totalBlocks = _territoryRenderers.Count;

            for (int i = 0; i < totalBlocks; i++)
            {
                if (i < playerBlocks)
                    _territoryRenderers[i].material.color = new Color(0.2f, 0.5f, 1f); // Player blue
                else if (i >= totalBlocks - rivalBlocks)
                    _territoryRenderers[i].material.color = new Color(0.9f, 0.2f, 0.2f); // Rival red
                else
                    _territoryRenderers[i].material.color = Color.gray; // Unclaimed
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
            // Label Y = 0.5 — just above slot surface (slots are at Y=0.05, height=0.1 → top at Y=0.1)
            float labelY = 0.5f;
            // Zone labels slightly larger, slot labels smaller
            float zoneFont = 3f;
            float slotFont = 2.5f;
            // Rotation matches camera angle (55°) so labels face the viewer
            Quaternion labelRot = Quaternion.Euler(55f, 0f, 0f);

            // === ZONE 1: PLAYER ZONE ===
            CreateLabel("OPERATION", new Vector3(0f, labelY, -2.65f), labelRot, slotFont,
                        new Color(0.4f, 0.6f, 1f));
            CreateLabel("STAFF", new Vector3(0f, labelY, -1.2f), labelRot, slotFont,
                        new Color(0.4f, 0.9f, 0.5f));
            CreateLabel("MARKETING", new Vector3(-3.3f, labelY, 0.4f), labelRot, slotFont,
                        new Color(0.8f, 0.5f, 0.9f));
            CreateLabel("SUPPLIERS", new Vector3(3.1f, labelY, 0.4f), labelRot, slotFont,
                        new Color(0.9f, 0.75f, 0.35f));
            CreateLabel("EVENTS", new Vector3(6.2f, labelY, -2.65f), labelRot, slotFont * 0.85f,
                        new Color(0.95f, 0.5f, 0.2f));
            CreateLabel("SELL", new Vector3(6.2f, labelY, 0.35f), labelRot, slotFont,
                        new Color(0.9f, 0.7f, 0.2f));
            CreateLabel("ACTION", new Vector3(6.2f, labelY, 1.65f), labelRot, slotFont,
                        new Color(0.9f, 0.3f, 0.3f));

            // === ZONE 2: CUSTOMER MARKET ZONE ===
            CreateLabel("CUSTOMER MARKET (100)", new Vector3(0f, labelY, 3.8f), labelRot, zoneFont,
                        new Color(0.9f, 0.9f, 0.9f));

            // === ZONE 3: RIVAL ZONE ===
            CreateLabel("RIVAL", new Vector3(0f, labelY, 5.4f), labelRot, zoneFont,
                        new Color(1f, 0.4f, 0.4f));

            // === COMPANY TIER — top-left corner ===
            _tierLabel = CreateLabel("TRADER", new Vector3(-6f, labelY, 6.2f), labelRot, zoneFont,
                        new Color(0.9f, 0.75f, 0.3f));
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
            tmp.textWrappingMode = TextWrappingModes.NoWrap;

            // Make sure the label is visible but doesn't interfere with raycasts
            var rt = go.GetComponent<RectTransform>();
            if (rt != null)
                rt.sizeDelta = new Vector2(6f, 1.5f);

            return tmp;
        }

        // ================================================================
        //  Tier Change -- update the board label when company tier changes
        // ================================================================

        private static readonly string[] TierNames = { "TRADER", "ENTREPRENEUR", "CORPORATION", "CONGLOMERATE" };
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
