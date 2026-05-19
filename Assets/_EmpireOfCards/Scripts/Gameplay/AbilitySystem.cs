using System.Collections.Generic;
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

        // --- Consultant scaling: maps employee instance -> accumulated bonus ---
        private readonly Dictionary<CardData, int> _scalingIncomeBonuses = new Dictionary<CardData, int>();

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

                case ActiveAbilityType.ReduceRivalCustomers:
                    // Bouncer "Intimidate": rival loses abilityValue2 customers
                    if (gm.RivalAI != null)
                    {
                        gm.RivalAI.ApplyCustomerPenalty(employee.abilityValue2);
                        Debug.Log($"[AbilitySystem] {employee.cardName}: rival lost {employee.abilityValue2} customers");
                    }
                    break;

                case ActiveAbilityType.ScaleIncomePerTurn:
                    // Consultant "Experience": income bonus grows by abilityValue2 each activation
                    if (!_scalingIncomeBonuses.ContainsKey(employee))
                        _scalingIncomeBonuses[employee] = 0;
                    _scalingIncomeBonuses[employee] += employee.abilityValue2;
                    int currentBonus = _scalingIncomeBonuses[employee];
                    _incomeMultiplierThisTurn += currentBonus * 0.01f; // +5 => +5% cumulative
                    Debug.Log($"[AbilitySystem] {employee.cardName}: scaling income bonus now +{currentBonus} (+{currentBonus}% income)");
                    break;

                case ActiveAbilityType.CopyRandomEmployeeAbility:
                    // Headhunter "Poach Talent": pick random other employee and fire their ability
                    CardData copied = PickRandomOtherEmployee(gm.BoardManager, employee);
                    if (copied != null && copied.activeAbilityType != ActiveAbilityType.None
                        && copied.activeAbilityType != ActiveAbilityType.CopyRandomEmployeeAbility)
                    {
                        // Refund the action we already spent -- the recursive call will spend one
                        gm.AddExtraAction(1);
                        bool copyResult = TryUseAbility(copied, businessIndex);
                        if (!copyResult)
                        {
                            // Recursive call failed and already refunded; nothing to do
                        }
                        Debug.Log($"[AbilitySystem] {employee.cardName}: copied ability from {copied.cardName}");
                    }
                    else
                    {
                        Debug.Log($"[AbilitySystem] {employee.cardName}: no valid employee to copy");
                    }
                    break;

                case ActiveAbilityType.SabotageCostIncrease:
                    // Lobbyist "Red Tape": rival's next business purchase costs 25% more
                    if (gm.RivalAI != null)
                    {
                        gm.RivalAI.ApplyNextPurchaseCostIncrease(employee.abilityValue1);
                        Debug.Log($"[AbilitySystem] {employee.cardName}: rival's next purchase costs +{employee.abilityValue1 * 100}% more");
                    }
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

        /// <summary>
        /// Picks a random employee on the board that is NOT the given employee
        /// and has a usable active ability (not None, not CopyRandomEmployeeAbility).
        /// </summary>
        private CardData PickRandomOtherEmployee(BoardManager boardManager, CardData exclude)
        {
            if (boardManager == null) return null;

            var candidates = new List<CardData>();
            foreach (var business in boardManager.PlayerBusinesses)
            {
                if (business == null || business.isClosed) continue;
                foreach (var emp in business.employees)
                {
                    if (emp == null) continue;
                    if (emp == exclude) continue;
                    if (emp.activeAbilityType == ActiveAbilityType.None) continue;
                    if (emp.activeAbilityType == ActiveAbilityType.CopyRandomEmployeeAbility) continue;
                    candidates.Add(emp);
                }
            }

            if (candidates.Count == 0) return null;
            return candidates[Random.Range(0, candidates.Count)];
        }
    }
}
