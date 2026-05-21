using UnityEngine;

namespace EmpireOfCards.Gameplay
{
    public class CrisisChainSystem : MonoBehaviour
    {
        BoardManager _board;
        EconomyManager _economy;

        public void Init(BoardManager board, EconomyManager economy)
        {
            _board = board;
            _economy = economy;
            Debug.Log("[CrisisChainSystem] Initialized.");
        }
    }
}
