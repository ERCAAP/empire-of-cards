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

            // 5. UIManager (central router)
            var uiMgrGo = new GameObject("UIManager");
            uiMgrGo.transform.SetParent(canvasGo.transform, false);
            bundle.uiManager = uiMgrGo.AddComponent<UIManager>();
            bundle.uiManager.Init(bundle.topBar, bundle.crisisPopup, bundle.turnReport);

            Debug.Log("[HUDBuilder] HUD built: TopBar + CrisisPopup + TurnReport + UIManager.");
            return bundle;
        }
    }
}
