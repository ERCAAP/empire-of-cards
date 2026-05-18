using UnityEngine;

namespace EmpireOfCards.Core.StateMachine.TurnStates
{
    /// <summary>
    /// Turn phase that calculates and applies all end-of-turn effects:
    /// income, salaries, tax, combos, FBI checks, and territory changes.
    /// Animations play sequentially before the UI is updated.
    /// </summary>
    public class ResolvePhaseState : IState
    {
        private readonly StateMachine stateMachine;
        private readonly IState nextState;

        private enum ResolveStep
        {
            Income,
            Salary,
            Tax,
            Combo,
            Territory,
            FBI,
            Complete
        }

        private ResolveStep currentStep;
        private bool stepAnimationComplete;

        public ResolvePhaseState(StateMachine stateMachine, IState nextState)
        {
            this.stateMachine = stateMachine;
            this.nextState = nextState;
        }

        public void Enter()
        {
            Debug.Log("[ResolvePhaseState] Enter - Calculating turn results");

            currentStep = ResolveStep.Income;
            stepAnimationComplete = false;

            // Begin the sequential resolution chain
            ProcessCurrentStep();
        }

        public void Execute()
        {
            if (!stepAnimationComplete)
                return;

            // Advance to the next step
            stepAnimationComplete = false;
            currentStep++;

            if (currentStep == ResolveStep.Complete)
            {
                stateMachine.ChangeState(nextState);
                return;
            }

            ProcessCurrentStep();
        }

        public void Exit()
        {
            // Update all UI elements to reflect the resolved values
            // TODO: UIManager.Instance.RefreshAllUI();

            Debug.Log("[ResolvePhaseState] Exit - All UI updated");
        }

        private void ProcessCurrentStep()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null)
            {
                stepAnimationComplete = true;
                return;
            }

            switch (currentStep)
            {
                case ResolveStep.Income:
                    ResolveIncome(gm);
                    break;

                case ResolveStep.Salary:
                    ResolveSalary(gm);
                    break;

                case ResolveStep.Tax:
                    ResolveTax(gm);
                    break;

                case ResolveStep.Combo:
                    ResolveCombo(gm);
                    break;

                case ResolveStep.Territory:
                    ResolveTerritory(gm);
                    break;

                case ResolveStep.FBI:
                    ResolveFBI(gm);
                    break;
            }
        }

        private void ResolveIncome(GameManager gm)
        {
            // Calculate total income from all active businesses
            // TODO: int totalIncome = EconomyManager.Instance.CalculateTurnIncome();
            // TODO: gm.GainMoney(totalIncome);
            // TODO: UIManager.Instance.PlayIncomeAnimation(totalIncome, OnStepAnimationComplete);

            Debug.Log("[ResolvePhaseState] Income calculated");
            stepAnimationComplete = true;
        }

        private void ResolveSalary(GameManager gm)
        {
            // Deduct salaries for all employed workers
            // TODO: int totalSalary = EconomyManager.Instance.CalculateTotalSalaries();
            // TODO: gm.SpendMoney(totalSalary);
            // TODO: UIManager.Instance.PlaySalaryAnimation(totalSalary, OnStepAnimationComplete);

            Debug.Log("[ResolvePhaseState] Salaries deducted");
            stepAnimationComplete = true;
        }

        private void ResolveTax(GameManager gm)
        {
            // Calculate and apply tax based on income
            // TODO: int taxAmount = EconomyManager.Instance.CalculateTax();
            // TODO: gm.SpendMoney(taxAmount);
            // TODO: UIManager.Instance.PlayTaxAnimation(taxAmount, OnStepAnimationComplete);

            Debug.Log("[ResolvePhaseState] Tax applied");
            stepAnimationComplete = true;
        }

        private void ResolveCombo(GameManager gm)
        {
            // Check for and apply any triggered combos
            // TODO: var triggeredCombos = ComboSystem.Instance.CheckCombos();
            // TODO: foreach (var combo in triggeredCombos)
            // TODO:     ComboSystem.Instance.ApplyCombo(combo);
            // TODO: UIManager.Instance.PlayComboAnimation(triggeredCombos, OnStepAnimationComplete);

            Debug.Log("[ResolvePhaseState] Combos resolved");
            stepAnimationComplete = true;
        }

        private void ResolveTerritory(GameManager gm)
        {
            // Calculate territory changes based on customer counts
            // TODO: int playerGain = TerritoryManager.Instance.CalculatePlayerTerritoryGain();
            // TODO: int rivalGain = TerritoryManager.Instance.CalculateRivalTerritoryGain();
            // TODO: gm.SetTerritories(gm.PlayerTerritories + playerGain, gm.RivalTerritories + rivalGain);
            // TODO: UIManager.Instance.PlayTerritoryAnimation(OnStepAnimationComplete);

            Debug.Log("[ResolvePhaseState] Territory updated");
            stepAnimationComplete = true;
        }

        private void ResolveFBI(GameManager gm)
        {
            // Check if FBI raid triggers based on current risk
            // TODO: float currentRisk = gm.FBIRisk;
            // TODO: bool raidTriggered = Random.value < currentRisk;
            // TODO: if (raidTriggered)
            // TODO: {
            // TODO:     gm.SpendMoney(Constants.FBI_RAID_PENALTY);
            // TODO:     EventBus.FBIRaid(Constants.FBI_RAID_PENALTY);
            // TODO:     gm.SetFBIRisk(0f);
            // TODO: }
            // TODO: UIManager.Instance.PlayFBIAnimation(raidTriggered, OnStepAnimationComplete);

            Debug.Log("[ResolvePhaseState] FBI check complete");
            stepAnimationComplete = true;
        }

        /// <summary>
        /// Callback for when a step's animation finishes playing.
        /// </summary>
        private void OnStepAnimationComplete()
        {
            stepAnimationComplete = true;
        }
    }
}
