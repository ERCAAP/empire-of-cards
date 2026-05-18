using System;
using UnityEngine;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "NewDeckPreset", menuName = "EmpireOfCards/Deck Preset")]
    public class DeckPresetData : ScriptableObject
    {
        [Tooltip("Display name for this deck preset")]
        public string presetName;

        public DeckEntry[] cards;

        [Tooltip("Starting money when using this deck")]
        public int startingMoney;
    }

    [Serializable]
    public class DeckEntry
    {
        public CardData card;

        [Tooltip("Number of copies of this card in the deck")]
        public int count;
    }
}
