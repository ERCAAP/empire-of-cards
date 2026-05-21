using UnityEngine;

namespace EmpireOfCards.Bootstrap
{
    public class HUDBundle
    {
        // Will hold references to HUD panels when UI layer is built.
        // For now this is a placeholder to keep the pipeline shape intact.
    }

    public static class HUDBuilder
    {
        public static HUDBundle Build()
        {
            // UI construction will happen here once the UI layer exists.
            Debug.Log("[HUDBuilder] HUD placeholder created.");
            return new HUDBundle();
        }
    }
}
