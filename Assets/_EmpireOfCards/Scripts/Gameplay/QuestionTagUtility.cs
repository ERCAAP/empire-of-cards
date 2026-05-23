using System.Collections.Generic;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public static class QuestionTagUtility
    {
        public static HashSet<string> InferTags(CardData card)
        {
            var tags = new HashSet<string>();
            if (card == null)
                return tags;

            switch (card.targetSlotType)
            {
                case SlotType.Operation:
                    tags.Add("Capacity");
                    tags.Add("Speed");
                    break;
                case SlotType.Staff:
                    tags.Add("Staff");
                    tags.Add("Capacity");
                    break;
                case SlotType.Marketing:
                    tags.Add("Demand");
                    tags.Add("Reputation");
                    break;
                case SlotType.Supplier:
                    tags.Add("Supply");
                    tags.Add("Quality");
                    tags.Add("Cash");
                    break;
                case SlotType.TempEffect:
                    tags.Add("Risk");
                    break;
            }

            if (card.legalRiskOnPlay > 0 || card.legalRiskPerTurn > 0 || card.legalRiskDeltaPerTurn > 0f)
                tags.Add("Risk");
            if (card.demandDelta > 0f || card.customersPerTurn > 0 || card.platformRatingGain > 0f)
                tags.Add("Demand");
            if (card.capacityDelta > 0f || card.employeeSlots > 0)
                tags.Add("Capacity");
            if (card.qualityDelta > 0f || card.qualityBoostAmount > 0f || card.qualityScore > 0f)
                tags.Add("Quality");
            if (card.staffStabilityDelta > 0f || card.customerBonus > 0 || card.salaryPerTurn > 0)
                tags.Add("Staff");
            if (card.ratingDeltaPerTurn > 0f || card.platformRatingOnPlay > 0f)
                tags.Add("Reputation");
            if (card.cashDeltaPerTurn > 0f || card.costReductionPercent > 0f || card.actionEffectType == ActionEffectType.AddMoneyInstant)
                tags.Add("Cash");
            if (card.loyaltyDeltaPerTurn > 0)
                tags.Add("Loyalty");
            if (card.workloadDeltaPerTurn < 0f || card.serviceSpeedScore > 0f)
                tags.Add("Speed");

            return tags;
        }

        public static bool Matches(CardData card, string requiredTag)
        {
            if (string.IsNullOrWhiteSpace(requiredTag))
                return true;

            return InferTags(card).Contains(requiredTag);
        }

        public static bool IsPersistentBuild(CardData card)
        {
            if (card == null)
                return false;

            return card.cardFamily == CardFamily.Setup ||
                   card.cardType == CardType.Business ||
                   card.cardType == CardType.Employee ||
                   (card.cardType == CardType.Upgrade && !card.entersTempEffectOnUse);
        }

        public static bool IsResponseCard(CardData card)
        {
            if (card == null)
                return false;

            if (card.entersTempEffectOnUse)
                return true;

            return card.cardType == CardType.Action
                   || card.cardFamily == CardFamily.Reaction
                   || card.cardFamily == CardFamily.Risk
                   || card.cardFamily == CardFamily.Growth;
        }
    }
}
