using UnityEngine;
using EmpireOfCards.World;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Thin orchestration layer that delegates runtime wiring to focused helpers.
    /// </summary>
    public static class WiringService
    {
        public static void WireAll(
            GameDataBundle data,
            ManagerBundle managers,
            Board3D board3D,
            CardFactory cardFactory,
            Hand3D hand3D,
            HUDBundle hud,
            Camera mainCamera)
        {
            managers.uiManager = hud.uiManager;

            ManagerReferenceWiring.Wire(data, managers, board3D, cardFactory, hand3D, hud, mainCamera);
            ButtonWiringService.Wire(hud, managers);
            InteractionWiringService.Wire(managers.inputManager3D, managers, board3D);

            Debug.Log("[WiringService] All wiring complete.");
        }
    }
}
