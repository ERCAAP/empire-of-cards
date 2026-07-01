using UnityEngine;

namespace EmpireOfCards.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // ── Resources ───────────────────────────────────────────────
        [SerializeField] PlayerResources resources = new PlayerResources();
        public PlayerResources Resources => resources;

        // ── Manager references (set by WiringService) ───────────────
        public TurnManager TurnManager { get; set; }

        // Gameplay managers are typed as MonoBehaviour so Core does not
        // depend on Gameplay namespace.  WiringService casts at wire time.
        public MonoBehaviour BoardManager { get; set; }
        public MonoBehaviour EconomyManager { get; set; }
        public MonoBehaviour DeckManager { get; set; }
        public MonoBehaviour SlotManager { get; set; }
        public MonoBehaviour RivalAI { get; set; }

        // ── Run state ───────────────────────────────────────────────
        public string BusinessName { get; private set; }
        public SectorType SelectedSector { get; private set; }
        public bool IsRunning { get; private set; }

        public int CurrentTurn => TurnManager != null ? TurnManager.CurrentTurn : 0;
        public Era CurrentEra => GetEra(CurrentTurn);
        public SeasonType CurrentSeason => GetSeason(CurrentTurn);

        // ── Lifecycle ───────────────────────────────────────────────

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void OnDestroy()
        {
            EventBus.ClearAll();
            if (Instance == this) Instance = null;
        }

        // ── Run control ─────────────────────────────────────────────

        public void StartNewRun(string businessName, SectorType sector)
        {
            BusinessName = businessName;
            SelectedSector = sector;

            resources.Initialize();
            resources.SetActionsPerTurn(GetActionsForEra(Era.Garage));

            IsRunning = true;

            if (TurnManager != null)
                TurnManager.BeginTurn();
        }

        public void EndRun(bool won, string reason)
        {
            IsRunning = false;
            EventBus.GameOver(won, reason);
        }

        // ── Static helpers ──────────────────────────────────────────

        public static Era GetEra(int turn)
        {
            if (turn <= Constants.ERA_1_END) return Era.Garage;
            if (turn <= Constants.ERA_2_END) return Era.Growth;
            if (turn <= Constants.ERA_3_END) return Era.Scale;
            return Era.Dominance;
        }

        public static SeasonType GetSeason(int turn)
        {
            if (turn <= 0) return SeasonType.Spring;
            int index = ((turn - 1) / Constants.TURNS_PER_SEASON) % 5;
            switch (index)
            {
                case 0: return SeasonType.Spring;
                case 1: return SeasonType.Summer;
                case 2: return SeasonType.Autumn;
                case 3: return SeasonType.Winter;
                case 4: return SeasonType.Ramadan;
                default: return SeasonType.Spring;
            }
        }

        public static int GetActionsForEra(Era era)
        {
            switch (era)
            {
                case Era.Garage:    return Constants.ACTIONS_ERA_1;
                case Era.Growth:    return Constants.ACTIONS_ERA_2;
                case Era.Scale:     return Constants.ACTIONS_ERA_3;
                case Era.Dominance: return Constants.ACTIONS_ERA_4;
                default:            return Constants.ACTIONS_ERA_1;
            }
        }

        public static int[] GetSlotsForEra(Era era)
        {
            switch (era)
            {
                case Era.Garage:    return Constants.SLOTS_ERA_1;
                case Era.Growth:    return Constants.SLOTS_ERA_2;
                case Era.Scale:     return Constants.SLOTS_ERA_3;
                case Era.Dominance: return Constants.SLOTS_ERA_4;
                default:            return Constants.SLOTS_ERA_1;
            }
        }

        public static float GetSeasonMultiplier(SeasonType season)
        {
            switch (season)
            {
                case SeasonType.Spring:  return Constants.SEASON_SPRING;
                case SeasonType.Summer:  return Constants.SEASON_SUMMER;
                case SeasonType.Autumn:  return Constants.SEASON_AUTUMN;
                case SeasonType.Winter:  return Constants.SEASON_WINTER;
                case SeasonType.Ramadan: return Constants.SEASON_RAMADAN;
                default:                 return 1f;
            }
        }
    }
}
