using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public class DeckManager : MonoBehaviour
    {
        CardData[] _allCards;
        CardData[] _shopPool;

        public void Init(CardData[] allCards, CardData[] shopPool)
        {
            _allCards = allCards;
            _shopPool = shopPool;
            Debug.Log($"[DeckManager] Initialized with {allCards.Length} cards, {shopPool.Length} in shop.");
        }
    }
}
