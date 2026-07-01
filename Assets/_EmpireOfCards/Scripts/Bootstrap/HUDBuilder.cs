using UnityEngine;
using UnityEngine.UI;
using EmpireOfCards.UI;
using EmpireOfCards.UI.Popups;

namespace EmpireOfCards.Bootstrap
{
    public class HUDBundle
    {
        public Canvas canvas;
        public TopBarUI topBar;
        public CrisisPopup crisisPopup;
        public TurnReportPanel turnReport;
        public GameSetupPanel gameSetup;
        public DashboardPanel dashboard;
        public StaffPromotionPopup staffPromotion;
        public GameOverScreen gameOver;
        public UIManager uiManager;
    }

    public static class HUDBuilder
    {
        public static HUDBundle Build()
        {
            var bundle = new HUDBundle();

            // 1. Create Canvas
            var canvasGo = new GameObject("[HUD_Canvas]");
            Object.DontDestroyOnLoad(canvasGo);

            bundle.canvas = canvasGo.AddComponent<Canvas>();
            bundle.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            bundle.canvas.sortingOrder = 100;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            // 2. TopBarUI
            var topBarGo = new GameObject("TopBarUI");
            topBarGo.transform.SetParent(canvasGo.transform, false);
            bundle.topBar = topBarGo.AddComponent<TopBarUI>();
            bundle.topBar.Build(canvasGo.transform);

            // 3. CrisisPopup (starts hidden)
            var crisisGo = new GameObject("CrisisPopupHost");
            crisisGo.transform.SetParent(canvasGo.transform, false);
            bundle.crisisPopup = crisisGo.AddComponent<CrisisPopup>();
            bundle.crisisPopup.Build(canvasGo.transform);

            // 4. TurnReportPanel (starts hidden)
            var reportGo = new GameObject("TurnReportHost");
            reportGo.transform.SetParent(canvasGo.transform, false);
            bundle.turnReport = reportGo.AddComponent<TurnReportPanel>();
            bundle.turnReport.Build(canvasGo.transform);

            // 5. GameSetupPanel (starts hidden)
            var setupGo = new GameObject("GameSetupHost");
            setupGo.transform.SetParent(canvasGo.transform, false);
            bundle.gameSetup = setupGo.AddComponent<GameSetupPanel>();
            bundle.gameSetup.Build(canvasGo.transform);

            // 6. DashboardPanel (starts hidden)
            var dashGo = new GameObject("DashboardHost");
            dashGo.transform.SetParent(canvasGo.transform, false);
            bundle.dashboard = dashGo.AddComponent<DashboardPanel>();
            bundle.dashboard.Build(canvasGo.transform);

            // 7. StaffPromotionPopup (starts hidden)
            var promoGo = new GameObject("StaffPromotionHost");
            promoGo.transform.SetParent(canvasGo.transform, false);
            bundle.staffPromotion = promoGo.AddComponent<StaffPromotionPopup>();
            bundle.staffPromotion.Build(canvasGo.transform);

            // 8. GameOverScreen (starts hidden)
            var gameOverGo = new GameObject("GameOverHost");
            gameOverGo.transform.SetParent(canvasGo.transform, false);
            bundle.gameOver = gameOverGo.AddComponent<GameOverScreen>();
            bundle.gameOver.Build(canvasGo.transform);

            // 9. UIManager (central router)
            var uiMgrGo = new GameObject("UIManager");
            uiMgrGo.transform.SetParent(canvasGo.transform, false);
            bundle.uiManager = uiMgrGo.AddComponent<UIManager>();
            bundle.uiManager.Init(
                bundle.topBar, bundle.crisisPopup, bundle.turnReport,
                bundle.gameSetup, bundle.dashboard,
                bundle.staffPromotion, bundle.gameOver);

            Debug.Log("[HUDBuilder] HUD built: TopBar + CrisisPopup + TurnReport + GameSetup + Dashboard + StaffPromotion + GameOver + UIManager.");
            return bundle;
        }
    }
}
