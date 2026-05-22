using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay.Economy;

namespace EmpireOfCards.Gameplay.Staff
{
    public partial class StaffStateSystem
    {
        public StaffRoleCoverage GetRoleCoverage(VentureType venture)
        {
            var required = GetRequiredRoles(venture);
            var covered = new HashSet<StaffRole>();
            for (int i = 0; i < _staffStates.Count; i++)
            {
                StaffState state = _staffStates[i];
                if (state == null || state.card == null)
                    continue;
                if (state.employmentStatus == EmploymentStatus.Quit || state.employmentStatus == EmploymentStatus.Poached)
                    continue;
                covered.Add(state.role);
            }

            var result = new StaffRoleCoverage
            {
                ventureType = venture,
                requiredRoleCount = required.Length
            };

            for (int i = 0; i < required.Length; i++)
            {
                if (covered.Contains(required[i]))
                    result.coveredRoleCount++;
                else
                    result.missingRoles.Add(required[i]);
            }

            result.coverageRatio = result.requiredRoleCount > 0
                ? Mathf.Clamp01((float)result.coveredRoleCount / result.requiredRoleCount)
                : 1f;
            return result;
        }

        public float GetWorkloadPressure()
        {
            return _lastWorkloadReport != null ? _lastWorkloadReport.workloadPressure : 0f;
        }

        public int GetNegotiatedSalary(CardData card)
        {
            StaffState state = GetState(card);
            if (state == null)
                return card != null ? Mathf.Max(0, card.salaryPerTurn > 0 ? card.salaryPerTurn : Mathf.RoundToInt(card.upkeepCostPerTurn)) : 0;
            return Mathf.Max(0, state.negotiatedSalary > 0 ? state.negotiatedSalary : state.baseSalary);
        }

        public StaffWorkloadReport ResolveWorkload(
            VentureType venture,
            float demand,
            float capacity,
            int operationCount,
            IReadOnlyList<CardData> tempEffects)
        {
            StaffRoleCoverage coverage = GetRoleCoverage(venture);
            float overload = Mathf.Max(0f, demand - capacity);
            float missingRolePressure = coverage.missingRoles.Count * 1.35f;
            float operationPressure = Mathf.Max(0, operationCount - CountWorkingStaff()) * 0.65f;
            float tempPressure = SumTemp(tempEffects, c => c.workloadDeltaPerTurn);
            float workloadPressure = Mathf.Max(0f, overload + missingRolePressure + operationPressure + tempPressure);

            int workingStaff = CountWorkingStaff();
            if (workingStaff > 0)
                ApplyWorkloadToStaff(workloadPressure / workingStaff, tempEffects);

            var report = new StaffWorkloadReport
            {
                ventureType = venture,
                demand = demand,
                capacity = capacity,
                workloadPressure = workloadPressure,
                coverage = coverage,
                capacityPenalty = coverage.missingRoles.Count * 0.35f + (workingStaff == 0 && demand > 1f ? 1.25f : 0f),
                qualityPenalty = coverage.missingRoles.Count * 0.18f + Mathf.Max(0f, workloadPressure - 4f) * 0.08f,
                ratingPenalty = coverage.missingRoles.Count * 0.08f + Mathf.Max(0f, workloadPressure - 5f) * 0.045f,
                staffStabilityPenalty = coverage.missingRoles.Count * 0.25f + Mathf.Max(0f, workloadPressure - 3f) * 0.12f
            };

            _lastWorkloadReport = report;
            EventBus.StaffWorkloadChanged(report);
            return report;
        }

        public void ApplySalaryResult(SalaryResult result)
        {
            if (result == null)
                return;

            for (int i = 0; i < _staffStates.Count; i++)
            {
                StaffState state = _staffStates[i];
                if (state == null)
                    continue;

                state.moral = Mathf.Clamp(state.moral + result.moraleChange, 0, Constants.STAFF_MORAL_MAX);
                state.loyalty = Mathf.Clamp(state.loyalty + result.loyaltyChange, 0, Constants.STAFF_LOYALTY_MAX);
                state.resignRisk = Mathf.Clamp01(state.resignRisk + result.resignRiskIncrease);
            }
        }

        public HiringDecisionResult ApplyHiringDecision(StaffApplicant applicant, HiringDecision decision)
        {
            var result = new HiringDecisionResult
            {
                applicant = applicant,
                decision = decision,
                accepted = false,
                agreedSalary = applicant != null ? applicant.requestedSalary : 0
            };

            if (applicant == null)
            {
                EventBus.HiringDecisionResolved(result);
                return result;
            }

            if (decision == HiringDecision.Hire || decision == HiringDecision.Trial || decision == HiringDecision.CounterOffer)
            {
                result.accepted = decision != HiringDecision.CounterOffer || applicant.reliabilityScore >= 5;
                result.agreedSalary = decision == HiringDecision.CounterOffer
                    ? Mathf.RoundToInt(applicant.requestedSalary * 0.9f)
                    : applicant.requestedSalary;
                result.moraleDelta = decision == HiringDecision.CounterOffer ? -1 : 0;
                result.loyaltyDelta = decision == HiringDecision.Hire ? 1 : 0;

                CardData template = result.accepted ? ResolveApplicantTemplate(applicant) : null;
                if (template != null && GameManager.Instance != null && GameManager.Instance.DeckManager != null)
                    GameManager.Instance.DeckManager.AddCardToDeck(template);
            }

            EventBus.HiringDecisionResolved(result);
            return result;
        }

        public StaffApplicant[] GenerateApplicantPool(VentureType venture, IReadOnlyDictionary<string, CardData> lookup, int count = 3)
        {
            var candidates = new List<CardData>();
            if (lookup != null)
            {
                foreach (KeyValuePair<string, CardData> item in lookup)
                {
                    CardData card = item.Value;
                    if (card == null || card.cardType != CardType.Employee)
                        continue;
                    if (!card.isGeneralCard && card.ventureType != venture)
                        continue;
                    candidates.Add(card);
                }
            }

            int take = Mathf.Min(Mathf.Max(1, count), candidates.Count);
            var pool = new StaffApplicant[take];
            for (int i = 0; i < take; i++)
            {
                CardData card = candidates[i];
                int salary = Mathf.Max(12, card.salaryPerTurn > 0 ? card.salaryPerTurn : Mathf.RoundToInt(card.upkeepCostPerTurn));
                pool[i] = new StaffApplicant
                {
                    applicantId = $"{card.cardId}_applicant_{i}",
                    templateCardId = card.cardId,
                    displayName = card.cardName,
                    ventureType = venture,
                    role = card.staffRole,
                    requestedSalary = salary,
                    skillScore = Mathf.Clamp(Mathf.RoundToInt(card.capacityDelta + card.qualityDelta + 5f), 1, 10),
                    reliabilityScore = Mathf.Clamp(Mathf.RoundToInt(card.staffStabilityDelta + 5f), 1, 10),
                    trialTurns = Mathf.Max(1, card.defaultTrialTurns)
                };
            }

            EventBus.ApplicantPoolGenerated(pool);
            return pool;
        }

        public bool TryResolveQuitChecks()
        {
            bool anyQuit = false;
            for (int i = _staffStates.Count - 1; i >= 0; i--)
            {
                StaffState state = _staffStates[i];
                if (state == null || state.card == null)
                    continue;

                QuitReason reason = ResolveQuitReason(state);
                if (reason == QuitReason.None)
                    continue;

                anyQuit = true;
                state.employmentStatus = reason == QuitReason.RivalPoach ? EmploymentStatus.Poached : EmploymentStatus.Quit;
                state.lastQuitReason = reason;
                ApplyTurnoverCost(state);
                RemoveStaffFromBoard(state.card);
                EventBus.StaffQuit(state.card, reason);
            }

            return anyQuit;
        }

        public List<StaffRuntimeSaveData> CaptureStaffRuntimeState()
        {
            var result = new List<StaffRuntimeSaveData>();
            for (int i = 0; i < _staffStates.Count; i++)
            {
                StaffState s = _staffStates[i];
                if (s == null || s.card == null)
                    continue;

                result.Add(new StaffRuntimeSaveData
                {
                    cardId = s.card.cardId,
                    role = s.role,
                    employmentStatus = s.employmentStatus,
                    baseSalary = s.baseSalary,
                    negotiatedSalary = s.negotiatedSalary,
                    trialTurnsRemaining = s.trialTurnsRemaining,
                    workload = s.workload,
                    burnoutRisk = s.burnoutRisk,
                    resignRisk = s.resignRisk,
                    lastQuitReason = s.lastQuitReason,
                    moral = s.moral,
                    fatigue = s.fatigue,
                    loyalty = s.loyalty,
                    experience = s.experience,
                    turnsWorked = s.turnsWorked,
                    consecutiveOvertimeTurns = s.consecutiveOvertimeTurns
                });
            }

            return result;
        }

        public void RestoreStaffRuntimeState(IList<StaffRuntimeSaveData> saved)
        {
            if (saved == null || saved.Count == 0)
                return;

            for (int i = 0; i < saved.Count; i++)
            {
                StaffRuntimeSaveData item = saved[i];
                if (item == null || string.IsNullOrWhiteSpace(item.cardId))
                    continue;

                StaffState state = FindStateByCardId(item.cardId);
                if (state == null)
                    continue;

                state.role = item.role;
                state.employmentStatus = item.employmentStatus;
                state.baseSalary = item.baseSalary;
                state.negotiatedSalary = item.negotiatedSalary;
                state.trialTurnsRemaining = item.trialTurnsRemaining;
                state.workload = item.workload;
                state.burnoutRisk = item.burnoutRisk;
                state.resignRisk = item.resignRisk;
                state.lastQuitReason = item.lastQuitReason;
                state.moral = item.moral;
                state.fatigue = item.fatigue;
                state.loyalty = item.loyalty;
                state.experience = item.experience;
                state.turnsWorked = item.turnsWorked;
                state.consecutiveOvertimeTurns = item.consecutiveOvertimeTurns;
            }
        }

        private void ApplyWorkloadToStaff(float perStaffPressure, IReadOnlyList<CardData> tempEffects)
        {
            int fatigueDelta = Mathf.RoundToInt(Mathf.Clamp(perStaffPressure * 0.55f, 0f, 3f)) + SumTempInt(tempEffects, c => c.fatigueDeltaPerTurn);
            int moraleDelta = -Mathf.RoundToInt(Mathf.Clamp(perStaffPressure * 0.25f, 0f, 2f)) + SumTempInt(tempEffects, c => c.moraleDeltaPerTurn);
            int loyaltyDelta = SumTempInt(tempEffects, c => c.loyaltyDeltaPerTurn);
            float burnoutDelta = Mathf.Max(0f, perStaffPressure * 0.045f) + SumTemp(tempEffects, c => c.burnoutRiskDeltaPerTurn);

            for (int i = 0; i < _staffStates.Count; i++)
            {
                StaffState state = _staffStates[i];
                if (state == null || state.card == null || state.employmentStatus == EmploymentStatus.Quit || state.employmentStatus == EmploymentStatus.Poached)
                    continue;

                state.workload = Mathf.Clamp(state.workload + perStaffPressure, 0f, 10f);
                state.fatigue = Mathf.Clamp(state.fatigue + fatigueDelta, 0, Constants.STAFF_FATIGUE_MAX);
                state.moral = Mathf.Clamp(state.moral + moraleDelta, 0, Constants.STAFF_MORAL_MAX);
                state.loyalty = Mathf.Clamp(state.loyalty + loyaltyDelta, 0, Constants.STAFF_LOYALTY_MAX);
                state.burnoutRisk = Mathf.Clamp01(state.burnoutRisk + burnoutDelta);
                state.resignRisk = Mathf.Clamp01(state.resignRisk + Mathf.Max(0f, perStaffPressure - 5f) * 0.035f + Mathf.Max(0, 4 - state.moral) * 0.025f);

                if (state.burnoutRisk >= 0.65f)
                    EventBus.StaffBurnoutRiskChanged(state.card, state.burnoutRisk);
            }
        }

        private QuitReason ResolveQuitReason(StaffState state)
        {
            if (state.employmentStatus == EmploymentStatus.Quit || state.employmentStatus == EmploymentStatus.Poached)
                return QuitReason.None;
            if (state.fatigue >= Constants.STAFF_FATIGUE_MAX && state.moral <= 2 && state.workload >= 7f)
                return QuitReason.Burnout;
            if (state.loyalty <= 0 && UnityEngine.Random.value < 0.65f)
                return QuitReason.LowMorale;
            if (state.resignRisk >= 0.98f)
                return state.loyalty <= 2 ? QuitReason.SalaryDelay : QuitReason.Burnout;
            if (state.resignRisk > 0.35f && UnityEngine.Random.value < state.resignRisk * 0.18f)
                return state.moral <= 3 ? QuitReason.LowMorale : QuitReason.Burnout;
            return QuitReason.None;
        }

        private void RemoveStaffFromBoard(CardData card)
        {
            var gm = GameManager.Instance;
            if (gm != null && gm.BoardManager != null)
            {
                int businessIndex = gm.BoardManager.FindBusinessWithEmployee(card);
                if (businessIndex >= 0 && gm.BoardManager.RemoveEmployeeByCard(businessIndex, card))
                    return;
            }

            RemoveStaff(card);
        }

        private static void ApplyTurnoverCost(StaffState state)
        {
            int cost = Mathf.Max(10, Mathf.RoundToInt((state.negotiatedSalary > 0 ? state.negotiatedSalary : state.baseSalary) * 0.5f));
            GameManager.Instance?.AdjustMoney(-cost);
        }

        private StaffState FindStateByCardId(string cardId)
        {
            for (int i = 0; i < _staffStates.Count; i++)
            {
                StaffState state = _staffStates[i];
                if (state != null && state.card != null && state.card.cardId == cardId)
                    return state;
            }

            return null;
        }

        private int CountWorkingStaff()
        {
            int count = 0;
            for (int i = 0; i < _staffStates.Count; i++)
            {
                StaffState state = _staffStates[i];
                if (state != null && state.card != null && state.employmentStatus != EmploymentStatus.Quit && state.employmentStatus != EmploymentStatus.Poached)
                    count++;
            }

            return count;
        }

        private static StaffRole[] GetRequiredRoles(VentureType venture)
        {
            return venture switch
            {
                VentureType.FastFood => new[] { StaffRole.Chef, StaffRole.Cashier, StaffRole.Cleaning },
                VentureType.Cafe => new[] { StaffRole.Barista, StaffRole.Floor },
                VentureType.TechApp => new[] { StaffRole.Developer, StaffRole.Support },
                VentureType.ClothingStore => new[] { StaffRole.Sales, StaffRole.Tailor },
                VentureType.GroceryStore => new[] { StaffRole.Cashier, StaffRole.Stocker },
                _ => new[] { StaffRole.Generalist }
            };
        }

        private CardData ResolveApplicantTemplate(StaffApplicant applicant)
        {
            var lookup = GameManager.Instance != null ? GameManager.Instance.CardLookup : null;
            if (lookup != null && lookup.TryGetValue(applicant.templateCardId, out CardData card))
                return card;
            return null;
        }

        private static float SumTemp(IReadOnlyList<CardData> cards, Func<CardData, float> selector)
        {
            if (cards == null || selector == null)
                return 0f;

            float total = 0f;
            for (int i = 0; i < cards.Count; i++)
                if (cards[i] != null)
                    total += selector(cards[i]);
            return total;
        }

        private static int SumTempInt(IReadOnlyList<CardData> cards, Func<CardData, int> selector)
        {
            if (cards == null || selector == null)
                return 0;

            int total = 0;
            for (int i = 0; i < cards.Count; i++)
                if (cards[i] != null)
                    total += selector(cards[i]);
            return total;
        }
    }
}
