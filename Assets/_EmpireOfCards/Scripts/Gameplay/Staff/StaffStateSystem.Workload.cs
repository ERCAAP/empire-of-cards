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
            IReadOnlyList<CardData> tempEffects,
            IReadOnlyList<CardData> operations = null,
            IReadOnlyList<CardData> marketing = null,
            IReadOnlyList<CardData> suppliers = null)
        {
            StaffRoleCoverage coverage = GetRoleCoverage(venture);
            float overload = Mathf.Max(0f, demand - capacity);
            float operationPressure = Mathf.Max(0, operationCount - CountWorkingStaff()) * 0.65f;
            float tempPressure = SumApplicableTemp(tempEffects, c => c.workloadDeltaPerTurn);
            Dictionary<StaffRole, float> rolePressure = BuildRolePressure(
                venture,
                overload,
                operationPressure,
                tempEffects,
                operations,
                marketing,
                suppliers,
                coverage);
            float rolePressureTotal = SumRolePressure(rolePressure);
            float missingRolePressure = coverage.missingRoles.Count * 1.35f;
            float workloadPressure = Mathf.Max(0f, overload + missingRolePressure + operationPressure + tempPressure);
            workloadPressure = Mathf.Max(workloadPressure, rolePressureTotal);

            int workingStaff = CountWorkingStaff();
            if (workingStaff > 0)
                ApplyWorkloadToStaff(rolePressure, workloadPressure, workingStaff, tempEffects);

            var report = new StaffWorkloadReport
            {
                ventureType = venture,
                demand = demand,
                capacity = capacity,
                workloadPressure = workloadPressure,
                coverage = coverage,
                rolePressures = BuildRolePressureReport(rolePressure),
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

        private void ApplyWorkloadToStaff(Dictionary<StaffRole, float> rolePressure, float totalPressure, int workingStaff, IReadOnlyList<CardData> tempEffects)
        {
            float fallbackPressure = workingStaff > 0 ? totalPressure / workingStaff : 0f;
            float uncoveredPressure = CalculateUncoveredPressure(rolePressure);
            float redistributedPressure = workingStaff > 0 ? uncoveredPressure / workingStaff : 0f;

            for (int i = 0; i < _staffStates.Count; i++)
            {
                StaffState state = _staffStates[i];
                if (state == null || state.card == null || state.employmentStatus == EmploymentStatus.Quit || state.employmentStatus == EmploymentStatus.Poached)
                    continue;

                float directPressure = rolePressure != null && rolePressure.TryGetValue(state.role, out float pressure)
                    ? pressure
                    : 0f;
                float perStaffPressure = Mathf.Max(directPressure + redistributedPressure, fallbackPressure * 0.35f);
                int fatigueDelta = Mathf.RoundToInt(Mathf.Clamp(perStaffPressure * 0.55f, 0f, 3f)) + SumApplicableTempInt(tempEffects, c => c.fatigueDeltaPerTurn);
                int moraleDelta = -Mathf.RoundToInt(Mathf.Clamp(perStaffPressure * 0.25f, 0f, 2f)) + SumApplicableTempInt(tempEffects, c => c.moraleDeltaPerTurn);
                int loyaltyDelta = SumApplicableTempInt(tempEffects, c => c.loyaltyDeltaPerTurn);
                float burnoutDelta = Mathf.Max(0f, perStaffPressure * 0.045f) + SumApplicableTemp(tempEffects, c => c.burnoutRiskDeltaPerTurn);

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
                VentureType.FastFood => new[] { StaffRole.Chef, StaffRole.Cashier, StaffRole.Courier, StaffRole.Cleaning, StaffRole.Manager },
                VentureType.Cafe => new[] { StaffRole.Barista, StaffRole.Cashier, StaffRole.Floor, StaffRole.Cleaning, StaffRole.Manager },
                VentureType.TechApp => new[] { StaffRole.Developer, StaffRole.Designer, StaffRole.Growth, StaffRole.Support, StaffRole.ProductManager },
                VentureType.ClothingStore => new[] { StaffRole.Sales, StaffRole.Cashier, StaffRole.Tailor, StaffRole.Stocker, StaffRole.Manager },
                VentureType.GroceryStore => new[] { StaffRole.Cashier, StaffRole.Stocker, StaffRole.FreshKeeper, StaffRole.Courier, StaffRole.Manager },
                _ => new[] { StaffRole.Generalist }
            };
        }

        private Dictionary<StaffRole, float> BuildRolePressure(
            VentureType venture,
            float overload,
            float operationPressure,
            IReadOnlyList<CardData> tempEffects,
            IReadOnlyList<CardData> operations,
            IReadOnlyList<CardData> marketing,
            IReadOnlyList<CardData> suppliers,
            StaffRoleCoverage coverage)
        {
            var pressures = new Dictionary<StaffRole, float>();
            AddPressure(pressures, GetServiceRoles(venture), overload * 0.75f);
            AddPressure(pressures, GetManagerRoles(venture), operationPressure);

            if (coverage != null && coverage.missingRoles != null)
                for (int i = 0; i < coverage.missingRoles.Count; i++)
                    AddPressure(pressures, coverage.missingRoles[i], 1.35f);

            AddCardPressure(pressures, venture, operations, 0.35f);
            AddCardPressure(pressures, venture, marketing, 0.85f);
            AddCardPressure(pressures, venture, suppliers, 0.45f);
            AddCardPressure(pressures, venture, tempEffects, 1.00f);

            return pressures;
        }

        private void AddCardPressure(Dictionary<StaffRole, float> pressures, VentureType venture, IReadOnlyList<CardData> cards, float multiplier)
        {
            if (cards == null)
                return;

            for (int i = 0; i < cards.Count; i++)
            {
                CardData card = cards[i];
                if (card == null)
                    continue;
                if (card.cardFamily == CardFamily.Reaction)
                    continue;

                float basePressure = Mathf.Max(0f, card.demandDelta) + Mathf.Max(0f, card.workloadDeltaPerTurn);
                if (card.cardFamily == CardFamily.Crisis)
                    basePressure += Mathf.Abs(card.qualityDelta) + Mathf.Abs(card.ratingDeltaPerTurn) + Mathf.Max(0f, card.legalRiskDeltaPerTurn) * 0.03f;
                else if (card.cardFamily == CardFamily.Risk)
                    basePressure += Mathf.Max(0f, card.legalRiskDeltaPerTurn) * 0.04f;

                if (basePressure <= 0f)
                    continue;

                AddPressure(pressures, ResolvePressureRoles(venture, card), basePressure * multiplier);
            }
        }

        private StaffRole[] ResolvePressureRoles(VentureType venture, CardData card)
        {
            if (card == null)
                return GetServiceRoles(venture);

            string subSlot = card.targetSubSlotId != null ? card.targetSubSlotId.ToLowerInvariant() : "";
            string[] crisisTags = card.crisisTags ?? Array.Empty<string>();
            string[] solutionTags = card.solutionTags ?? Array.Empty<string>();

            if (ContainsAny(subSlot, crisisTags, solutionTags, "delivery", "courier", "whatsapp", "online", "platform"))
                return new[] { StaffRole.Courier, StaffRole.Manager };
            if (ContainsAny(subSlot, crisisTags, solutionTags, "hygiene", "clean", "skt", "freshness", "spoilage", "fresh", "dairy"))
                return venture == VentureType.GroceryStore
                    ? new[] { StaffRole.FreshKeeper, StaffRole.Stocker, StaffRole.Manager }
                    : new[] { StaffRole.Cleaning, StaffRole.Manager };
            if (ContainsAny(subSlot, crisisTags, solutionTags, "bean", "kitchen", "ingredient", "butcher", "bakery", "quality"))
                return venture == VentureType.Cafe
                    ? new[] { StaffRole.Barista, StaffRole.Manager }
                    : new[] { StaffRole.Chef, StaffRole.Manager };
            if (ContainsAny(subSlot, crisisTags, solutionTags, "backend", "stability", "crash", "corruption", "infra", "cloud", "api", "cost"))
                return new[] { StaffRole.Developer, StaffRole.Support, StaffRole.ProductManager };
            if (ContainsAny(subSlot, crisisTags, solutionTags, "aso", "ads", "community", "creator", "growth", "privacy", "dark_pattern", "retention", "clone"))
                return new[] { StaffRole.Growth, StaffRole.Support, StaffRole.ProductManager };
            if (ContainsAny(subSlot, crisisTags, solutionTags, "inventory", "stock", "returns", "tailor", "fit", "fabric", "atelier", "wholesale"))
                return new[] { StaffRole.Sales, StaffRole.Tailor, StaffRole.Stocker, StaffRole.Manager };
            if (ContainsAny(subSlot, crisisTags, solutionTags, "cashier", "checkout", "maps", "instagram", "reels", "loyalty", "flyers", "google", "discount", "shoppingads", "latenight"))
                return GetServiceRoles(venture);

            return card.targetSlotType == SlotType.Marketing ? GetServiceRoles(venture) : GetRoleFallback(venture, card);
        }

        private StaffRole[] GetRoleFallback(VentureType venture, CardData card)
        {
            if (card != null && card.cardType == CardType.Employee)
                return new[] { card.staffRole };

            return venture switch
            {
                VentureType.FastFood => new[] { StaffRole.Chef, StaffRole.Cashier, StaffRole.Manager },
                VentureType.Cafe => new[] { StaffRole.Barista, StaffRole.Floor, StaffRole.Cashier },
                VentureType.TechApp => new[] { StaffRole.Developer, StaffRole.Support, StaffRole.ProductManager },
                VentureType.ClothingStore => new[] { StaffRole.Sales, StaffRole.Stocker, StaffRole.Manager },
                VentureType.GroceryStore => new[] { StaffRole.Cashier, StaffRole.Stocker, StaffRole.FreshKeeper },
                _ => new[] { StaffRole.Generalist }
            };
        }

        private static StaffRole[] GetServiceRoles(VentureType venture)
        {
            return venture switch
            {
                VentureType.FastFood => new[] { StaffRole.Cashier, StaffRole.Chef, StaffRole.Courier },
                VentureType.Cafe => new[] { StaffRole.Floor, StaffRole.Cashier, StaffRole.Barista },
                VentureType.TechApp => new[] { StaffRole.Support, StaffRole.Developer, StaffRole.Growth },
                VentureType.ClothingStore => new[] { StaffRole.Sales, StaffRole.Cashier, StaffRole.Stocker },
                VentureType.GroceryStore => new[] { StaffRole.Cashier, StaffRole.Stocker, StaffRole.Courier },
                _ => new[] { StaffRole.Generalist }
            };
        }

        private static StaffRole[] GetManagerRoles(VentureType venture)
        {
            return venture == VentureType.TechApp
                ? new[] { StaffRole.ProductManager }
                : new[] { StaffRole.Manager };
        }

        private bool IsRoleCovered(StaffRole role)
        {
            for (int i = 0; i < _staffStates.Count; i++)
            {
                StaffState state = _staffStates[i];
                if (state != null && state.card != null && state.role == role && state.employmentStatus != EmploymentStatus.Quit && state.employmentStatus != EmploymentStatus.Poached)
                    return true;
            }

            return false;
        }

        private float CalculateUncoveredPressure(Dictionary<StaffRole, float> rolePressure)
        {
            if (rolePressure == null)
                return 0f;

            float total = 0f;
            foreach (KeyValuePair<StaffRole, float> pair in rolePressure)
                if (!IsRoleCovered(pair.Key))
                    total += pair.Value * 0.75f;
            return total;
        }

        private List<StaffRolePressure> BuildRolePressureReport(Dictionary<StaffRole, float> rolePressure)
        {
            var result = new List<StaffRolePressure>();
            if (rolePressure == null)
                return result;

            foreach (KeyValuePair<StaffRole, float> pair in rolePressure)
            {
                if (pair.Value <= 0f)
                    continue;

                result.Add(new StaffRolePressure
                {
                    role = pair.Key,
                    pressure = pair.Value,
                    covered = IsRoleCovered(pair.Key)
                });
            }

            return result;
        }

        private static void AddPressure(Dictionary<StaffRole, float> pressures, StaffRole role, float pressure)
        {
            if (pressure <= 0f)
                return;

            if (pressures.ContainsKey(role))
                pressures[role] += pressure;
            else
                pressures[role] = pressure;
        }

        private static void AddPressure(Dictionary<StaffRole, float> pressures, StaffRole[] roles, float pressure)
        {
            if (roles == null || roles.Length == 0 || pressure <= 0f)
                return;

            float perRole = pressure / roles.Length;
            for (int i = 0; i < roles.Length; i++)
                AddPressure(pressures, roles[i], perRole);
        }

        private static float SumRolePressure(Dictionary<StaffRole, float> rolePressure)
        {
            if (rolePressure == null)
                return 0f;

            float total = 0f;
            foreach (KeyValuePair<StaffRole, float> pair in rolePressure)
                total += pair.Value;
            return total;
        }

        private static bool ContainsAny(string subSlot, string[] crisisTags, string[] solutionTags, params string[] needles)
        {
            for (int i = 0; i < needles.Length; i++)
            {
                string needle = needles[i];
                if (!string.IsNullOrEmpty(subSlot) && subSlot.Contains(needle))
                    return true;
                if (ContainsTag(crisisTags, needle) || ContainsTag(solutionTags, needle))
                    return true;
            }

            return false;
        }

        private static bool ContainsTag(string[] tags, string needle)
        {
            if (tags == null || string.IsNullOrEmpty(needle))
                return false;

            for (int i = 0; i < tags.Length; i++)
                if (!string.IsNullOrEmpty(tags[i]) && tags[i].ToLowerInvariant().Contains(needle))
                    return true;
            return false;
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

        private static float SumApplicableTemp(IReadOnlyList<CardData> cards, Func<CardData, float> selector)
        {
            if (cards == null || selector == null)
                return 0f;

            float total = 0f;
            for (int i = 0; i < cards.Count; i++)
                if (IsTempModifierApplicable(cards, cards[i]))
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

        private static int SumApplicableTempInt(IReadOnlyList<CardData> cards, Func<CardData, int> selector)
        {
            if (cards == null || selector == null)
                return 0;

            int total = 0;
            for (int i = 0; i < cards.Count; i++)
                if (IsTempModifierApplicable(cards, cards[i]))
                    total += selector(cards[i]);
            return total;
        }

        private static bool IsTempModifierApplicable(IReadOnlyList<CardData> activeTempEffects, CardData card)
        {
            if (card == null)
                return false;
            if (card.cardFamily != CardFamily.Reaction)
                return true;
            if (card.solutionTags == null || card.solutionTags.Length == 0)
                return false;

            for (int i = 0; i < activeTempEffects.Count; i++)
            {
                CardData active = activeTempEffects[i];
                if (active == null || active == card || active.crisisTags == null)
                    continue;

                for (int s = 0; s < card.solutionTags.Length; s++)
                {
                    for (int c = 0; c < active.crisisTags.Length; c++)
                    {
                        if (!string.IsNullOrEmpty(card.solutionTags[s]) && card.solutionTags[s] == active.crisisTags[c])
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
