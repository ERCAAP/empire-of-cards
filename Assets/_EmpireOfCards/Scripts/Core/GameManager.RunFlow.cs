using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core.GameStates;
using EmpireOfCards.Data;
using EmpireOfCards.Save;

namespace EmpireOfCards.Core
{
    public partial class GameManager
    {
        private void ResetRunState()
        {
            resources.Reset(balanceData);
            fbiRisk = 0f;
            playerCustomers = 0;
            rivalCustomers = 0;
            playerMarketBlocks = 0;
            rivalMarketBlocks = 0;
        }

        private void PrepareNewRunRuntime(bool autoStartTurn)
        {
            VentureType chosenVenture = selectedVenture != null ? selectedVenture.ventureType : VentureType.FastFood;

            if (activeBoardProfile != null && slotManager != null)
            {
                slotManager.Configure(activeBoardProfile);
                resources.SetBusinessSlots(activeBoardProfile.startingOperationSlots);
            }

            if (boardManager != null)
            {
                boardManager.Reset();
                boardManager.ConfigureForVenture(chosenVenture, activeBoardProfile);
                boardManager.SetMaxSlots(resources.BusinessSlots);
            }

            if (activeEconomyProfile != null && economyManager != null)
            {
                economyManager.SetActiveProfile(activeEconomyProfile);
                resources.SetMoney(Mathf.RoundToInt(activeEconomyProfile.startingCash) + (selectedVenture != null ? selectedVenture.bonusMoney : 0));
                economyManager.SyncCashFromResources(resources.Money);
            }

            if (deckManager != null && activeDeckProfile != null && _cardLookup != null)
                deckManager.InitializeDeck(activeDeckProfile, _cardLookup);

            if (selectedVenture != null)
            {
                if (selectedVenture.startingBusiness != null && boardManager != null)
                    boardManager.PlaceBusiness(selectedVenture.startingBusiness, 0);
                if (selectedVenture.bonusDeckCard != null && deckManager != null)
                    deckManager.AddCardToDeck(selectedVenture.bonusDeckCard);
            }

            if (rivalAI != null)
            {
                if (selectedVenture != null)
                    rivalAI.Initialize(chosenVenture);
                else
                    rivalAI.Initialize();
            }

            if (shopManager != null)
            {
                if (selectedVenture != null)
                {
                    shopManager.SetVentureBias(chosenVenture);
                    shopManager.FilterPoolByVenture(chosenVenture);
                }

                shopManager.RefreshShop();
            }

            staffStateSystem?.Reset();
            chainReactionSystem?.Reset();
            WinLoseChecker.Reset();

            EventBus.MoneyUpdated(resources.Money);
            EventBus.MarketBlocksUpdated(0, 0);
            EventBus.LegalRiskUpdated(0);

            SetGameState(GameState.Playing);
            _gameStateMachine.Initialize(new InGameState());

            if (autoStartTurn)
                StartNextTurn();
        }

        private void RestoreRunState(RunSaveData runData)
        {
            SetRunDisplayName(runData.runName);
            SetRunCategory(runData.runCategoryId, runData.runCategoryLabel);
            currentTurn = Mathf.Max(1, runData.currentTurn);

            resources.SetMoney(runData.playerMoney);
            resources.SetActions(runData.playerActions, runData.playerMaxActions);
            resources.SetBusinessSlots(runData.playerBusinessSlots);
            fbiRisk = runData.fbiRisk;
            playerCustomers = Mathf.Max(0, runData.playerCustomers);
            rivalCustomers = Mathf.Max(0, runData.rivalCustomers);
            SetMarketBlocks(runData.playerMarketBlocks, runData.rivalMarketBlocks);
            EventBus.LegalRiskUpdated(Mathf.RoundToInt(fbiRisk * 100f));

            if (activeBoardProfile != null && slotManager != null)
                slotManager.Configure(activeBoardProfile);

            slotManager?.RestoreState(
                ResolveCardsFromIds(runData.operationSlotIds),
                ResolveCardsFromIds(runData.staffSlotIds),
                ResolveCardsFromIds(runData.marketingSlotIds),
                ResolveCardsFromIds(runData.supplierSlotIds),
                ResolveCardsFromIds(runData.tempEffectSlotIds));
            boardManager?.RebuildFromSlots();

            deckManager?.RestoreState(
                activeDeckProfile,
                _cardLookup,
                runData.drawPileIds,
                runData.handIds,
                runData.discardPileIds,
                runData.redrawsRemaining);

            if (economyManager != null && runData.economySnapshot != null)
                economyManager.RestoreSnapshot(runData.economySnapshot);
            else
                economyManager?.SyncCashFromResources(resources.Money);

            activeVentureRuntime?.RestoreState(runData.ventureRuntimeState, runData.openingArcState, runData.eventChainState);
            activeVentureRuntime?.OnTurnStarted(currentTurn);
            rivalAI?.RestoreState(runData.rivalState);

            EventBus.MoneyUpdated(resources.Money);
            EventBus.TurnStarted(currentTurn);
            turnManager?.ResumePlayPhase(currentTurn);
        }

        private bool TryContinueRunAfterTurn()
        {
            int winCustomers = Constants.WIN_CUSTOMER_SHARE;
            bool dominationActive = currentTurn >= Constants.DOMINATION_CHECK_START_TURN;

            if (dominationActive && WinLoseChecker.CheckWin(playerCustomers, winCustomers))
            {
                EndRun(true);
                return false;
            }

            if (resources.Money <= 0)
            {
                EndRun(false);
                return false;
            }

            if (dominationActive && rivalCustomers >= winCustomers)
            {
                EndRun(false);
                return false;
            }

            if (economyManager != null && economyManager.Snapshot != null &&
                WinLoseChecker.CheckExtendedLose(economyManager.Snapshot.rating, economyManager.Snapshot.legalRisk))
            {
                EndRun(false);
                return false;
            }

            if (currentTurn >= MaxTurns)
            {
                EndRun(playerCustomers >= rivalCustomers);
                return false;
            }

            return true;
        }

        private RunSaveData BuildRunSaveData()
        {
            return new RunSaveData
            {
                saveVersion = SaveManager.CurrentRunSaveVersion,
                slotId = currentRunSlotId,
                runName = RunDisplayName,
                runCategoryId = runCategoryId,
                runCategoryLabel = runCategoryLabel,
                ventureType = (int)(selectedVenture != null ? selectedVenture.ventureType : VentureType.FastFood),
                currentTurn = currentTurn,
                playerMoney = resources.Money,
                playerActions = resources.Actions,
                playerMaxActions = resources.MaxActions,
                playerBusinessSlots = resources.BusinessSlots,
                playerCustomers = playerCustomers,
                rivalCustomers = rivalCustomers,
                playerMarketBlocks = playerMarketBlocks,
                rivalMarketBlocks = rivalMarketBlocks,
                fbiRisk = fbiRisk,
                redrawsRemaining = deckManager != null ? deckManager.RedrawsRemaining : 0,
                economySnapshot = economyManager != null ? economyManager.Snapshot : null,
                drawPileIds = deckManager != null ? deckManager.GetDrawPileIds() : new List<string>(),
                handIds = deckManager != null ? deckManager.GetHandIds() : new List<string>(),
                discardPileIds = deckManager != null ? deckManager.GetDiscardPileIds() : new List<string>(),
                operationSlotIds = slotManager != null ? slotManager.GetSlotIds(SlotType.Operation) : new List<string>(),
                staffSlotIds = slotManager != null ? slotManager.GetSlotIds(SlotType.Staff) : new List<string>(),
                marketingSlotIds = slotManager != null ? slotManager.GetSlotIds(SlotType.Marketing) : new List<string>(),
                supplierSlotIds = slotManager != null ? slotManager.GetSlotIds(SlotType.Supplier) : new List<string>(),
                tempEffectSlotIds = slotManager != null ? slotManager.GetSlotIds(SlotType.TempEffect) : new List<string>(),
                ventureRuntimeState = activeVentureRuntime != null ? activeVentureRuntime.CaptureRuntimeState() : null,
                openingArcState = activeVentureRuntime != null ? activeVentureRuntime.CaptureOpeningArcState() : null,
                eventChainState = activeVentureRuntime != null ? activeVentureRuntime.CaptureEventChainState() : null,
                rivalState = rivalAI != null ? rivalAI.CaptureState() : null
            };
        }

        private List<CardData> ResolveCardsFromIds(IList<string> ids)
        {
            var cards = new List<CardData>();
            if (ids == null)
                return cards;

            for (int i = 0; i < ids.Count; i++)
            {
                string id = ids[i];
                if (string.IsNullOrWhiteSpace(id))
                {
                    cards.Add(null);
                    continue;
                }

                cards.Add(_cardLookup != null && _cardLookup.TryGetValue(id, out var card) ? card : null);
            }

            return cards;
        }
    }
}
