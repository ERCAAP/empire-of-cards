using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay.Staff;

namespace EmpireOfCards.Gameplay
{
    public class ChainReactionSystem : MonoBehaviour
    {
        // --- Cumulative Trackers ---
        private int _cheapSupplierTurns;
        private int _salaryDelayedTurns;
        private int _taxUnpaidTurns;
        private int _uninsuredStaffTurns;

        // --- References (set via Init) ---
        private BoardManager _boardManager;
        private EconomyManager _economyManager;
        private StaffStateSystem _staffStateSystem;

        // --- Properties for EventPhase deterministic triggers ---
        public int CheapSupplierTurns => _cheapSupplierTurns;
        public int SalaryDelayedTurns => _salaryDelayedTurns;
        public int TaxUnpaidTurns => _taxUnpaidTurns;
        public int UninsuredStaffTurns => _uninsuredStaffTurns;

        public void Init(BoardManager boardManager, EconomyManager economyManager, StaffStateSystem staffStateSystem)
        {
            _boardManager = boardManager;
            _economyManager = economyManager;
            _staffStateSystem = staffStateSystem;
        }

        // ----------------------------------------------------------------
        // Chain Evaluation (called during ResolvePhase DeteriorationCheck)
        // ----------------------------------------------------------------

        public void EvaluateChains(int currentTurn)
        {
            if (_boardManager == null) return;

            TrackCheapSupplier();
            TrackSalaryDelay();
            TrackTaxUnpaid();
            TrackUninsuredStaff();
            CheckGrowthTrap();

            Debug.Log($"[ChainReactionSystem] Turn {currentTurn} chain eval: " +
                      $"cheapSupplier={_cheapSupplierTurns}, salaryDelay={_salaryDelayedTurns}, " +
                      $"taxUnpaid={_taxUnpaidTurns}, uninsuredStaff={_uninsuredStaffTurns}");
        }

        // ----------------------------------------------------------------
        // Individual Chain Trackers
        // ----------------------------------------------------------------

        private void TrackCheapSupplier()
        {
            // Check if any supplier-slot card has low quality (cheap supplier indicator)
            bool hasCheapSupplier = false;

            var businesses = _boardManager.PlayerBusinesses;
            for (int i = 0; i < businesses.Count; i++)
            {
                if (businesses[i].isClosed) continue;
                var biz = businesses[i];
                if (biz.businessCard == null) continue;

                // Check if business has a cheap supplier via low qualityBoostAmount or costReduction
                foreach (var emp in biz.employees)
                {
                    if (emp == null) continue;
                    if (emp.costReductionPercent > 0 && emp.qualityBoostAmount < 0)
                    {
                        hasCheapSupplier = true;
                        break;
                    }
                }

                // Also check upgrades for supplier effects
                foreach (var upgrade in biz.upgrades)
                {
                    if (upgrade == null) continue;
                    if (upgrade.costReductionPercent > 0 && upgrade.qualityBoostAmount < 0)
                    {
                        hasCheapSupplier = true;
                        break;
                    }
                }

                if (hasCheapSupplier) break;
            }

            if (hasCheapSupplier)
                _cheapSupplierTurns++;
            else
                _cheapSupplierTurns = Mathf.Max(0, _cheapSupplierTurns - 1); // Slowly recover

            // Deterministic trigger: 4+ turns of cheap supplier -> QualityCrisis (GDD 12.2)
            if (_cheapSupplierTurns >= Constants.CHAIN_CHEAP_SUPPLIER_THRESHOLD)
            {
                EventBus.ChainReactionTriggered("QualityCrisis", _cheapSupplierTurns);
                Debug.Log("[ChainReactionSystem] QUALITY CRISIS triggered from cheap supplier chain!");
                _cheapSupplierTurns = 0; // Reset after triggering
            }
        }

        private void TrackSalaryDelay()
        {
            // Salary delay = net income is negative (can't afford salaries)
            var gm = GameManager.Instance;
            if (gm == null) return;

            bool canAffordSalaries = true;
            if (_economyManager != null)
            {
                canAffordSalaries = gm.PlayerMoney >= _economyManager.TotalSalaries;
            }

            if (!canAffordSalaries)
                _salaryDelayedTurns++;
            else
                _salaryDelayedTurns = 0; // Reset on payment

            // Deterministic trigger: 3+ turns of salary delay -> StaffStrike risk (GDD 12.2)
            if (_salaryDelayedTurns >= Constants.CHAIN_SALARY_DELAY_THRESHOLD)
            {
                EventBus.ChainReactionTriggered("StaffStrike", _salaryDelayedTurns);
                Debug.Log("[ChainReactionSystem] STAFF STRIKE triggered from salary delay chain!");
                // Don't reset -- keeps escalating
            }
        }

        private void TrackTaxUnpaid()
        {
            // Track if player has been dodging tax (e.g., using illegal methods)
            var gm = GameManager.Instance;
            if (gm == null) return;

            // Check if player has negative cash and tax was applied
            bool taxUnpaid = false;
            if (_economyManager != null && _economyManager.TaxAmount > 0)
            {
                // If player money after tax goes negative, tax is effectively unpaid
                if (gm.PlayerMoney < 0)
                    taxUnpaid = true;
            }

            if (taxUnpaid)
                _taxUnpaidTurns++;
            else
                _taxUnpaidTurns = 0;

            // Deterministic trigger: 2+ turns unpaid tax -> TaxAudit (GDD 12.2)
            if (_taxUnpaidTurns >= Constants.CHAIN_TAX_UNPAID_THRESHOLD)
            {
                EventBus.ChainReactionTriggered("TaxAudit", _taxUnpaidTurns);
                Debug.Log("[ChainReactionSystem] TAX AUDIT triggered from unpaid tax chain!");
                _taxUnpaidTurns = 0;
            }
        }

        private void TrackUninsuredStaff()
        {
            // Check if there are employees without insurance (no Security upgrade / legal card)
            bool hasUninsuredStaff = false;
            var businesses = _boardManager.PlayerBusinesses;

            for (int i = 0; i < businesses.Count; i++)
            {
                if (businesses[i].isClosed) continue;
                if (businesses[i].employees.Count == 0) continue;

                bool hasInsurance = false;
                foreach (var upgrade in businesses[i].upgrades)
                {
                    if (upgrade != null && upgrade.upgradeEffectType == UpgradeEffectType.ReduceFBIRisk)
                    {
                        hasInsurance = true;
                        break;
                    }
                }

                if (!hasInsurance && businesses[i].employees.Count > 0)
                {
                    hasUninsuredStaff = true;
                    break;
                }
            }

            if (hasUninsuredStaff)
                _uninsuredStaffTurns++;
            else
                _uninsuredStaffTurns = 0;

            // Deterministic trigger: 3+ turns of uninsured staff -> SGKAudit (GDD 12.2)
            if (_uninsuredStaffTurns >= Constants.CHAIN_UNINSURED_STAFF_THRESHOLD)
            {
                EventBus.ChainReactionTriggered("SGKAudit", _uninsuredStaffTurns);
                Debug.Log("[ChainReactionSystem] SGK AUDIT triggered from uninsured staff chain!");
                _uninsuredStaffTurns = 0;
            }
        }

        private void CheckGrowthTrap()
        {
            // "Growth Trap" warning (GDD 11.3):
            // Marketing slots full + operation capacity insufficient -> warn player
            var gm = GameManager.Instance;
            if (gm == null || gm.SlotManager == null) return;

            int marketingUsed = gm.SlotManager.GetOccupiedCount(SlotType.Marketing);
            int marketingMax = gm.SlotManager.MarketingMax;
            int operationUsed = gm.SlotManager.GetOccupiedCount(SlotType.Operation);
            int operationMax = gm.SlotManager.OperationMax;

            bool marketingFull = marketingMax > 0 && marketingUsed >= marketingMax;
            bool operationInsufficient = operationMax > 0
                && (float)operationUsed / operationMax < Constants.CHAIN_GROWTH_TRAP_OPERATION_RATIO;

            if (marketingFull && operationInsufficient)
            {
                EventBus.ChainReactionTriggered("GrowthTrapWarning", 0);
                Debug.Log("[ChainReactionSystem] GROWTH TRAP WARNING: marketing full but operation insufficient!");
            }
        }

        // ----------------------------------------------------------------
        // Platform Rating check (called by EventPhase for deterministic events)
        // ----------------------------------------------------------------

        public bool IsPlatformRatingCritical()
        {
            if (_economyManager == null) return false;
            return _economyManager.PlatformRating <= Constants.CHAIN_PLATFORM_RATING_CRISIS;
        }

        // ----------------------------------------------------------------
        // Reset
        // ----------------------------------------------------------------

        public void Reset()
        {
            _cheapSupplierTurns = 0;
            _salaryDelayedTurns = 0;
            _taxUnpaidTurns = 0;
            _uninsuredStaffTurns = 0;
        }
    }
}
