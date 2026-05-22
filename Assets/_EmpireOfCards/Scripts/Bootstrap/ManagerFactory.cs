using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Gameplay;
using EmpireOfCards.Gameplay.Staff;
using EmpireOfCards.UI;
using EmpireOfCards.Audio;
using EmpireOfCards.VFX;
using EmpireOfCards.Save;
using EmpireOfCards.World;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Creates all manager GameObjects and components at runtime.
    /// No Inspector assignments required.
    /// </summary>
    public static class ManagerFactory
    {
        /// <summary>
        /// Creates every manager the game needs and returns them bundled.
        /// </summary>
        public static ManagerBundle CreateAll()
        {
            var b = new ManagerBundle();

            // --- LocalizationManager (first, so translations are available) ---
            b.localizationManager = CreateRoot<LocalizationManager>("[LocalizationManager]");

            // --- GameManager (Singleton root) ---
            b.gameManager = CreateRoot<GameManager>("[GameManager]");
            var gmGo = b.gameManager.gameObject;

            // --- TurnManager ---
            b.turnManager = CreateChild<TurnManager>(gmGo.transform, "TurnManager");

            // --- EconomyManager ---
            b.economyManager = CreateChild<EconomyManager>(gmGo.transform, "EconomyManager");

            // --- DeckManager ---
            b.deckManager = CreateChild<DeckManager>(gmGo.transform, "DeckManager");

            // --- BoardManager ---
            b.boardManager = CreateChild<BoardManager>(gmGo.transform, "BoardManager");

            // --- MarketShareVisualizer (TerritoryManager compatibility component) ---
            b.marketShareVisualizer = CreateChild<TerritoryManager>(gmGo.transform, "MarketShareVisualizer");

            // --- RivalAI ---
            b.rivalAI = CreateChild<RivalAI>(gmGo.transform, "RivalAI");

            // --- ShopManager ---
            b.shopManager = CreateChild<ShopManager>(gmGo.transform, "ShopManager");

            // --- AudioManager ---
            var audioGo = CreateRoot<AudioManager>("[AudioManager]");
            b.audioManager = audioGo;
            var audioHost = audioGo.gameObject;
            b.musicSourceA = audioHost.AddComponent<AudioSource>();
            b.musicSourceA.loop = true;
            b.musicSourceA.playOnAwake = false;
            b.musicSourceB = audioHost.AddComponent<AudioSource>();
            b.musicSourceB.loop = true;
            b.musicSourceB.playOnAwake = false;
            b.sfxSource = audioHost.AddComponent<AudioSource>();
            b.sfxSource.playOnAwake = false;

            // --- VFXManager ---
            b.vfxManager = CreateRoot<VFXManager>("VFXManager");

            // --- SaveManager ---
            b.saveManager = CreateRoot<SaveManager>("[SaveManager]");

            // --- InputManager3D ---
            b.inputManager3D = CreateChild<InputManager3D>(gmGo.transform, "InputManager3D");

            // --- AbilitySystem ---
            b.abilitySystem = CreateChild<AbilitySystem>(gmGo.transform, "AbilitySystem");

            // --- LevelManager ---
            b.levelManager = CreateRoot<LevelManager>("[LevelManager]");

            // --- ActionCardResolver ---
            b.actionCardResolver = CreateChild<ActionCardResolver>(gmGo.transform, "ActionCardResolver");

            // --- SlotManager ---
            b.slotManager = CreateChild<SlotManager>(gmGo.transform, "SlotManager");

            // --- StaffStateSystem (GDD Section 6) ---
            b.staffStateSystem = CreateChild<StaffStateSystem>(gmGo.transform, "StaffStateSystem");

            // --- ChainReactionSystem (GDD Section 11, 12) ---
            b.chainReactionSystem = CreateChild<ChainReactionSystem>(gmGo.transform, "ChainReactionSystem");

            Debug.Log("[ManagerFactory] All managers created.");

            return b;
        }

        private static T CreateRoot<T>(string objectName) where T : Component
        {
            return new GameObject(objectName).AddComponent<T>();
        }

        private static T CreateChild<T>(Transform parent, string objectName) where T : Component
        {
            var go = new GameObject(objectName);
            go.transform.SetParent(parent);
            return go.AddComponent<T>();
        }
    }

    /// <summary>
    /// Holds references to every manager created at runtime.
    /// </summary>
    public class ManagerBundle
    {
        public LocalizationManager localizationManager;
        public GameManager gameManager;
        public TurnManager turnManager;
        public EconomyManager economyManager;
        public DeckManager deckManager;
        public BoardManager boardManager;
        public MarketShareVisualizer marketShareVisualizer;
        public RivalAI rivalAI;
        public ShopManager shopManager;
        public UIManager uiManager; // Set later by HUDBuilder
        public AudioManager audioManager;
        public VFXManager vfxManager;
        public SaveManager saveManager;
        public InputManager3D inputManager3D;
        public AbilitySystem abilitySystem;
        public LevelManager levelManager;
        public ActionCardResolver actionCardResolver;
        public SlotManager slotManager;
        public StaffStateSystem staffStateSystem;
        public ChainReactionSystem chainReactionSystem;

        // AudioManager sub-sources
        public AudioSource musicSourceA;
        public AudioSource musicSourceB;
        public AudioSource sfxSource;
    }
}
