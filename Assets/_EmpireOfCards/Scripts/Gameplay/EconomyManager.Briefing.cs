using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.UI.Clarity;

namespace EmpireOfCards.Gameplay
{
    public partial class EconomyManager
    {
        private TurnBriefData BuildTurnBriefInternal(int currentTurn)
        {
            var runtime = GameManager.Instance != null ? GameManager.Instance.ActiveVentureRuntime : null;
            TurnScriptBeat beat = runtime != null ? runtime.GetBeatForTurn(currentTurn) : null;
            if (beat != null)
            {
                return new TurnBriefData
                {
                    currentTurn = currentTurn,
                    pressure = currentPressure,
                    headline = beat.headline,
                    detail = beat.detail,
                    recommendedMove = beat.recommendedMove,
                    buildIdentity = GameClarityFormatter.GetBuildIdentity(GameManager.Instance)
                };
            }

            if (TryBuildOpeningBrief(currentTurn, out var openingBrief))
                return openingBrief;

            return new TurnBriefData
            {
                currentTurn = currentTurn,
                pressure = currentPressure,
                headline = GetBriefHeadline(currentPressure),
                detail = GetBriefDetail(currentPressure),
                recommendedMove = GetRecommendedMove(currentPressure),
                buildIdentity = GameClarityFormatter.GetBuildIdentity(GameManager.Instance)
            };
        }
    }
}
