using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public class EconomyManager : MonoBehaviour
    {
        BoardManager _board;
        CustomerSystem _customers;
        StaffSystem _staff;
        HygieneSystem _hygiene;

        MenuPricing _currentPricing = MenuPricing.Standard;
        int _lastServed;
        int _lastWaited;
        int _lastLeft;
        int _lastGrossIncome;
        int _lastExpenses;
        int _lastNetIncome;
        float _lastRatingDelta;

        public MenuPricing CurrentPricing => _currentPricing;
        public int LastServed => _lastServed;
        public int LastNetIncome => _lastNetIncome;

        public void Init()
        {
            Debug.Log("[EconomyManager] Initialized.");
        }

        public void SetReferences(BoardManager board, CustomerSystem customers,
            StaffSystem staff, HygieneSystem hygiene)
        {
            _board = board;
            _customers = customers;
            _staff = staff;
            _hygiene = hygiene;
        }

        public void SetMenuPricing(MenuPricing pricing)
        {
            _currentPricing = pricing;
        }

        // ── EventBus subscriptions ─────────────────────────────────

        void OnEnable()
        {
            EventBus.OnResolveStep += HandleResolveStep;
        }

        void OnDisable()
        {
            EventBus.OnResolveStep -= HandleResolveStep;
        }

        void HandleResolveStep(ResolveStep step)
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            var res = gm.Resources;

            switch (step)
            {
                case ResolveStep.CalculateMetrics:
                    StepCalculateMetrics(res);
                    break;
                case ResolveStep.UpdateHygiene:
                    StepUpdateHygiene(res);
                    break;
                case ResolveStep.CalculateDemand:
                    StepCalculateDemand(res);
                    break;
                case ResolveStep.UpdateRating:
                    StepUpdateRating(res);
                    break;
                case ResolveStep.CustomerFlow:
                    StepCustomerFlow(res);
                    break;
                case ResolveStep.CalculateIncome:
                    StepCalculateIncome(res);
                    break;
                case ResolveStep.UpdateStaff:
                    StepUpdateStaff(res);
                    break;
                case ResolveStep.UpdateMarketShare:
                    StepUpdateMarketShare(res);
                    break;
            }
        }

        // ── Step 1: Calculate Metrics ──────────────────────────────

        void StepCalculateMetrics(PlayerResources res)
        {
            if (_board == null) return;

            float qualitySum = _board.SumStat(c => c.qualityDelta);
            float capacitySum = _board.SumStat(c => c.capacityDelta);

            res.AdjustQuality(qualitySum * 0.1f);
            res.AdjustCapacity(capacitySum * 0.1f);
        }

        // ── Step 2: Update Hygiene ─────────────────────────────────

        void StepUpdateHygiene(PlayerResources res)
        {
            if (_board == null || _hygiene == null) return;

            int cleanerCount = CountCleaners();
            float supplierQuality = SumSupplierHygiene();
            bool overloaded = res.GetDemand() > res.GetCapacity();

            float newHygiene = _hygiene.CalculateHygiene(
                cleanerCount, supplierQuality, overloaded, res.GetHygiene());

            res.SetHygiene(newHygiene);
            _hygiene.CheckThresholds(newHygiene);
        }

        // ── Step 3: Calculate Demand ───────────────────────────────

        void StepCalculateDemand(PlayerResources res)
        {
            if (_board == null) return;

            float marketingDelta = _board.SumStat(c => c.demandDelta);
            float organicDemand = CalculateOrganicDemand(res.GetRating());
            float seasonMult = GetCurrentSeasonMultiplier();

            float newDemand = (res.GetDemand() + marketingDelta * 0.1f + organicDemand)
                              * seasonMult;

            res.SetDemand(Mathf.Clamp(newDemand, Constants.STAT_MIN, Constants.STAT_MAX));
        }

        // ── Step 4: Update Rating ──────────────────────────────────

        void StepUpdateRating(PlayerResources res)
        {
            float qualityImpact = CalculateQualityRatingImpact(res.GetQuality());
            float demandCapacityImpact = CalculateDemandCapacityImpact(
                res.GetDemand(), res.GetCapacity());
            float stabilityImpact = CalculateStabilityRatingImpact(
                res.GetStaffStability());

            float totalDelta = qualityImpact + demandCapacityImpact + stabilityImpact;
            _lastRatingDelta = totalDelta;
            res.AdjustRating(totalDelta);
        }

        // ── Step 5: Customer Flow ──────────────────────────────────

        void StepCustomerFlow(PlayerResources res)
        {
            if (_customers == null) return;

            var (served, waited, left) = _customers.CalculateCustomerFlow(
                res.GetDemand(), res.GetCapacity(), res.GetQuality(),
                res.GetRating(), res.GetHygiene(), _currentPricing);

            _lastServed = served;
            _lastWaited = waited;
            _lastLeft = left;

            EventBus.CustomersServed(served, waited, left);
        }

        // ── Step 6: Calculate Income ───────────────────────────────

        void StepCalculateIncome(PlayerResources res)
        {
            float menuPrice = GetMenuPrice(_currentPricing);
            float qualityMult = GetQualityMultiplier(res.GetQuality());

            int grossIncome = Mathf.RoundToInt(_lastServed * menuPrice * qualityMult);
            int salaries = CalculateTotalSalaries();
            int upkeep = _board != null ? _board.TotalUpkeep() : 0;
            int rent = Mathf.RoundToInt(Constants.BASE_RENT);
            int subtotal = salaries + upkeep + rent;
            int tax = Mathf.RoundToInt(Mathf.Max(0, grossIncome - subtotal) * Constants.TAX_RATE);
            int totalExpenses = subtotal + tax;
            int netIncome = grossIncome - totalExpenses;

            _lastGrossIncome = grossIncome;
            _lastExpenses = totalExpenses;
            _lastNetIncome = netIncome;

            res.AdjustMoney(netIncome);
            EventBus.IncomeCalculated(grossIncome, totalExpenses, netIncome);
        }

        // ── Step 7: Update Staff ───────────────────────────────────

        void StepUpdateStaff(PlayerResources res)
        {
            if (_staff == null) return;

            _staff.TickStaff();
            float stabilityBonus = _staff.GetTotalStabilityBonus();
            res.AdjustStaffStability(stabilityBonus * 0.1f);
        }

        // ── Step 8: Update Market Share ────────────────────────────

        void StepUpdateMarketShare(PlayerResources res)
        {
            float ratingScore = Normalize(res.GetRating(), Constants.RATING_MIN, Constants.RATING_MAX);
            float qualityScore = Normalize(res.GetQuality(), Constants.STAT_MIN, Constants.STAT_MAX);
            float servedScore = _lastServed > 0 ? Mathf.Clamp01(_lastServed / 10f) : 0f;
            float rivalPressure = 0.5f;

            float composite = ratingScore * Constants.SHARE_RATING_WEIGHT
                            + qualityScore * Constants.SHARE_QUALITY_WEIGHT
                            + servedScore * Constants.SHARE_SERVED_WEIGHT
                            - rivalPressure * Constants.SHARE_RIVAL_PRESSURE_WEIGHT;

            float delta = (composite - 0.5f) * Constants.SHARE_SHIFT_SCALE;
            res.AdjustMarketShare(delta);

            FireTurnReport(res);
        }

        // ── Helpers ────────────────────────────────────────────────

        int CountCleaners()
        {
            if (_board == null) return 0;
            int count = 0;
            foreach (var card in _board.GetCardsInSlot(SlotType.Salon))
                if (card.hygieneDelta > 0.5f) count++;
            return count;
        }

        float SumSupplierHygiene()
        {
            if (_board == null) return 0f;
            return _board.SumStat(c =>
                c.cardType == CardType.Supplier ? c.hygieneDelta : 0f);
        }

        float CalculateOrganicDemand(float rating)
        {
            if (rating >= Constants.ORGANIC_DEMAND_THRESHOLD)
                return (rating - Constants.ORGANIC_DEMAND_THRESHOLD) * 0.3f;
            return (rating - Constants.ORGANIC_DEMAND_THRESHOLD) * 0.2f;
        }

        float GetCurrentSeasonMultiplier()
        {
            var gm = GameManager.Instance;
            if (gm == null) return 1f;
            return GameManager.GetSeasonMultiplier(gm.CurrentSeason);
        }

        float CalculateQualityRatingImpact(float quality)
        {
            return (quality - 5f) * 0.04f;
        }

        float CalculateDemandCapacityImpact(float demand, float capacity)
        {
            if (demand <= capacity) return 0.05f;
            float overload = (demand - capacity) / Mathf.Max(capacity, 1f);
            return -overload * 0.15f;
        }

        float CalculateStabilityRatingImpact(float stability)
        {
            return (stability - 5f) * 0.02f;
        }

        float GetMenuPrice(MenuPricing pricing)
        {
            switch (pricing)
            {
                case MenuPricing.Economy:  return Constants.MENU_PRICE_ECONOMY;
                case MenuPricing.Standard: return Constants.MENU_PRICE_STANDARD;
                case MenuPricing.Premium:  return Constants.MENU_PRICE_PREMIUM;
                case MenuPricing.Seasonal: return Constants.MENU_PRICE_SEASONAL;
                default:                   return Constants.MENU_PRICE_STANDARD;
            }
        }

        float GetQualityMultiplier(float quality)
        {
            if (quality < 4f) return Constants.QUALITY_MULTIPLIER_LOW;
            if (quality > 7f) return Constants.QUALITY_MULTIPLIER_HIGH;
            return Constants.QUALITY_MULTIPLIER_MID;
        }

        int CalculateTotalSalaries()
        {
            if (_board == null) return 0;
            int total = 0;
            foreach (var card in _board.GetAllActiveCards())
                if (card.cardType == CardType.Staff) total += card.upkeepPerTurn;
            return total;
        }

        float Normalize(float value, float min, float max)
        {
            return Mathf.Clamp01((value - min) / (max - min));
        }

        void FireTurnReport(PlayerResources res)
        {
            EventBus.TurnReport(
                _lastGrossIncome, _lastExpenses, _lastNetIncome,
                _lastRatingDelta, _lastServed, _lastWaited);
        }
    }
}
