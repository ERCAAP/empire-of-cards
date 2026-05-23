using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public class CustomerFlowManager : MonoBehaviour
    {
        [SerializeField] private CustomerFlowSnapshot currentSnapshot = new CustomerFlowSnapshot();
        [SerializeField] private int tokenCount = 12;

        public CustomerFlowSnapshot CurrentSnapshot => currentSnapshot;

        public void ResetState()
        {
            currentSnapshot = new CustomerFlowSnapshot();
            EventBus.CustomerFlowResolved(currentSnapshot);
        }

        public void ResolveSnapshot(int turnNumber, int playerCustomers, int rivalCustomers, int totalMarket, string dominantReason)
        {
            int playerTokens = Mathf.Clamp(Mathf.RoundToInt((playerCustomers / Mathf.Max(1f, totalMarket)) * tokenCount), 0, tokenCount);
            int rivalTokens = Mathf.Clamp(Mathf.RoundToInt((rivalCustomers / Mathf.Max(1f, totalMarket)) * tokenCount), 0, tokenCount - playerTokens);
            int neutralTokens = Mathf.Max(0, tokenCount - playerTokens - rivalTokens);

            currentSnapshot = new CustomerFlowSnapshot
            {
                turnNumber = turnNumber,
                movedToPlayer = playerTokens,
                movedToRival = rivalTokens,
                neutralCount = neutralTokens,
                loyalPlayerCount = Mathf.Clamp(playerTokens / 2, 0, playerTokens),
                loyalRivalCount = Mathf.Clamp(rivalTokens / 2, 0, rivalTokens),
                dominantReason = string.IsNullOrWhiteSpace(dominantReason) ? "Customer flow shifted." : dominantReason
            };

            EventBus.CustomerFlowResolved(currentSnapshot);
        }
    }
}
