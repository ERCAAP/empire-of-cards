using UnityEngine;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "NewDeckPreset", menuName = "EmpireOfCards/Deck Preset")]
    public class DeckPresetData : ScriptableObject
    {
        public string presetName;           // "Starter Deck"
        public int startingMoney = 500;
        public DeckEntry[] cards;           // 14 cards: 2xDiner, 3xIntern, 2xJrMarketer, 3xFlyer, 2xSmallInvestment, 2xOfficeSupplies
    }

    [System.Serializable]
    public class DeckEntry
    {
        public CardData card;
        public int count;
    }
}
