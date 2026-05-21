using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Manages slot capacity per type. Expands on era changes.
    /// BoardManager asks SlotManager before placing a card.
    /// </summary>
    public class SlotManager : MonoBehaviour
    {
        readonly Dictionary<SlotType, int> _maxSlots      = new();
        readonly Dictionary<SlotType, int> _occupiedSlots = new();

        SectorProfile _profile;
        Era _currentEra;

        public void Init(SectorProfile[] profiles)
        {
            // MVP: use first profile (Restaurant)
            _profile = profiles[0];
            Configure(Era.Garage);
        }

        // ── Configuration ───────────────────────────────────────────

        public void Configure(Era era)
        {
            _currentEra = era;
            var layout = FindLayout(era);

            SetMax(SlotType.Kitchen,    layout.kitchenSlots);
            SetMax(SlotType.Salon,      layout.salonSlots);
            SetMax(SlotType.Storage,    layout.storageSlots);
            SetMax(SlotType.Marketing,  layout.marketingSlots);
            SetMax(SlotType.TempEffect, layout.tempEffectSlots);

            Debug.Log($"[Slots] Configured for {era}: total {layout.TotalSlots} slots, {layout.actionsPerTurn} actions");
        }

        // ── Queries ─────────────────────────────────────────────────

        public bool HasRoom(SlotType slot)
        {
            return GetOccupied(slot) < GetMax(slot);
        }

        public int GetOccupied(SlotType slot)
        {
            return _occupiedSlots.TryGetValue(slot, out int count) ? count : 0;
        }

        public int GetMax(SlotType slot)
        {
            return _maxSlots.TryGetValue(slot, out int max) ? max : 0;
        }

        public int GetAvailable(SlotType slot)
        {
            return GetMax(slot) - GetOccupied(slot);
        }

        public int GetActionsForCurrentEra()
        {
            var layout = FindLayout(_currentEra);
            return layout.actionsPerTurn;
        }

        // ── Occupation (called by BoardManager) ─────────────────────

        public void Occupy(SlotType slot, int count)
        {
            if (!_occupiedSlots.ContainsKey(slot))
                _occupiedSlots[slot] = 0;

            _occupiedSlots[slot] += count;
        }

        public void Vacate(SlotType slot, int count)
        {
            if (!_occupiedSlots.ContainsKey(slot))
                return;

            _occupiedSlots[slot] = Mathf.Max(0, _occupiedSlots[slot] - count);
        }

        // ── Era Change ──────────────────────────────────────────────

        void OnEnable()
        {
            EventBus.OnEraChanged += HandleEraChanged;
        }

        void OnDisable()
        {
            EventBus.OnEraChanged -= HandleEraChanged;
        }

        void HandleEraChanged(Era newEra)
        {
            var oldMax = new Dictionary<SlotType, int>(_maxSlots);
            Configure(newEra);

            // Fire unlock events for any slot that gained capacity
            foreach (var kvp in _maxSlots)
            {
                int prev = oldMax.TryGetValue(kvp.Key, out int p) ? p : 0;
                if (kvp.Value > prev)
                    EventBus.SlotUnlocked(kvp.Key, kvp.Value);
            }
        }

        // ── Helpers ─────────────────────────────────────────────────

        void SetMax(SlotType slot, int max)
        {
            _maxSlots[slot] = max;
            if (!_occupiedSlots.ContainsKey(slot))
                _occupiedSlots[slot] = 0;
        }

        EraSlotLayout FindLayout(Era era)
        {
            foreach (var layout in _profile.eraSlotLayouts)
                if (layout.era == era)
                    return layout;

            Debug.LogWarning($"[Slots] No layout found for era {era}, falling back to Garage");
            return _profile.eraSlotLayouts[0];
        }
    }
}
