using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Data;
using EmpireOfCards.Save;
using EmpireOfCards.UI;
using EmpireOfCards.World;

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
        OnResolvePhase,     // Show during the resolve phase
        Manual              // Advances only when player clicks Next
    }

    /// <summary>
    /// Display mode for a tutorial step.
    /// </summary>
    public enum TutorialDisplayMode
    {
        FullScreen,     // Big dark overlay, story text, dramatic
        FloatingTip     // Smaller panel, in-game guidance
    }

    /// <summary>
    /// What kind of indicator to show during a step.
    /// </summary>
    public enum TutorialIndicator
    {
        None,
        Arrow3D,        // 3D world-space arrow pointing to board element
        Arrow2D,        // 2D screen-space arrow pointing to UI element
        CardGlow        // Brief glow on cards (for card colors step)
    }

    /// <summary>
    /// One step of the tutorial sequence.
    /// </summary>
    [Serializable]
    public class TutorialStep
    {
        public string stepId;
        public string locKey;               // Localization key for the message text
        public string message;              // Fallback message if localization not found
        public TutorialTrigger trigger;
        public TurnPhase triggerPhase;      // Relevant when trigger == OnPhaseStart
        public TutorialDisplayMode displayMode;
        public TutorialIndicator indicator;
        public string buttonLabel;          // "Begin", "Got it", "Next", etc.
        public bool pauseGame;
        public bool autoAdvance;            // Step advances automatically (no button needed)
        public float autoAdvanceDelay;      // Seconds before auto-advancing

        // 3D arrow config
        public Vector3 arrow3DPosition;
        public Vector3 arrow3DDirection;
        public Color arrowColor;

        // 2D arrow config (screen-space position and direction)
        public Vector2 arrow2DScreenPos;
        public Vector2 arrow2DDirection;
    }

    // ================================================================
    //  TutorialManager
    // ================================================================

    /// <summary>
    /// Story-driven tutorial system that guides new players through their
    /// first game with narrative context, 3D arrows on the board, 2D
    /// indicators on UI elements, and typewriter text effects.
    ///
    /// The player is a scrappy entrepreneur who just arrived in a city
    /// dominated by MegaCorp. They have $500 and a dream.
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        // === State ===
        private List<TutorialStep> _steps;
        private int _currentIndex = -1;
        private bool _tutorialActive;
        private bool _waitingForEvent;
        private float _savedTimeScale = 1f;

        // Auto-advance timer
        private bool _autoAdvanceActive;
        private float _autoAdvanceTimer;
        private float _autoAdvanceTarget;

        // === External references (set via Init) ===
        private TutorialUI _ui;

        // === Active 3D arrow ===
        private TutorialArrow3D _currentArrow3D;

        // === 3D World positions (from Board3D layout) ===
        private static readonly Vector3 POS_TERRITORY_AREA = new Vector3(0f, 1.5f, 3.5f);
        private static readonly Vector3 POS_HAND_AREA = new Vector3(0f, 1f, -3f);
        private static readonly Vector3 POS_BUSINESS_SLOTS = new Vector3(0f, 1f, -0.5f);
        private static readonly Vector3 POS_FIRST_SLOT = new Vector3(-5f, 1f, -0.5f);

        // === 2D Screen positions (approximate, based on HUDBuilder layout) ===
        // These are anchoredPosition values relative to canvas center.
        private static readonly Vector2 SCREEN_END_TURN = new Vector2(750f, -340f);
        private static readonly Vector2 SCREEN_SHOP = new Vector2(-750f, -340f);
        private static readonly Vector2 SCREEN_MONEY = new Vector2(-700f, 500f);
        private static readonly Vector2 SCREEN_ACTION_DOTS = new Vector2(0f, -340f);

        // Arrow colors
        private static readonly Color COLOR_GOLD = new Color(1f, 0.85f, 0.2f);
        private static readonly Color COLOR_CYAN = new Color(0.2f, 0.85f, 1f);
        private static readonly Color COLOR_GREEN = new Color(0.2f, 1f, 0.4f);

        // === Public API ===
        public bool IsTutorialActive => _tutorialActive;
        public int CurrentStepIndex => _currentIndex;

        /// <summary>Fired when the entire tutorial sequence finishes.</summary>
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
            _steps = new List<TutorialStep>();
        }

        /// <summary>
        /// Starts the tutorial if it has not been completed before.
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
            _steps = BuildSteps();
            _tutorialActive = true;
            _currentIndex = -1;
            _waitingForEvent = false;
            _autoAdvanceActive = false;

            SubscribeEvents();

            if (_ui != null)
            {
                _ui.OnNextClicked += OnNextClicked;
                _ui.OnSkipClicked += OnSkipClicked;
            }

            AdvanceToNextStep();

            Debug.Log("[TutorialManager] Story tutorial started.");
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
            CleanupArrow3D();

            if (_ui != null)
            {
                _ui.OnNextClicked -= OnNextClicked;
                _ui.OnSkipClicked -= OnSkipClicked;
            }
        }

        private void Update()
        {
            if (!_tutorialActive) return;

            // Auto-advance timer
            if (_autoAdvanceActive)
            {
                _autoAdvanceTimer += Time.unscaledDeltaTime;
                if (_autoAdvanceTimer >= _autoAdvanceTarget)
                {
                    _autoAdvanceActive = false;
                    AdvanceToNextStep();
                }
            }
        }

        // ------------------------------------------------------------------
        //  Step Advancement
        // ------------------------------------------------------------------

        private void AdvanceToNextStep()
        {
            RestoreTimeScale();
            CleanupArrow3D();

            if (_ui != null)
            {
                _ui.HideArrow2D();
                _ui.ShowButton();
            }

            _autoAdvanceActive = false;
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

            // Show appropriate UI mode
            if (_ui != null)
            {
                string text = !string.IsNullOrWhiteSpace(step.locKey)
                    ? LocalizationManager.GetWithFallback(step.locKey, step.message)
                    : step.message;
                string btnLabel = step.buttonLabel ?? "Got it";

                if (step.displayMode == TutorialDisplayMode.FullScreen)
                    _ui.ShowFullScreen(text, btnLabel);
                else
                    _ui.ShowTip(text, btnLabel);

                // Auto-advance: hide the button, set timer
                if (step.autoAdvance)
                {
                    _ui.HideButton();
                    _autoAdvanceActive = true;
                    _autoAdvanceTimer = 0f;
                    _autoAdvanceTarget = step.autoAdvanceDelay > 0f
                        ? step.autoAdvanceDelay
                        : 5f;
                }
            }

            // Set up indicators
            SetupIndicator(step);

            Debug.Log($"[TutorialManager] Step {_currentIndex + 1}/{_steps.Count}: {step.stepId}");
        }

        private void SetupIndicator(TutorialStep step)
        {
            switch (step.indicator)
            {
                case TutorialIndicator.Arrow3D:
                    CleanupArrow3D();
                    _currentArrow3D = TutorialArrow3D.Create(
                        step.arrow3DPosition,
                        step.arrow3DDirection,
                        step.arrowColor
                    );
                    break;

                case TutorialIndicator.Arrow2D:
                    if (_ui != null)
                        _ui.ShowArrow2D(step.arrow2DScreenPos, step.arrow2DDirection);
                    break;

                case TutorialIndicator.CardGlow:
                    // Card glow is purely visual feedback. In a full implementation
                    // this would pulse card meshes. For now the text description
                    // in the tutorial step suffices.
                    break;

                case TutorialIndicator.None:
                default:
                    break;
            }
        }

        // ------------------------------------------------------------------
        //  Player interaction
        // ------------------------------------------------------------------

        private void OnNextClicked()
        {
            if (!_tutorialActive) return;

            AdvanceToNextStep();
        }

        private void OnSkipClicked()
        {
            if (!_tutorialActive) return;

            Debug.Log("[TutorialManager] Player skipped tutorial.");
            CompleteTutorial();
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

            if (step.trigger == TutorialTrigger.OnResolvePhase && phase == TurnPhase.ResolvePhase)
                ShowStep(step);
        }

        private void OnBusinessPlaced(CardData card, int slotIndex)
        {
            TryTrigger(TutorialTrigger.OnCardPlaced);
        }

        private void OnCardPlayed(CardData card)
        {
            TryTrigger(TutorialTrigger.OnCardPlaced);
        }

        private void OnTurnEnded(int turnNumber)
        {
            TryTrigger(TutorialTrigger.OnTurnEnd);
        }

        /// <summary>
        /// Call from the shop panel open logic to trigger shop-related steps.
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
        //  Arrow cleanup
        // ------------------------------------------------------------------

        private void CleanupArrow3D()
        {
            if (_currentArrow3D != null)
            {
                _currentArrow3D.DestroyArrow();
                _currentArrow3D = null;
            }
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
            _autoAdvanceActive = false;
            RestoreTimeScale();
            CleanupArrow3D();
            UnsubscribeEvents();

            if (_ui != null)
            {
                _ui.OnNextClicked -= OnNextClicked;
                _ui.OnSkipClicked -= OnSkipClicked;
                _ui.HideArrow2D();
                _ui.Hide();
            }

            MarkTutorialCompleted();
            OnTutorialCompleted?.Invoke();

            Debug.Log("[TutorialManager] Tutorial complete! Good luck, entrepreneur.");
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
        //  Step Definitions -- 14-Step Story Tutorial
        // ------------------------------------------------------------------

        /// <summary>
        /// Builds the full 14-step story-driven tutorial sequence.
        /// Each step has narrative context, appropriate indicators, and
        /// trigger conditions matching real game flow.
        /// </summary>
        private List<TutorialStep> BuildSteps()
        {
            var gm = GameManager.Instance;
            VentureType ventureType = gm != null && gm.SelectedVenture != null
                ? gm.SelectedVenture.ventureType
                : VentureType.FastFood;
            string runName = gm != null ? gm.RunDisplayName : "New Venture";
            string categoryLabel = gm != null ? gm.RunCategoryLabel : null;

            return new List<TutorialStep>
            {
                // ============================================================
                // Step 1: STORY INTRO (full screen, dramatic)
                // ============================================================
                new TutorialStep
                {
                    stepId = "story_intro",
                    locKey = string.Empty,
                    message = GetStoryIntroMessage(ventureType, runName, categoryLabel),
                    trigger = TutorialTrigger.Immediate,
                    displayMode = TutorialDisplayMode.FullScreen,
                    indicator = TutorialIndicator.None,
                    buttonLabel = "Begin",
                    pauseGame = true
                },

                // ============================================================
                // Step 2: THE TABLE
                // ============================================================
                new TutorialStep
                {
                    stepId = "the_table",
                    locKey = "tutorial.step2",
                    message = "This is your battlefield \u2014 the market table.\n" +
                              "Bottom: your business board.\n" +
                              "Middle: district pressure, traffic, ratings, and market pull.\n" +
                              "Top: the rival board and their visible moves.",
                    trigger = TutorialTrigger.Manual,
                    displayMode = TutorialDisplayMode.FloatingTip,
                    indicator = TutorialIndicator.Arrow3D,
                    arrow3DPosition = POS_TERRITORY_AREA,
                    arrow3DDirection = Vector3.down,
                    arrowColor = COLOR_CYAN,
                    buttonLabel = "Got it",
                    pauseGame = true
                },

                // ============================================================
                // Step 3: YOUR CARDS
                // ============================================================
                new TutorialStep
                {
                    stepId = "your_cards",
                    locKey = "tutorial.step3",
                    message = "These are your cards \u2014 your weapons.\n" +
                              "Each turn you draw options and spend actions to build, react,\n" +
                              "or pressure the market. Some cards stay on the board.\n" +
                              "Some are short-term responses.",
                    trigger = TutorialTrigger.Manual,
                    displayMode = TutorialDisplayMode.FloatingTip,
                    indicator = TutorialIndicator.Arrow3D,
                    arrow3DPosition = POS_HAND_AREA,
                    arrow3DDirection = Vector3.down,
                    arrowColor = COLOR_CYAN,
                    buttonLabel = "Got it",
                    pauseGame = true
                },

                // ============================================================
                // Step 4: CARD COLORS MATTER
                // ============================================================
                new TutorialStep
                {
                    stepId = "card_colors",
                    locKey = "tutorial.step4",
                    message = "BLUE cards are Businesses \u2014 they make money.\n" +
                              "Setup cards build capacity.\n" +
                              "Growth cards create demand.\n" +
                              "Risk cards spike power but raise danger.\n" +
                              "Reaction cards repair trust, quality, or stability.",
                    trigger = TutorialTrigger.Manual,
                    displayMode = TutorialDisplayMode.FloatingTip,
                    indicator = TutorialIndicator.CardGlow,
                    buttonLabel = "Got it",
                    pauseGame = true
                },

                // ============================================================
                // Step 5: YOUR VENTURE ANCHOR
                // ============================================================
                new TutorialStep
                {
                    stepId = "first_move",
                    locKey = string.Empty,
                    message = GetAnchorMessage(ventureType, categoryLabel),
                    trigger = TutorialTrigger.Manual,
                    displayMode = TutorialDisplayMode.FloatingTip,
                    indicator = TutorialIndicator.Arrow3D,
                    arrow3DPosition = POS_FIRST_SLOT,
                    arrow3DDirection = Vector3.down,
                    arrowColor = COLOR_GOLD,
                    buttonLabel = "Continue",
                    pauseGame = true
                },

                // ============================================================
                // Step 6: CORE ECONOMY LOOP
                // ============================================================
                new TutorialStep
                {
                    stepId = "business_placed",
                    locKey = "tutorial.step6",
                    message = "Watch the main loop:\n" +
                              "Marketing raises demand.\n" +
                              "Operations and staff raise capacity.\n" +
                              "Suppliers improve quality or margin.\n" +
                              "If demand beats capacity, rating falls and organic traffic slows.",
                    trigger = TutorialTrigger.Manual,
                    displayMode = TutorialDisplayMode.FloatingTip,
                    indicator = TutorialIndicator.Arrow3D,
                    arrow3DPosition = POS_BUSINESS_SLOTS,
                    arrow3DDirection = Vector3.down,
                    arrowColor = COLOR_GREEN,
                    buttonLabel = "Got it",
                    pauseGame = true
                },

                // ============================================================
                // Step 7: ACTIONS REMAINING
                // ============================================================
                new TutorialStep
                {
                    stepId = "actions_remaining",
                    locKey = "tutorial.step7",
                    message = "You used 1 action. See the green dots?\n" +
                              "These pips show how many moves you still have.\n" +
                              "A strong turn usually mixes growth with stability,\n" +
                              "not just raw expansion.",
                    trigger = TutorialTrigger.Manual,
                    displayMode = TutorialDisplayMode.FloatingTip,
                    indicator = TutorialIndicator.Arrow2D,
                    arrow2DScreenPos = SCREEN_ACTION_DOTS,
                    arrow2DDirection = Vector2.up,
                    buttonLabel = "Or what?",
                    pauseGame = true
                },

                // ============================================================
                // Step 8: END YOUR TURN
                // ============================================================
                new TutorialStep
                {
                    stepId = "end_turn",
                    locKey = "tutorial.step8",
                    message = "...press END TURN to let the market do its thing.\n" +
                              "Then the rival acts, the market resolves,\n" +
                              "and your cash, rating, and market share update.",
                    trigger = TutorialTrigger.Manual,
                    displayMode = TutorialDisplayMode.FloatingTip,
                    indicator = TutorialIndicator.Arrow2D,
                    arrow2DScreenPos = SCREEN_END_TURN,
                    arrow2DDirection = Vector2.right,
                    buttonLabel = "Let's go!",
                    pauseGame = false   // Player needs to click END TURN
                },

                // ============================================================
                // Step 9: RIVAL ACTIONS
                // ============================================================
                new TutorialStep
                {
                    stepId = "market_resolves",
                    locKey = "tutorial.step9",
                    message = "Rival phase: watch which lane they pressure.\n" +
                              "Price, quality, growth, staffing, or expansion.\n" +
                              "Their cards are signals you can answer next turn.",
                    trigger = TutorialTrigger.OnRivalPhase,
                    displayMode = TutorialDisplayMode.FloatingTip,
                    indicator = TutorialIndicator.None,
                    autoAdvance = true,
                    autoAdvanceDelay = 4.5f,
                    pauseGame = false
                },

                // ============================================================
                // Step 10: THE MARKET RESOLVES
                // ============================================================
                new TutorialStep
                {
                    stepId = "money_update",
                    locKey = "tutorial.step10",
                    message = "Resolve phase compares demand, capacity, quality,\n" +
                              "rating, upkeep, and risk.\n" +
                              "Good sequencing becomes profit.\n" +
                              "Bad sequencing becomes pressure.",
                    trigger = TutorialTrigger.OnResolvePhase,
                    displayMode = TutorialDisplayMode.FloatingTip,
                    indicator = TutorialIndicator.None,
                    autoAdvance = true,
                    autoAdvanceDelay = 5.5f,
                    pauseGame = false
                },

                // ============================================================
                // Step 11: TURN REPORT
                // ============================================================
                new TutorialStep
                {
                    stepId = "megacorp_turn",
                    locKey = "tutorial.step11",
                    message = "Turn report: check why your money, rating,\n" +
                              "and market share changed.\n" +
                              "If trust slips or overload hits, the report tells you why.",
                    trigger = TutorialTrigger.OnTurnEnd,
                    displayMode = TutorialDisplayMode.FloatingTip,
                    indicator = TutorialIndicator.Arrow2D,
                    arrow2DScreenPos = SCREEN_MONEY,
                    arrow2DDirection = Vector2.left,
                    buttonLabel = "Understood",
                    pauseGame = true
                },

                // ============================================================
                // Step 12: THE SHOP
                // ============================================================
                new TutorialStep
                {
                    stepId = "the_shop",
                    locKey = "tutorial.step12",
                    message = "Need a different answer? Open SHOP.\n" +
                              "Buy cost gets a card into your economy.\n" +
                              "Play cost activates it.\n" +
                              "Upkeep and salary are what you carry every turn.",
                    trigger = TutorialTrigger.Immediate,
                    displayMode = TutorialDisplayMode.FloatingTip,
                    indicator = TutorialIndicator.Arrow2D,
                    arrow2DScreenPos = SCREEN_SHOP,
                    arrow2DDirection = Vector2.left,
                    buttonLabel = "Got it",
                    pauseGame = true
                },

                // ============================================================
                // Step 13: WINNING STRATEGY
                // ============================================================
                new TutorialStep
                {
                    stepId = "winning_strategy",
                    locKey = string.Empty,
                    message = GetStrategyMessage(ventureType, categoryLabel),
                    trigger = TutorialTrigger.Manual,
                    displayMode = TutorialDisplayMode.FullScreen,
                    indicator = TutorialIndicator.None,
                    buttonLabel = "Got it!",
                    pauseGame = true
                },

                // ============================================================
                // Step 14: GO!
                // ============================================================
                new TutorialStep
                {
                    stepId = "go",
                    locKey = "tutorial.step14",
                    message = "Build the board. Protect trust. Scale at the right speed.\n" +
                              "Finish stronger than the rival by turn 25.\n\n" +
                              "Good luck.",
                    trigger = TutorialTrigger.Manual,
                    displayMode = TutorialDisplayMode.FullScreen,
                    indicator = TutorialIndicator.None,
                    buttonLabel = "LET'S GO!",
                    pauseGame = false
                }
            };
        }

        private static string GetStoryIntroMessage(VentureType ventureType, string runName, string categoryLabel)
        {
            return ventureType switch
            {
                VentureType.TechApp => $"You just launched {runName}.\nCategory: {categoryLabel ?? "Tech Product"}.\nYour rival starts in the same app market, chases the same users,\nand will punish weak backend, weak reviews, and sloppy growth.\n\nYour goal: survive the 25-turn product war with the bigger market share.",
                VentureType.FastFood => $"You just opened {runName}.\nThe rival starts in the same food lane, chasing the same local traffic.\nKitchen flow, waiters, runners, cleaning, supplier quality, and reviews\nwill decide whether you scale or spiral.\n\nYour goal: end the run with the bigger market share.",
                VentureType.Cafe => $"You just opened {runName}.\nThe rival cafe starts in the same neighborhood demand loop.\nBarista rhythm, service consistency, bean quality, floor cleanliness,\nand regular loyalty decide who owns the block.\n\nYour goal: finish the 25-turn run ahead on market share.",
                VentureType.ClothingStore => $"You just launched {runName}.\nYour rival store competes in the same seasonal fashion cycle.\nDisplay quality, stock timing, returns, fabric trust, and trend relevance\nwill decide who captures demand.\n\nYour goal: finish stronger than the rival by turn 25.",
                VentureType.GroceryStore => $"You just opened {runName}.\nThe rival store competes for the same repeat neighborhood traffic.\nFreshness, shelf discipline, night service, local trust, and spoilage control\nwill decide who keeps the market.\n\nYour goal: end the run with the bigger market share.",
                _ => "You picked your first venture. Your rival starts in the same sector. Finish the run with the bigger market share."
            };
        }

        private static string GetAnchorMessage(VentureType ventureType, string categoryLabel)
        {
            return ventureType switch
            {
                VentureType.TechApp => $"On the left is your app HQ.\nThe sign carries your name and category: {categoryLabel ?? "Product"}.\nYour first core operation starts here, then expands into backend, growth,\nand support pressure as the app scales.",
                VentureType.FastFood => "On the left is your storefront anchor.\nThis is where your physical presence begins before kitchen, service,\nand delivery pressure expand around it.",
                VentureType.Cafe => "On the left is your cafe anchor.\nThis is the brand customers remember before seating, bar flow,\nand takeaway pressure expand the board.",
                VentureType.ClothingStore => "On the left is your storefront anchor.\nThis is where your brand starts before display, stock,\nand checkout pressure expand into a full retail board.",
                VentureType.GroceryStore => "On the left is your store anchor.\nThis is your neighborhood identity before shelves, fresh goods,\nand local delivery pressure start stretching the operation.",
                _ => "On the left is your venture anchor. This is where the board starts growing."
            };
        }

        private static string GetStrategyMessage(VentureType ventureType, string categoryLabel)
        {
            string ventureTip = ventureType switch
            {
                VentureType.TechApp => $"A {categoryLabel ?? "tech"} app dies when growth outruns stability.\nBackend, reviews, data trust, and churn all punish sloppy scale.",
                VentureType.FastFood => "A food chain dies when marketing outruns kitchen speed.\nCleaning, pest control, waiters, and meat quality protect your review loop.",
                VentureType.Cafe => "A cafe dies when quality drifts while traffic rises.\nConsistency, mood, and staff rhythm matter as much as reach.",
                VentureType.ClothingStore => "Retail dies when trend timing is wrong.\nWindow quality, stock turns, and returns punish bad inventory bets.",
                VentureType.GroceryStore => "Grocery dies on thin margins.\nSpoilage, freshness, shelf gaps, and local trust are the real fight.",
                _ => "Pressure comes from weak sequencing, not random punishment."
            };

            return "Tips to dominate:\n\n" +
                   "\u2022 Demand without capacity destroys rating\n" +
                   "\u2022 Supplier and staff quality create organic growth\n" +
                   "\u2022 Risk cards are short-term power, not free value\n" +
                   "\u2022 Watch the rival lane and answer the right pressure\n\n" +
                   ventureTip;
        }
    }
}
