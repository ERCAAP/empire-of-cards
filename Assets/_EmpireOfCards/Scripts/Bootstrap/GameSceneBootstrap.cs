using UnityEngine;
using EmpireOfCards.Bootstrap.Data;
using EmpireOfCards.Core;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Scene entry point. Attach to a single GameObject in the game scene.
    /// Runs the full bootstrap pipeline: Create -> Build -> Wire -> Start.
    /// </summary>
    public class GameSceneBootstrap : MonoBehaviour
    {
        ManagerBundle _managers;

        void Awake()
        {
            EventBus.ClearAll();

            // 1. Create all manager GameObjects
            _managers = ManagerFactory.CreateAll();

            // 2. Create all runtime card data and sector profiles
            GameDataBundle data = ContentFactory.CreateAllData();

            // 3. Build HUD (placeholder until UI layer is implemented)
            HUDBundle hud = HUDBuilder.Build();

            // 4. Wire everything together
            WiringService.WireAll(_managers, data, hud);

            Debug.Log("[Bootstrap] Pipeline complete. Managers wired.");
        }

        void Start()
        {
            // MVP: skip setup UI, start directly with Restaurant sector
            _managers.gameManager.StartNewRun("Benim Dukkanim", SectorType.Restaurant);
        }

        void OnDestroy()
        {
            EventBus.ClearAll();
        }
    }
}
