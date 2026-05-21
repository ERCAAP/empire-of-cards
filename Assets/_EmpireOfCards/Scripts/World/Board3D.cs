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
        [SerializeField] private GameManager gameManager;
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private SlotManager slotManager;

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
        private readonly List<GameObject> _rivalOperationSlots = new List<GameObject>();
        private readonly List<GameObject> _rivalStaffSlots = new List<GameObject>();
        private readonly List<GameObject> _rivalMarketingSlots = new List<GameObject>();
        private readonly List<GameObject> _rivalSupplierSlots = new List<GameObject>();
        private readonly List<GameObject> _rivalSignalSlots = new List<GameObject>();
        private readonly Dictionary<GameObject, GameObject> _rivalVisualsBySlot = new Dictionary<GameObject, GameObject>();
        private readonly Dictionary<string, GameObject> _themePropRoots = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, MeshRenderer> _themePropBodies = new Dictionary<string, MeshRenderer>();
        private readonly Dictionary<string, TextMeshPro> _themePropLabels = new Dictionary<string, TextMeshPro>();
        private int _rivalOperationCursor;
        private int _rivalStaffCursor;
        private int _rivalMarketingCursor;
        private int _rivalSupplierCursor;
        private int _rivalSignalCursor;
        private VentureBoardThemeProfile _activeThemeProfile;

        private SlotZone3D _sellZone;
        private SlotZone3D _actionZone;
        private TextMeshPro _tierLabel;
        private TextMeshPro _operationsHeader;
        private TextMeshPro _staffHeader;
        private TextMeshPro _marketingHeader;
        private TextMeshPro _supplierHeader;
        private TextMeshPro _districtTrafficLabel;
        private TextMeshPro _districtRatingLabel;
        private TextMeshPro _districtPullLabel;
        private TextMeshPro _rivalCardLabel;
        private TextMeshPro _rivalStyleLabel;
        private TextMeshPro _rivalCrisisLabel;
        private TextMeshPro _businessAnchorLabel;
        private GameObject _businessAnchorVisual;

        public IReadOnlyList<SlotZone3D> BusinessSlots => _operationSlots;
        public IReadOnlyList<SlotZone3D> AllSlots => _allSlots;

        private readonly List<SlotZone3D> _allSlots = new List<SlotZone3D>();

        public void Init(GameManager game, BoardManager board, SlotManager slots)
        {
            gameManager = game;
            boardManager = board;
            slotManager = slots;
            BindBoardContextToSlots();
        }

        public void UpdateVisibleSlots(int maxSlots)
        {
            for (int i = 0; i < _operationSlots.Count; i++)
                _operationSlots[i].gameObject.SetActive(i < maxSlots);
        }

        public void RefreshSlotOccupancyVisuals()
        {
            if (slotManager == null)
                return;

            ApplySlotVisuals(_operationSlots, slotManager.OperationSlots);
            ApplySlotVisuals(_staffSlots, slotManager.StaffSlots);
            ApplySlotVisuals(_marketingSlots, slotManager.MarketingSlots);
            ApplySlotVisuals(_supplierSlots, slotManager.SupplierSlots);
            ApplySlotVisuals(_tempEffectSlots, slotManager.TempEffectSlots);
            UpdateBusinessAnchor();
            EnsureThemeProps(ResolveThemeProfile());
            RefreshThemeProps();
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

            CreateFramedPanel("PlayerBand", new Vector3(0f, 0.055f, -0.92f), new Vector3(13.9f, 0.05f, 5.45f), ControlDeskTheme.PlayerBand, ControlDeskTheme.SurfaceBorder);
            CreateFramedPanel("MarketBand", new Vector3(0f, 0.06f, 2.75f), new Vector3(13.9f, 0.05f, 2.05f), ControlDeskTheme.MidBand, ControlDeskTheme.SurfaceBorder);
            CreateFramedPanel("RivalBand", new Vector3(0f, 0.055f, 6.05f), new Vector3(13.9f, 0.05f, 4.45f), ControlDeskTheme.RivalBand, ControlDeskTheme.Darken(ControlDeskTheme.RivalLane, 0.2f));
            CreateFramedPanel("HandDock", new Vector3(0f, 0.06f, -4.18f), new Vector3(11.2f, 0.05f, 1.62f), ControlDeskTheme.PanelSoft, ControlDeskTheme.SurfaceBorder);
        }

        private void BuildPlayerBands()
        {
            CreateFramedPanel("OperationLane", new Vector3(-0.22f, 0.07f, -2.00f), new Vector3(10.55f, 0.05f, 1.22f), ControlDeskTheme.OperationLane, ControlDeskTheme.Lighten(ControlDeskTheme.OperationLane, 0.2f));
            CreateFramedPanel("StaffLane", new Vector3(-0.22f, 0.075f, -0.60f), new Vector3(10.55f, 0.05f, 1.10f), ControlDeskTheme.StaffLane, ControlDeskTheme.Lighten(ControlDeskTheme.StaffLane, 0.2f));
            CreateFramedPanel("MarketingLane", new Vector3(-2.55f, 0.08f, 0.85f), new Vector3(6.05f, 0.05f, 0.98f), ControlDeskTheme.MarketingLane, ControlDeskTheme.Lighten(ControlDeskTheme.MarketingLane, 0.15f));
            CreateFramedPanel("SupplierLane", new Vector3(2.90f, 0.08f, 0.85f), new Vector3(4.45f, 0.05f, 0.98f), ControlDeskTheme.SupplierLane, ControlDeskTheme.Lighten(ControlDeskTheme.SupplierLane, 0.15f));
            CreateFramedPanel("UtilityRail", new Vector3(6.52f, 0.075f, -1.00f), new Vector3(1.62f, 0.05f, 4.75f), ControlDeskTheme.UtilityLane, ControlDeskTheme.Lighten(ControlDeskTheme.UtilityLane, 0.15f));

            for (int i = 0; i < Constants.STARTING_OPERATION_SLOTS; i++)
            {
                var slot = CreateBoardSlot($"OperationSlot_{i + 1:D2}",
                    new Vector3(-2.90f + i * 1.90f, 0.11f, -2.00f),
                    new Vector3(1.56f, 0.08f, 0.84f),
                    ControlDeskTheme.OperationSlot,
                    DropZoneType.OperationSlot, i);

                _operationSlots.Add(slot);
                _businessSlots.Add(slot);
                _businessSlotRenderers.Add(slot.GetComponent<MeshRenderer>());
            }

            for (int i = 0; i < Constants.STARTING_STAFF_SLOTS; i++)
            {
                var slot = CreateBoardSlot($"StaffSlot_{i + 1:D2}",
                    new Vector3(-2.90f + i * 1.90f, 0.115f, -0.60f),
                    new Vector3(1.56f, 0.08f, 0.76f),
                    ControlDeskTheme.StaffSlot,
                    DropZoneType.StaffSlot, i);

                _staffSlots.Add(slot);
            }

            for (int i = 0; i < Constants.STARTING_MARKETING_SLOTS; i++)
            {
                var slot = CreateBoardSlot($"MarketingSlot_{i + 1:D2}",
                    new Vector3(-3.75f + i * 1.82f, 0.12f, 0.85f),
                    new Vector3(1.54f, 0.08f, 0.76f),
                    ControlDeskTheme.MarketingSlot,
                    DropZoneType.MarketingSlot, i);

                _marketingSlots.Add(slot);
            }

            for (int i = 0; i < Constants.STARTING_SUPPLIER_SLOTS; i++)
            {
                var slot = CreateBoardSlot($"SupplierSlot_{i + 1:D2}",
                    new Vector3(2.18f + i * 1.82f, 0.12f, 0.85f),
                    new Vector3(1.54f, 0.08f, 0.76f),
                    ControlDeskTheme.SupplierSlot,
                    DropZoneType.SupplierSlot, i);

                _supplierSlots.Add(slot);
            }

            for (int i = 0; i < Constants.STARTING_TEMP_EFFECT_SLOTS; i++)
            {
                float z = 0.86f - i * 1.28f;
                var slot = CreateBoardSlot($"TempEffectSlot_{i + 1:D2}",
                    new Vector3(6.52f, 0.115f, z),
                    new Vector3(1.06f, 0.08f, 0.78f),
                    ControlDeskTheme.EventSlot,
                    DropZoneType.TempEffectSlot, i);

                _tempEffectSlots.Add(slot);
            }

            _sellZone = CreateBoardSlot("SellZone",
                new Vector3(6.52f, 0.115f, -2.74f),
                new Vector3(1.06f, 0.08f, 0.92f),
                ControlDeskTheme.UtilitySlot,
                DropZoneType.SellZone, 0);

            _actionZone = CreateBoardSlot("ActionZone",
                new Vector3(6.52f, 0.115f, -3.78f),
                new Vector3(1.06f, 0.08f, 0.92f),
                ControlDeskTheme.ActionSlot,
                DropZoneType.ActionZone, 0);

            BuildBusinessAnchor();
        }

        private void BuildMarketBand()
        {
            CreateFramedPanel("MarketContestTray", new Vector3(0f, 0.09f, 2.55f), new Vector3(10.9f, 0.03f, 1.34f), ControlDeskTheme.MarketLane, ControlDeskTheme.WithAlpha(ControlDeskTheme.TextMuted, 0.35f));
            CreateDivider("MarketDividerTop", new Vector3(0f, 0.095f, 1.82f), new Vector3(12.8f, 0.03f, 0.06f));
            CreateDivider("MarketDividerBottom", new Vector3(0f, 0.095f, 3.68f), new Vector3(12.8f, 0.03f, 0.06f));

            for (int i = 0; i < 10; i++)
            {
                var terr = CreateCube($"CustomerBlock_{i + 1:D2}",
                    new Vector3(-4.35f + i * 0.97f, 0.13f, 2.55f),
                    new Vector3(0.62f, 0.18f, 0.42f),
                    ControlDeskTheme.NeutralBlock);

                Destroy(terr.GetComponent<Collider>());
                _territoryBlocks.Add(terr);
                _territoryRenderers.Add(terr.GetComponent<MeshRenderer>());
            }
        }

        private void BuildRivalBand()
        {
            Color rivalOpLane = ControlDeskTheme.Darken(ControlDeskTheme.RivalSlot, 0.18f);
            Color rivalStaffLane = ControlDeskTheme.Darken(ControlDeskTheme.AccentRed, 0.42f);
            Color rivalMarketingLane = new Color(0.33f, 0.13f, 0.16f);
            Color rivalSupplierLane = new Color(0.39f, 0.17f, 0.14f);
            Color rivalUtilityLane = new Color(0.46f, 0.19f, 0.15f);

            CreateFramedPanel("RivalOperationLane", new Vector3(-0.22f, 0.08f, 7.12f), new Vector3(10.55f, 0.04f, 1.10f), rivalOpLane, ControlDeskTheme.Lighten(rivalOpLane, 0.18f));
            CreateFramedPanel("RivalStaffLane", new Vector3(-0.22f, 0.08f, 5.75f), new Vector3(10.55f, 0.04f, 1.00f), rivalStaffLane, ControlDeskTheme.Lighten(rivalStaffLane, 0.16f));
            CreateFramedPanel("RivalMarketingLane", new Vector3(-2.55f, 0.08f, 4.34f), new Vector3(6.05f, 0.04f, 0.96f), rivalMarketingLane, ControlDeskTheme.Lighten(rivalMarketingLane, 0.15f));
            CreateFramedPanel("RivalSupplierLane", new Vector3(2.90f, 0.08f, 4.34f), new Vector3(4.45f, 0.04f, 0.96f), rivalSupplierLane, ControlDeskTheme.Lighten(rivalSupplierLane, 0.15f));
            CreateFramedPanel("RivalSignalRail", new Vector3(6.52f, 0.08f, 5.73f), new Vector3(1.62f, 0.04f, 3.72f), rivalUtilityLane, ControlDeskTheme.Lighten(rivalUtilityLane, 0.15f));

            _rivalOperationSlots.Clear();
            _rivalStaffSlots.Clear();
            _rivalMarketingSlots.Clear();
            _rivalSupplierSlots.Clear();
            _rivalSignalSlots.Clear();
            _rivalVisualsBySlot.Clear();
            _rivalOperationCursor = 0;
            _rivalStaffCursor = 0;
            _rivalMarketingCursor = 0;
            _rivalSupplierCursor = 0;
            _rivalSignalCursor = 0;

            for (int i = 0; i < 4; i++)
            {
                var rivalSlot = CreateCube($"RivalOperation_{i + 1:D2}",
                    new Vector3(-2.90f + i * 1.90f, 0.11f, 7.12f),
                    new Vector3(1.56f, 0.08f, 0.78f),
                    ControlDeskTheme.Darken(ControlDeskTheme.RivalSlot, 0.04f));
                Destroy(rivalSlot.GetComponent<Collider>());
                _rivalOperationSlots.Add(rivalSlot);
            }

            for (int i = 0; i < 4; i++)
            {
                var rivalSupport = CreateCube($"RivalStaff_{i + 1:D2}",
                    new Vector3(-2.90f + i * 1.90f, 0.11f, 5.75f),
                    new Vector3(1.56f, 0.08f, 0.72f),
                    ControlDeskTheme.Darken(ControlDeskTheme.AccentRed, 0.18f));
                Destroy(rivalSupport.GetComponent<Collider>());
                _rivalStaffSlots.Add(rivalSupport);
            }

            for (int i = 0; i < 3; i++)
            {
                var rivalGrowth = CreateCube($"RivalMarketing_{i + 1:D2}",
                    new Vector3(-3.75f + i * 1.82f, 0.11f, 4.34f),
                    new Vector3(1.54f, 0.08f, 0.70f),
                    ControlDeskTheme.Darken(new Color(0.69f, 0.26f, 0.30f), 0.12f));
                Destroy(rivalGrowth.GetComponent<Collider>());
                _rivalMarketingSlots.Add(rivalGrowth);
            }

            for (int i = 0; i < 2; i++)
            {
                var rivalSupply = CreateCube($"RivalSupplier_{i + 1:D2}",
                    new Vector3(2.18f + i * 1.82f, 0.11f, 4.34f),
                    new Vector3(1.54f, 0.08f, 0.70f),
                    ControlDeskTheme.Darken(new Color(0.74f, 0.34f, 0.24f), 0.18f));
                Destroy(rivalSupply.GetComponent<Collider>());
                _rivalSupplierSlots.Add(rivalSupply);
            }

            for (int i = 0; i < 2; i++)
            {
                float z = 6.82f - i * 1.55f;
                var rivalEvent = CreateCube($"RivalSignal_{i + 1:D2}",
                    new Vector3(6.52f, 0.11f, z),
                    new Vector3(1.04f, 0.08f, 1.00f),
                    ControlDeskTheme.Darken(ControlDeskTheme.AccentAmber, 0.08f));
                Destroy(rivalEvent.GetComponent<Collider>());
                _rivalSignalSlots.Add(rivalEvent);
            }
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
            _allSlots.Add(zone);
            return zone;
        }

        private void BindBoardContextToSlots()
        {
            for (int i = 0; i < _allSlots.Count; i++)
            {
                if (_allSlots[i] != null)
                    _allSlots[i].SetBoardManager(boardManager);
            }
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

            _operationsHeader = CreateDeskText("OPERATIONS", new Vector3(-5.94f, 0.16f, -2.00f), flatText, 1.18f, ControlDeskTheme.Lighten(ControlDeskTheme.OperationSlot, 0.30f), TextAlignmentOptions.Left);
            _staffHeader = CreateDeskText("TEAM", new Vector3(-5.94f, 0.16f, -0.60f), flatText, 1.12f, ControlDeskTheme.Lighten(ControlDeskTheme.StaffSlot, 0.30f), TextAlignmentOptions.Left);
            _marketingHeader = CreateDeskText("GROWTH", new Vector3(-5.94f, 0.16f, 0.85f), flatText, 1.00f, ControlDeskTheme.Lighten(ControlDeskTheme.MarketingSlot, 0.28f), TextAlignmentOptions.Left);
            _supplierHeader = CreateDeskText("SUPPLY", new Vector3(0.85f, 0.16f, 0.85f), flatText, 1.00f, ControlDeskTheme.Lighten(ControlDeskTheme.SupplierSlot, 0.28f), TextAlignmentOptions.Left);
            _operationsHeader.textWrappingMode = TextWrappingModes.Normal;
            _staffHeader.textWrappingMode = TextWrappingModes.Normal;
            _marketingHeader.textWrappingMode = TextWrappingModes.Normal;
            _supplierHeader.textWrappingMode = TextWrappingModes.Normal;
            CreateDeskText("EVENTS", new Vector3(6.52f, 0.16f, 0.85f), flatText, 0.76f, ControlDeskTheme.Lighten(ControlDeskTheme.EventSlot, 0.22f), TextAlignmentOptions.Center);
            CreateDeskText("SELL", new Vector3(6.52f, 0.16f, -2.74f), flatText, 0.88f, ControlDeskTheme.Lighten(ControlDeskTheme.UtilitySlot, 0.24f), TextAlignmentOptions.Center);
            CreateDeskText("PLAY", new Vector3(6.52f, 0.16f, -3.78f), flatText, 0.88f, ControlDeskTheme.Lighten(ControlDeskTheme.ActionSlot, 0.24f), TextAlignmentOptions.Center);
            CreateDeskText("SHARED MARKET", new Vector3(0f, 0.16f, 3.58f), flatText, 1.10f, ControlDeskTheme.Lighten(ControlDeskTheme.TextPrimary, 0.12f), TextAlignmentOptions.Center);
            CreateDeskText("RIVAL BOARD", new Vector3(0f, 0.16f, 8.10f), flatText, 1.18f, ControlDeskTheme.Lighten(ControlDeskTheme.RivalSlot, 0.28f), TextAlignmentOptions.Center);
            CreateDeskText("OPERATIONS", new Vector3(-5.94f, 0.16f, 7.12f), flatText, 0.92f, ControlDeskTheme.Lighten(ControlDeskTheme.RivalSlot, 0.22f), TextAlignmentOptions.Left);
            CreateDeskText("TEAM", new Vector3(-5.94f, 0.16f, 5.75f), flatText, 0.88f, ControlDeskTheme.Lighten(ControlDeskTheme.RivalSlot, 0.18f), TextAlignmentOptions.Left);
            CreateDeskText("GROWTH", new Vector3(-5.94f, 0.16f, 4.34f), flatText, 0.82f, ControlDeskTheme.Lighten(ControlDeskTheme.RivalSlot, 0.16f), TextAlignmentOptions.Left);
            CreateDeskText("SUPPLY", new Vector3(0.85f, 0.16f, 4.34f), flatText, 0.82f, ControlDeskTheme.Lighten(ControlDeskTheme.RivalSlot, 0.16f), TextAlignmentOptions.Left);
            CreateDeskText("SIGNAL", new Vector3(6.52f, 0.16f, 7.78f), flatText, 0.78f, ControlDeskTheme.TextMuted, TextAlignmentOptions.Center);
            CreateDeskText("PLAYER BOARD", new Vector3(0f, 0.16f, -3.95f), flatText, 1.16f, ControlDeskTheme.Lighten(ControlDeskTheme.PlayerBlock, 0.12f), TextAlignmentOptions.Center);

            _tierLabel = CreateDeskText("TRADER", new Vector3(-6.05f, 0.16f, -4.22f), flatText, 0.94f, ControlDeskTheme.MoneyGold, TextAlignmentOptions.Left);
            _districtTrafficLabel = CreateDeskText("CUSTOMER FLOW", new Vector3(-3.45f, 0.16f, 3.20f), flatText, 0.50f, ControlDeskTheme.AccentAmber, TextAlignmentOptions.Center);
            _districtRatingLabel = CreateDeskText("TRUST", new Vector3(0f, 0.16f, 3.20f), flatText, 0.50f, ControlDeskTheme.AccentBlue, TextAlignmentOptions.Center);
            _districtPullLabel = CreateDeskText("PULL", new Vector3(3.45f, 0.16f, 3.20f), flatText, 0.50f, ControlDeskTheme.AccentGreen, TextAlignmentOptions.Center);
            _rivalCardLabel = CreateDeskText("LAST CARD: WAITING", new Vector3(-3.90f, 0.16f, 3.98f), flatText, 0.48f, ControlDeskTheme.TextPrimary, TextAlignmentOptions.Center);
            _rivalStyleLabel = CreateDeskText("PRESSURE: BALANCED", new Vector3(0f, 0.16f, 3.98f), flatText, 0.48f, ControlDeskTheme.TextMuted, TextAlignmentOptions.Center);
            _rivalCrisisLabel = CreateDeskText("THREAT: WATCHING", new Vector3(3.90f, 0.16f, 3.98f), flatText, 0.48f, ControlDeskTheme.TextMuted, TextAlignmentOptions.Center);
        }

        private void BuildBusinessAnchor()
        {
            _businessAnchorVisual = CreateCube("BusinessAnchor", new Vector3(-5.35f, 0.18f, -2.62f), new Vector3(0.92f, 0.48f, 0.92f), ControlDeskTheme.Darken(ControlDeskTheme.OperationSlot, 0.12f));
            Destroy(_businessAnchorVisual.GetComponent<Collider>());
            CreateCube("BusinessAnchorSign", new Vector3(-5.35f, 0.66f, -3.18f), new Vector3(1.40f, 0.18f, 0.10f), ControlDeskTheme.PanelLine).GetComponent<Collider>().enabled = false;
            _businessAnchorLabel = CreateDeskText("NEW VENTURE", new Vector3(-5.35f, 0.72f, -3.18f), Quaternion.Euler(35f, 0f, 0f), 0.68f, ControlDeskTheme.TextPrimary, TextAlignmentOptions.Center);
            _businessAnchorLabel.textWrappingMode = TextWrappingModes.Normal;
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
            tmp.fontStyle = FontStyles.Bold;
            tmp.textWrappingMode = TextWrappingModes.NoWrap;
            tmp.enableAutoSizing = false;
            tmp.outlineWidth = 0.14f;
            tmp.outlineColor = new Color(0f, 0f, 0f, 0.62f);
            return tmp;
        }

        private void OnEnable()
        {
            EventBus.OnBusinessNeglected += HandleBusinessNeglected;
            EventBus.OnEmployeePlaced += HandleSlotRefresh;
            EventBus.OnUpgradePlaced += HandleSlotRefresh;
            EventBus.OnMarketBlocksChanged += HandleTerritoryChanged;
            EventBus.OnBusinessSlotsChanged += UpdateVisibleSlots;
            EventBus.OnTurnStarted += HandleTurnStarted;
            EventBus.OnTurnBriefGenerated += HandleTurnBrief;
            EventBus.OnTurnReportGenerated += HandleTurnReport;
            EventBus.OnRivalActionQueued += HandleRivalActionQueued;
            EventBus.OnCardPlacedInSlot += HandleCardPlacedInSlot;
            EventBus.OnCardRemovedFromSlot += HandleCardRemovedFromSlot;
            LocalizationManager.OnLanguageChanged += ApplyVentureHeaders;
        }

        private void OnDisable()
        {
            EventBus.OnBusinessNeglected -= HandleBusinessNeglected;
            EventBus.OnEmployeePlaced -= HandleSlotRefresh;
            EventBus.OnUpgradePlaced -= HandleSlotRefresh;
            EventBus.OnMarketBlocksChanged -= HandleTerritoryChanged;
            EventBus.OnBusinessSlotsChanged -= UpdateVisibleSlots;
            EventBus.OnTurnStarted -= HandleTurnStarted;
            EventBus.OnTurnBriefGenerated -= HandleTurnBrief;
            EventBus.OnTurnReportGenerated -= HandleTurnReport;
            EventBus.OnRivalActionQueued -= HandleRivalActionQueued;
            EventBus.OnCardPlacedInSlot -= HandleCardPlacedInSlot;
            EventBus.OnCardRemovedFromSlot -= HandleCardRemovedFromSlot;
            LocalizationManager.OnLanguageChanged -= ApplyVentureHeaders;
        }

        private void HandleTurnStarted(int turnNumber)
        {
            ResetRivalVisuals();
            ApplyVentureHeaders();
            UpdateBusinessAnchor();
            RefreshThemeProps();
        }

        private void HandleTerritoryChanged(int playerCount, int rivalCount)
        {
            UpdateTerritoryVisuals(playerCount, rivalCount);
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
            RefreshThemeProps();
        }

        private void HandleCardPlacedInSlot(CardData card, SlotType slotType)
        {
            RefreshThemeProps();
        }

        private void HandleCardRemovedFromSlot(CardData card, SlotType slotType)
        {
            RefreshThemeProps();
        }

        private void HandleTurnBrief(TurnBriefData brief)
        {
            if (brief == null) return;
            var theme = ResolveThemeProfile();

            if (_districtTrafficLabel != null)
                _districtTrafficLabel.text = theme != null ? theme.marketFocusLabel : "TRAFFIC WINDOW";
            if (_districtRatingLabel != null)
                _districtRatingLabel.text = theme != null ? theme.trustFocusLabel : "TRUST LOOP";
            if (_districtPullLabel != null)
                _districtPullLabel.text = theme != null ? theme.pullFocusLabel : "MARKET PULL";
        }

        private void HandleTurnReport(TurnReportData report)
        {
            if (report == null) return;
        }

        private void HandleRivalActionQueued(RivalQueuedAction action)
        {
            if (action == null) return;

            if (_rivalCardLabel != null)
                _rivalCardLabel.text = $"Last Card: {action.displayName}";
            if (_rivalStyleLabel != null)
                _rivalStyleLabel.text = $"Pressure: {action.laneLabel}";
            if (_rivalCrisisLabel != null)
                _rivalCrisisLabel.text = $"Threat: {action.shortDescription}";

            SpawnRivalActionVisual(action);
        }

        private void ApplyVentureHeaders()
        {
            var profile = gameManager != null ? gameManager.ActiveBoardProfile : null;
            var theme = ResolveThemeProfile();
            if (_operationsHeader != null)
                _operationsHeader.text = BuildHeader("board.ops", "OPS", profile != null ? profile.operationSubSlots : null);
            if (_staffHeader != null)
                _staffHeader.text = BuildHeader("board.staff", "STAFF", profile != null ? profile.staffSubSlots : null);
            if (_marketingHeader != null)
                _marketingHeader.text = BuildHeader("board.growth", "GROWTH", profile != null ? profile.marketingSubSlots : null);
            if (_supplierHeader != null)
                _supplierHeader.text = BuildHeader("board.supply", "SUPPLY", profile != null ? profile.supplierSubSlots : null);
            if (_districtTrafficLabel != null)
                _districtTrafficLabel.text = theme != null ? theme.marketFocusLabel : "TRAFFIC WINDOW";
            if (_districtRatingLabel != null)
                _districtRatingLabel.text = theme != null ? theme.trustFocusLabel : "TRUST LOOP";
            if (_districtPullLabel != null)
                _districtPullLabel.text = theme != null ? theme.pullFocusLabel : "MARKET PULL";
            if (_rivalCrisisLabel != null && theme != null && !string.IsNullOrWhiteSpace(theme.rivalPressureLabel))
                _rivalCrisisLabel.text = theme.rivalPressureLabel;
            EnsureThemeProps(theme);
            UpdateBusinessAnchor();
        }

        private void UpdateBusinessAnchor()
        {
            if (_businessAnchorVisual == null || _businessAnchorLabel == null)
                return;

            var gm = gameManager;
            VentureType venture = gm != null && gm.ActiveBoardProfile != null
                ? gm.ActiveBoardProfile.ventureType
                : VentureType.FastFood;
            string displayName = gm != null ? gm.RunDisplayName : "New Venture";
            string categoryLabel = gm != null ? gm.RunCategoryLabel : null;

            _businessAnchorVisual.GetComponent<MeshRenderer>().material.color = venture switch
            {
                VentureType.FastFood => ControlDeskTheme.Darken(new Color(0.77f, 0.29f, 0.17f), 0.1f),
                VentureType.Cafe => ControlDeskTheme.Darken(new Color(0.50f, 0.36f, 0.22f), 0.1f),
                VentureType.TechApp => ControlDeskTheme.Darken(new Color(0.23f, 0.50f, 0.83f), 0.05f),
                VentureType.ClothingStore => ControlDeskTheme.Darken(new Color(0.66f, 0.21f, 0.46f), 0.05f),
                VentureType.GroceryStore => ControlDeskTheme.Darken(new Color(0.23f, 0.60f, 0.28f), 0.05f),
                _ => ControlDeskTheme.OperationSlot
            };

            if (!string.IsNullOrWhiteSpace(categoryLabel))
            {
                _businessAnchorLabel.fontSize = 0.68f;
                _businessAnchorLabel.text = $"{displayName.ToUpperInvariant()}\n{categoryLabel.ToUpperInvariant()}";
            }
            else
            {
                _businessAnchorLabel.fontSize = 0.82f;
                _businessAnchorLabel.text = displayName.ToUpperInvariant();
            }
        }

        private static string BuildHeader(string headerKey, string fallbackHeader, BoardSubSlotDefinition[] _)
        {
            return LocalizationManager.GetWithFallback(headerKey, fallbackHeader).ToUpperInvariant();
        }

        private static void ApplySlotVisuals(IReadOnlyList<SlotZone3D> zones, IReadOnlyList<CardData> cards)
        {
            int count = Mathf.Min(zones.Count, cards.Count);
            for (int i = 0; i < count; i++)
                zones[i].ApplyRestoredCard(cards[i]);
        }

        private void SpawnRivalActionVisual(RivalQueuedAction action)
        {
            GameObject targetSlot = ResolveRivalTargetSlot(action);
            if (targetSlot == null)
                return;

            if (_rivalVisualsBySlot.TryGetValue(targetSlot, out var existing) && existing != null)
                Destroy(existing);

            var visualRoot = new GameObject($"RivalVisual_{action.displayName.Replace(" ", string.Empty)}");
            visualRoot.transform.SetParent(targetSlot.transform, false);
            visualRoot.transform.localPosition = new Vector3(0f, 0.18f, 0f);

            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = "Body";
            block.transform.SetParent(visualRoot.transform, false);
            block.transform.localPosition = Vector3.zero;
            block.transform.localScale = targetSlot.name.Contains("RivalSignal")
                    ? new Vector3(1.06f, 0.18f, 0.74f)
                    : targetSlot.name.Contains("RivalOperation")
                        ? new Vector3(1.14f, 0.18f, 0.46f)
                    : targetSlot.name.Contains("RivalStaff")
                        ? new Vector3(0.90f, 0.18f, 0.34f)
                    : targetSlot.name.Contains("RivalMarketing") || targetSlot.name.Contains("RivalSupplier")
                        ? new Vector3(0.88f, 0.18f, 0.34f)
                    : new Vector3(0.96f, 0.18f, 0.46f);
            block.GetComponent<MeshRenderer>().material.color = GetRivalVisualColor(action);
            Destroy(block.GetComponent<Collider>());

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(visualRoot.transform, false);
            labelGo.transform.localPosition = new Vector3(0f, 0.24f, 0f);
            labelGo.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            labelGo.transform.localScale = Vector3.one * 0.055f;

            var tmp = labelGo.AddComponent<TextMeshPro>();
            tmp.text = BuildRivalVisualLabel(action);
            tmp.fontSize = 5.3f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = ControlDeskTheme.TextPrimary;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.outlineWidth = 0.18f;
            tmp.outlineColor = new Color(0f, 0f, 0f, 0.72f);

            _rivalVisualsBySlot[targetSlot] = visualRoot;
        }

        private GameObject ResolveRivalTargetSlot(RivalQueuedAction action)
        {
            if (action == null)
                return null;

            string lane = action.laneLabel ?? string.Empty;
            if (lane.Contains("Expansion"))
                return NextRivalSlot(_rivalOperationSlots, ref _rivalOperationCursor);
            if (lane.Contains("Price") || lane.Contains("Growth"))
                return NextRivalSlot(_rivalMarketingSlots, ref _rivalMarketingCursor);
            if (lane.Contains("Staff"))
                return NextRivalSlot(_rivalStaffSlots, ref _rivalStaffCursor);
            if (lane.Contains("Quality") || lane.Contains("Capital"))
                return NextRivalSlot(_rivalSupplierSlots, ref _rivalSupplierCursor);

            return NextRivalSlot(_rivalSignalSlots, ref _rivalSignalCursor);
        }

        private static GameObject NextRivalSlot(List<GameObject> slots, ref int cursor)
        {
            if (slots == null || slots.Count == 0)
                return null;

            int index = Mathf.Abs(cursor) % slots.Count;
            cursor++;
            return slots[index];
        }

        private static Color GetRivalVisualColor(RivalQueuedAction action)
        {
            string lane = action != null ? action.laneLabel ?? string.Empty : string.Empty;
            if (lane.Contains("Price") || lane.Contains("Growth"))
                return ControlDeskTheme.Lighten(new Color(0.62f, 0.20f, 0.22f), 0.08f);
            if (lane.Contains("Quality"))
                return new Color(0.52f, 0.18f, 0.18f);
            if (lane.Contains("Staff"))
                return new Color(0.58f, 0.22f, 0.24f);
            if (lane.Contains("Capital"))
                return new Color(0.66f, 0.28f, 0.18f);
            if (lane.Contains("Risk"))
                return ControlDeskTheme.Lighten(ControlDeskTheme.AccentRed, 0.04f);
            return ControlDeskTheme.RivalSlot;
        }

        private static string BuildRivalVisualLabel(RivalQueuedAction action)
        {
            if (action == null)
                return string.Empty;

            string shortName = action.displayName;
            if (shortName.Length > 20)
                shortName = shortName.Substring(0, 20).TrimEnd();

            string lane = string.IsNullOrWhiteSpace(action.laneLabel) ? "PRESSURE" : action.laneLabel.ToUpperInvariant();
            return $"{shortName.ToUpperInvariant()}\n{lane}";
        }

        private void ResetRivalVisuals()
        {
            foreach (var kvp in _rivalVisualsBySlot)
            {
                if (kvp.Value != null)
                    Destroy(kvp.Value);
            }

            _rivalVisualsBySlot.Clear();
            _rivalOperationCursor = 0;
            _rivalStaffCursor = 0;
            _rivalMarketingCursor = 0;
            _rivalSupplierCursor = 0;
            _rivalSignalCursor = 0;

            if (_rivalCardLabel != null)
                _rivalCardLabel.text = "Last Card: Waiting";
            if (_rivalStyleLabel != null)
                _rivalStyleLabel.text = "Pressure: Waiting";
            if (_rivalCrisisLabel != null)
                _rivalCrisisLabel.text = "Threat: Watching";
        }

        private VentureBoardThemeProfile ResolveThemeProfile()
        {
            if (gameManager != null && gameManager.ActiveVentureRuntime != null && gameManager.ActiveVentureRuntime.BoardTheme != null)
                return gameManager.ActiveVentureRuntime.BoardTheme;

            if (gameManager != null && gameManager.SelectedVenture != null)
                return gameManager.SelectedVenture.themeProfile;

            return null;
        }

        private void EnsureThemeProps(VentureBoardThemeProfile theme)
        {
            if (_activeThemeProfile == theme && _themePropRoots.Count > 0)
            {
                RefreshThemeProps();
                return;
            }

            ClearThemeProps();
            _activeThemeProfile = theme;

            if (theme == null || theme.props == null)
                return;

            for (int i = 0; i < theme.props.Length; i++)
                CreateThemeProp(theme.props[i]);

            RefreshThemeProps();
        }

        private void ClearThemeProps()
        {
            foreach (var kvp in _themePropRoots)
            {
                if (kvp.Value != null)
                    Destroy(kvp.Value);
            }

            _themePropRoots.Clear();
            _themePropBodies.Clear();
            _themePropLabels.Clear();
        }

        private void CreateThemeProp(VentureBoardProp prop)
        {
            if (prop == null || string.IsNullOrWhiteSpace(prop.propId))
                return;

            var root = new GameObject($"ThemeProp_{prop.propId}");
            root.transform.SetParent(transform, false);
            root.transform.localPosition = prop.localPosition;
            root.transform.localRotation = Quaternion.identity;

            var body = CreateCube("Body", Vector3.zero, prop.localScale, prop.idleColor);
            body.transform.SetParent(root.transform, false);
            Destroy(body.GetComponent<Collider>());

            var label = CreateDeskText(string.IsNullOrWhiteSpace(prop.label) ? prop.propId.ToUpperInvariant() : prop.label, new Vector3(0f, prop.localScale.y * 0.65f + 0.16f, 0f), Quaternion.Euler(60f, 0f, 0f), 0.62f, ControlDeskTheme.TextMuted, TextAlignmentOptions.Center);
            label.transform.SetParent(root.transform, false);
            label.enableAutoSizing = true;
            label.fontSizeMin = 0.38f;
            label.fontSizeMax = 0.74f;

            _themePropRoots[prop.propId] = root;
            _themePropBodies[prop.propId] = body.GetComponent<MeshRenderer>();
            _themePropLabels[prop.propId] = label;
        }

        private void RefreshThemeProps()
        {
            if (_activeThemeProfile == null || _activeThemeProfile.props == null || slotManager == null)
                return;

            for (int i = 0; i < _activeThemeProfile.props.Length; i++)
            {
                VentureBoardProp prop = _activeThemeProfile.props[i];
                if (prop == null || string.IsNullOrWhiteSpace(prop.propId))
                    continue;

                bool isActive = IsCardPresent(prop.triggerCardId);

                if (_themePropBodies.TryGetValue(prop.propId, out var body) && body != null)
                {
                    body.material.color = isActive ? prop.activeColor : prop.idleColor;
                    body.transform.localScale = isActive ? prop.localScale * 1.08f : prop.localScale;
                }

                if (_themePropLabels.TryGetValue(prop.propId, out var label) && label != null)
                {
                    label.color = isActive ? ControlDeskTheme.TextPrimary : ControlDeskTheme.TextMuted;
                    label.text = isActive
                        ? $"{(string.IsNullOrWhiteSpace(prop.label) ? prop.propId.ToUpperInvariant() : prop.label)} LIVE"
                        : string.IsNullOrWhiteSpace(prop.label) ? prop.propId.ToUpperInvariant() : prop.label;
                }
            }
        }

        private bool IsCardPresent(string cardId)
        {
            if (string.IsNullOrWhiteSpace(cardId))
                return false;

            return ContainsCard(slotManager.OperationSlots, cardId)
                || ContainsCard(slotManager.StaffSlots, cardId)
                || ContainsCard(slotManager.MarketingSlots, cardId)
                || ContainsCard(slotManager.SupplierSlots, cardId)
                || ContainsCard(slotManager.TempEffectSlots, cardId);
        }

        private static bool ContainsCard(IReadOnlyList<CardData> cards, string cardId)
        {
            if (cards == null)
                return false;

            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] != null && cards[i].cardId == cardId)
                    return true;
            }

            return false;
        }
    }
}
