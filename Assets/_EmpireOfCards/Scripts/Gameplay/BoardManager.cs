using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    [Serializable]
    public class ActiveBusiness
    {
        public CardData businessCard;
        public List<CardData> employees = new List<CardData>();
        public List<int> employeeTenure = new List<int>();
        public List<CardData> upgrades = new List<CardData>();
        public int turnsActive;
        public int totalCustomersAttracted;
        public bool isClosed;
        public int closedTurnsRemaining;
        public int neglectTurns;
        public BusinessLevel currentLevel = BusinessLevel.Level1;

        public int GetAvailableEmployeeSlots()
        {
            return Mathf.Max(0, businessCard != null ? businessCard.employeeSlots : 1);
        }
    }

    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private List<ActiveBusiness> playerBusinesses = new List<ActiveBusiness>();
        [SerializeField] private List<CardData> globalUpgrades = new List<CardData>();
        [SerializeField] private int maxSlots = Constants.STARTING_OPERATION_SLOTS;
        [SerializeField] private bool productionDisabledNextTurn;
        [SerializeField] private CardData activeEvent;
        [SerializeField] private int activeEventTurnsRemaining;
        [SerializeField] private VentureType activeVenture = VentureType.FastFood;

        private SlotManager _slotManager;
        private VentureBoardProfile _boardProfile;
        private readonly Dictionary<CardData, int> _tempDurations = new Dictionary<CardData, int>();
        private readonly Dictionary<int, CardData> _operationCardMap = new Dictionary<int, CardData>();

        public IReadOnlyList<ActiveBusiness> PlayerBusinesses => playerBusinesses;
        public IReadOnlyList<CardData> GlobalUpgrades => globalUpgrades;
        public int MaxSlots => maxSlots;
        public CardData ActiveEvent => activeEvent;
        public int ActiveEventTurnsRemaining => activeEventTurnsRemaining;
        public VentureType ActiveVenture => activeVenture;
        public VentureBoardProfile BoardProfile => _boardProfile;

        public void Init(SlotManager slotManager)
        {
            _slotManager = slotManager;
        }

        public void ConfigureForVenture(VentureType ventureType, VentureBoardProfile profile)
        {
            activeVenture = ventureType;
            _boardProfile = profile;
            maxSlots = profile != null ? profile.startingOperationSlots : Constants.STARTING_OPERATION_SLOTS;
        }

        public bool PlaceCardInSlot(CardData card, SlotType slotType, int slotIndex, int businessIndex = -1)
        {
            return slotType switch
            {
                SlotType.Operation => PlaceBusiness(card, slotIndex),
                SlotType.Staff => PlaceEmployee(card, businessIndex >= 0 ? businessIndex : 0, slotIndex),
                SlotType.Marketing => PlaceUpgrade(card, businessIndex, SlotType.Marketing, slotIndex),
                SlotType.Supplier => PlaceUpgrade(card, businessIndex, SlotType.Supplier, slotIndex),
                SlotType.TempEffect => PlaceUpgrade(card, businessIndex, SlotType.TempEffect, slotIndex),
                _ => false
            };
        }

        public bool PlaceBusiness(CardData card, int slotIndex)
        {
            if (card == null) return false;
            if (!TryPlaceCard(card, SlotType.Operation, slotIndex)) return false;

            var active = new ActiveBusiness
            {
                businessCard = card,
                currentLevel = BusinessLevel.Level1
            };

            if (slotIndex >= playerBusinesses.Count)
                playerBusinesses.Add(active);
            else if (slotIndex >= 0)
            {
                while (playerBusinesses.Count <= slotIndex)
                    playerBusinesses.Add(new ActiveBusiness());
                playerBusinesses[slotIndex] = active;
            }

            _operationCardMap[slotIndex] = card;
            EventBus.BusinessPlaced(card, slotIndex);
            return true;
        }

        public bool PlaceEmployee(CardData card, int businessIndex)
        {
            int slotIndex = _slotManager != null ? _slotManager.GetFirstEmptyIndex(SlotType.Staff) : -1;
            return PlaceEmployee(card, businessIndex, slotIndex);
        }

        private bool PlaceEmployee(CardData card, int businessIndex, int slotIndex)
        {
            if (card == null) return false;
            if (slotIndex < 0 || !TryPlaceCard(card, SlotType.Staff, slotIndex)) return false;

            if (businessIndex >= 0 && businessIndex < playerBusinesses.Count && playerBusinesses[businessIndex] != null)
            {
                playerBusinesses[businessIndex].employees.Add(card);
                playerBusinesses[businessIndex].employeeTenure.Add(0);
            }

            EventBus.EmployeePlaced(card, businessIndex);
            return true;
        }

        public bool PlaceUpgrade(CardData card, int businessIndex)
        {
            return PlaceUpgrade(card, businessIndex, card != null ? card.targetSlotType : SlotType.Operation, -1);
        }

        private bool PlaceUpgrade(CardData card, int businessIndex, SlotType forcedSlotType, int preferredSlotIndex)
        {
            if (card == null) return false;

            SlotType target = forcedSlotType;
            if (target == SlotType.TempEffect || target == SlotType.Marketing || target == SlotType.Supplier || target == SlotType.Operation)
            {
                int idx = preferredSlotIndex >= 0 ? preferredSlotIndex : (_slotManager != null ? _slotManager.GetFirstEmptyIndex(target) : -1);
                if (idx < 0)
                {
                    if (target == SlotType.TempEffect && _slotManager != null)
                    {
                        _slotManager.ClearOldestTempEffect();
                        idx = _slotManager.GetFirstEmptyIndex(target);
                    }
                }

                if (idx < 0 || !TryPlaceCard(card, target, idx))
                    return false;

                if (target == SlotType.TempEffect)
                    _tempDurations[card] = Mathf.Max(1, card.tempEffectDuration);
                else if (target == SlotType.Operation)
                    globalUpgrades.Add(card);

                EventBus.UpgradePlaced(card, businessIndex);
                return true;
            }

            globalUpgrades.Add(card);
            EventBus.UpgradePlaced(card, businessIndex);
            return true;
        }

        public void SetActiveEvent(CardData eventCard)
        {
            activeEvent = eventCard;
            activeEventTurnsRemaining = eventCard != null ? Mathf.Max(1, eventCard.eventDuration) : 0;

            if (eventCard != null)
            {
                int idx = _slotManager != null ? _slotManager.GetFirstEmptyIndex(SlotType.TempEffect) : -1;
                if (idx < 0 && _slotManager != null)
                {
                    _slotManager.ClearOldestTempEffect();
                    idx = _slotManager.GetFirstEmptyIndex(SlotType.TempEffect);
                }

                if (idx >= 0)
                    TryPlaceCard(eventCard, SlotType.TempEffect, idx);

                _tempDurations[eventCard] = activeEventTurnsRemaining;
            }
        }

        public CardData RemoveEmployee(int businessIndex, int employeeIndex)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count)
                return null;

            var business = playerBusinesses[businessIndex];
            if (business == null || employeeIndex < 0 || employeeIndex >= business.employees.Count)
                return null;

            var removed = business.employees[employeeIndex];
            business.employees.RemoveAt(employeeIndex);
            if (employeeIndex < business.employeeTenure.Count)
                business.employeeTenure.RemoveAt(employeeIndex);

            RemoveCardFromSlots(removed, SlotType.Staff);
            return removed;
        }

        public bool RemoveEmployeeByCard(int businessIndex, CardData employee)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count || employee == null)
                return false;

            int idx = playerBusinesses[businessIndex].employees.IndexOf(employee);
            if (idx < 0) return false;
            RemoveEmployee(businessIndex, idx);
            return true;
        }

        public void RemoveBusiness(int businessIndex)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count)
                return;

            var card = playerBusinesses[businessIndex].businessCard;
            playerBusinesses.RemoveAt(businessIndex);
            if (card != null)
                RemoveCardFromSlots(card, SlotType.Operation);
            _operationCardMap.Remove(businessIndex);
        }

        public void CloseBusiness(int businessIndex, int turns)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count) return;
            playerBusinesses[businessIndex].isClosed = true;
            playerBusinesses[businessIndex].closedTurnsRemaining = Mathf.Max(1, turns);
            EventBus.BusinessClosed(businessIndex);
        }

        public void ReopenBusiness(int businessIndex)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count) return;
            playerBusinesses[businessIndex].isClosed = false;
            playerBusinesses[businessIndex].closedTurnsRemaining = 0;
            EventBus.BusinessReopened(businessIndex);
        }

        public void AddCustomersAttracted(int businessIndex, int customers)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count) return;
            playerBusinesses[businessIndex].totalCustomersAttracted += Mathf.Max(0, customers);
        }

        public void TickBusinesses()
        {
            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                var business = playerBusinesses[i];
                if (business == null) continue;

                if (business.isClosed)
                {
                    business.closedTurnsRemaining--;
                    if (business.closedTurnsRemaining <= 0)
                        ReopenBusiness(i);
                    continue;
                }

                business.turnsActive++;
                business.neglectTurns++;
                for (int e = 0; e < business.employeeTenure.Count; e++)
                    business.employeeTenure[e]++;
            }

            TickTempEffects();
        }

        public void TickClosedBusinesses()
        {
            TickBusinesses();
        }

        public int GetActiveBusinessCount()
        {
            int count = 0;
            foreach (var business in playerBusinesses)
            {
                if (business != null && business.businessCard != null && !business.isClosed)
                    count++;
            }
            return count;
        }

        public List<string> GetAllActiveCardIds()
        {
            var ids = new List<string>();
            foreach (var card in GetAllActiveCards())
                ids.Add(card.cardId);
            return ids;
        }

        public HashSet<CardTag> GetAllActiveTags()
        {
            var tags = new HashSet<CardTag>();
            foreach (var card in GetAllActiveCards())
            {
                if (card.tags == null) continue;
                foreach (var tag in card.tags)
                    tags.Add(tag);
            }
            return tags;
        }

        public bool HasTag(CardTag tag)
        {
            return GetAllActiveTags().Contains(tag);
        }

        public int CalculatePlayerCustomers()
        {
            float total = 0f;
            foreach (var card in GetCardsInSlotType(SlotType.Operation))
                total += Mathf.Max(0f, card.demandDelta + card.customersPerTurn);
            foreach (var card in GetCardsInSlotType(SlotType.Marketing))
                total += Mathf.Max(0f, card.demandDelta);
            return Mathf.RoundToInt(total);
        }

        public int FindBusinessWithEmployee(CardData employee)
        {
            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                if (playerBusinesses[i] != null && playerBusinesses[i].employees.Contains(employee))
                    return i;
            }
            return -1;
        }

        public int CountEmployeesById(string cardId)
        {
            int count = 0;
            foreach (var card in GetCardsInSlotType(SlotType.Staff))
                if (card.cardId == cardId)
                    count++;
            return count;
        }

        public List<(CardData employee, int businessIndex, int employeeIndex)> GetAllIllegalEmployees()
        {
            var result = new List<(CardData employee, int businessIndex, int employeeIndex)>();
            for (int b = 0; b < playerBusinesses.Count; b++)
            {
                var business = playerBusinesses[b];
                if (business == null) continue;
                for (int e = 0; e < business.employees.Count; e++)
                {
                    var employee = business.employees[e];
                    if (employee != null && employee.legalRiskDeltaPerTurn > 0f)
                        result.Add((employee, b, e));
                }
            }
            return result;
        }

        public void Reset()
        {
            playerBusinesses.Clear();
            globalUpgrades.Clear();
            activeEvent = null;
            activeEventTurnsRemaining = 0;
            productionDisabledNextTurn = false;
            _tempDurations.Clear();
            _operationCardMap.Clear();
        }

        public void RebuildFromSlots()
        {
            playerBusinesses.Clear();
            globalUpgrades.Clear();
            _tempDurations.Clear();
            _operationCardMap.Clear();
            activeEvent = null;
            activeEventTurnsRemaining = 0;

            if (_slotManager == null)
                return;

            for (int i = 0; i < _slotManager.OperationSlots.Count; i++)
            {
                var card = _slotManager.OperationSlots[i];
                var business = new ActiveBusiness
                {
                    businessCard = card,
                    currentLevel = BusinessLevel.Level1
                };
                playerBusinesses.Add(business);
                if (card != null)
                    _operationCardMap[i] = card;
            }

            foreach (var card in _slotManager.MarketingSlots)
            {
                if (card != null)
                    globalUpgrades.Add(card);
            }

            foreach (var card in _slotManager.SupplierSlots)
            {
                if (card != null)
                    globalUpgrades.Add(card);
            }

            foreach (var card in _slotManager.TempEffectSlots)
            {
                if (card == null) continue;
                _tempDurations[card] = Mathf.Max(1, card.tempEffectDuration);
                if (activeEvent == null && (card.cardType == CardType.Event || card.cardFamily == CardFamily.Crisis))
                {
                    activeEvent = card;
                    activeEventTurnsRemaining = Mathf.Max(1, card.eventDuration > 0 ? card.eventDuration : card.tempEffectDuration);
                }
            }
        }

        public void SetMaxSlots(int slots)
        {
            maxSlots = Mathf.Max(1, slots);
        }

        public void SetProductionDisabledNextTurn(bool disabled)
        {
            productionDisabledNextTurn = disabled;
        }

        public bool ConsumeProductionDisabled()
        {
            bool current = productionDisabledNextTurn;
            productionDisabledNextTurn = false;
            return current;
        }

        public IReadOnlyList<CardData> GetCardsInSlotType(SlotType slotType)
        {
            if (_slotManager == null)
                return Array.Empty<CardData>();

            return slotType switch
            {
                SlotType.Operation => CollectNonNull(_slotManager.OperationSlots),
                SlotType.Staff => CollectNonNull(_slotManager.StaffSlots),
                SlotType.Marketing => CollectNonNull(_slotManager.MarketingSlots),
                SlotType.Supplier => CollectNonNull(_slotManager.SupplierSlots),
                SlotType.TempEffect => CollectNonNull(_slotManager.TempEffectSlots),
                _ => Array.Empty<CardData>()
            };
        }

        public IEnumerable<CardData> GetAllActiveCards()
        {
            foreach (var card in GetCardsInSlotType(SlotType.Operation)) yield return card;
            foreach (var card in GetCardsInSlotType(SlotType.Staff)) yield return card;
            foreach (var card in GetCardsInSlotType(SlotType.Marketing)) yield return card;
            foreach (var card in GetCardsInSlotType(SlotType.Supplier)) yield return card;
            foreach (var card in GetCardsInSlotType(SlotType.TempEffect)) yield return card;
        }

        private bool TryPlaceCard(CardData card, SlotType slotType, int slotIndex)
        {
            if (_slotManager == null || card == null)
                return false;

            return _slotManager.TryPlace(card, slotType, slotIndex);
        }

        private void RemoveCardFromSlots(CardData card, SlotType slotType)
        {
            if (_slotManager == null || card == null)
                return;

            int count = slotType switch
            {
                SlotType.Operation => _slotManager.OperationSlots.Count,
                SlotType.Staff => _slotManager.StaffSlots.Count,
                SlotType.Marketing => _slotManager.MarketingSlots.Count,
                SlotType.Supplier => _slotManager.SupplierSlots.Count,
                SlotType.TempEffect => _slotManager.TempEffectSlots.Count,
                _ => 0
            };

            for (int i = 0; i < count; i++)
            {
                IReadOnlyList<CardData> cards = slotType switch
                {
                    SlotType.Operation => _slotManager.OperationSlots,
                    SlotType.Staff => _slotManager.StaffSlots,
                    SlotType.Marketing => _slotManager.MarketingSlots,
                    SlotType.Supplier => _slotManager.SupplierSlots,
                    SlotType.TempEffect => _slotManager.TempEffectSlots,
                    _ => null
                };

                if (cards == null || cards[i] != card) continue;
                _slotManager.TryRemove(slotType, i, out _);
                break;
            }
        }

        private void TickTempEffects()
        {
            var expired = new List<CardData>();
            var updated = new List<(CardData card, int turns)>();

            foreach (var pair in _tempDurations)
            {
                int next = pair.Value - 1;
                if (next <= 0)
                    expired.Add(pair.Key);
                else
                    updated.Add((pair.Key, next));
            }

            foreach (var item in updated)
            {
                _tempDurations[item.card] = item.turns;
            }

            foreach (var card in expired)
            {
                _tempDurations.Remove(card);
                RemoveCardFromSlots(card, SlotType.TempEffect);
                if (activeEvent == card)
                {
                    EventBus.EventExpired(card);
                    activeEvent = null;
                    activeEventTurnsRemaining = 0;
                }
            }

            if (activeEvent != null && activeEventTurnsRemaining > 0)
                activeEventTurnsRemaining--;
        }

        private static IReadOnlyList<CardData> CollectNonNull(IReadOnlyList<CardData> source)
        {
            var list = new List<CardData>();
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i] != null)
                    list.Add(source[i]);
            }
            return list;
        }
    }
}
