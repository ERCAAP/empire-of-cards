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
    public class SlotManager : MonoBehaviour
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
            var list = GetList(slotType);
            if (list == null || slotIndex < 0 || slotIndex >= list.Count) return false;
            if (list[slotIndex] != null) return false; // Already occupied

            list[slotIndex] = card;
            EventBus.CardPlacedInSlot(card, slotType);
            return true;
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

        // ====================================================================
        //  TEMP EFFECT MANAGEMENT
        // ====================================================================

        /// <summary>
        /// Removes all expired TempEffect cards (called each ResolvePhase).
        /// </summary>
        public void ClearExpiredTempEffects()
        {
            for (int i = 0; i < _tempEffectSlots.Count; i++)
            {
                if (_tempEffectSlots[i] != null)
                {
                    EventBus.CardRemovedFromSlot(_tempEffectSlots[i], SlotType.TempEffect);
                    _tempEffectSlots[i] = null;
                }
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
    }
}
