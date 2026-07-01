using UnityEngine;
using EmpireOfCards.Bootstrap.Data;
using EmpireOfCards.Core;

namespace EmpireOfCards.Bootstrap
{
    public static class ManagerReferenceWiring
    {
        public static void WireManagers(ManagerBundle m, GameDataBundle data)
        {
            // SlotManager first -- BoardManager depends on it
            m.slotManager.Init(data.sectorProfiles);

            // BoardManager needs SlotManager for capacity checks
            m.boardManager.Init(m.slotManager);

            // DeckManager needs the card pool
            m.deckManager.Init(data.allCards, data.shopPool);

            // EconomyManager is standalone at init
            m.economyManager.Init();

            // Systems that observe board state
            m.hygieneSystem.Init(m.boardManager);
            m.staffSystem.Init(m.boardManager);
            m.customerSystem.Init(m.boardManager, m.economyManager);
            m.crisisSystem.Init(m.boardManager, m.economyManager);

            // EconomyManager needs subsystem references for resolve pipeline
            m.economyManager.SetReferences(m.boardManager, m.customerSystem,
                m.staffSystem, m.hygieneSystem);

            // CrisisChainSystem needs the crisis card pool
            m.crisisSystem.SetCrisisPool(
                System.Array.FindAll(data.allCards,
                    c => c.cardFamily == CardFamily.Crisis));

            // Rival needs economy info for decisions
            m.rivalAI.Init(m.economyManager);

            // GameManager uses property setters (Singleton pattern)
            m.gameManager.TurnManager = m.turnManager;
            m.gameManager.BoardManager = m.boardManager;
            m.gameManager.EconomyManager = m.economyManager;
            m.gameManager.DeckManager = m.deckManager;
            m.gameManager.SlotManager = m.slotManager;
            m.gameManager.RivalAI = m.rivalAI;

            // Audio + Save are independent
            m.audioManager.Init();
            m.saveManager.Init(m.gameManager);

            // ── 3D World wiring ─────────────────────────────────────

            // Initialize DOTween
            DG.Tweening.DOTween.Init(false, true, DG.Tweening.LogBehaviour.ErrorsOnly);
            DG.Tweening.DOTween.defaultAutoPlay = DG.Tweening.AutoPlay.All;

            // Build the board (zones + slot markers)
            m.board3D.BuildBoard();
            m.cardFactory3D.transform.SetParent(m.board3D.transform);

            // Build the full 3D scene (ground, walls, lighting, decorations)
            m.sceneBuilder.Build();

            // Camera system
            m.cameraController.Init();

            // Hand3D
            m.hand3D.Init(m.cardFactory3D);
            m.hand3D.transform.position = m.board3D.HandZone != null
                ? m.board3D.HandZone.position + Vector3.up * 0.2f
                : new Vector3(0f, 0.2f, -6f);

            // Input (use CameraController's main camera reference)
            m.inputManager3D.Init(m.hand3D, m.board3D, m.boardManager);
        }

        public static void WireHUD(ManagerBundle m, HUDBundle hud)
        {
            // HUD components subscribe to EventBus for stat updates.
            // UIManager is already wired inside HUDBuilder.Build().
            // No additional direct references needed.
        }
    }
}
