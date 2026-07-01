using EmpireOfCards.Bootstrap.Data;

namespace EmpireOfCards.Bootstrap
{
    public static class WiringService
    {
        /// <summary>
        /// Single entry point: wires all managers with their dependencies.
        /// Called once during bootstrap, after all objects are created.
        /// </summary>
        public static void WireAll(ManagerBundle managers, GameDataBundle data, HUDBundle hud)
        {
            ManagerReferenceWiring.WireManagers(managers, data);
            ManagerReferenceWiring.WireHUD(managers, hud);
        }
    }
}
