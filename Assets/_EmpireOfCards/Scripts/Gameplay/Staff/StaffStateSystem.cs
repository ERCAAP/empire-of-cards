using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay.Staff
{
    [Serializable]
    public class StaffState
    {
        public CardData card;
        public StaffRole role;
        public EmploymentStatus employmentStatus;
        public int baseSalary;
        public int negotiatedSalary;
        public int trialTurnsRemaining;
        public float workload;
        public float burnoutRisk;
        public float resignRisk;
        public QuitReason lastQuitReason;
        public int moral;
        public int fatigue;
        public int loyalty;
        public int experience;
        public int turnsWorked;
        public int consecutiveOvertimeTurns;
        public bool overtimeThisTurn;

        public int Level => Mathf.Clamp(experience / Constants.STAFF_XP_PER_LEVEL, 0, Constants.STAFF_MAX_LEVEL);
    }

    public partial class StaffStateSystem : MonoBehaviour
    {
        private List<StaffState> _staffStates = new List<StaffState>();
        private StaffWorkloadReport _lastWorkloadReport = new StaffWorkloadReport();

        // Exposed for other systems to query staff modifiers
        public IReadOnlyList<StaffState> StaffStates => _staffStates;

        // ----------------------------------------------------------------
        // EventBus Subscriptions
        // ----------------------------------------------------------------

        private void OnEnable()
        {
            EventBus.OnCardPlacedInSlot += HandleCardPlacedInSlot;
            EventBus.OnCardRemovedFromSlot += HandleCardRemovedFromSlot;
            EventBus.OnEmployeePlaced += HandleEmployeePlaced;
            EventBus.OnEmployeeLeft += HandleEmployeeLeft;
        }

        private void OnDisable()
        {
            EventBus.OnCardPlacedInSlot -= HandleCardPlacedInSlot;
            EventBus.OnCardRemovedFromSlot -= HandleCardRemovedFromSlot;
            EventBus.OnEmployeePlaced -= HandleEmployeePlaced;
            EventBus.OnEmployeeLeft -= HandleEmployeeLeft;
        }

        private void HandleCardPlacedInSlot(CardData card, SlotType slotType)
        {
            if (card != null && card.cardType == CardType.Employee && slotType == SlotType.Staff)
                RegisterStaff(card);
        }

        private void HandleCardRemovedFromSlot(CardData card, SlotType slotType)
        {
            if (card != null && card.cardType == CardType.Employee && slotType == SlotType.Staff)
                RemoveStaff(card);
        }

        private void HandleEmployeePlaced(CardData card, int businessIndex)
        {
            if (card != null && card.cardType == CardType.Employee)
                RegisterStaff(card);
        }

        private void HandleEmployeeLeft(CardData card, int businessIndex)
        {
            if (card != null)
                RemoveStaff(card);
        }

        // ----------------------------------------------------------------
        // Registration
        // ----------------------------------------------------------------

        public void RegisterStaff(CardData card)
        {
            if (card == null) return;

            // Prevent duplicate registration
            for (int i = 0; i < _staffStates.Count; i++)
            {
                if (_staffStates[i].card == card) return;
            }

            var state = new StaffState
            {
                card = card,
                role = card.staffRole,
                employmentStatus = EmploymentStatus.Active,
                baseSalary = Mathf.Max(0, card.salaryPerTurn > 0 ? card.salaryPerTurn : Mathf.RoundToInt(card.upkeepCostPerTurn)),
                negotiatedSalary = Mathf.Max(0, card.salaryPerTurn > 0 ? card.salaryPerTurn : Mathf.RoundToInt(card.upkeepCostPerTurn)),
                trialTurnsRemaining = Mathf.Max(0, card.defaultTrialTurns),
                workload = 0f,
                burnoutRisk = 0f,
                resignRisk = 0f,
                lastQuitReason = QuitReason.None,
                moral = Constants.STAFF_DEFAULT_MORAL,
                fatigue = Constants.STAFF_DEFAULT_FATIGUE,
                loyalty = Constants.STAFF_DEFAULT_LOYALTY,
                experience = 0,
                turnsWorked = 0,
                consecutiveOvertimeTurns = 0,
                overtimeThisTurn = false
            };

            _staffStates.Add(state);
            Debug.Log($"[StaffStateSystem] Registered: {card.cardName} (moral={state.moral}, loyalty={state.loyalty})");
        }

        public void RemoveStaff(CardData card)
        {
            if (card == null) return;

            for (int i = _staffStates.Count - 1; i >= 0; i--)
            {
                if (_staffStates[i].card == card)
                {
                    Debug.Log($"[StaffStateSystem] Removed: {card.cardName}");
                    _staffStates.RemoveAt(i);
                    return;
                }
            }
        }

        // ----------------------------------------------------------------
        // Per-Turn Tick (called during ResolvePhase DeteriorationCheck)
        // ----------------------------------------------------------------

        public void TickAll()
        {
            for (int i = _staffStates.Count - 1; i >= 0; i--)
            {
                var s = _staffStates[i];
                s.turnsWorked++;

                if (s.employmentStatus == EmploymentStatus.Trial)
                {
                    s.trialTurnsRemaining = Mathf.Max(0, s.trialTurnsRemaining - 1);
                    if (s.trialTurnsRemaining == 0)
                    {
                        s.employmentStatus = EmploymentStatus.Active;
                        EventBus.StaffTrialCompleted(s.card);
                    }
                }

                // XP gain (GDD 6.4): +5 XP per turn
                s.experience = Mathf.Min(s.experience + Constants.STAFF_XP_PER_TURN, Constants.STAFF_MAX_LEVEL * Constants.STAFF_XP_PER_LEVEL);

                // Fatigue natural recovery: -1 per turn if no overtime
                if (!s.overtimeThisTurn)
                {
                    s.fatigue = Mathf.Max(0, s.fatigue - 1);
                    s.consecutiveOvertimeTurns = 0;
                }

                // Moral natural drift toward default (slow recovery/decline)
                if (s.moral < Constants.STAFF_DEFAULT_MORAL && s.fatigue <= 3)
                    s.moral = Mathf.Min(s.moral + 1, Constants.STAFF_MORAL_MAX);

                // High fatigue drains moral
                if (s.fatigue > 7)
                    s.moral = Mathf.Max(0, s.moral - 1);

                // Low moral drains loyalty
                if (s.moral < 3)
                {
                    s.loyalty = Mathf.Max(0, s.loyalty - 1);
                    s.resignRisk = Mathf.Clamp01(s.resignRisk + 0.05f);
                }

                // High moral boosts loyalty slowly
                if (s.moral > 8 && s.loyalty < Constants.STAFF_LOYALTY_MAX)
                {
                    s.loyalty = Mathf.Min(s.loyalty + 1, Constants.STAFF_LOYALTY_MAX);
                    s.resignRisk = Mathf.Max(0f, s.resignRisk - 0.04f);
                }

                // Check fatigue > 9: "Staff Strike" risk (GDD 6.3)
                if (s.fatigue > 9)
                {
                    if (UnityEngine.Random.value < Constants.STAFF_STRIKE_FATIGUE_CHANCE)
                    {
                        EventBus.StaffStrikeTriggered(s.card);
                        Debug.Log($"[StaffStateSystem] Staff strike triggered by extreme fatigue: {s.card.cardName}");
                    }
                }

                // Check low loyalty: rival can steal (GDD 6.5)
                if (s.loyalty < 3)
                {
                    if (UnityEngine.Random.value < Constants.STAFF_RIVAL_STEAL_CHANCE)
                    {
                        EventBus.StaffStolenByRival(s.card);
                        Debug.Log($"[StaffStateSystem] Rival stole low-loyalty employee: {s.card.cardName}");
                    }
                }

                // Level-up check: notify when level changes
                int newLevel = s.Level;
                int prevLevel = Mathf.Clamp((s.experience - Constants.STAFF_XP_PER_TURN) / Constants.STAFF_XP_PER_LEVEL, 0, Constants.STAFF_MAX_LEVEL);
                if (newLevel > prevLevel)
                {
                    EventBus.StaffLeveledUp(s.card, newLevel);
                    Debug.Log($"[StaffStateSystem] {s.card.cardName} leveled up to {newLevel}");

                    // Level 4: unlock active ability (GDD 6.4)
                    if (newLevel >= Constants.STAFF_ABILITY_UNLOCK_LEVEL)
                    {
                        Debug.Log($"[StaffStateSystem] {s.card.cardName} unlocked active ability at level {newLevel}");
                    }
                }

                // Reset overtime flag for next turn
                s.overtimeThisTurn = false;
                s.workload = Mathf.Max(0f, s.workload - 0.75f);
                s.burnoutRisk = Mathf.Clamp01(s.burnoutRisk + Mathf.Max(0, s.fatigue - 7) * 0.015f - (s.moral >= 7 ? 0.02f : 0f));
                if (s.burnoutRisk >= 0.75f)
                    EventBus.StaffBurnoutRiskChanged(s.card, s.burnoutRisk);

                // Fire state update event
                EventBus.StaffStateUpdated(s.card, s.moral, s.fatigue, s.loyalty, s.experience);
            }

            TryResolveQuitChecks();
        }

        // ----------------------------------------------------------------
        // Overtime (GDD 6.3) - called by ActionCardResolver
        // ----------------------------------------------------------------

        public void ApplyOvertime()
        {
            for (int i = 0; i < _staffStates.Count; i++)
            {
                var s = _staffStates[i];
                s.overtimeThisTurn = true;
                s.fatigue = Mathf.Min(s.fatigue + Constants.STAFF_OVERTIME_FATIGUE, Constants.STAFF_FATIGUE_MAX);
                s.consecutiveOvertimeTurns++;

                // 3 consecutive overtime turns -> strike risk (GDD 6.3)
                if (s.consecutiveOvertimeTurns >= Constants.STAFF_OVERTIME_STRIKE_THRESHOLD)
                {
                    EventBus.StaffStrikeTriggered(s.card);
                    Debug.Log($"[StaffStateSystem] Staff strike from consecutive overtime: {s.card.cardName}");
                }
            }
        }

        // ----------------------------------------------------------------
        // Modifier Queries (used by EconomyManager)
        // ----------------------------------------------------------------

        public StaffState GetState(CardData card)
        {
            if (card == null) return null;
            for (int i = 0; i < _staffStates.Count; i++)
            {
                if (_staffStates[i].card == card)
                    return _staffStates[i];
            }
            return null;
        }

        public float GetEfficiencyMultiplier(CardData card)
        {
            var s = GetState(card);
            if (s == null) return 1f;

            float multiplier = 1f;

            // Moral effects (GDD 6.1)
            if (s.moral < 3)
                multiplier -= Constants.STAFF_LOW_MORAL_ERROR_PENALTY;
            else if (s.moral < 5)
                multiplier -= Constants.STAFF_MID_MORAL_EFFICIENCY_PENALTY;
            else if (s.moral > 8)
                multiplier += Constants.STAFF_HIGH_MORAL_BONUS;

            // Fatigue effects (GDD 6.1)
            if (s.fatigue > 7)
                multiplier -= Constants.STAFF_HIGH_FATIGUE_PENALTY;

            // Level bonuses (GDD 6.4)
            int level = s.Level;
            if (level >= 2)
                multiplier += Constants.STAFF_LEVEL2_ERROR_REDUCTION;
            if (level >= 3)
                multiplier += Constants.STAFF_LEVEL3_EFFICIENCY_BONUS;

            return Mathf.Max(0.1f, multiplier); // Never go below 10%
        }

        public int GetCustomerSatisfactionBonus(CardData card)
        {
            var s = GetState(card);
            if (s == null) return 0;

            int bonus = 0;

            // Level 3+: +1 customer satisfaction (GDD 6.4)
            if (s.Level >= 3)
                bonus += Constants.STAFF_LEVEL3_CUSTOMER_SATISFACTION;

            return bonus;
        }

        public bool IsAbilityUnlocked(CardData card)
        {
            var s = GetState(card);
            if (s == null) return false;
            return s.Level >= Constants.STAFF_ABILITY_UNLOCK_LEVEL
                && card.activeAbilityType != ActiveAbilityType.None;
        }

        public bool HasTransferProtection(CardData card)
        {
            var s = GetState(card);
            if (s == null) return false;
            return s.loyalty >= Constants.STAFF_TRANSFER_PROTECTION_LOYALTY;
        }

        // ----------------------------------------------------------------
        // Overtime capacity bonus query
        // ----------------------------------------------------------------

        public bool IsOvertimeActive()
        {
            for (int i = 0; i < _staffStates.Count; i++)
            {
                if (_staffStates[i].overtimeThisTurn) return true;
            }
            return false;
        }

        // ----------------------------------------------------------------
        // Reset
        // ----------------------------------------------------------------

        public void Reset()
        {
            _staffStates.Clear();
            _lastWorkloadReport = new StaffWorkloadReport();
        }
    }
}
