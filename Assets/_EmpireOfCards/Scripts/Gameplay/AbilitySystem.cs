using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Manages employee active abilities (GDD Section 3.2).
    /// Players click a placed employee to activate their ability (costs 1 action).
    /// Turn-scoped buffs are read by EconomyManager during the resolve phase
    /// and reset at the start of each new turn.
    /// </summary>
    public class AbilitySystem : MonoBehaviour
    {
        // --- Turn-scoped buffs (reset at turn end) ---
        private float _customerMultiplierThisTurn = 1f;
        private float _incomeMultiplierThisTurn = 1f;
        private int _extraCustomersThisTurn = 0;
        private bool _taxFreeThisTurn = false;
        private int _ponziDebtNextTurn = 0;

        // --- Public accessors for EconomyManager to read ---
        public float CustomerMultiplier => _customerMultiplierThisTurn;
        public float IncomeMultiplier => _incomeMultiplierThisTurn;
        public int ExtraCustomers => _extraCustomersThisTurn;
        public bool TaxFree => _taxFreeThisTurn;

        // ----------------------------------------------------------------
        // Lifecycle
        // ----------------------------------------------------------------

        private void OnEnable()
        {
            EventBus.OnTurnStarted += HandleTurnStarted;
        }

        private void OnDisable()
        {
            EventBus.OnTurnStarted -= HandleTurnStarted;
        }

        private void HandleTurnStarted(int turn)
        {
            ResetTurnBuffs();
        }

        // ----------------------------------------------------------------
        // Main API
        // ----------------------------------------------------------------

        /// <summary>
        /// Attempts to use the active ability of a placed employee.
        /// Costs 1 action. Returns true if the ability was activated.
        /// </summary>
        public bool TryUseAbility(CardData employee, int businessIndex)
        {
            if (employee == null) return false;
            if (employee.activeAbilityType == ActiveAbilityType.None) return false;

            var gm = GameManager.Instance;
            if (gm == null) return false;
            if (gm.PlayerActions <= 0) return false;
            if (!gm.UseAction()) return false;

            switch (employee.activeAbilityType)
            {
                case ActiveAbilityType.AddCustomersThisTurn:
                    // Intern "Hustle": +3, Junior Marketer "Campaign": +5
                    _extraCustomersThisTurn += employee.abilityValue2;
                    Debug.Log($"[AbilitySystem] {employee.cardName}: +{employee.abilityValue2} customers this turn");
                    break;

                case ActiveAbilityType.MultiplyCustomersThisTurn:
                    // Barista "Latte Festival": customers x2
                    _customerMultiplierThisTurn *= employee.abilityValue1;
                    Debug.Log($"[AbilitySystem] {employee.cardName}: customers x{employee.abilityValue1} this turn");
                    break;

                case ActiveAbilityType.MultiplyIncomeThisTurn:
                    // Chef "Special Menu": income x1.5
                    _incomeMultiplierThisTurn *= employee.abilityValue1;
                    Debug.Log($"[AbilitySystem] {employee.cardName}: income x{employee.abilityValue1} this turn");
                    break;

                case ActiveAbilityType.StealCustomersFromRival:
                    // Influencer "Post Story": steal 5 customers from rival
                    if (gm.RivalAI != null)
                    {
                        gm.RivalAI.ApplyCustomerPenalty(employee.abilityValue2);
                        Debug.Log($"[AbilitySystem] {employee.cardName}: stole {employee.abilityValue2} customers from rival");
                    }
                    break;

                case ActiveAbilityType.AddCustomersToAll:
                    // Marketing Guru "Viral Campaign": +3 customers to ALL businesses
                    int activeCount = gm.BoardManager != null ? gm.BoardManager.GetActiveBusinessCount() : 1;
                    _extraCustomersThisTurn += employee.abilityValue2 * activeCount;
                    Debug.Log($"[AbilitySystem] {employee.cardName}: +{employee.abilityValue2} customers to {activeCount} businesses");
                    break;

                case ActiveAbilityType.NullifyTaxThisTurn:
                    // Accountant "Tax Plan": 0% tax this turn
                    _taxFreeThisTurn = true;
                    Debug.Log($"[AbilitySystem] {employee.cardName}: tax nullified this turn");
                    break;

                case ActiveAbilityType.BonusIncomeWithPenalty:
                    // Fraudster "Ponzi": +300 money, but -150 next turn
                    gm.GainMoney(employee.abilityValue2);
                    _ponziDebtNextTurn += 150; // Per GDD
                    Debug.Log($"[AbilitySystem] {employee.cardName}: +{employee.abilityValue2} money, -150 next turn");
                    break;

                case ActiveAbilityType.MotivateAllEmployees:
                    // Loyal Manager "Motivation": all employees +1 customer
                    int totalEmployees = CountAllBoardEmployees(gm.BoardManager);
                    _extraCustomersThisTurn += totalEmployees;
                    Debug.Log($"[AbilitySystem] {employee.cardName}: +{totalEmployees} customers ({totalEmployees} employees motivated)");
                    break;

                default:
                    // Unknown ability type, refund the action
                    gm.AddExtraAction(1);
                    return false;
            }

            EventBus.CardPlayed(employee);
            return true;
        }

        // ----------------------------------------------------------------
        // Turn Reset
        // ----------------------------------------------------------------

        /// <summary>
        /// Resets turn-scoped buffs and applies any pending debts (Ponzi).
        /// Called at the start of each turn.
        /// </summary>
        public void ResetTurnBuffs()
        {
            // Apply ponzi debt carried over from previous turn
            if (_ponziDebtNextTurn > 0)
            {
                GameManager.Instance?.SpendMoney(_ponziDebtNextTurn);
                Debug.Log($"[AbilitySystem] Ponzi debt applied: -{_ponziDebtNextTurn}");
                _ponziDebtNextTurn = 0;
            }

            _customerMultiplierThisTurn = 1f;
            _incomeMultiplierThisTurn = 1f;
            _extraCustomersThisTurn = 0;
            _taxFreeThisTurn = false;
        }

        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        /// <summary>
        /// Counts all employees across all active businesses on the board.
        /// </summary>
        private int CountAllBoardEmployees(BoardManager boardManager)
        {
            if (boardManager == null) return 0;

            int count = 0;
            foreach (var business in boardManager.PlayerBusinesses)
            {
                if (business == null || business.isClosed) continue;
                count += business.employees.Count;
            }
            return count;
        }
    }
}
