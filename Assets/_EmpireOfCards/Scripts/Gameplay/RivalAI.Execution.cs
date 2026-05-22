using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public partial class RivalAI
    {
        private void FinalizeQueuedTurnInternal(float playerShare)
        {
            var gm = GameManager.Instance;
            if (gm == null)
                return;

            rivalCustomers = Mathf.Clamp(Mathf.RoundToInt(100f - playerShare + rivalPressure + rivalMomentumCustomers), 0, 100);
            rivalIncome = Mathf.RoundToInt(45f + rivalQuality * 6f + rivalRating * 8f);
            rivalMoney += Mathf.Max(10, rivalIncome / 4);
            rivalMomentumCustomers = 0f;

            if (rivalBusinesses.Count > 0)
            {
                var lead = rivalBusinesses[0];
                lead.customers = rivalCustomers;
                lead.income = rivalIncome;
                lead.platformRating = rivalRating;
                lead.qualityScore = rivalQuality;
            }

            if (_queuedActions.Count > 0)
            {
                RivalQueuedAction resolved = _queuedActions[_queuedActions.Count - 1];
                lastPlayedCardName = resolved.displayName;
                lastLaneLabel = resolved.laneLabel;
                lastPressureStyle = resolved.shortDescription;
            }

            _runtimeState.lastResolvedTurn = gm.CurrentTurn;
            _runtimeState.pressureBank = rivalPressure;
            if (!string.IsNullOrWhiteSpace(lastLaneLabel))
                _runtimeState.activePlan = lastLaneLabel;

            gm.EconomyManager?.RegisterRivalPressure(rivalPressure, lastPressureStyle);
            EventBus.RivalActed(_queuedActions.Count > 0 ? DescribeAction(_queuedActions[_queuedActions.Count - 1]) : "Rival adapts.");
            EventBus.RivalMoodChanged(_queuedActions.Count > 0 ? _queuedActions[_queuedActions.Count - 1].moodIcon : "!");
        }

        private void ResolveQueuedActionInternal(RivalQueuedAction action, float playerShare)
        {
            if (action == null)
                return;

            lastPlayedCardName = action.displayName;
            lastLaneLabel = action.laneLabel;
            lastPressureStyle = action.shortDescription;

            rivalPressure += action.pressureDelta;
            rivalRating = Mathf.Clamp(rivalRating + action.ratingDelta, 1f, 5f);
            rivalQuality = Mathf.Clamp(rivalQuality + action.qualityDelta, 0f, 10f);

            if (action.displayName.Contains("Poach") && playerShare > 45f)
                rivalMomentumCustomers += 3f;
            else
                rivalMomentumCustomers += action.demandSteal;

            switch (action.displayName)
            {
                case "Price Drop Campaign":
                    rivalMomentumCustomers += 2f;
                    break;
                case "Funding Round":
                    rivalMoney += 120;
                    break;
                case "Expansion Lease":
                    rivalIncome += 15;
                    break;
                case "Staff Poach":
                    totalRivalEmployees++;
                    break;
                case "Ops Disruption":
                    GameManager.Instance?.BoardManager?.SetProductionDisabledNextTurn(true);
                    break;
            }
        }

        private void ApplyPoachResponse(RivalMove fallbackMove, float pressureDelta, float ratingDelta, int bonusCash)
        {
            rivalPressure += pressureDelta;
            rivalRating = Mathf.Clamp(rivalRating + ratingDelta, 1f, 5f);
            rivalMoney += bonusCash;
            lastPlayedCardName = GetMoveCardName(fallbackMove);
            lastLaneLabel = _sectorController != null ? _sectorController.GetLaneLabel(fallbackMove) : "Staff Lane";
            lastPressureStyle = GetMoveDescription(fallbackMove);
        }

        private RivalRuntimeState CaptureStateInternal()
        {
            return new RivalRuntimeState
            {
                escalationLevel = _runtimeState.escalationLevel,
                campaignsLaunched = _runtimeState.campaignsLaunched,
                pressureBank = rivalPressure,
                activePlan = _runtimeState.activePlan,
                lastResolvedTurn = _runtimeState.lastResolvedTurn
            };
        }

        private void RestoreStateInternal(RivalRuntimeState state)
        {
            _runtimeState = state ?? new RivalRuntimeState();
            rivalPressure = _runtimeState.pressureBank;
        }
    }
}
