using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Manages the player's business location, passive foot traffic,
    /// and rent costs (GDD Section 10.1-10.3).
    /// </summary>
    public class LocationSystem : MonoBehaviour
    {
        [Header("Location State (Read Only)")]
        [SerializeField] private LocationType currentLocation;
        [SerializeField] private int passiveCustomersPerTurn;
        [SerializeField] private int rentPerTurn;
        [SerializeField] private bool rivalInSameArea;

        public LocationType CurrentLocation => currentLocation;
        public int PassiveCustomersPerTurn => passiveCustomersPerTurn;
        public int RentPerTurn => rentPerTurn;
        public bool RivalInSameArea => rivalInSameArea;

        public void Init()
        {
            currentLocation = LocationType.RemoteCorner;
            passiveCustomersPerTurn = Constants.LOCATION_TRAFFIC_REMOTE;
            rentPerTurn = Constants.LOCATION_RENT_REMOTE;
            rivalInSameArea = false;
        }

        /// <summary>
        /// Sets the business location and updates traffic/rent values.
        /// </summary>
        public void SetLocation(LocationType location)
        {
            currentLocation = location;

            switch (location)
            {
                case LocationType.RemoteCorner:
                    passiveCustomersPerTurn = Constants.LOCATION_TRAFFIC_REMOTE;
                    rentPerTurn = Constants.LOCATION_RENT_REMOTE;
                    break;
                case LocationType.SideStreet:
                    passiveCustomersPerTurn = Constants.LOCATION_TRAFFIC_SIDE;
                    rentPerTurn = Constants.LOCATION_RENT_SIDE;
                    break;
                case LocationType.MainStreet:
                    passiveCustomersPerTurn = Constants.LOCATION_TRAFFIC_MAIN;
                    rentPerTurn = Constants.LOCATION_RENT_MAIN;
                    break;
                case LocationType.ShoppingMall:
                    passiveCustomersPerTurn = Constants.LOCATION_TRAFFIC_MALL;
                    rentPerTurn = Constants.LOCATION_RENT_MALL;
                    break;
            }

            EventBus.LocationChanged(passiveCustomersPerTurn, rentPerTurn);
            Debug.Log($"[LocationSystem] Set to {location}: traffic={passiveCustomersPerTurn}/turn, rent={rentPerTurn}/turn");
        }

        /// <summary>
        /// Marks whether the rival is in the same traffic area.
        /// When true, passive traffic is split (GDD 10.3).
        /// </summary>
        public void SetRivalInSameArea(bool inSameArea)
        {
            rivalInSameArea = inSameArea;
        }

        /// <summary>
        /// Returns the effective passive customers this turn,
        /// accounting for rival competition in the same area.
        /// </summary>
        public int GetPassiveCustomers()
        {
            if (rivalInSameArea)
                return Mathf.CeilToInt(passiveCustomersPerTurn / 2f);
            return passiveCustomersPerTurn;
        }

        /// <summary>
        /// Returns the rent cost for this turn.
        /// </summary>
        public int GetRent()
        {
            return rentPerTurn;
        }

        /// <summary>
        /// Charges rent via GameManager and fires the event.
        /// Called during income calculation phase.
        /// </summary>
        public void ChargeRent()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || rentPerTurn <= 0) return;

            gm.SpendMoney(rentPerTurn);
            EventBus.RentCharged(rentPerTurn);
            Debug.Log($"[LocationSystem] Rent charged: {rentPerTurn}");
        }

        public void Reset()
        {
            currentLocation = LocationType.RemoteCorner;
            passiveCustomersPerTurn = Constants.LOCATION_TRAFFIC_REMOTE;
            rentPerTurn = Constants.LOCATION_RENT_REMOTE;
            rivalInSameArea = false;
        }
    }
}
