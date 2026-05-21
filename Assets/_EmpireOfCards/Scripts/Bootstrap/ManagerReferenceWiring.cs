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
        }

        public static void WireHUD(ManagerBundle m, HUDBundle hud)
        {
            // HUD wiring will be implemented when UI layer is built.
            // HUD components subscribe to EventBus -- no direct manager refs needed.
        }
    }
}
