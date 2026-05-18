using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Data;
using EmpireOfCards.Save;
using EmpireOfCards.UI;

namespace EmpireOfCards.Core
{
    // ================================================================
    //  Tutorial Data Types
    // ================================================================

    /// <summary>
    /// Determines when a tutorial step should trigger.
    /// </summary>
    public enum TutorialTrigger
    {
        Immediate,          // Show right away when step becomes current
        OnPhaseStart,       // Show when a specific TurnPhase starts
        OnCardPlaced,       // Show after player places their first card
        OnTurnEnd,          // Show after the first END TURN
        OnShopOpen,         // Show when the shop panel opens
        OnRivalPhase,       // Show during the rival phase
        Manual              // Advances only when player clicks Next
    }

    /// <summary>
    /// One step of the tutorial sequence.
    /// </summary>
    [Serializable]
    public class TutorialStep
    {
        public string stepId;
        public string message;
        public TutorialTrigger trigger;
        public TurnPhase triggerPhase;      // Relevant when trigger == OnPhaseStart
        public string highlightTarget;      // Optional UI element name (for future highlighting)
        public string arrowDirection;       // Optional arrow hint: "up", "down", "left", "right", or null
        public bool pauseGame;              // Freeze Time.timeScale during this step
    }

    // ================================================================
    //  TutorialManager
    // ================================================================

    /// <summary>
    /// State machine that guides new players through their first game.
    /// Checks SaveData.tutorialCompleted to skip on subsequent runs.
    /// Subscribes to EventBus for game-event triggers (card placed, phase
    /// started, turn ended, etc.) and advances steps accordingly.
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        // === State ===
        private List<TutorialStep> _steps;
        private int _currentIndex = -1;
        private bool _tutorialActive;
        private bool _waitingForEvent;          // True when current step waits for a game event
        private float _savedTimeScale = 1f;

        // === External references (set via Init) ===
        private TutorialUI _ui;

        // === Public API ===
        public bool IsTutorialActive => _tutorialActive;
        public int CurrentStepIndex => _currentIndex;

        /// <summary>
        /// Fired when the entire tutorial sequence finishes.
        /// </summary>
        public event Action OnTutorialCompleted;

        // ------------------------------------------------------------------
        //  Lifecycle
        // ------------------------------------------------------------------

        /// <summary>
        /// Call once after the TutorialUI has been created.
        /// </summary>
        public void Init(TutorialUI ui)
        {
            _ui = ui;
            _steps = BuildSteps();
        }

        /// <summary>
        /// Starts the tutorial if it has not been completed before.
        /// Call from GameSceneBootstrap after StartNewRun().
        /// </summary>
        public void TryStartTutorial()
        {
            if (HasCompletedTutorial())
            {
                Debug.Log("[TutorialManager] Tutorial already completed. Skipping.");
                return;
            }

            StartTutorial();
        }

        /// <summary>
        /// Force-start regardless of save state (for a "replay tutorial" button).
        /// </summary>
        public void StartTutorial()
        {
            _tutorialActive = true;
            _currentIndex = -1;
            _waitingForEvent = false;

            SubscribeEvents();

            if (_ui != null)
                _ui.OnNextClicked += OnNextClicked;

            AdvanceToNextStep();

            Debug.Log("[TutorialManager] Tutorial started.");
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();

            if (_ui != null)
                _ui.OnNextClicked -= OnNextClicked;
        }

        // ------------------------------------------------------------------
        //  Step Advancement
        // ------------------------------------------------------------------

        private void AdvanceToNextStep()
        {
            // Restore time if previous step paused it
            RestoreTimeScale();

            _currentIndex++;

            if (_currentIndex >= _steps.Count)
            {
                CompleteTutorial();
                return;
            }

            var step = _steps[_currentIndex];

            if (step.trigger == TutorialTrigger.Immediate || step.trigger == TutorialTrigger.Manual)
            {
                ShowStep(step);
            }
            else
            {
                // Hide UI while waiting for the game event
                _waitingForEvent = true;
                if (_ui != null)
                    _ui.Hide();
            }
        }

        private void ShowStep(TutorialStep step)
        {
            _waitingForEvent = false;

            if (step.pauseGame)
                PauseTimeScale();

            if (_ui != null)
                _ui.Show(step.message, step.arrowDirection);
        }

        // ------------------------------------------------------------------
        //  Player interaction
        // ------------------------------------------------------------------

        private void OnNextClicked()
        {
            if (!_tutorialActive) return;

            AdvanceToNextStep();
        }

        // ------------------------------------------------------------------
        //  EventBus Listeners
        // ------------------------------------------------------------------

        private void SubscribeEvents()
        {
            EventBus.OnPhaseStarted += OnPhaseStarted;
            EventBus.OnBusinessPlaced += OnBusinessPlaced;
            EventBus.OnTurnEnded += OnTurnEnded;
            EventBus.OnCardPlayed += OnCardPlayed;
        }

        private void UnsubscribeEvents()
        {
            EventBus.OnPhaseStarted -= OnPhaseStarted;
            EventBus.OnBusinessPlaced -= OnBusinessPlaced;
            EventBus.OnTurnEnded -= OnTurnEnded;
            EventBus.OnCardPlayed -= OnCardPlayed;
        }

        private void OnPhaseStarted(TurnPhase phase)
        {
            if (!_tutorialActive || !_waitingForEvent) return;

            var step = CurrentStep;
            if (step == null) return;

            if (step.trigger == TutorialTrigger.OnPhaseStart && step.triggerPhase == phase)
                ShowStep(step);

            if (step.trigger == TutorialTrigger.OnRivalPhase && phase == TurnPhase.RivalPhase)
                ShowStep(step);
        }

        private void OnBusinessPlaced(CardData card, int slotIndex)
        {
            TryTrigger(TutorialTrigger.OnCardPlaced);
        }

        private void OnCardPlayed(CardData card)
        {
            // OnCardPlaced also fires for non-business cards; kept separate for clarity.
            TryTrigger(TutorialTrigger.OnCardPlaced);
        }

        private void OnTurnEnded(int turnNumber)
        {
            TryTrigger(TutorialTrigger.OnTurnEnd);
        }

        /// <summary>
        /// Call this from the shop panel open logic if desired.
        /// </summary>
        public void NotifyShopOpened()
        {
            TryTrigger(TutorialTrigger.OnShopOpen);
        }

        private void TryTrigger(TutorialTrigger expectedTrigger)
        {
            if (!_tutorialActive || !_waitingForEvent) return;

            var step = CurrentStep;
            if (step == null) return;

            if (step.trigger == expectedTrigger)
                ShowStep(step);
        }

        // ------------------------------------------------------------------
        //  Time scale helpers
        // ------------------------------------------------------------------

        private void PauseTimeScale()
        {
            _savedTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        private void RestoreTimeScale()
        {
            if (Time.timeScale == 0f)
                Time.timeScale = _savedTimeScale > 0f ? _savedTimeScale : 1f;
        }

        // ------------------------------------------------------------------
        //  Completion & Persistence
        // ------------------------------------------------------------------

        private void CompleteTutorial()
        {
            _tutorialActive = false;
            RestoreTimeScale();
            UnsubscribeEvents();

            if (_ui != null)
            {
                _ui.OnNextClicked -= OnNextClicked;
                _ui.Hide();
            }

            MarkTutorialCompleted();
            OnTutorialCompleted?.Invoke();

            Debug.Log("[TutorialManager] Tutorial complete!");
        }

        private bool HasCompletedTutorial()
        {
            var save = SaveManager.Instance;
            if (save == null) return false;

            var data = save.Load();
            return data != null && data.tutorialCompleted;
        }

        private void MarkTutorialCompleted()
        {
            var save = SaveManager.Instance;
            if (save == null) return;

            var data = save.Load() ?? new SaveData();
            data.tutorialCompleted = true;
            save.Save(data);
        }

        // ------------------------------------------------------------------
        //  Helpers
        // ------------------------------------------------------------------

        private TutorialStep CurrentStep =>
            (_currentIndex >= 0 && _currentIndex < _steps.Count) ? _steps[_currentIndex] : null;

        // ------------------------------------------------------------------
        //  Step Definitions
        // ------------------------------------------------------------------

        /// <summary>
        /// Builds the full 12-step tutorial sequence described in the GDD.
        /// </summary>
        private static List<TutorialStep> BuildSteps()
        {
            return new List<TutorialStep>
            {
                // Step 1: WELCOME
                new TutorialStep
                {
                    stepId = "welcome",
                    message = "Welcome to Empire of Cards!\n\nYour goal: dominate the market by controlling 6 out of 10 territories.",
                    trigger = TutorialTrigger.Immediate,
                    arrowDirection = "up",
                    highlightTarget = "TerritoryBar",
                    pauseGame = true
                },

                // Step 2: YOUR HAND
                new TutorialStep
                {
                    stepId = "hand",
                    message = "These are your cards. You start with a basic deck of 14 cards. Each turn you draw 5.",
                    trigger = TutorialTrigger.Manual,
                    arrowDirection = "down",
                    highlightTarget = "Hand3D",
                    pauseGame = true
                },

                // Step 3: CARD TYPES
                new TutorialStep
                {
                    stepId = "card_types",
                    message = "Blue = Business (place on board, earns money)\nGreen = Employee (attach to business, boosts it)\nRed = Action (instant effect)\nPurple = Upgrade (permanent bonus)",
                    trigger = TutorialTrigger.Manual,
                    highlightTarget = "Hand3D",
                    pauseGame = true
                },

                // Step 4: PLACE A BUSINESS
                new TutorialStep
                {
                    stepId = "place_business",
                    message = "Drag a blue Business card to an empty slot on the board. Try placing your Diner!",
                    trigger = TutorialTrigger.Manual,
                    arrowDirection = "up",
                    highlightTarget = "BusinessSlots",
                    pauseGame = false       // Player needs to interact
                },

                // Step 5: FIRST BUSINESS PLACED (auto-triggered)
                new TutorialStep
                {
                    stepId = "first_placed",
                    message = "Great! Your business now earns income and attracts customers each turn. More customers = more market share!",
                    trigger = TutorialTrigger.OnCardPlaced,
                    highlightTarget = "BusinessSlots",
                    pauseGame = true
                },

                // Step 6: ACTIONS
                new TutorialStep
                {
                    stepId = "actions",
                    message = "You have 3 actions per turn. Each card played costs 1 action. The green dots show your remaining actions.",
                    trigger = TutorialTrigger.Manual,
                    arrowDirection = "down",
                    highlightTarget = "ActionDots",
                    pauseGame = true
                },

                // Step 7: END TURN
                new TutorialStep
                {
                    stepId = "end_turn",
                    message = "When you're done, press END TURN. The system will calculate your income, pay salaries, and check territories.",
                    trigger = TutorialTrigger.Manual,
                    arrowDirection = "right",
                    highlightTarget = "EndTurnButton",
                    pauseGame = false       // Player needs to click End Turn
                },

                // Step 8: ECONOMY EXPLAINED (auto after first END TURN)
                new TutorialStep
                {
                    stepId = "economy",
                    message = "Income from your businesses minus employee salaries minus 15% tax = your profit. Watch your money grow!",
                    trigger = TutorialTrigger.OnTurnEnd,
                    highlightTarget = "MoneyDisplay",
                    pauseGame = true
                },

                // Step 9: TERRITORIES
                new TutorialStep
                {
                    stepId = "territories",
                    message = "The bar at the top shows territory control.\nBlue = yours, Red = rival, Gray = unclaimed.\nGet 6 blue to WIN!",
                    trigger = TutorialTrigger.Manual,
                    arrowDirection = "up",
                    highlightTarget = "TerritoryBar",
                    pauseGame = true
                },

                // Step 10: RIVAL (auto during rival phase)
                new TutorialStep
                {
                    stepId = "rival",
                    message = "MegaCorp is your rival. They also build businesses and steal customers. Watch what they do each turn.",
                    trigger = TutorialTrigger.OnRivalPhase,
                    highlightTarget = "RivalArea",
                    pauseGame = true
                },

                // Step 11: SHOP
                new TutorialStep
                {
                    stepId = "shop",
                    message = "Press SHOP to buy new cards. Better cards = stronger combos = more profit!",
                    trigger = TutorialTrigger.Manual,
                    arrowDirection = "left",
                    highlightTarget = "ShopButton",
                    pauseGame = true
                },

                // Step 12: TUTORIAL COMPLETE
                new TutorialStep
                {
                    stepId = "complete",
                    message = "You're ready! Tips:\n- Combine cards for combos\n- Watch out for the FBI if you play illegal cards\n- Always expand your business empire!\n\nGood luck!",
                    trigger = TutorialTrigger.Manual,
                    pauseGame = false
                }
            };
        }
    }
}
