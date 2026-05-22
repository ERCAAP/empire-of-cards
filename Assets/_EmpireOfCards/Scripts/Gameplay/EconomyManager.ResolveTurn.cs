using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public partial class EconomyManager
    {
        private readonly struct TurnLanes
        {
            public readonly IReadOnlyList<CardData> operations;
            public readonly IReadOnlyList<CardData> staff;
            public readonly IReadOnlyList<CardData> marketing;
            public readonly IReadOnlyList<CardData> suppliers;
            public readonly IReadOnlyList<CardData> temp;

            public TurnLanes(
                IReadOnlyList<CardData> operations,
                IReadOnlyList<CardData> staff,
                IReadOnlyList<CardData> marketing,
                IReadOnlyList<CardData> suppliers,
                IReadOnlyList<CardData> temp)
            {
                this.operations = operations;
                this.staff = staff;
                this.marketing = marketing;
                this.suppliers = suppliers;
                this.temp = temp;
            }
        }

        private readonly struct BaselineEconomyMetrics
        {
            public readonly float demand;
            public readonly float capacity;
            public readonly float quality;
            public readonly float staffStability;
            public readonly float legalRisk;
            public readonly float organicDemand;
            public readonly float loyaltyDemand;

            public BaselineEconomyMetrics(float demand, float capacity, float quality, float staffStability, float legalRisk, float organicDemand, float loyaltyDemand)
            {
                this.demand = demand;
                this.capacity = capacity;
                this.quality = quality;
                this.staffStability = staffStability;
                this.legalRisk = legalRisk;
                this.organicDemand = organicDemand;
                this.loyaltyDemand = loyaltyDemand;
            }
        }

        private readonly struct TurnOutcomeMetrics
        {
            public readonly float servedDemand;
            public readonly float overload;
            public readonly float rating;
            public readonly float marketShare;
            public readonly int grossIncome;
            public readonly int totalCustomersThisTurn;

            public TurnOutcomeMetrics(float servedDemand, float overload, float rating, float marketShare, int grossIncome, int totalCustomersThisTurn)
            {
                this.servedDemand = servedDemand;
                this.overload = overload;
                this.rating = rating;
                this.marketShare = marketShare;
                this.grossIncome = grossIncome;
                this.totalCustomersThisTurn = totalCustomersThisTurn;
            }
        }

        private TurnLanes CaptureTurnLanes()
        {
            return new TurnLanes(
                boardManager.GetCardsInSlotType(SlotType.Operation),
                boardManager.GetCardsInSlotType(SlotType.Staff),
                boardManager.GetCardsInSlotType(SlotType.Marketing),
                boardManager.GetCardsInSlotType(SlotType.Supplier),
                boardManager.GetCardsInSlotType(SlotType.TempEffect));
        }

        private BaselineEconomyMetrics ResolveBaselineMetrics(TurnLanes lanes, TechCategoryProfile techCategory)
        {
            float opDemand = Sum(lanes.operations, c => c.demandDelta + c.customersPerTurn * 0.2f);
            float marketingDemand = Sum(lanes.marketing, c => c.demandDelta);
            float organicDemand = Mathf.Max(0f, (snapshot.rating - 3f) * _activeProfile.ratingToOrganicDemandWeight);
            float loyaltyDemand = Mathf.Max(0f, snapshot.loyalty - 4f) * 0.45f;
            float tempDemand = Sum(lanes.temp, c => c.demandDelta);

            float demand = Mathf.Max(0f, _activeProfile.baseDemand + opDemand + marketingDemand + organicDemand + loyaltyDemand + tempDemand + (techCategory != null ? techCategory.demandModifier : 0f));
            float capacity = Mathf.Max(1f,
                _activeProfile.startingCapacity +
                (techCategory != null ? techCategory.capacityModifier : 0f) +
                Sum(lanes.operations, c => c.capacityDelta) +
                Sum(lanes.staff, c => c.capacityDelta) +
                Sum(lanes.suppliers, c => c.capacityDelta) +
                Sum(lanes.temp, c => c.capacityDelta));

            float quality = Mathf.Clamp(
                _activeProfile.startingQuality +
                (techCategory != null ? techCategory.qualityModifier : 0f) +
                Sum(lanes.operations, c => c.qualityDelta) +
                Sum(lanes.staff, c => c.qualityDelta) +
                Sum(lanes.suppliers, c => c.qualityDelta) +
                Sum(lanes.temp, c => c.qualityDelta),
                0f, 10f);

            float staffStability = Mathf.Clamp(
                _activeProfile.startingStaffStability +
                Sum(lanes.staff, c => c.staffStabilityDelta) +
                Sum(lanes.temp, c => c.staffStabilityDelta) -
                Mathf.Max(0f, lanes.marketing.Count - lanes.staff.Count) * 0.4f,
                0f, 10f);

            float legalRisk = Mathf.Clamp(
                snapshot.legalRisk - _activeProfile.legalRiskDecayPerTurn +
                (techCategory != null ? techCategory.legalRiskModifier : 0f) +
                Sum(lanes.suppliers, c => c.legalRiskDeltaPerTurn) +
                Sum(lanes.marketing, c => c.legalRiskDeltaPerTurn) +
                Sum(lanes.temp, c => c.legalRiskDeltaPerTurn),
                0f, Constants.LEGAL_RISK_MAX);

            return new BaselineEconomyMetrics(demand, capacity, quality, staffStability, legalRisk, organicDemand, loyaltyDemand);
        }

        private void ApplyOperationalEconomyEffects(int staffCount, bool hasSupplier)
        {
            float adjustedStaffStability = snapshot.staffStability;
            float adjustedLegalRisk = snapshot.legalRisk;
            float adjustedQuality = snapshot.quality;

            ApplySalaryEffects(ref adjustedStaffStability);
            ApplyInsuranceEffects(ref adjustedLegalRisk, staffCount);
            ApplyStockEffects(ref adjustedQuality, hasSupplier);

            snapshot.staffStability = adjustedStaffStability;
            snapshot.legalRisk = adjustedLegalRisk;
            snapshot.quality = adjustedQuality;
        }

        private TurnOutcomeMetrics ResolveTurnOutcome(TurnLanes lanes, TechCategoryProfile techCategory)
        {
            float servedDemand = Mathf.Min(snapshot.demand, snapshot.capacity);
            float overload = Mathf.Max(0f, snapshot.demand - snapshot.capacity);
            float ratingDelta = ((snapshot.quality - 5f) * _activeProfile.qualityToRatingWeight)
                - (overload * _activeProfile.capacityPenaltyMultiplier)
                - Mathf.Max(0f, 4f - snapshot.staffStability) * _activeProfile.staffInstabilityPenalty
                - _staffRatingPenaltyThisTurn
                + (techCategory != null ? techCategory.ratingModifier : 0f)
                + Sum(lanes.marketing, c => c.ratingDeltaPerTurn)
                + Sum(lanes.temp, c => c.ratingDeltaPerTurn);

            float rating = Mathf.Clamp(snapshot.rating + ratingDelta, _activeProfile.minRating, _activeProfile.maxRating);
            int gross = Mathf.RoundToInt(
                servedDemand * _activeProfile.baseRevenuePerDemand +
                Sum(lanes.operations, c => c.cashDeltaPerTurn) +
                Sum(lanes.marketing, c => c.cashDeltaPerTurn) +
                Sum(lanes.suppliers, c => c.cashDeltaPerTurn) +
                Sum(lanes.temp, c => c.cashDeltaPerTurn));

            int customers = Mathf.RoundToInt(servedDemand * 6f);
            float marketShare = Mathf.Clamp(
                snapshot.marketShare +
                ((rating - 3f) * 2f) +
                ((snapshot.quality - 5f) * 1.5f) +
                (servedDemand - overload) * 0.5f -
                (_rivalPressureImpact * 1.15f),
                0f, 100f);

            return new TurnOutcomeMetrics(servedDemand, overload, rating, marketShare, gross, customers);
        }

        private int ApplyFinancialEconomyEffects(int currentTurn, int gross, int salaries, int upkeep, int supplierCount)
        {
            int subsystemExpenses = 0;
            float taxLegalRisk = snapshot.legalRisk;
            float adjustedCapacity = snapshot.capacity;
            float adjustedRating = snapshot.rating;
            ResetFinancialTelemetry();
            ApplyInflationProgress(currentTurn);
            ApplyInsuranceCostEffects(ref subsystemExpenses);
            ApplyCreditEffects(ref subsystemExpenses);
            ApplyStockCostEffects(ref subsystemExpenses, gross, currentTurn, supplierCount);
            ApplyInflationExpense(ref subsystemExpenses, salaries, upkeep);
            ApplySupplierFailureEffects(ref subsystemExpenses, gross, supplierCount, currentTurn);
            ApplyTaxEffects(ref taxLegalRisk, ref subsystemExpenses, currentTurn);
            TryTriggerLegalIncident(currentTurn, boardManager != null ? boardManager.GetCardsInSlotType(SlotType.Staff).Count : 0, supplierCount);
            ApplyLegalIncidentEffects(ref subsystemExpenses, ref adjustedCapacity, ref adjustedRating, ref taxLegalRisk);
            snapshot.capacity = adjustedCapacity;
            snapshot.rating = adjustedRating;
            snapshot.legalRisk = taxLegalRisk;
            return subsystemExpenses;
        }
    }
}
