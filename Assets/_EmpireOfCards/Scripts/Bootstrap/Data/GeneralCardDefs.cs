using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Definitions for venture-neutral (general) cards.
    /// These cards have isGeneralCard=true and survive FilterByVenture().
    /// IDs use the GEN prefix to match AssetGenerator conventions.
    /// </summary>
    public static class GeneralCardDefs
    {
        public static CardData[] Create()
        {
            var cards = new CardData[8];

            // --- Operations ---

            // GEN01 - Pop-Up Shop (general business)
            cards[0] = CardHelper.CreateBusiness("GEN01_PopUpShop", "Pop-Up Shop", Rarity.Common,
                "Temporary but effective. Test the waters.", 60, 25, 2, 1,
                new CardTag[0]);
            cards[0].isGeneralCard = true;
            cards[0].targetSlotType = SlotType.Operation;

            // GEN02 - Franchise Desk (general business, uncommon)
            cards[1] = CardHelper.CreateBusiness("GEN02_FranchiseDesk", "Franchise Desk", Rarity.Uncommon,
                "Expand your brand through partnerships.", 180, 30, 1, 1,
                new CardTag[0]);
            cards[1].isGeneralCard = true;
            cards[1].targetSlotType = SlotType.Operation;

            // --- Staff ---

            // GEN03 - Intern (general employee)
            cards[2] = CardHelper.CreateEmployee("GEN03_Intern", "Intern", Rarity.Common,
                "Eager to learn. Cheap to keep.", 5,
                new CardTag[0]);
            cards[2].customerBonus = 1;
            cards[2].isGeneralCard = true;
            cards[2].targetSlotType = SlotType.Staff;

            // GEN04 - Accountant (general employee, uncommon)
            cards[3] = CardHelper.CreateEmployee("GEN04_Accountant", "Accountant", Rarity.Uncommon,
                "Saves money on taxes.", 20,
                new CardTag[0]);
            cards[3].taxReduction = 0.5f;
            cards[3].activeAbilityType = ActiveAbilityType.NullifyTaxThisTurn;
            cards[3].activeAbilityName = "Tax Plan";
            cards[3].activeAbilityDesc = "Tax is nullified this turn.";
            cards[3].isGeneralCard = true;
            cards[3].targetSlotType = SlotType.Staff;

            // GEN05 - Security Guard (general employee)
            cards[4] = CardHelper.CreateEmployee("GEN05_SecurityGuard", "Security Guard", Rarity.Common,
                "Reduces legal risk across the board.", 15,
                new CardTag[0]);
            cards[4].isGeneralCard = true;
            cards[4].targetSlotType = SlotType.Staff;

            // --- Marketing ---

            // GEN07 - Social Media Ad (general action)
            cards[5] = CardHelper.CreateAction("GEN07_SocialMediaAd", "Social Media Ad", Rarity.Common,
                "Cheap reach. Everyone scrolls.", 60,
                ActionEffectType.AddCustomersToRandom, 2, 0f, 0,
                new[] { CardTag.Marketing });
            cards[5].isGeneralCard = true;
            cards[5].targetSlotType = SlotType.Marketing;

            // --- Suppliers ---

            // GEN16 - Bulk Dealer (general upgrade/supplier)
            cards[6] = CardHelper.CreateUpgrade("GEN16_BulkDealer", "Bulk Dealer", Rarity.Common,
                "Generic cost savings for any venture.", 60,
                UpgradeEffectType.IncomePercentSingle, 8f, false, 0, 0,
                new[] { CardTag.Support });
            cards[6].isGeneralCard = true;
            cards[6].targetSlotType = SlotType.Supplier;
            cards[6].costReductionPercent = 8f;
            cards[6].qualityBoostAmount = 0.5f;

            // GEN08 - Billboard Ad (general marketing, uncommon)
            cards[7] = CardHelper.CreateAction("GEN08_BillboardAd", "Billboard Ad", Rarity.Uncommon,
                "Old school but effective. High visibility.", 120,
                ActionEffectType.AddCustomersToRandom, 4, 0f, 0,
                new[] { CardTag.Marketing });
            cards[7].isGeneralCard = true;
            cards[7].targetSlotType = SlotType.Marketing;

            return cards;
        }
    }
}
