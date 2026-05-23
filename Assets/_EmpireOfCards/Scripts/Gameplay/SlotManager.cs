using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Manages all 5 slot types (Operation, Staff, Marketing, Supplier, TempEffect).
    /// Tracks occupancy, enforces max limits, fires EventBus events on changes.
    /// Wired by WiringService. Not a Singleton — referenced through GameManager.
    /// </summary>
    public partial class SlotManager : MonoBehaviour
    {
        // Current slot counts per type (starts at GDD v3.0 defaults)
        private int _operationMax;
        private int _staffMax;
        private int _marketingMax;
        private int _supplierMax;
        private readonly int _tempEffectMax = Constants.STARTING_TEMP_EFFECT_SLOTS; // Fixed

        // Occupied cards per slot type: list of CardData (null = empty slot)
        private readonly List<CardData> _operationSlots = new List<CardData>();
        private readonly List<CardData> _staffSlots = new List<CardData>();
        private readonly List<CardData> _marketingSlots = new List<CardData>();
        private readonly List<CardData> _supplierSlots = new List<CardData>();
        private readonly List<CardData> _tempEffectSlots = new List<CardData>();

        public int OperationMax => _operationMax;
        public int StaffMax => _staffMax;
        public int MarketingMax => _marketingMax;
        public int SupplierMax => _supplierMax;
        public int TempEffectMax => _tempEffectMax;

        public IReadOnlyList<CardData> OperationSlots => _operationSlots;
        public IReadOnlyList<CardData> StaffSlots => _staffSlots;
        public IReadOnlyList<CardData> MarketingSlots => _marketingSlots;
        public IReadOnlyList<CardData> SupplierSlots => _supplierSlots;
        public IReadOnlyList<CardData> TempEffectSlots => _tempEffectSlots;

        public void Init()
        {
            _operationMax = Constants.STARTING_OPERATION_SLOTS;
            _staffMax = Constants.STARTING_STAFF_SLOTS;
            _marketingMax = Constants.STARTING_MARKETING_SLOTS;
            _supplierMax = Constants.STARTING_SUPPLIER_SLOTS;

            InitializeSlotList(_operationSlots, _operationMax);
            InitializeSlotList(_staffSlots, _staffMax);
            InitializeSlotList(_marketingSlots, _marketingMax);
            InitializeSlotList(_supplierSlots, _supplierMax);
            InitializeSlotList(_tempEffectSlots, _tempEffectMax);
        }

        public void Configure(VentureBoardProfile profile)
        {
            if (profile == null)
            {
                Init();
                return;
            }

            _operationMax = profile.startingOperationSlots;
            _staffMax = profile.startingStaffSlots;
            _marketingMax = profile.startingMarketingSlots;
            _supplierMax = profile.startingSupplierSlots;

            InitializeSlotList(_operationSlots, _operationMax);
            InitializeSlotList(_staffSlots, _staffMax);
            InitializeSlotList(_marketingSlots, _marketingMax);
            InitializeSlotList(_supplierSlots, _supplierMax);
            InitializeSlotList(_tempEffectSlots, _tempEffectMax);
        }

        public void ResetAll()
        {
            for (int i = 0; i < _operationSlots.Count; i++) _operationSlots[i] = null;
            for (int i = 0; i < _staffSlots.Count; i++) _staffSlots[i] = null;
            for (int i = 0; i < _marketingSlots.Count; i++) _marketingSlots[i] = null;
            for (int i = 0; i < _supplierSlots.Count; i++) _supplierSlots[i] = null;
            for (int i = 0; i < _tempEffectSlots.Count; i++) _tempEffectSlots[i] = null;
        }

        private static void InitializeSlotList(List<CardData> list, int count)
        {
            list.Clear();
            for (int i = 0; i < count; i++)
                list.Add(null);
        }

        // ====================================================================
        //  PLACEMENT
        // ====================================================================

        public bool TryPlace(CardData card, SlotType slotType, int slotIndex)
        {
            if (card == null) return false;
            var list = GetList(slotType);
            if (list == null || slotIndex < 0 || slotIndex >= list.Count) return false;
            if (list[slotIndex] != null) return false; // Already occupied

            // Validate card type matches slot type (GDD Section 4.3)
            if (!IsCardValidForSlot(card, slotType)) return false;

            list[slotIndex] = card;
            EventBus.CardPlacedInSlot(card, slotType);
            EventBus.BoardAnchorStateChanged(new BoardAnchorViewModel
            {
                slotType = slotType,
                slotIndex = slotIndex,
                laneId = slotType.ToString(),
                label = slotType.ToString(),
                occupied = true,
                residentCardId = card.cardId
            });
            return true;
        }

        private static bool IsCardValidForSlot(CardData card, SlotType slotType)
        {
            // Prefer v2 targetSlotType when set
            if (card.targetSlotType == slotType) return true;

            // Fallback: CardType-based validation
            return slotType switch
            {
                SlotType.Operation  => card.cardType == CardType.Business,
                SlotType.Staff      => card.cardType == CardType.Employee,
                SlotType.Marketing  => card.cardType == CardType.Action || card.cardType == CardType.Upgrade,
                SlotType.Supplier   => card.cardType == CardType.Upgrade,
                SlotType.TempEffect => card.cardType == CardType.Event,
                _                   => false
            };
        }

        public bool TryRemove(SlotType slotType, int slotIndex, out CardData removed)
        {
            removed = null;
            var list = GetList(slotType);
            if (list == null || slotIndex < 0 || slotIndex >= list.Count) return false;
            if (list[slotIndex] == null) return false;

            removed = list[slotIndex];
            list[slotIndex] = null;
            EventBus.CardRemovedFromSlot(removed, slotType);
            EventBus.BoardAnchorStateChanged(new BoardAnchorViewModel
            {
                slotType = slotType,
                slotIndex = slotIndex,
                laneId = slotType.ToString(),
                label = slotType.ToString(),
                occupied = false,
                residentCardId = null
            });
            return true;
        }

        // ====================================================================
        //  EXPANSION
        // ====================================================================

        public bool TryExpandSlot(SlotType slotType)
        {
            switch (slotType)
            {
                case SlotType.Operation:
                    if (_operationMax >= Constants.MAX_OPERATION_SLOTS) return false;
                    _operationMax++;
                    _operationSlots.Add(null);
                    EventBus.SlotExpanded(slotType, _operationMax);
                    return true;

                case SlotType.Staff:
                    if (_staffMax >= Constants.MAX_STAFF_SLOTS) return false;
                    _staffMax++;
                    _staffSlots.Add(null);
                    EventBus.SlotExpanded(slotType, _staffMax);
                    return true;

                case SlotType.Marketing:
                    if (_marketingMax >= Constants.MAX_MARKETING_SLOTS) return false;
                    _marketingMax++;
                    _marketingSlots.Add(null);
                    EventBus.SlotExpanded(slotType, _marketingMax);
                    return true;

                case SlotType.Supplier:
                    if (_supplierMax >= Constants.MAX_SUPPLIER_SLOTS) return false;
                    _supplierMax++;
                    _supplierSlots.Add(null);
                    EventBus.SlotExpanded(slotType, _supplierMax);
                    return true;

                case SlotType.TempEffect:
                    return false; // TempEffect slots are fixed
            }
            return false;
        }

        // ====================================================================
        //  QUERIES
        // ====================================================================

        public int GetOccupiedCount(SlotType slotType)
        {
            var list = GetList(slotType);
            if (list == null) return 0;
            int count = 0;
            foreach (var c in list)
                if (c != null) count++;
            return count;
        }

        public int GetFirstEmptyIndex(SlotType slotType)
        {
            var list = GetList(slotType);
            if (list == null) return -1;
            for (int i = 0; i < list.Count; i++)
                if (list[i] == null) return i;
            return -1;
        }

        public bool HasEmpty(SlotType slotType) => GetFirstEmptyIndex(slotType) >= 0;

        public int FindCardIndex(SlotType slotType, CardData card)
        {
            if (card == null)
                return -1;

            var list = GetList(slotType);
            if (list == null)
                return -1;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == card)
                    return i;
            }

            return -1;
        }

        public List<string> GetSlotIds(SlotType slotType)
        {
            var list = GetList(slotType);
            var result = new List<string>();
            if (list == null) return result;

            for (int i = 0; i < list.Count; i++)
                result.Add(list[i] != null ? list[i].cardId : string.Empty);

            return result;
        }

        public IReadOnlyList<CardData> GetActiveCards(SlotType slotType)
        {
            var source = GetList(slotType);
            var cards = new List<CardData>();
            if (source == null)
                return cards;

            for (int i = 0; i < source.Count; i++)
            {
                if (source[i] != null)
                    cards.Add(source[i]);
            }

            return cards;
        }

        public void RestoreState(
            IList<CardData> operationCards,
            IList<CardData> staffCards,
            IList<CardData> marketingCards,
            IList<CardData> supplierCards,
            IList<CardData> tempCards)
        {
            _operationMax = operationCards != null ? operationCards.Count : Constants.STARTING_OPERATION_SLOTS;
            _staffMax = staffCards != null ? staffCards.Count : Constants.STARTING_STAFF_SLOTS;
            _marketingMax = marketingCards != null ? marketingCards.Count : Constants.STARTING_MARKETING_SLOTS;
            _supplierMax = supplierCards != null ? supplierCards.Count : Constants.STARTING_SUPPLIER_SLOTS;

            RestoreList(_operationSlots, operationCards, _operationMax);
            RestoreList(_staffSlots, staffCards, _staffMax);
            RestoreList(_marketingSlots, marketingCards, _marketingMax);
            RestoreList(_supplierSlots, supplierCards, _supplierMax);
            RestoreList(_tempEffectSlots, tempCards, _tempEffectMax);
        }

        // ====================================================================
        //  TEMP EFFECT MANAGEMENT
        // ====================================================================

        /// <summary>
        /// Removes the oldest TempEffect card when slots are full (GDD Section 4.3).
        /// Called each ResolvePhase or when a new temp event needs space.
        /// </summary>
        public void ClearOldestTempEffect()
        {
            // Remove first occupied slot (oldest)
            for (int i = 0; i < _tempEffectSlots.Count; i++)
            {
                if (_tempEffectSlots[i] != null)
                {
                    TryRemove(SlotType.TempEffect, i, out _);
                    return;
                }
            }
        }

        /// <summary>
        /// Removes all TempEffect cards (called at end of resolve for expired effects).
        /// </summary>
        public void ClearAllTempEffects()
        {
            for (int i = 0; i < _tempEffectSlots.Count; i++)
            {
                if (_tempEffectSlots[i] != null)
                    TryRemove(SlotType.TempEffect, i, out _);
            }
        }

        // ====================================================================
        //  HELPERS
        // ====================================================================

        private List<CardData> GetList(SlotType slotType)
        {
            return slotType switch
            {
                SlotType.Operation  => _operationSlots,
                SlotType.Staff      => _staffSlots,
                SlotType.Marketing  => _marketingSlots,
                SlotType.Supplier   => _supplierSlots,
                SlotType.TempEffect => _tempEffectSlots,
                _                   => null
            };
        }

        private static void RestoreList(List<CardData> target, IList<CardData> source, int count)
        {
            InitializeSlotList(target, Mathf.Max(0, count));
            if (source == null) return;

            for (int i = 0; i < source.Count && i < target.Count; i++)
                target[i] = source[i];
        }
    }
}
