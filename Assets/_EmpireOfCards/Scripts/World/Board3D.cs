using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;
using EmpireOfCards.Presentation;
using EmpireOfCards.UI.Cards;

namespace EmpireOfCards.World
{
    public class Board3D : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManager;

        private readonly List<SlotZone3D> _operationSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _staffSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _marketingSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _supplierSlots = new List<SlotZone3D>();
        private readonly List<SlotZone3D> _tempEffectSlots = new List<SlotZone3D>();

        // Legacy compatibility for systems still thinking in "business slots".
        private readonly List<SlotZone3D> _businessSlots = new List<SlotZone3D>();
        private readonly List<MeshRenderer> _businessSlotRenderers = new List<MeshRenderer>();

        private readonly List<GameObject> _territoryBlocks = new List<GameObject>();
        private readonly List<MeshRenderer> _territoryRenderers = new List<MeshRenderer>();

        private SlotZone3D _sellZone;
        private SlotZone3D _actionZone;
        private TextMeshPro _tierLabel;

        public IReadOnlyList<SlotZone3D> BusinessSlots => _operationSlots;

        public void Init(BoardManager board)
        {
            boardManager = board;
        }

        public void UpdateVisibleSlots(int maxSlots)
        {
            for (int i = 0; i < _operationSlots.Count; i++)
                _operationSlots[i].gameObject.SetActive(i < maxSlots);
        }

        public void BuildBoard()
        {
            BuildDeskFoundation();
            BuildPlayerBands();
            BuildMarketBand();
            BuildRivalBand();
            CreateSurfaceHeaders();
        }

        private void BuildDeskFoundation()
        {
            var table = CreateCube("DeskOuter", new Vector3(0f, -0.40f, 1.55f), new Vector3(17.8f, 0.80f, 15.8f), ControlDeskTheme.DeskOuter);
            Destroy(table.GetComponent<Collider>());

            var inset = CreateCube("DeskInset", new Vector3(0f, -0.05f, 1.55f), new Vector3(16.0f, 0.16f, 14.0f), ControlDeskTheme.DeskInner);
            Destroy(inset.GetComponent<Collider>());

            var felt = CreateCube("ControlSurface", new Vector3(0f, 0.02f, 1.35f), new Vector3(15.0f, 0.05f, 13.1f), ControlDeskTheme.FeltSurface);
            Destroy(felt.GetComponent<Collider>());

            CreateFramedPanel("PlayerBand", new Vector3(0f, 0.055f, -0.25f), new Vector3(13.8f, 0.05f, 6.0f), ControlDeskTheme.PlayerBand, ControlDeskTheme.SurfaceBorder);
            CreateFramedPanel("MarketBand", new Vector3(0f, 0.06f, 3.55f), new Vector3(13.4f, 0.05f, 1.75f), ControlDeskTheme.MidBand, ControlDeskTheme.SurfaceBorder);
            CreateFramedPanel("RivalBand", new Vector3(0f, 0.055f, 5.70f), new Vector3(13.4f, 0.05f, 2.20f), ControlDeskTheme.RivalBand, ControlDeskTheme.Darken(ControlDeskTheme.RivalLane, 0.2f));
            CreateFramedPanel("HandDock", new Vector3(0f, 0.06f, -3.25f), new Vector3(9.0f, 0.05f, 1.65f), ControlDeskTheme.PanelSoft, ControlDeskTheme.SurfaceBorder);
        }

        private void BuildPlayerBands()
        {
            CreateFramedPanel("OperationLane", new Vector3(0f, 0.07f, -1.45f), new Vector3(9.4f, 0.05f, 1.90f), ControlDeskTheme.OperationLane, ControlDeskTheme.Lighten(ControlDeskTheme.OperationLane, 0.2f));
            CreateFramedPanel("StaffLane", new Vector3(0f, 0.075f, -0.15f), new Vector3(10.2f, 0.05f, 1.50f), ControlDeskTheme.StaffLane, ControlDeskTheme.Lighten(ControlDeskTheme.StaffLane, 0.2f));
            CreateFramedPanel("MarketingLane", new Vector3(-2.70f, 0.08f, 1.15f), new Vector3(5.9f, 0.05f, 1.25f), ControlDeskTheme.MarketingLane, ControlDeskTheme.Lighten(ControlDeskTheme.MarketingLane, 0.15f));
            CreateFramedPanel("SupplierLane", new Vector3(2.90f, 0.08f, 1.15f), new Vector3(4.2f, 0.05f, 1.25f), ControlDeskTheme.SupplierLane, ControlDeskTheme.Lighten(ControlDeskTheme.SupplierLane, 0.15f));
            CreateFramedPanel("UtilityRail", new Vector3(6.10f, 0.075f, -0.50f), new Vector3(1.85f, 0.05f, 4.90f), ControlDeskTheme.UtilityLane, ControlDeskTheme.Lighten(ControlDeskTheme.UtilityLane, 0.15f));

            for (int i = 0; i < Constants.STARTING_OPERATION_SLOTS; i++)
            {
                var slot = CreateBoardSlot($"OperationSlot_{i + 1:D2}",
                    new Vector3((i - 1.5f) * 2.10f, 0.11f, -1.45f),
                    new Vector3(1.92f, 0.08f, 1.28f),
                    ControlDeskTheme.OperationSlot,
                    DropZoneType.OperationSlot, i);

                _operationSlots.Add(slot);
                _businessSlots.Add(slot);
                _businessSlotRenderers.Add(slot.GetComponent<MeshRenderer>());
            }

            for (int i = 0; i < Constants.STARTING_STAFF_SLOTS; i++)
            {
                var slot = CreateBoardSlot($"StaffSlot_{i + 1:D2}",
                    new Vector3((i - 2f) * 1.72f, 0.115f, -0.15f),
                    new Vector3(1.36f, 0.08f, 0.98f),
                    ControlDeskTheme.StaffSlot,
                    DropZoneType.StaffSlot, i);

                _staffSlots.Add(slot);
            }

            for (int i = 0; i < Constants.STARTING_MARKETING_SLOTS; i++)
            {
                var slot = CreateBoardSlot($"MarketingSlot_{i + 1:D2}",
                    new Vector3(-4.35f + i * 1.78f, 0.12f, 1.15f),
                    new Vector3(1.42f, 0.08f, 0.88f),
                    ControlDeskTheme.MarketingSlot,
                    DropZoneType.MarketingSlot, i);

                _marketingSlots.Add(slot);
            }

            for (int i = 0; i < Constants.STARTING_SUPPLIER_SLOTS; i++)
            {
                var slot = CreateBoardSlot($"SupplierSlot_{i + 1:D2}",
                    new Vector3(2.15f + i * 1.78f, 0.12f, 1.15f),
                    new Vector3(1.42f, 0.08f, 0.88f),
                    ControlDeskTheme.SupplierSlot,
                    DropZoneType.SupplierSlot, i);

                _supplierSlots.Add(slot);
            }

            for (int i = 0; i < Constants.STARTING_TEMP_EFFECT_SLOTS; i++)
            {
                float z = 0.90f - i * 1.25f;
                var slot = CreateBoardSlot($"TempEffectSlot_{i + 1:D2}",
                    new Vector3(6.10f, 0.115f, z),
                    new Vector3(1.25f, 0.08f, 0.80f),
                    ControlDeskTheme.EventSlot,
                    DropZoneType.TempEffectSlot, i);

                _tempEffectSlots.Add(slot);
            }

            _sellZone = CreateBoardSlot("SellZone",
                new Vector3(6.10f, 0.115f, -1.55f),
                new Vector3(1.25f, 0.08f, 1.05f),
                ControlDeskTheme.UtilitySlot,
                DropZoneType.SellZone, 0);

            _actionZone = CreateBoardSlot("ActionZone",
                new Vector3(6.10f, 0.115f, -2.75f),
                new Vector3(1.25f, 0.08f, 1.05f),
                ControlDeskTheme.ActionSlot,
                DropZoneType.ActionZone, 0);
        }

        private void BuildMarketBand()
        {
            CreateDivider("MarketDividerTop", new Vector3(0f, 0.095f, 2.92f), new Vector3(12.5f, 0.03f, 0.06f));
            CreateDivider("MarketDividerBottom", new Vector3(0f, 0.095f, 4.18f), new Vector3(12.5f, 0.03f, 0.06f));

            for (int i = 0; i < 10; i++)
            {
                var terr = CreateCube($"CustomerBlock_{i + 1:D2}",
                    new Vector3((i - 4.5f) * 1.0f, 0.13f, 3.55f),
                    new Vector3(0.72f, 0.18f, 0.48f),
                    ControlDeskTheme.NeutralBlock);

                Destroy(terr.GetComponent<Collider>());
                _territoryBlocks.Add(terr);
                _territoryRenderers.Add(terr.GetComponent<MeshRenderer>());
            }
        }

        private void BuildRivalBand()
        {
            CreateFramedPanel("RivalOpsLane", new Vector3(0f, 0.08f, 5.95f), new Vector3(8.7f, 0.04f, 0.88f), ControlDeskTheme.RivalLane, ControlDeskTheme.Darken(ControlDeskTheme.RivalLane, 0.1f));
            CreateFramedPanel("RivalSupportLane", new Vector3(0f, 0.08f, 5.10f), new Vector3(6.1f, 0.04f, 0.62f), ControlDeskTheme.Darken(ControlDeskTheme.RivalLane, 0.1f), ControlDeskTheme.Darken(ControlDeskTheme.RivalLane, 0.2f));

            for (int i = 0; i < 4; i++)
            {
                var rivalSlot = CreateCube($"RivalOp_{i + 1:D2}",
                    new Vector3((i - 1.5f) * 1.95f, 0.11f, 5.95f),
                    new Vector3(1.35f, 0.08f, 0.64f),
                    ControlDeskTheme.RivalSlot);
                Destroy(rivalSlot.GetComponent<Collider>());
            }

            for (int i = 0; i < 3; i++)
            {
                var rivalSupport = CreateCube($"RivalSupport_{i + 1:D2}",
                    new Vector3((i - 1f) * 1.45f, 0.11f, 5.12f),
                    new Vector3(0.92f, 0.08f, 0.44f),
                    ControlDeskTheme.Darken(ControlDeskTheme.RivalSlot, 0.1f));
                Destroy(rivalSupport.GetComponent<Collider>());
            }

            var rivalEvent = CreateCube("RivalSignal",
                new Vector3(5.55f, 0.11f, 5.55f),
                new Vector3(1.40f, 0.08f, 1.00f),
                ControlDeskTheme.AccentAmber);
            rivalEvent.GetComponent<MeshRenderer>().material.color = ControlDeskTheme.WithAlpha(ControlDeskTheme.AccentAmber, 0.9f);
            Destroy(rivalEvent.GetComponent<Collider>());
        }

        private SlotZone3D CreateBoardSlot(string slotName, Vector3 localPos, Vector3 localScale, Color color, DropZoneType zoneType, int slotIndex)
        {
            var slot = CreateCube(slotName, localPos, localScale, color);
            var zone = slot.AddComponent<SlotZone3D>();
            zone.RuntimeInit(zoneType, slotIndex);
            zone.ConfigureVisuals(
                color,
                ControlDeskTheme.Lighten(color, 0.15f),
                ControlDeskTheme.GuidedPulse,
                ControlDeskTheme.ValidHighlight,
                ControlDeskTheme.InvalidHighlight);
            return zone;
        }

        private GameObject CreateCube(string objectName, Vector3 localPos, Vector3 localScale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = objectName;
            go.transform.SetParent(transform);
            go.transform.localPosition = localPos;
            go.transform.localScale = localScale;
            go.GetComponent<MeshRenderer>().material.color = color;
            return go;
        }

        private void CreateFramedPanel(string objectName, Vector3 localPos, Vector3 size, Color fillColor, Color borderColor)
        {
            var root = new GameObject(objectName);
            root.transform.SetParent(transform);
            root.transform.localPosition = localPos;

            var fill = CreateCube("Fill", Vector3.zero, size, fillColor);
            fill.transform.SetParent(root.transform, false);
            Destroy(fill.GetComponent<Collider>());

            float edgeHeight = size.y + 0.01f;
            float edgeWidth = 0.09f;
            float edgeDepth = 0.09f;

            CreateEdge(root.transform, "EdgeTop", new Vector3(0f, 0.02f, size.z * 0.5f), new Vector3(size.x, edgeHeight, edgeDepth), borderColor);
            CreateEdge(root.transform, "EdgeBottom", new Vector3(0f, 0.02f, -size.z * 0.5f), new Vector3(size.x, edgeHeight, edgeDepth), borderColor);
            CreateEdge(root.transform, "EdgeLeft", new Vector3(-size.x * 0.5f, 0.02f, 0f), new Vector3(edgeWidth, edgeHeight, size.z), borderColor);
            CreateEdge(root.transform, "EdgeRight", new Vector3(size.x * 0.5f, 0.02f, 0f), new Vector3(edgeWidth, edgeHeight, size.z), borderColor);
        }

        private void CreateEdge(Transform parent, string objectName, Vector3 localPos, Vector3 localScale, Color color)
        {
            var edge = CreateCube(objectName, localPos, localScale, color);
            edge.transform.SetParent(parent, false);
            Destroy(edge.GetComponent<Collider>());
        }

        private void CreateDivider(string objectName, Vector3 localPos, Vector3 localScale)
        {
            var divider = CreateCube(objectName, localPos, localScale, ControlDeskTheme.Divider);
            Destroy(divider.GetComponent<Collider>());
        }

        public void UpdateTerritoryVisuals(int playerCount, int rivalCount)
        {
            int playerBlocks = Mathf.Clamp(playerCount, 0, _territoryRenderers.Count);
            int rivalBlocks = Mathf.Clamp(rivalCount, 0, _territoryRenderers.Count - playerBlocks);

            for (int i = 0; i < _territoryRenderers.Count; i++)
            {
                if (i < playerBlocks)
                    _territoryRenderers[i].material.color = ControlDeskTheme.PlayerBlock;
                else if (i >= _territoryRenderers.Count - rivalBlocks)
                    _territoryRenderers[i].material.color = ControlDeskTheme.RivalBlock;
                else
                    _territoryRenderers[i].material.color = ControlDeskTheme.NeutralBlock;
            }
        }

        private void CreateSurfaceHeaders()
        {
            Quaternion flatText = Quaternion.Euler(90f, 0f, 0f);

            CreateDeskText("OPERATIONS", new Vector3(-3.75f, 0.16f, -2.10f), flatText, 1.65f, ControlDeskTheme.Lighten(ControlDeskTheme.OperationSlot, 0.25f), TextAlignmentOptions.Left);
            CreateDeskText("STAFF", new Vector3(-4.20f, 0.16f, -0.78f), flatText, 1.45f, ControlDeskTheme.Lighten(ControlDeskTheme.StaffSlot, 0.25f), TextAlignmentOptions.Left);
            CreateDeskText("MARKETING", new Vector3(-5.15f, 0.16f, 0.52f), flatText, 1.25f, ControlDeskTheme.Lighten(ControlDeskTheme.MarketingSlot, 0.22f), TextAlignmentOptions.Left);
            CreateDeskText("SUPPLIERS", new Vector3(1.15f, 0.16f, 0.52f), flatText, 1.25f, ControlDeskTheme.Lighten(ControlDeskTheme.SupplierSlot, 0.22f), TextAlignmentOptions.Left);
            CreateDeskText("CONTROL", new Vector3(5.35f, 0.16f, 1.55f), flatText, 1.15f, ControlDeskTheme.Lighten(ControlDeskTheme.UtilitySlot, 0.18f), TextAlignmentOptions.Left);
            CreateDeskText("SHARED MARKET", new Vector3(-5.45f, 0.16f, 2.95f), flatText, 1.20f, ControlDeskTheme.TextMuted, TextAlignmentOptions.Left);
            CreateDeskText("RIVAL PRESSURE", new Vector3(-5.45f, 0.16f, 5.05f), flatText, 1.15f, ControlDeskTheme.Lighten(ControlDeskTheme.RivalSlot, 0.20f), TextAlignmentOptions.Left);
            CreateDeskText("HAND", new Vector3(-3.95f, 0.16f, -3.95f), flatText, 1.10f, ControlDeskTheme.TextMuted, TextAlignmentOptions.Left);
            CreateDeskText("PLAY", new Vector3(5.55f, 0.16f, -3.10f), flatText, 1.05f, ControlDeskTheme.Lighten(ControlDeskTheme.ActionSlot, 0.20f), TextAlignmentOptions.Center);
            CreateDeskText("SELL", new Vector3(5.55f, 0.16f, -1.85f), flatText, 1.05f, ControlDeskTheme.Lighten(ControlDeskTheme.UtilitySlot, 0.22f), TextAlignmentOptions.Center);
            CreateDeskText("EVENTS", new Vector3(5.55f, 0.16f, 1.45f), flatText, 1.00f, ControlDeskTheme.Lighten(ControlDeskTheme.EventSlot, 0.22f), TextAlignmentOptions.Center);

            _tierLabel = CreateDeskText("TRADER", new Vector3(-5.95f, 0.16f, -2.95f), flatText, 1.25f, ControlDeskTheme.MoneyGold, TextAlignmentOptions.Left);
        }

        private TextMeshPro CreateDeskText(string text, Vector3 localPos, Quaternion localRot, float fontSize, Color color, TextAlignmentOptions alignment)
        {
            var go = new GameObject($"DeskText_{text.Replace(" ", string.Empty)}");
            go.transform.SetParent(transform);
            go.transform.localPosition = localPos;
            go.transform.localRotation = localRot;
            go.transform.localScale = Vector3.one;

            var tmp = go.AddComponent<TextMeshPro>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.textWrappingMode = TextWrappingModes.NoWrap;
            tmp.enableAutoSizing = false;
            return tmp;
        }

        private static readonly string[] TierNames = { "TRADER", "ENTREPRENEUR", "CORPORATION", "CONGLOMERATE" };
        private static readonly Color[] TierColors =
        {
            ControlDeskTheme.MoneyGold,
            ControlDeskTheme.AccentGreen,
            ControlDeskTheme.AccentBlue,
            ControlDeskTheme.AccentAmber
        };

        private void OnEnable()
        {
            EventBus.OnCompanyTierChanged += HandleTierChanged;
            EventBus.OnBusinessNeglected += HandleBusinessNeglected;
            EventBus.OnEmployeePlaced += HandleSlotRefresh;
            EventBus.OnUpgradePlaced += HandleSlotRefresh;
            EventBus.OnMarketBlocksChanged += HandleTerritoryChanged;
            EventBus.OnBusinessSlotsChanged += UpdateVisibleSlots;
        }

        private void OnDisable()
        {
            EventBus.OnCompanyTierChanged -= HandleTierChanged;
            EventBus.OnBusinessNeglected -= HandleBusinessNeglected;
            EventBus.OnEmployeePlaced -= HandleSlotRefresh;
            EventBus.OnUpgradePlaced -= HandleSlotRefresh;
            EventBus.OnMarketBlocksChanged -= HandleTerritoryChanged;
            EventBus.OnBusinessSlotsChanged -= UpdateVisibleSlots;
        }

        private void HandleTerritoryChanged(int playerCount, int rivalCount)
        {
            UpdateTerritoryVisuals(playerCount, rivalCount);
        }

        private void HandleTierChanged(CompanyTier newTier)
        {
            if (_tierLabel == null) return;

            int index = (int)newTier;
            _tierLabel.text = TierNames[index];
            _tierLabel.color = TierColors[index];
        }

        private void HandleBusinessNeglected(int businessIndex, int neglectTurns)
        {
            if (businessIndex < 0 || businessIndex >= _businessSlotRenderers.Count) return;

            var renderer = _businessSlotRenderers[businessIndex];
            if (renderer == null) return;

            renderer.material.color = neglectTurns >= 6
                ? ControlDeskTheme.Darken(ControlDeskTheme.OperationSlot, 0.55f)
                : ControlDeskTheme.Darken(ControlDeskTheme.OperationSlot, 0.35f);
        }

        private void HandleSlotRefresh(CardData card, int businessIndex)
        {
            if (businessIndex < 0 || businessIndex >= _businessSlotRenderers.Count) return;

            var renderer = _businessSlotRenderers[businessIndex];
            if (renderer != null)
                renderer.material.color = ControlDeskTheme.OperationSlot;
        }
    }
}
