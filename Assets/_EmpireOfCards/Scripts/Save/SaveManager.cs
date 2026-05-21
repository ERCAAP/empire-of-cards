using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Save
{
    public class SaveManager : MonoBehaviour
    {
        GameManager _gameManager;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            Debug.Log("[SaveManager] Initialized.");
        }
    }
}
