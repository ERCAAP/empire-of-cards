using UnityEngine;

namespace EmpireOfCards.Gameplay
{
    public class StaffSystem : MonoBehaviour
    {
        BoardManager _board;

        public void Init(BoardManager board)
        {
            _board = board;
            Debug.Log("[StaffSystem] Initialized.");
        }
    }
}
