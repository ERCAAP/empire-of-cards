using UnityEngine;

namespace EmpireOfCards.Gameplay
{
    public class HygieneSystem : MonoBehaviour
    {
        BoardManager _board;

        public void Init(BoardManager board)
        {
            _board = board;
            Debug.Log("[HygieneSystem] Initialized.");
        }
    }
}
