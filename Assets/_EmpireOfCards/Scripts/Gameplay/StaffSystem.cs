using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public class StaffSystem : MonoBehaviour
    {
        BoardManager _board;

        readonly Dictionary<CardData, StaffState> _staffStates = new();

        public void Init(BoardManager board)
        {
            _board = board;
            Debug.Log("[StaffSystem] Initialized.");
        }

        // ── EventBus subscriptions ─────────────────────────────────

        void OnEnable()
        {
            EventBus.OnCardPlaced += HandleCardPlaced;
            EventBus.OnCardRemoved += HandleCardRemoved;
        }

        void OnDisable()
        {
            EventBus.OnCardPlaced -= HandleCardPlaced;
            EventBus.OnCardRemoved -= HandleCardRemoved;
        }

        void HandleCardPlaced(CardData card, SlotType slot)
        {
            if (card.cardType != CardType.Staff) return;
            if (_staffStates.ContainsKey(card)) return;

            _staffStates[card] = new StaffState
            {
                morale = 5f,
                experience = 0,
                tier = card.startingTier,
                overworkTurns = 0
            };

            EventBus.StaffHired(card.cardName);
        }

        void HandleCardRemoved(CardData card, SlotType slot)
        {
            if (card.cardType != CardType.Staff) return;
            _staffStates.Remove(card);
        }

        // ── Tick (called by EconomyManager during UpdateStaff step) ─

        public void TickStaff()
        {
            if (_board == null) return;

            var gm = GameManager.Instance;
            if (gm == null) return;
            var res = gm.Resources;

            bool overworked = res.GetDemand() > res.GetCapacity();
            bool goodEnvironment = res.GetHygiene() > 7f
                                   && res.GetStaffStability() > 7f;

            var toRemove = new List<CardData>();

            foreach (var kvp in _staffStates)
            {
                var card = kvp.Key;
                var state = kvp.Value;

                IncrementExperience(state);
                UpdateMorale(state, overworked, goodEnvironment);

                if (ShouldResign(state))
                {
                    toRemove.Add(card);
                    EventBus.StaffQuit(card.cardName, ResignReason(state));
                    EventBus.MoraleChanged(card.cardName, state.morale);
                }
                else
                {
                    EventBus.MoraleChanged(card.cardName, state.morale);
                }
            }

            foreach (var card in toRemove)
            {
                _staffStates.Remove(card);
                _board.RemoveCard(card);
            }

            if (toRemove.Count > 0) ApplyColleagueQuitPenalty();

            CheckPromotions();
        }

        // ── Experience ─────────────────────────────────────────────

        void IncrementExperience(StaffState state)
        {
            state.experience++;
        }

        // ── Morale ─────────────────────────────────────────────────

        void UpdateMorale(StaffState state, bool overworked, bool goodEnvironment)
        {
            if (overworked)
            {
                state.overworkTurns++;
                state.morale += Constants.OVERWORK_MORALE_DECAY;
            }
            else
            {
                state.overworkTurns = Mathf.Max(0, state.overworkTurns - 1);
            }

            if (goodEnvironment)
                state.morale += Constants.MORALE_GOOD_ENVIRONMENT;

            state.morale = Mathf.Clamp(state.morale, 0f, 10f);
        }

        // ── Salary Processing ──────────────────────────────────────

        public void ProcessSalaryChoice(SalaryChoice choice)
        {
            float delta = GetSalaryMoraleDelta(choice);
            foreach (var state in _staffStates.Values)
            {
                state.morale = Mathf.Clamp(state.morale + delta, 0f, 10f);
            }
        }

        public void ApplyMoraleChange(string reason, float delta)
        {
            foreach (var kvp in _staffStates)
            {
                var state = kvp.Value;
                state.morale = Mathf.Clamp(state.morale + delta, 0f, 10f);
                EventBus.MoraleChanged(kvp.Key.cardName, state.morale);
            }
        }

        // ── Promotions ─────────────────────────────────────────────

        void CheckPromotions()
        {
            foreach (var kvp in _staffStates)
            {
                var card = kvp.Key;
                var state = kvp.Value;

                if (!card.canPromote) continue;
                if (state.experience < Constants.PROMOTION_EXPERIENCE_REQUIRED) continue;
                if (state.tier >= StaffTier.Master) continue;

                state.tier++;
                state.experience = 0;
                state.morale = Mathf.Min(state.morale + 3f, 10f);

                EventBus.StaffPromoted(card.cardName, state.tier);
            }
        }

        // ── Resignations ──────────────────────────────────────────

        bool ShouldResign(StaffState state)
        {
            if (state.morale < Constants.RESIGNATION_MORALE_CRITICAL)
                return Random.value < Constants.RESIGNATION_CHANCE_HIGH;

            if (state.morale < Constants.RESIGNATION_MORALE_THRESHOLD)
                return Random.value < Constants.RESIGNATION_CHANCE_LOW;

            return false;
        }

        string ResignReason(StaffState state)
        {
            if (state.morale < Constants.RESIGNATION_MORALE_CRITICAL)
                return "Moral cok dusuk";
            if (state.overworkTurns >= 3)
                return "Fazla mesai tukenmesi";
            return "Dusuk moral";
        }

        void ApplyColleagueQuitPenalty()
        {
            foreach (var state in _staffStates.Values)
                state.morale = Mathf.Clamp(
                    state.morale + Constants.MORALE_COLLEAGUE_QUIT, 0f, 10f);
        }

        // ── Stability Bonus ────────────────────────────────────────

        public float GetTotalStabilityBonus()
        {
            if (_board == null) return 0f;

            float total = 0f;
            foreach (var kvp in _staffStates)
            {
                total += kvp.Key.staffStabilityDelta;
                if (kvp.Value.morale > 7f) total += 0.2f;
                if (kvp.Value.morale < 3f) total -= 0.3f;
            }
            return total;
        }

        // ── Queries ────────────────────────────────────────────────

        public int StaffCount => _staffStates.Count;

        public float GetAverageMorale()
        {
            if (_staffStates.Count == 0) return 5f;
            float sum = 0f;
            foreach (var state in _staffStates.Values) sum += state.morale;
            return sum / _staffStates.Count;
        }

        // ── Helpers ────────────────────────────────────────────────

        float GetSalaryMoraleDelta(SalaryChoice choice)
        {
            switch (choice)
            {
                case SalaryChoice.PayOnTime:  return Constants.MORALE_SALARY_ON_TIME;
                case SalaryChoice.Delay:      return Constants.MORALE_SALARY_DELAY;
                case SalaryChoice.PartialPay: return Constants.MORALE_SALARY_PARTIAL;
                case SalaryChoice.Advance:    return Constants.MORALE_SALARY_ADVANCE;
                default:                      return 0f;
            }
        }

        // ── Inner Types ────────────────────────────────────────────

        class StaffState
        {
            public float morale;
            public int experience;
            public StaffTier tier;
            public int overworkTurns;
        }
    }
}
