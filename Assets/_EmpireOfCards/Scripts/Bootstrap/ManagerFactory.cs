using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Gameplay;
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
            var locGo = new GameObject("[LocalizationManager]");
            b.localizationManager = locGo.AddComponent<LocalizationManager>();

            // --- GameManager (Singleton root) ---
            var gmGo = new GameObject("[GameManager]");
            b.gameManager = gmGo.AddComponent<GameManager>();

            // --- TurnManager ---
            var tmGo = new GameObject("TurnManager");
            tmGo.transform.SetParent(gmGo.transform);
            b.turnManager = tmGo.AddComponent<TurnManager>();

            // --- EconomyManager ---
            var emGo = new GameObject("EconomyManager");
            emGo.transform.SetParent(gmGo.transform);
            b.economyManager = emGo.AddComponent<EconomyManager>();

            // --- DeckManager ---
            var dmGo = new GameObject("DeckManager");
            dmGo.transform.SetParent(gmGo.transform);
            b.deckManager = dmGo.AddComponent<DeckManager>();

            // --- BoardManager ---
            var bmGo = new GameObject("BoardManager");
            bmGo.transform.SetParent(gmGo.transform);
            b.boardManager = bmGo.AddComponent<BoardManager>();

            // --- ComboSystem ---
            var csGo = new GameObject("ComboSystem");
            csGo.transform.SetParent(gmGo.transform);
            b.comboSystem = csGo.AddComponent<ComboSystem>();

            // --- TerritoryManager ---
            var terGo = new GameObject("TerritoryManager");
            terGo.transform.SetParent(gmGo.transform);
            b.territoryManager = terGo.AddComponent<TerritoryManager>();

            // --- FBISystem ---
            var fbiGo = new GameObject("FBISystem");
            fbiGo.transform.SetParent(gmGo.transform);
            b.fbiSystem = fbiGo.AddComponent<FBISystem>();

            // --- RivalAI ---
            var aiGo = new GameObject("RivalAI");
            aiGo.transform.SetParent(gmGo.transform);
            b.rivalAI = aiGo.AddComponent<RivalAI>();

            // --- ShopManager ---
            var shopGo = new GameObject("ShopManager");
            shopGo.transform.SetParent(gmGo.transform);
            b.shopManager = shopGo.AddComponent<ShopManager>();

            // --- AudioManager ---
            var audioGo = new GameObject("[AudioManager]");
            b.audioManager = audioGo.AddComponent<AudioManager>();
            b.musicSourceA = audioGo.AddComponent<AudioSource>();
            b.musicSourceA.loop = true;
            b.musicSourceA.playOnAwake = false;
            b.musicSourceB = audioGo.AddComponent<AudioSource>();
            b.musicSourceB.loop = true;
            b.musicSourceB.playOnAwake = false;
            b.sfxSource = audioGo.AddComponent<AudioSource>();
            b.sfxSource.playOnAwake = false;

            // --- VFXManager ---
            var vfxGo = new GameObject("VFXManager");
            b.vfxManager = vfxGo.AddComponent<VFXManager>();

            // --- SaveManager ---
            var saveGo = new GameObject("[SaveManager]");
            b.saveManager = saveGo.AddComponent<SaveManager>();

            // --- InputManager3D ---
            var inputGo = new GameObject("InputManager3D");
            inputGo.transform.SetParent(gmGo.transform);
            b.inputManager3D = inputGo.AddComponent<InputManager3D>();

            // --- AbilitySystem ---
            var abGo = new GameObject("AbilitySystem");
            abGo.transform.SetParent(gmGo.transform);
            b.abilitySystem = abGo.AddComponent<AbilitySystem>();

            // --- LevelManager ---
            var lvlGo = new GameObject("[LevelManager]");
            b.levelManager = lvlGo.AddComponent<LevelManager>();

            // --- MetaProgressionSystem ---
            var metaGo = new GameObject("MetaProgressionSystem");
            metaGo.transform.SetParent(gmGo.transform);
            b.metaProgressionSystem = metaGo.AddComponent<MetaProgressionSystem>();

            // --- ActionCardResolver ---
            var acrGo = new GameObject("ActionCardResolver");
            acrGo.transform.SetParent(gmGo.transform);
            b.actionCardResolver = acrGo.AddComponent<ActionCardResolver>();

            Debug.Log("[ManagerFactory] All managers created.");

            return b;
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
        public ComboSystem comboSystem;
        public TerritoryManager territoryManager;
        public FBISystem fbiSystem;
        public RivalAI rivalAI;
        public ShopManager shopManager;
        public UIManager uiManager; // Set later by HUDBuilder
        public AudioManager audioManager;
        public VFXManager vfxManager;
        public SaveManager saveManager;
        public InputManager3D inputManager3D;
        public AbilitySystem abilitySystem;
        public LevelManager levelManager;
        public MetaProgressionSystem metaProgressionSystem;
        public ActionCardResolver actionCardResolver;

        // AudioManager sub-sources
        public AudioSource musicSourceA;
        public AudioSource musicSourceB;
        public AudioSource sfxSource;
    }
}
