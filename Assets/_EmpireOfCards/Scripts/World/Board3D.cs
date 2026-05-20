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
            table.transform.localPosition = new Vector3(0, -0.25f, 2f);
            table.transform.localScale = new Vector3(16, 0.5f, 14);
            table.GetComponent<MeshRenderer>().material.color = new Color(0.35f, 0.22f, 0.12f);
            table.layer = LayerMask.NameToLayer("Default");
            Destroy(table.GetComponent<Collider>());

            // ====================================================================
            // ZONE 1 — PLAYER ZONE
            // Row A (Z = -2.0): Operation slots — business infrastructure   Y=0.05
            // Row B (Z = -0.5): Staff slots — employees                     Y=0.06
            // Row C (Z = 1.0):  Marketing (left) + Supplier (right)         Y=0.07
            // Right column (X = 5.2): TempEffect slots + Sell + Action      Y=0.04
            //
            // Right column pulled in by 0.3 (5.5 → 5.2) so it stays on screen.
            // Zone gap: 0.3 unit breathing room between rows via Z offsets.
            // ====================================================================

            // Row A: Operation Slots (4 starting) — spacing 2.2 → total 6.6 wide
            // Y = 0.05 — ground level (deepest)
            for (int i = 0; i < Constants.STARTING_OPERATION_SLOTS; i++)
            {
                var slot = CreateSlotCube($"OperationSlot_{i + 1:D2}",
                    new Vector3((i - 1.5f) * 2.2f, 0.05f, -2.0f),
                    new Vector3(2.0f, 0.08f, 1.5f),
                    new Color(0.18f, 0.25f, 0.42f));

                var zone = slot.AddComponent<SlotZone3D>();
                zone.RuntimeInit(DropZoneType.OperationSlot, i);
                _operationSlots.Add(zone);
                _businessSlots.Add(zone);
                _businessSlotRenderers.Add(slot.GetComponent<MeshRenderer>());
            }

            // Row B: Staff Slots (5 starting) — spacing 1.8 → total 7.2 wide
            // Y = 0.06 — slightly raised (depth layer 2)
            for (int i = 0; i < Constants.STARTING_STAFF_SLOTS; i++)
            {
                var slot = CreateSlotCube($"StaffSlot_{i + 1:D2}",
                    new Vector3((i - 2f) * 1.8f, 0.06f, -0.5f),
                    new Vector3(1.5f, 0.08f, 1.2f),
                    new Color(0.18f, 0.38f, 0.22f));

                var zone = slot.AddComponent<SlotZone3D>();
                zone.RuntimeInit(DropZoneType.StaffSlot, i);
                _staffSlots.Add(zone);
                _employeeSlots.Add(zone);
            }

            // Row C-Left: Marketing Slots (3 starting) — spacing 1.8, left cluster
            // Y = 0.07 — highest player row (depth layer 3)
            for (int i = 0; i < Constants.STARTING_MARKETING_SLOTS; i++)
            {
                var slot = CreateSlotCube($"MarketingSlot_{i + 1:D2}",
                    new Vector3(-4.5f + i * 1.8f, 0.07f, 1.0f),
                    new Vector3(1.5f, 0.08f, 1.0f),
                    new Color(0.42f, 0.2f, 0.42f));

                var zone = slot.AddComponent<SlotZone3D>();
                zone.RuntimeInit(DropZoneType.MarketingSlot, i);
                _marketingSlots.Add(zone);
                _upgradeSlots.Add(zone);
            }

            // Row C-Right: Supplier Slots (2 starting) — spacing 1.8, right of center
            // Y = 0.07 — same depth layer as Marketing
            for (int i = 0; i < Constants.STARTING_SUPPLIER_SLOTS; i++)
            {
                var slot = CreateSlotCube($"SupplierSlot_{i + 1:D2}",
                    new Vector3(2.2f + i * 1.8f, 0.07f, 1.0f),
                    new Vector3(1.5f, 0.08f, 1.0f),
                    new Color(0.38f, 0.28f, 0.12f));

                var zone = slot.AddComponent<SlotZone3D>();
                zone.RuntimeInit(DropZoneType.SupplierSlot, i);
                _supplierSlots.Add(zone);
            }

            // Right column: TempEffect Slots (3 fixed) — vertical stack, pulled in to X=5.2
            // Y = 0.04 — lowest depth (recessed utility row)
            for (int i = 0; i < Constants.STARTING_TEMP_EFFECT_SLOTS; i++)
            {
                var slot = CreateSlotCube($"TempEffectSlot_{i + 1:D2}",
                    new Vector3(5.2f, 0.04f, -2.0f + i * 1.3f),
                    new Vector3(1.2f, 0.08f, 1.0f),
                    new Color(0.5f, 0.28f, 0.08f));

                var zone = slot.AddComponent<SlotZone3D>();
                zone.RuntimeInit(DropZoneType.TempEffectSlot, i);
                _tempEffectSlots.Add(zone);
            }

            // Sell Zone — right column, pulled in to X=5.2
            var sell = CreateSlotCube("SellZone",
                new Vector3(5.2f, 0.04f, 1.0f),
                new Vector3(1.2f, 0.08f, 1.5f),
                new Color(0.5f, 0.33f, 0.08f));
            _sellZone = sell.AddComponent<SlotZone3D>();
            _sellZone.RuntimeInit(DropZoneType.SellZone, 0);

            // Action Zone — right column, above Sell, pulled in to X=5.2
            var action = CreateSlotCube("ActionZone",
                new Vector3(5.2f, 0.04f, 2.2f),
                new Vector3(1.2f, 0.08f, 1.5f),
                new Color(0.6f, 0.12f, 0.12f));
            _actionZone = action.AddComponent<SlotZone3D>();
            _actionZone.RuntimeInit(DropZoneType.ActionZone, 0);

            // ====================================================================
            // ZONE 2 — CUSTOMER MARKET ZONE (Z = 3.8)
            // 10 blocks, spacing 1.1, each = 10 customers from shared pool of 100
            // Blocks are smaller (0.8 x 0.2 x 0.4) with tighter spacing for clarity
            // ====================================================================

            // Thicker, brighter divider — Y scale 0.06, near-white color
            CreateDivider("Divider_PlayerMarket", new Vector3(0, 0.07f, 3.1f), new Vector3(13f, 0.06f, 0.08f));

            int marketBlocks = 10;
            for (int i = 0; i < marketBlocks; i++)
            {
                var terr = GameObject.CreatePrimitive(PrimitiveType.Cube);
                terr.name = $"CustomerBlock_{i + 1:D2}";
                terr.transform.SetParent(transform);
                float x = (i - 4.5f) * 1.1f;
                terr.transform.localPosition = new Vector3(x, 0.1f, 3.8f);
                terr.transform.localScale = new Vector3(0.8f, 0.2f, 0.4f);
                terr.GetComponent<MeshRenderer>().material.color = Color.gray;
                Destroy(terr.GetComponent<Collider>());
                _territoryBlocks.Add(terr);
                _territoryRenderers.Add(terr.GetComponent<MeshRenderer>());
            }

            // Thicker, brighter divider — Y scale 0.06, near-white color
            CreateDivider("Divider_MarketRival", new Vector3(0, 0.07f, 4.5f), new Vector3(13f, 0.06f, 0.08f));

            // ====================================================================
            // ZONE 3 — RIVAL ZONE (top, Z = 5 to 6.5) — visual only, no drop zones
            // Rival slots are smaller to convey visual distance / threat scale
            // ====================================================================

            // Rival operation slots (4 visual-only) — compact: 1.5 x 0.08 x 0.8
            for (int i = 0; i < 4; i++)
            {
                var rivalSlot = CreateSlotCube($"RivalOp_{i + 1:D2}",
                    new Vector3((i - 1.5f) * 2.0f, 0.05f, 6.0f),
                    new Vector3(1.5f, 0.08f, 0.8f),
                    new Color(0.5f, 0.15f, 0.15f));
                Destroy(rivalSlot.GetComponent<Collider>());
            }

            // Rival staff slots (3 visual-only) — compact: 1.0 x 0.08 x 0.6
            for (int i = 0; i < 3; i++)
            {
                var rivalStaff = CreateSlotCube($"RivalStaff_{i + 1:D2}",
                    new Vector3((i - 1f) * 1.6f, 0.05f, 4.9f),
                    new Vector3(1.0f, 0.08f, 0.6f),
                    new Color(0.45f, 0.12f, 0.12f));
                Destroy(rivalStaff.GetComponent<Collider>());
            }

            // Event display (rival side, top-right corner)
            var eventArea = CreateSlotCube("EventDisplay",
                new Vector3(5.2f, 0.05f, 5.8f),
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
            // Bright near-white with slight warm tint — clearly visible against the table
            div.GetComponent<MeshRenderer>().material.color = new Color(0.82f, 0.82f, 0.78f);
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
            // Labels sit BELOW each slot row (higher Z = closer to viewer in this camera setup).
            // Offset = slot center Z + (slot depth / 2) + 0.2 gap — label never hidden by a placed card.
            // Y slightly above surface (0.15) so text clears the table geometry.
            float labelY = 0.15f;
            float zoneFont = 3f;
            float slotFont = 2.5f;
            // Rotation matches camera angle (55°) so labels face the viewer
            Quaternion labelRot = Quaternion.Euler(55f, 0f, 0f);

            // === ZONE 1: PLAYER ZONE ===
            // Operation row: center Z=-2.0, depth=1.5 → label Z = -2.0 + 0.75 + 0.2 = -1.05
            CreateLabel("OPERATION", new Vector3(0f, labelY, -1.05f), labelRot, slotFont,
                        new Color(0.5f, 0.7f, 1f));
            // Staff row: center Z=-0.5, depth=1.2 → label Z = -0.5 + 0.6 + 0.2 = 0.3
            CreateLabel("STAFF", new Vector3(0f, labelY, 0.3f), labelRot, slotFont,
                        new Color(0.45f, 0.9f, 0.55f));
            // Marketing row: center Z=1.0, depth=1.0 → label Z = 1.0 + 0.5 + 0.2 = 1.7
            CreateLabel("MARKETING", new Vector3(-3.3f, labelY, 1.7f), labelRot, slotFont,
                        new Color(0.85f, 0.55f, 0.95f));
            // Supplier row: center Z=1.0, depth=1.0 → same bottom edge as Marketing
            CreateLabel("SUPPLIERS", new Vector3(3.1f, labelY, 1.7f), labelRot, slotFont,
                        new Color(0.95f, 0.78f, 0.4f));
            // Right column — TempEffect stack bottom: Z=-2.0, depth=1.0 → label Z = -2.0 + 0.5 + 0.2 = -1.3
            CreateLabel("EVENTS", new Vector3(5.2f, labelY, -1.3f), labelRot, slotFont * 0.85f,
                        new Color(0.95f, 0.55f, 0.25f));
            // Sell zone: center Z=1.0, depth=1.5 → label Z = 1.0 + 0.75 + 0.2 = 1.95
            CreateLabel("SELL", new Vector3(5.2f, labelY, 1.95f), labelRot, slotFont,
                        new Color(0.9f, 0.72f, 0.25f));
            // Action zone: center Z=2.2, depth=1.5 → label Z = 2.2 + 0.75 + 0.2 = 3.15
            CreateLabel("ACTION", new Vector3(5.2f, labelY, 3.15f), labelRot, slotFont,
                        new Color(0.92f, 0.35f, 0.35f));

            // === ZONE 2: CUSTOMER MARKET ZONE ===
            // Market blocks at Z=3.8, depth=0.4 → label Z = 3.8 + 0.2 + 0.2 = 4.2
            CreateLabel("CUSTOMER MARKET (100)", new Vector3(0f, labelY, 4.2f), labelRot, zoneFont,
                        new Color(0.9f, 0.9f, 0.9f));

            // === ZONE 3: RIVAL ZONE ===
            // Rival staff row at Z=4.9, depth=0.6 → label Z = 4.9 + 0.3 + 0.2 = 5.4
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
            EventBus.OnMarketBlocksChanged += HandleTerritoryChanged;
            EventBus.OnBusinessSlotsChanged += UpdateVisibleSlots;
        }

        private void OnDisable()
        {
            EventBus.OnCompanyTierChanged -= HandleTierChanged;
            EventBus.OnBusinessNeglected -= HandleBusinessNeglected;
            EventBus.OnEmployeePlaced -= HandleSlotRefresh;
            EventBus.OnUpgradePlaced -= HandleSlotRefresh;
            EventBus.OnEmployeeLeft -= HandleEmployeeLeft;
            EventBus.OnMarketBlocksChanged -= HandleTerritoryChanged;
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
