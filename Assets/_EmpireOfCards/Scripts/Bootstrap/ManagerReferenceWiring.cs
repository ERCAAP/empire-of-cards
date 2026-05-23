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
            WireCore(data, m);
            WireUI(m, hud);
            WireWorld(data, m, board3D, cardFactory, hand3D, mainCamera);
        }

        private static void WireCore(GameDataBundle data, ManagerBundle m)
        {
            m.gameManager.Init(
                data.balanceData,
                m.turnManager, m.economyManager, m.deckManager, m.boardManager,
                m.questionManager, m.decisionHistoryManager, m.customerFlowManager,
                m.marketShareVisualizer, m.rivalAI,
                m.shopManager, m.uiManager, m.audioManager, m.vfxManager, m.saveManager,
                m.slotManager, m.staffStateSystem, m.chainReactionSystem);
            m.gameManager.SetCardLookup(data.cardLookup);

            m.economyManager.Init(data.balanceData, m.boardManager, m.abilitySystem, m.slotManager, m.staffStateSystem);
            m.rivalAI.Init(data.rivalData);
            m.slotManager.Init();
            m.boardManager.Init(m.slotManager);
            m.decisionHistoryManager.ResetState();
            m.customerFlowManager.ResetState();
            m.questionManager.Init(m.decisionHistoryManager);
            m.chainReactionSystem.Init(m.boardManager, m.economyManager, m.staffStateSystem);
            m.shopManager.Init(data.shopPool, m.deckManager, m.economyManager);
            m.audioManager.Init(m.musicSourceA, m.musicSourceB, m.sfxSource);
        }

        private static void WireUI(ManagerBundle m, HUDBundle hud)
        {
            m.uiManager.Init(
                hud.topBarUI, hud.actionBarUI, hud.shopPanel,
                hud.eventPopup, hud.rivalPopup,
                hud.scoreScreen, hud.gameOverScreen);

            if (hud.neglectWarningText != null)
                m.uiManager.SetNeglectWarningText(hud.neglectWarningText);
            if (hud.turnBriefText != null || hud.turnReportText != null)
                m.uiManager.SetFlowTexts(hud.turnBriefText, hud.turnReportText);
            if (hud.clarityPanel != null)
                m.uiManager.SetClarityPanel(hud.clarityPanel);
            if (hud.analyticsPanel != null)
                m.uiManager.SetAnalyticsPanel(hud.analyticsPanel);
            if (hud.boardGuidePanel != null)
                m.uiManager.SetBoardGuidePanel(hud.boardGuidePanel);
            if (hud.rivalIntentPanel != null)
                m.uiManager.SetRivalIntentPanel(hud.rivalIntentPanel);

            hud.topBarUI.Init(hud.moneyText, hud.turnText, hud.companyTierText, hud.buildIdentityText, hud.pressureText);
            hud.actionBarUI.Init(hud.actionDotImages);
            hud.shopPanel.Init(m.shopManager);
            if (hud.shopBiasText != null)
                hud.shopPanel.SetBiasText(hud.shopBiasText);
        }

        private static void WireWorld(
            GameDataBundle data,
            ManagerBundle m,
            Board3D board3D,
            CardFactory cardFactory,
            Hand3D hand3D,
            UnityEngine.Camera mainCamera)
        {
            m.inputManager3D.InitRuntime(mainCamera, m.gameManager, m.abilitySystem, board3D.AllSlots, board3D.QuestionZones);
            BoardStageAuthoring stage = board3D != null ? board3D.StageAuthoring : null;
            hand3D.Init(
                cardFactory,
                m.deckManager,
                stage != null && stage.BuildHandAnchor != null ? stage.BuildHandAnchor : hand3D.transform.parent,
                stage != null && stage.ResponseHandAnchor != null ? stage.ResponseHandAnchor : hand3D.transform.parent);
            board3D.Init(m.gameManager, m.boardManager, m.slotManager, stage);
            board3D.RefreshSlotOccupancyVisuals();
            cardFactory.Init(data.allCards);

            var eventCards = data.allCards
                .Where(c => c != null && c.cardType == CardType.Event && c.cardFamily == CardFamily.Crisis)
                .ToList();
            m.turnManager.SetEventDeck(eventCards);
        }
    }
}
