using System.Linq;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.World;

namespace EmpireOfCards.Bootstrap
{
    public static class ManagerReferenceWiring
    {
        public static void Wire(
            GameDataBundle data,
            ManagerBundle m,
            Board3D board3D,
            CardFactory cardFactory,
            Hand3D hand3D,
            HUDBundle hud,
            UnityEngine.Camera mainCamera)
        {
            m.gameManager.Init(
                data.balanceData, data.startingDeck,
                m.turnManager, m.economyManager, m.deckManager, m.boardManager,
                m.comboSystem, m.territoryManager, m.fbiSystem, m.rivalAI,
                m.shopManager, m.uiManager, m.audioManager, m.vfxManager, m.saveManager);
            m.gameManager.SetCardLookup(data.cardLookup);

            m.economyManager.Init(data.balanceData, m.boardManager, m.comboSystem, m.abilitySystem, m.slotManager);
            m.comboSystem.Init(data.combos, m.boardManager);
            m.fbiSystem.Init(data.balanceData, m.boardManager, m.comboSystem);
            m.rivalAI.Init(data.rivalData);
            m.metaProgressionSystem.Init(data.metaProgressionData);
            m.gameManager.SetMetaProgressionSystem(m.metaProgressionSystem);

            m.companyTierSystem.Init(m.boardManager, m.comboSystem);
            m.gameManager.SetCompanyTierSystem(m.companyTierSystem);

            m.slotManager.Init();
            m.gameManager.SetSlotManager(m.slotManager);
            m.boardManager.Init(m.slotManager);

            m.gameManager.SetStaffStateSystem(m.staffStateSystem);

            m.chainReactionSystem.Init(m.boardManager, m.economyManager, m.staffStateSystem);
            m.gameManager.SetChainReactionSystem(m.chainReactionSystem);

            m.shopManager.Init(data.shopPool, m.deckManager, m.economyManager, m.comboSystem);
            m.audioManager.Init(m.musicSourceA, m.musicSourceB, m.sfxSource);

            m.uiManager.Init(
                hud.topBarUI, hud.actionBarUI, hud.shopPanel,
                hud.comboPopup, hud.eventPopup, hud.rivalPopup,
                hud.scoreScreen, hud.gameOverScreen);

            if (hud.neglectWarningText != null)
                m.uiManager.SetNeglectWarningText(hud.neglectWarningText);
            if (hud.turnBriefText != null || hud.turnReportText != null)
                m.uiManager.SetFlowTexts(hud.turnBriefText, hud.turnReportText);
            if (hud.clarityPanel != null)
                m.uiManager.SetClarityPanel(hud.clarityPanel);
            if (hud.analyticsPanel != null)
                m.uiManager.SetAnalyticsPanel(hud.analyticsPanel);

            hud.topBarUI.Init(hud.moneyText, hud.turnText, hud.fbiBarFillImg, null, hud.companyTierText, hud.buildIdentityText, hud.pressureText);
            hud.actionBarUI.Init(hud.actionDotImages);
            hud.shopPanel.Init(m.shopManager);

            if (hud.shopBiasText != null)
                hud.shopPanel.SetBiasText(hud.shopBiasText);

            m.inputManager3D.InitRuntime(mainCamera, m.gameManager, m.abilitySystem, board3D.AllSlots);

            hand3D.Init(cardFactory, m.deckManager, hand3D.transform.parent);
            board3D.Init(m.gameManager, m.boardManager, m.slotManager);
            cardFactory.Init(data.allCards);

            var eventCards = data.allCards
                .Where(c => c != null && c.cardType == CardType.Event && c.cardFamily == CardFamily.Crisis)
                .ToList();
            m.turnManager.SetEventDeck(eventCards);
        }
    }
}
