using UnityEngine;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "NewDeckPreset", menuName = "EmpireOfCards/Deck Preset")]
    public class DeckPresetData : ScriptableObject
    {
        public string presetName;           // "Başlangıç Destesi"
        public int startingMoney = 500;
        public DeckEntry[] cards;           // 14 cards: 2xBüfe, 3xStajyer, 2xÇaylak, 3xElİlanı, 2xKüçükYatırım, 2xOfisMalzemeleri
    }

    [System.Serializable]
    public class DeckEntry
    {
        public CardData card;
        public int count;
    }
}
