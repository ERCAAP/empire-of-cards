using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;
using EmpireOfCards.Presentation;
using EmpireOfCards.UI.Cards;
using EmpireOfCards.UI.Clarity;

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
        private readonly List<GameObject> _rivalPrimarySlots = new List<GameObject>();
        private readonly List<GameObject> _rivalSupportSlots = new List<GameObject>();
        private readonly List<GameObject> _rivalSignalSlots = new List<GameObject>();
        private readonly Dictionary<GameObject, GameObject> _rivalVisualsBySlot = new Dictionary<GameObject, GameObject>();
        private int _rivalPrimaryCursor;
        private int _rivalSupportCursor;
        private int _rivalSignalCursor;

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

        public void Init(BoardManager board)
        {
            boardManager = board;
        }

        public void UpdateVisibleSlots(int maxSlots)
        {
            for (int i = 0; i < _operationSlots.Count; i++)
                _operationSlots[i].gameObject.SetActive(i < maxSlots);
        }

        public void RefreshSlotOccupancyVisuals()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.SlotManager == null)
                return;

            ApplySlotVisuals(_operationSlots, gm.SlotManager.OperationSlots);
            ApplySlotVisuals(_staffSlots, gm.SlotManager.StaffSlots);
            ApplySlotVisuals(_marketingSlots, gm.SlotManager.MarketingSlots);
            ApplySlotVisuals(_supplierSlots, gm.SlotManager.SupplierSlots);
            ApplySlotVisuals(_tempEffectSlots, gm.SlotManager.TempEffectSlots);
            UpdateBusinessAnchor();
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
            CreateFramedPanel("HandDock", new Vector3(0f, 0.06f, -3.25f), new Vector3(10.6f, 0.05f, 1.95f), ControlDeskTheme.PanelSoft, ControlDeskTheme.SurfaceBorder);
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

            BuildBusinessAnchor();
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
            _rivalPrimarySlots.Clear();
            _rivalSupportSlots.Clear();
            _rivalSignalSlots.Clear();
            _rivalVisualsBySlot.Clear();
            _rivalPrimaryCursor = 0;
            _rivalSupportCursor = 0;
            _rivalSignalCursor = 0;

            for (int i = 0; i < 4; i++)
            {
                var rivalSlot = CreateCube($"RivalOp_{i + 1:D2}",
                    new Vector3((i - 1.5f) * 1.95f, 0.11f, 5.95f),
                    new Vector3(1.35f, 0.08f, 0.64f),
                    ControlDeskTheme.RivalSlot);
                Destroy(rivalSlot.GetComponent<Collider>());
                _rivalPrimarySlots.Add(rivalSlot);
            }

            for (int i = 0; i < 3; i++)
            {
                var rivalSupport = CreateCube($"RivalSupport_{i + 1:D2}",
                    new Vector3((i - 1f) * 1.45f, 0.11f, 5.12f),
                    new Vector3(0.92f, 0.08f, 0.44f),
                    ControlDeskTheme.Darken(ControlDeskTheme.RivalSlot, 0.1f));
                Destroy(rivalSupport.GetComponent<Collider>());
                _rivalSupportSlots.Add(rivalSupport);
            }

            var rivalEvent = CreateCube("RivalSignal",
                new Vector3(5.55f, 0.11f, 5.55f),
                new Vector3(1.40f, 0.08f, 1.00f),
                ControlDeskTheme.AccentAmber);
            rivalEvent.GetComponent<MeshRenderer>().material.color = ControlDeskTheme.WithAlpha(ControlDeskTheme.AccentAmber, 0.9f);
            Destroy(rivalEvent.GetComponent<Collider>());
            _rivalSignalSlots.Add(rivalEvent);
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

            _operationsHeader = CreateDeskText("OPERATIONS", new Vector3(-3.75f, 0.16f, -2.10f), flatText, 1.65f, ControlDeskTheme.Lighten(ControlDeskTheme.OperationSlot, 0.25f), TextAlignmentOptions.Left);
            _staffHeader = CreateDeskText("STAFF", new Vector3(-4.20f, 0.16f, -0.78f), flatText, 1.45f, ControlDeskTheme.Lighten(ControlDeskTheme.StaffSlot, 0.25f), TextAlignmentOptions.Left);
            _marketingHeader = CreateDeskText("MARKETING", new Vector3(-5.15f, 0.16f, 0.52f), flatText, 1.25f, ControlDeskTheme.Lighten(ControlDeskTheme.MarketingSlot, 0.22f), TextAlignmentOptions.Left);
            _supplierHeader = CreateDeskText("SUPPLIERS", new Vector3(1.15f, 0.16f, 0.52f), flatText, 1.25f, ControlDeskTheme.Lighten(ControlDeskTheme.SupplierSlot, 0.22f), TextAlignmentOptions.Left);
            _operationsHeader.textWrappingMode = TextWrappingModes.Normal;
            _staffHeader.textWrappingMode = TextWrappingModes.Normal;
            _marketingHeader.textWrappingMode = TextWrappingModes.Normal;
            _supplierHeader.textWrappingMode = TextWrappingModes.Normal;
            CreateDeskText("CONTROL", new Vector3(5.35f, 0.16f, 1.55f), flatText, 1.15f, ControlDeskTheme.Lighten(ControlDeskTheme.UtilitySlot, 0.18f), TextAlignmentOptions.Left);
            CreateDeskText("SHARED MARKET", new Vector3(-5.45f, 0.16f, 2.95f), flatText, 1.20f, ControlDeskTheme.TextMuted, TextAlignmentOptions.Left);
            CreateDeskText("RIVAL PRESSURE", new Vector3(-5.45f, 0.16f, 5.05f), flatText, 1.15f, ControlDeskTheme.Lighten(ControlDeskTheme.RivalSlot, 0.20f), TextAlignmentOptions.Left);
            CreateDeskText("RIVAL BUILD", new Vector3(-5.25f, 0.16f, 6.32f), flatText, 0.92f, ControlDeskTheme.Lighten(ControlDeskTheme.RivalSlot, 0.28f), TextAlignmentOptions.Left);
            CreateDeskText("RIVAL SUPPORT", new Vector3(-1.15f, 0.16f, 5.38f), flatText, 0.82f, ControlDeskTheme.TextMuted, TextAlignmentOptions.Left);
            CreateDeskText("RIVAL SIGNAL", new Vector3(4.65f, 0.16f, 5.05f), flatText, 0.82f, ControlDeskTheme.TextMuted, TextAlignmentOptions.Left);
            CreateDeskText("HAND", new Vector3(-3.95f, 0.16f, -3.95f), flatText, 1.10f, ControlDeskTheme.TextMuted, TextAlignmentOptions.Left);
            CreateDeskText("PLAY", new Vector3(5.55f, 0.16f, -3.10f), flatText, 1.05f, ControlDeskTheme.Lighten(ControlDeskTheme.ActionSlot, 0.20f), TextAlignmentOptions.Center);
            CreateDeskText("SELL", new Vector3(5.55f, 0.16f, -1.85f), flatText, 1.05f, ControlDeskTheme.Lighten(ControlDeskTheme.UtilitySlot, 0.22f), TextAlignmentOptions.Center);
            CreateDeskText("EVENTS", new Vector3(5.55f, 0.16f, 1.45f), flatText, 1.00f, ControlDeskTheme.Lighten(ControlDeskTheme.EventSlot, 0.22f), TextAlignmentOptions.Center);

            _tierLabel = CreateDeskText("TRADER", new Vector3(-5.95f, 0.16f, -2.95f), flatText, 1.25f, ControlDeskTheme.MoneyGold, TextAlignmentOptions.Left);
            _districtTrafficLabel = CreateDeskText("TRAFFIC WINDOW", new Vector3(-5.35f, 0.16f, 3.35f), flatText, 0.82f, ControlDeskTheme.AccentAmber, TextAlignmentOptions.Left);
            _districtRatingLabel = CreateDeskText("TRUST LOOP", new Vector3(-1.35f, 0.16f, 3.35f), flatText, 0.82f, ControlDeskTheme.AccentBlue, TextAlignmentOptions.Left);
            _districtPullLabel = CreateDeskText("MARKET PULL", new Vector3(2.65f, 0.16f, 3.35f), flatText, 0.82f, ControlDeskTheme.AccentGreen, TextAlignmentOptions.Left);
            _rivalCardLabel = CreateDeskText("Last Card: Waiting", new Vector3(-5.35f, 0.16f, 5.35f), flatText, 0.80f, ControlDeskTheme.TextPrimary, TextAlignmentOptions.Left);
            _rivalStyleLabel = CreateDeskText("Pressure: Balanced", new Vector3(-1.35f, 0.16f, 5.35f), flatText, 0.80f, ControlDeskTheme.TextMuted, TextAlignmentOptions.Left);
            _rivalCrisisLabel = CreateDeskText("Crisis Read: Clean", new Vector3(2.65f, 0.16f, 5.35f), flatText, 0.80f, ControlDeskTheme.TextMuted, TextAlignmentOptions.Left);
        }

        private void BuildBusinessAnchor()
        {
            _businessAnchorVisual = CreateCube("BusinessAnchor", new Vector3(-5.35f, 0.18f, -1.45f), new Vector3(1.05f, 0.55f, 1.05f), ControlDeskTheme.Darken(ControlDeskTheme.OperationSlot, 0.12f));
            Destroy(_businessAnchorVisual.GetComponent<Collider>());
            CreateCube("BusinessAnchorSign", new Vector3(-5.35f, 0.72f, -0.80f), new Vector3(1.55f, 0.22f, 0.12f), ControlDeskTheme.PanelLine).GetComponent<Collider>().enabled = false;
            _businessAnchorLabel = CreateDeskText("NEW VENTURE", new Vector3(-5.35f, 0.79f, -0.80f), Quaternion.Euler(35f, 0f, 0f), 0.82f, ControlDeskTheme.TextPrimary, TextAlignmentOptions.Center);
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
            EventBus.OnTurnStarted += HandleTurnStarted;
            EventBus.OnTurnBriefGenerated += HandleTurnBrief;
            EventBus.OnTurnReportGenerated += HandleTurnReport;
            EventBus.OnRivalActionQueued += HandleRivalActionQueued;
            LocalizationManager.OnLanguageChanged += ApplyVentureHeaders;
        }

        private void OnDisable()
        {
            EventBus.OnCompanyTierChanged -= HandleTierChanged;
            EventBus.OnBusinessNeglected -= HandleBusinessNeglected;
            EventBus.OnEmployeePlaced -= HandleSlotRefresh;
            EventBus.OnUpgradePlaced -= HandleSlotRefresh;
            EventBus.OnMarketBlocksChanged -= HandleTerritoryChanged;
            EventBus.OnBusinessSlotsChanged -= UpdateVisibleSlots;
            EventBus.OnTurnStarted -= HandleTurnStarted;
            EventBus.OnTurnBriefGenerated -= HandleTurnBrief;
            EventBus.OnTurnReportGenerated -= HandleTurnReport;
            EventBus.OnRivalActionQueued -= HandleRivalActionQueued;
            LocalizationManager.OnLanguageChanged -= ApplyVentureHeaders;
        }

        private void HandleTurnStarted(int turnNumber)
        {
            ApplyVentureHeaders();
            UpdateBusinessAnchor();
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

        private void HandleTurnBrief(TurnBriefData brief)
        {
            if (brief == null) return;

            if (_districtTrafficLabel != null)
                _districtTrafficLabel.text = "TRAFFIC WINDOW";
            if (_districtRatingLabel != null)
                _districtRatingLabel.text = "TRUST LOOP";
            if (_districtPullLabel != null)
                _districtPullLabel.text = "MARKET PULL";
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
            var profile = GameManager.Instance != null ? GameManager.Instance.ActiveBoardProfile : null;
            if (_operationsHeader != null)
                _operationsHeader.text = BuildHeader("board.ops", "OPS", profile != null ? profile.operationSubSlots : null);
            if (_staffHeader != null)
                _staffHeader.text = BuildHeader("board.staff", "STAFF", profile != null ? profile.staffSubSlots : null);
            if (_marketingHeader != null)
                _marketingHeader.text = BuildHeader("board.growth", "GROWTH", profile != null ? profile.marketingSubSlots : null);
            if (_supplierHeader != null)
                _supplierHeader.text = BuildHeader("board.supply", "SUPPLY", profile != null ? profile.supplierSubSlots : null);
            UpdateBusinessAnchor();
        }

        private void UpdateBusinessAnchor()
        {
            if (_businessAnchorVisual == null || _businessAnchorLabel == null)
                return;

            var gm = GameManager.Instance;
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

        private static string BuildHeader(string headerKey, string fallbackHeader, BoardSubSlotDefinition[] defs)
        {
            string prefix = LocalizationManager.GetWithFallback(headerKey, fallbackHeader);
            SlotType slotType = headerKey switch
            {
                "board.ops" => SlotType.Operation,
                "board.staff" => SlotType.Staff,
                "board.growth" => SlotType.Marketing,
                "board.supply" => SlotType.Supplier,
                _ => SlotType.Operation
            };
            string purpose = GameClarityFormatter.GetSlotPurposeTitle(slotType).ToUpperInvariant();
            if (defs == null || defs.Length == 0)
                return $"{prefix}\n{purpose}";

            int count = Mathf.Min(defs.Length, 3);
            string label = $"{prefix}  {purpose}\n";
            for (int i = 0; i < count; i++)
            {
                if (i > 0) label += " / ";
                string fallback = string.IsNullOrWhiteSpace(defs[i].fallbackLabel) ? defs[i].id : defs[i].fallbackLabel;
                label += LocalizationManager.GetWithFallback(defs[i].labelKey, fallback).ToUpperInvariant();
            }
            return label;
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
            block.transform.localScale = targetSlot.name.Contains("RivalSupport")
                ? new Vector3(0.68f, 0.18f, 0.34f)
                : targetSlot.name.Contains("RivalSignal")
                    ? new Vector3(1.06f, 0.18f, 0.74f)
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
            tmp.fontSize = 4.6f;
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
            if (lane.Contains("Price") || lane.Contains("Growth") || lane.Contains("Expansion"))
                return NextRivalSlot(_rivalPrimarySlots, ref _rivalPrimaryCursor);

            if (lane.Contains("Quality") || lane.Contains("Staff") || lane.Contains("Capital"))
                return NextRivalSlot(_rivalSupportSlots, ref _rivalSupportCursor);

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
                return ControlDeskTheme.Lighten(ControlDeskTheme.RivalSlot, 0.14f);
            if (lane.Contains("Quality"))
                return ControlDeskTheme.Darken(ControlDeskTheme.AccentBlue, 0.05f);
            if (lane.Contains("Staff"))
                return ControlDeskTheme.Darken(ControlDeskTheme.AccentGreen, 0.10f);
            if (lane.Contains("Capital"))
                return ControlDeskTheme.AccentAmber;
            if (lane.Contains("Risk"))
                return ControlDeskTheme.AccentRed;
            return ControlDeskTheme.RivalSlot;
        }

        private static string BuildRivalVisualLabel(RivalQueuedAction action)
        {
            if (action == null)
                return string.Empty;

            string shortName = action.displayName;
            if (shortName.Length > 16)
                shortName = shortName.Substring(0, 16).TrimEnd();

            string lane = string.IsNullOrWhiteSpace(action.laneLabel) ? "PRESSURE" : action.laneLabel.ToUpperInvariant();
            return $"{shortName.ToUpperInvariant()}\n{lane}";
        }
    }
}
