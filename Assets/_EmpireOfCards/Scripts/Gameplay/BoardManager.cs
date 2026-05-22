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

    public partial class BoardManager : MonoBehaviour
    {
        private struct EmployeeAssignment
        {
            public int businessIndex;
            public int tenure;
        }

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
            SetMaxSlots(slotManager != null ? slotManager.OperationMax : Constants.STARTING_OPERATION_SLOTS);
            RebuildFromSlots();
        }

        public void ConfigureForVenture(VentureType ventureType, VentureBoardProfile profile)
        {
            activeVenture = ventureType;
            _boardProfile = profile;
            maxSlots = profile != null ? profile.startingOperationSlots : Constants.STARTING_OPERATION_SLOTS;
            EnsureBusinessSlotMetadataCount(maxSlots);
        }

        public bool PlaceCardInSlot(CardData card, SlotType slotType, int slotIndex, int businessIndex = -1)
        {
            if (!CanPlaceInSubSlot(card, slotType, slotIndex))
                return false;

            return slotType switch
            {
                SlotType.Operation => PlaceBusiness(card, slotIndex),
                SlotType.Staff => PlaceEmployee(card, businessIndex, slotIndex),
                SlotType.Marketing => PlaceUpgrade(card, businessIndex, SlotType.Marketing, slotIndex),
                SlotType.Supplier => PlaceUpgrade(card, businessIndex, SlotType.Supplier, slotIndex),
                SlotType.TempEffect => PlaceUpgrade(card, businessIndex, SlotType.TempEffect, slotIndex),
                _ => false
            };
        }

        public bool CanPlaceInSubSlot(CardData card, SlotType slotType, int slotIndex)
        {
            if (card == null || card.isGeneralCard || string.IsNullOrWhiteSpace(card.targetSubSlotId))
                return true;
            if (_boardProfile == null)
                return true;

            BoardSubSlotDefinition[] definitions = slotType switch
            {
                SlotType.Operation => _boardProfile.operationSubSlots,
                SlotType.Staff => _boardProfile.staffSubSlots,
                SlotType.Marketing => _boardProfile.marketingSubSlots,
                SlotType.Supplier => _boardProfile.supplierSubSlots,
                _ => null
            };

            if (definitions == null || slotIndex < 0 || slotIndex >= definitions.Length)
                return true;

            return definitions[slotIndex] != null && definitions[slotIndex].id == card.targetSubSlotId;
        }

        public bool PlaceBusiness(CardData card, int slotIndex)
        {
            if (card == null || !TryPlaceCard(card, SlotType.Operation, slotIndex))
                return false;

            SyncOperationMetadata();
            ReconcileEmployeeAssignments();
            RefreshGlobalUpgradesCache();
            EventBus.BusinessPlaced(card, slotIndex);
            return true;
        }

        public bool PlaceEmployee(CardData card, int businessIndex)
        {
            int slotIndex = _slotManager != null ? _slotManager.GetFirstEmptyIndex(SlotType.Staff) : -1;
            return PlaceEmployee(card, businessIndex, slotIndex);
        }

        public bool PlaceUpgrade(CardData card, int businessIndex)
        {
            return PlaceUpgrade(card, businessIndex, card != null ? card.targetSlotType : SlotType.Operation, -1);
        }

        public void SetActiveEvent(CardData eventCard)
        {
            activeEvent = eventCard;
            activeEventTurnsRemaining = eventCard != null ? ResolveInitialTempDuration(eventCard) : 0;

            if (eventCard == null)
            {
                RefreshTempEffectsFromSlots();
                return;
            }

            int slotIndex = _slotManager != null ? _slotManager.GetFirstEmptyIndex(SlotType.TempEffect) : -1;
            if (slotIndex < 0 && _slotManager != null)
            {
                _slotManager.ClearOldestTempEffect();
                slotIndex = _slotManager.GetFirstEmptyIndex(SlotType.TempEffect);
            }

            if (slotIndex < 0 || !TryPlaceCard(eventCard, SlotType.TempEffect, slotIndex))
            {
                activeEvent = null;
                activeEventTurnsRemaining = 0;
                return;
            }

            _tempDurations[eventCard] = activeEventTurnsRemaining;
            RefreshTempEffectsFromSlots(_tempDurations);
        }

        public CardData RemoveEmployee(int businessIndex, int employeeIndex)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count)
                return null;

            var business = playerBusinesses[businessIndex];
            if (business == null || employeeIndex < 0 || employeeIndex >= business.employees.Count)
                return null;

            CardData removed = business.employees[employeeIndex];
            if (removed == null)
                return null;

            RemoveCardFromSlots(removed, SlotType.Staff);
            ReconcileEmployeeAssignments();
            return removed;
        }

        public bool RemoveEmployeeByCard(int businessIndex, CardData employee)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count || employee == null)
                return false;

            int idx = playerBusinesses[businessIndex].employees.IndexOf(employee);
            if (idx < 0)
                return false;

            return RemoveEmployee(businessIndex, idx) != null;
        }

        public void RemoveBusiness(int businessIndex)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count)
                return;

            CardData card = playerBusinesses[businessIndex] != null ? playerBusinesses[businessIndex].businessCard : null;
            if (card == null)
                return;

            RemoveCardFromSlots(card, SlotType.Operation);
            SyncOperationMetadata();
            ReconcileEmployeeAssignments();
            RefreshGlobalUpgradesCache();
        }

        public void CloseBusiness(int businessIndex, int turns)
        {
            if (!HasActiveBusinessAt(businessIndex))
                return;

            playerBusinesses[businessIndex].isClosed = true;
            playerBusinesses[businessIndex].closedTurnsRemaining = Mathf.Max(1, turns);
            EventBus.BusinessClosed(businessIndex);
        }

        public void ReopenBusiness(int businessIndex)
        {
            if (!HasActiveBusinessAt(businessIndex))
                return;

            playerBusinesses[businessIndex].isClosed = false;
            playerBusinesses[businessIndex].closedTurnsRemaining = 0;
            EventBus.BusinessReopened(businessIndex);
        }

        public void AddCustomersAttracted(int businessIndex, int customers)
        {
            if (!HasActiveBusinessAt(businessIndex))
                return;

            playerBusinesses[businessIndex].totalCustomersAttracted += Mathf.Max(0, customers);
        }

        public void TickBusinesses()
        {
            SyncOperationMetadata();

            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                ActiveBusiness business = playerBusinesses[i];
                if (business == null || business.businessCard == null)
                    continue;

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
            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                ActiveBusiness business = playerBusinesses[i];
                if (business != null && business.businessCard != null && !business.isClosed)
                    count++;
            }

            return count;
        }

        public List<string> GetAllActiveCardIds()
        {
            var ids = new List<string>();
            foreach (CardData card in GetAllActiveCards())
                ids.Add(card.cardId);

            return ids;
        }

        public HashSet<CardTag> GetAllActiveTags()
        {
            var tags = new HashSet<CardTag>();
            foreach (CardData card in GetAllActiveCards())
            {
                if (card == null || card.tags == null)
                    continue;

                foreach (CardTag tag in card.tags)
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
            foreach (CardData card in GetCardsInSlotType(SlotType.Operation))
                total += Mathf.Max(0f, card.demandDelta + card.customersPerTurn);
            foreach (CardData card in GetCardsInSlotType(SlotType.Marketing))
                total += Mathf.Max(0f, card.demandDelta);

            return Mathf.RoundToInt(total);
        }

        public int FindBusinessWithEmployee(CardData employee)
        {
            if (employee == null)
                return -1;

            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                ActiveBusiness business = playerBusinesses[i];
                if (business != null && business.employees.Contains(employee))
                    return i;
            }

            return -1;
        }

        public int CountEmployeesById(string cardId)
        {
            int count = 0;
            foreach (CardData card in GetCardsInSlotType(SlotType.Staff))
            {
                if (card != null && card.cardId == cardId)
                    count++;
            }

            return count;
        }

        public List<(CardData employee, int businessIndex, int employeeIndex)> GetAllIllegalEmployees()
        {
            var result = new List<(CardData employee, int businessIndex, int employeeIndex)>();
            for (int b = 0; b < playerBusinesses.Count; b++)
            {
                ActiveBusiness business = playerBusinesses[b];
                if (business == null || business.businessCard == null)
                    continue;

                for (int e = 0; e < business.employees.Count; e++)
                {
                    CardData employee = business.employees[e];
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
            EnsureBusinessSlotMetadataCount(maxSlots);
        }

        public void RebuildFromSlots()
        {
            SyncOperationMetadata();
            ReconcileEmployeeAssignments();
            RefreshGlobalUpgradesCache();
            RefreshTempEffectsFromSlots();
        }

        public void SetMaxSlots(int slots)
        {
            maxSlots = Mathf.Max(1, slots);
            EnsureBusinessSlotMetadataCount(maxSlots);
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

            return _slotManager.GetActiveCards(slotType);
        }

        public IEnumerable<CardData> GetAllActiveCards()
        {
            foreach (CardData card in GetCardsInSlotType(SlotType.Operation)) yield return card;
            foreach (CardData card in GetCardsInSlotType(SlotType.Staff)) yield return card;
            foreach (CardData card in GetCardsInSlotType(SlotType.Marketing)) yield return card;
            foreach (CardData card in GetCardsInSlotType(SlotType.Supplier)) yield return card;
            foreach (CardData card in GetCardsInSlotType(SlotType.TempEffect)) yield return card;
        }

        private bool PlaceEmployee(CardData card, int businessIndex, int slotIndex)
        {
            if (card == null || slotIndex < 0 || !TryPlaceCard(card, SlotType.Staff, slotIndex))
                return false;

            Dictionary<CardData, int> overrides = null;
            if (businessIndex >= 0)
                overrides = new Dictionary<CardData, int> { [card] = businessIndex };

            if (!ReconcileEmployeeAssignments(overrides, true))
            {
                RemoveCardFromSlots(card, SlotType.Staff);
                ReconcileEmployeeAssignments();
                return false;
            }

            int resolvedBusinessIndex = FindBusinessWithEmployee(card);
            EventBus.EmployeePlaced(card, resolvedBusinessIndex);
            return resolvedBusinessIndex >= 0;
        }

        private bool PlaceUpgrade(CardData card, int businessIndex, SlotType forcedSlotType, int preferredSlotIndex)
        {
            if (card == null)
                return false;

            SlotType target = forcedSlotType;
            if (target != SlotType.TempEffect && target != SlotType.Marketing && target != SlotType.Supplier && target != SlotType.Operation)
                return false;

            int slotIndex = preferredSlotIndex >= 0 ? preferredSlotIndex : (_slotManager != null ? _slotManager.GetFirstEmptyIndex(target) : -1);
            if (slotIndex < 0 && target == SlotType.TempEffect && _slotManager != null)
            {
                _slotManager.ClearOldestTempEffect();
                RefreshTempEffectsFromSlots();
                slotIndex = _slotManager.GetFirstEmptyIndex(target);
            }

            if (slotIndex < 0 || !TryPlaceCard(card, target, slotIndex))
                return false;

            RefreshGlobalUpgradesCache();
            if (target == SlotType.TempEffect)
            {
                _tempDurations[card] = ResolveInitialTempDuration(card);
                RefreshTempEffectsFromSlots(_tempDurations);
            }

            EventBus.UpgradePlaced(card, businessIndex);
            return true;
        }

        private bool ReconcileEmployeeAssignments(Dictionary<CardData, int> explicitAssignments = null, bool failOnUnassigned = false)
        {
            if (_slotManager == null)
                return false;

            SyncOperationMetadata();
            Dictionary<CardData, EmployeeAssignment> previousAssignments = CaptureEmployeeAssignments();
            ClearEmployeeAssignments();

            IReadOnlyList<CardData> staffSlots = _slotManager.StaffSlots;
            for (int i = 0; i < staffSlots.Count; i++)
            {
                CardData employee = staffSlots[i];
                if (employee == null)
                    continue;

                if (explicitAssignments != null && explicitAssignments.TryGetValue(employee, out int explicitBusinessIndex))
                {
                    if (!TryAttachEmployeeToBusiness(employee, explicitBusinessIndex, previousAssignments))
                        return false;
                    continue;
                }

                int preferredBusinessIndex = previousAssignments.TryGetValue(employee, out EmployeeAssignment previous)
                    ? previous.businessIndex
                    : -1;

                if (!TryAssignEmployee(employee, preferredBusinessIndex, previousAssignments) && failOnUnassigned)
                    return false;
            }

            return true;
        }

        private bool TryAssignEmployee(CardData employee, int preferredBusinessIndex, Dictionary<CardData, EmployeeAssignment> previousAssignments)
        {
            if (TryAttachEmployeeToBusiness(employee, preferredBusinessIndex, previousAssignments))
                return true;

            int autoBusinessIndex = FindLeastLoadedBusinessIndex();
            return TryAttachEmployeeToBusiness(employee, autoBusinessIndex, previousAssignments);
        }

        private bool TryAttachEmployeeToBusiness(CardData employee, int businessIndex, Dictionary<CardData, EmployeeAssignment> previousAssignments)
        {
            if (!CanAssignEmployeeToBusiness(businessIndex))
                return false;

            ActiveBusiness business = playerBusinesses[businessIndex];
            if (business.employees.Count >= business.GetAvailableEmployeeSlots())
                return false;

            int tenure = 0;
            if (previousAssignments != null && previousAssignments.TryGetValue(employee, out EmployeeAssignment previous) && previous.businessIndex == businessIndex)
                tenure = previous.tenure;

            business.employees.Add(employee);
            business.employeeTenure.Add(tenure);
            return true;
        }

        private int FindLeastLoadedBusinessIndex()
        {
            int bestIndex = -1;
            int lowestLoad = int.MaxValue;

            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                if (!CanAssignEmployeeToBusiness(i))
                    continue;

                ActiveBusiness business = playerBusinesses[i];
                int load = business.employees.Count;
                if (load >= business.GetAvailableEmployeeSlots())
                    continue;

                if (load < lowestLoad)
                {
                    lowestLoad = load;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private bool CanAssignEmployeeToBusiness(int businessIndex)
        {
            return HasActiveBusinessAt(businessIndex) && !playerBusinesses[businessIndex].isClosed;
        }

        private bool HasActiveBusinessAt(int businessIndex)
        {
            return businessIndex >= 0
                && businessIndex < playerBusinesses.Count
                && playerBusinesses[businessIndex] != null
                && playerBusinesses[businessIndex].businessCard != null;
        }

        private Dictionary<CardData, EmployeeAssignment> CaptureEmployeeAssignments()
        {
            var assignments = new Dictionary<CardData, EmployeeAssignment>();
            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                ActiveBusiness business = playerBusinesses[i];
                if (business == null)
                    continue;

                for (int e = 0; e < business.employees.Count; e++)
                {
                    CardData employee = business.employees[e];
                    if (employee == null || assignments.ContainsKey(employee))
                        continue;

                    assignments[employee] = new EmployeeAssignment
                    {
                        businessIndex = i,
                        tenure = e < business.employeeTenure.Count ? business.employeeTenure[e] : 0
                    };
                }
            }

            return assignments;
        }

        private void ClearEmployeeAssignments()
        {
            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                ActiveBusiness business = EnsureBusinessMetadata(i);
                business.employees.Clear();
                business.employeeTenure.Clear();
            }
        }

        private void SyncOperationMetadata()
        {
            int slotCount = _slotManager != null ? _slotManager.OperationSlots.Count : maxSlots;
            EnsureBusinessSlotMetadataCount(slotCount);
            maxSlots = slotCount;

            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                CardData slotCard = _slotManager != null && i < _slotManager.OperationSlots.Count ? _slotManager.OperationSlots[i] : null;
                CardData businessCard = slotCard != null && slotCard.cardType == CardType.Business ? slotCard : null;
                ActiveBusiness existing = EnsureBusinessMetadata(i);

                if (existing.businessCard == businessCard)
                    continue;

                if (businessCard == null)
                {
                    playerBusinesses[i] = new ActiveBusiness();
                    continue;
                }

                playerBusinesses[i] = new ActiveBusiness
                {
                    businessCard = businessCard,
                    currentLevel = BusinessLevel.Level1
                };
            }
        }

        private void RefreshGlobalUpgradesCache()
        {
            globalUpgrades.Clear();
            if (_slotManager == null)
                return;

            AddUpgradesFromSlots(_slotManager.OperationSlots);
            AddUpgradesFromSlots(_slotManager.MarketingSlots);
            AddUpgradesFromSlots(_slotManager.SupplierSlots);
        }

        private void AddUpgradesFromSlots(IReadOnlyList<CardData> cards)
        {
            if (cards == null)
                return;

            for (int i = 0; i < cards.Count; i++)
            {
                CardData card = cards[i];
                if (card != null && card.cardType == CardType.Upgrade)
                    globalUpgrades.Add(card);
            }
        }

        private void RefreshTempEffectsFromSlots(Dictionary<CardData, int> durationOverrides = null)
        {
            Dictionary<CardData, int> nextDurations = new Dictionary<CardData, int>();
            if (_slotManager != null)
            {
                IReadOnlyList<CardData> tempSlots = _slotManager.TempEffectSlots;
                for (int i = 0; i < tempSlots.Count; i++)
                {
                    CardData card = tempSlots[i];
                    if (card == null)
                        continue;

                    int duration;
                    if (durationOverrides != null && durationOverrides.TryGetValue(card, out duration))
                    {
                    }
                    else if (_tempDurations.TryGetValue(card, out duration))
                    {
                    }
                    else
                    {
                        duration = ResolveInitialTempDuration(card);
                    }

                    nextDurations[card] = Mathf.Max(1, duration);
                }
            }

            _tempDurations.Clear();
            foreach (KeyValuePair<CardData, int> pair in nextDurations)
                _tempDurations[pair.Key] = pair.Value;

            activeEvent = null;
            activeEventTurnsRemaining = 0;

            if (_slotManager == null)
                return;

            IReadOnlyList<CardData> refreshedSlots = _slotManager.TempEffectSlots;
            for (int i = 0; i < refreshedSlots.Count; i++)
            {
                CardData card = refreshedSlots[i];
                if (card == null)
                    continue;

                if (card.cardType == CardType.Event || card.cardFamily == CardFamily.Crisis)
                {
                    activeEvent = card;
                    activeEventTurnsRemaining = _tempDurations.TryGetValue(card, out int duration)
                        ? duration
                        : ResolveInitialTempDuration(card);
                    break;
                }
            }
        }

        private static int ResolveInitialTempDuration(CardData card)
        {
            if (card == null)
                return 1;

            int duration = card.eventDuration > 0 ? card.eventDuration : card.tempEffectDuration;
            return Mathf.Max(1, duration);
        }

        private bool TryPlaceCard(CardData card, SlotType slotType, int slotIndex)
        {
            return _slotManager != null && card != null && _slotManager.TryPlace(card, slotType, slotIndex);
        }

        private void RemoveCardFromSlots(CardData card, SlotType slotType)
        {
            if (_slotManager == null || card == null)
                return;

            int index = _slotManager.FindCardIndex(slotType, card);
            if (index >= 0)
                _slotManager.TryRemove(slotType, index, out _);
        }

        private void TickTempEffects()
        {
            Dictionary<CardData, int> nextDurations = new Dictionary<CardData, int>();
            List<CardData> expired = new List<CardData>();

            foreach (KeyValuePair<CardData, int> pair in _tempDurations)
            {
                int next = pair.Value - 1;
                if (next <= 0)
                    expired.Add(pair.Key);
                else
                    nextDurations[pair.Key] = next;
            }

            for (int i = 0; i < expired.Count; i++)
            {
                CardData card = expired[i];
                bool wasActiveEvent = activeEvent == card;
                RemoveCardFromSlots(card, SlotType.TempEffect);
                if (wasActiveEvent)
                    EventBus.EventExpired(card);
            }

            RefreshTempEffectsFromSlots(nextDurations);
        }

        private void EnsureBusinessSlotMetadataCount(int count)
        {
            count = Mathf.Max(0, count);
            while (playerBusinesses.Count < count)
                playerBusinesses.Add(new ActiveBusiness());

            while (playerBusinesses.Count > count)
                playerBusinesses.RemoveAt(playerBusinesses.Count - 1);
        }

        private ActiveBusiness EnsureBusinessMetadata(int index)
        {
            EnsureBusinessSlotMetadataCount(index + 1);
            if (playerBusinesses[index] == null)
                playerBusinesses[index] = new ActiveBusiness();

            return playerBusinesses[index];
        }
    }
}
