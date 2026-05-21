using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Gameplay;
using EmpireOfCards.Audio;
using EmpireOfCards.Save;
using EmpireOfCards.World;

namespace EmpireOfCards.Bootstrap
{
    public class ManagerBundle
    {
        // ── Core / Gameplay ─────────────────────────────────────────
        public GameManager gameManager;
        public TurnManager turnManager;
        public BoardManager boardManager;
        public SlotManager slotManager;
        public EconomyManager economyManager;
        public DeckManager deckManager;
        public RivalAI rivalAI;
        public CustomerSystem customerSystem;
        public StaffSystem staffSystem;
        public HygieneSystem hygieneSystem;
        public CrisisChainSystem crisisSystem;
        public EmpireOfCards.Audio.AudioManager audioManager;
        public EmpireOfCards.Save.SaveManager saveManager;

        // ── 3D World ────────────────────────────────────────────────
        public Board3D board3D;
        public Hand3D hand3D;
        public CardFactory3D cardFactory3D;
        public CustomerVisuals customerVisuals;
        public InputManager3D inputManager3D;
    }

    public static class ManagerFactory
    {
        public static ManagerBundle CreateAll()
        {
            var root = new GameObject("[Managers]");
            Object.DontDestroyOnLoad(root);

            // 3D World root (separate from manager root)
            var worldRoot = new GameObject("[World3D]");

            var bundle = new ManagerBundle
            {
                // Core / Gameplay
                gameManager    = Create<GameManager>(root),
                turnManager    = Create<TurnManager>(root),
                boardManager   = Create<BoardManager>(root),
                slotManager    = Create<SlotManager>(root),
                economyManager = Create<EconomyManager>(root),
                deckManager    = Create<DeckManager>(root),
                rivalAI        = Create<RivalAI>(root),
                customerSystem = Create<CustomerSystem>(root),
                staffSystem    = Create<StaffSystem>(root),
                hygieneSystem  = Create<HygieneSystem>(root),
                crisisSystem   = Create<CrisisChainSystem>(root),
                audioManager   = Create<AudioManager>(root),
                saveManager    = Create<SaveManager>(root),

                // 3D World
                board3D          = Create<Board3D>(worldRoot),
                hand3D           = Create<Hand3D>(worldRoot),
                cardFactory3D    = Create<CardFactory3D>(worldRoot),
                customerVisuals  = Create<CustomerVisuals>(worldRoot),
                inputManager3D   = Create<InputManager3D>(root)
            };

            return bundle;
        }

        static T Create<T>(GameObject parent) where T : MonoBehaviour
        {
            var go = new GameObject(typeof(T).Name);
            go.transform.SetParent(parent.transform);
            return go.AddComponent<T>();
        }
    }
}
