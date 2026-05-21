using UnityEngine;

namespace EmpireOfCards.Gameplay
{
    public class RivalAI : MonoBehaviour
    {
        EconomyManager _economy;

        public void Init(EconomyManager economy)
        {
            _economy = economy;
            Debug.Log("[RivalAI] Initialized.");
        }
    }
}
