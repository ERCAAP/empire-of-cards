using UnityEngine;

namespace EmpireOfCards.Presentation
{
    /// <summary>
    /// Shared visual language for the runtime-built control desk presentation.
    /// Keeps board, HUD, cards, and interaction feedback on the same palette.
    /// </summary>
    public static class ControlDeskTheme
    {
        public static readonly Color SceneBackground = new Color(0.07f, 0.07f, 0.08f);
        public static readonly Color AmbientWarm = new Color(0.16f, 0.13f, 0.11f);

        public static readonly Color DeskOuter = new Color(0.12f, 0.09f, 0.07f);
        public static readonly Color DeskInner = new Color(0.18f, 0.15f, 0.12f);
        public static readonly Color FeltSurface = new Color(0.13f, 0.11f, 0.10f);
        public static readonly Color SurfaceBorder = new Color(0.30f, 0.24f, 0.19f);
        public static readonly Color Divider = new Color(0.42f, 0.36f, 0.28f);

        public static readonly Color PlayerBand = new Color(0.14f, 0.12f, 0.11f);
        public static readonly Color MidBand = new Color(0.15f, 0.13f, 0.12f);
        public static readonly Color RivalBand = new Color(0.12f, 0.09f, 0.09f);

        public static readonly Color OperationLane = new Color(0.15f, 0.19f, 0.27f);
        public static readonly Color StaffLane = new Color(0.14f, 0.24f, 0.18f);
        public static readonly Color MarketingLane = new Color(0.23f, 0.16f, 0.25f);
        public static readonly Color SupplierLane = new Color(0.28f, 0.22f, 0.14f);
        public static readonly Color UtilityLane = new Color(0.24f, 0.15f, 0.10f);
        public static readonly Color MarketLane = new Color(0.17f, 0.16f, 0.14f);
        public static readonly Color RivalLane = new Color(0.23f, 0.11f, 0.11f);

        public static readonly Color OperationSlot = new Color(0.27f, 0.35f, 0.52f);
        public static readonly Color StaffSlot = new Color(0.30f, 0.48f, 0.34f);
        public static readonly Color MarketingSlot = new Color(0.58f, 0.35f, 0.60f);
        public static readonly Color SupplierSlot = new Color(0.62f, 0.47f, 0.24f);
        public static readonly Color UtilitySlot = new Color(0.71f, 0.42f, 0.22f);
        public static readonly Color ActionSlot = new Color(0.73f, 0.25f, 0.24f);
        public static readonly Color EventSlot = new Color(0.65f, 0.37f, 0.18f);
        public static readonly Color RivalSlot = new Color(0.53f, 0.19f, 0.19f);
        public static readonly Color NeutralBlock = new Color(0.46f, 0.46f, 0.44f);
        public static readonly Color PlayerBlock = new Color(0.26f, 0.58f, 0.96f);
        public static readonly Color RivalBlock = new Color(0.89f, 0.25f, 0.25f);

        public static readonly Color Panel = new Color(0.08f, 0.08f, 0.10f, 0.88f);
        public static readonly Color PanelSoft = new Color(0.11f, 0.10f, 0.10f, 0.78f);
        public static readonly Color PanelLine = new Color(0.31f, 0.27f, 0.21f, 0.95f);
        public static readonly Color TextPrimary = new Color(0.94f, 0.91f, 0.84f);
        public static readonly Color TextMuted = new Color(0.65f, 0.63f, 0.58f);
        public static readonly Color MoneyGold = new Color(1.00f, 0.83f, 0.26f);
        public static readonly Color AccentBlue = new Color(0.34f, 0.73f, 1.00f);
        public static readonly Color AccentGreen = new Color(0.36f, 0.88f, 0.50f);
        public static readonly Color AccentRed = new Color(0.95f, 0.34f, 0.30f);
        public static readonly Color AccentAmber = new Color(0.98f, 0.66f, 0.26f);

        public static readonly Color ValidHighlight = new Color(0.45f, 0.84f, 0.56f, 0.95f);
        public static readonly Color InvalidHighlight = new Color(0.95f, 0.35f, 0.28f, 0.95f);
        public static readonly Color GuidedPulse = new Color(0.55f, 0.71f, 0.92f, 0.72f);

        public static Color WithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Color Lighten(Color color, float amount)
        {
            return Color.Lerp(color, Color.white, Mathf.Clamp01(amount));
        }

        public static Color Darken(Color color, float amount)
        {
            return Color.Lerp(color, Color.black, Mathf.Clamp01(amount));
        }
    }
}
