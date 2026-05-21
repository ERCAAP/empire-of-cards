using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Gameplay;
using EmpireOfCards.Audio;
using EmpireOfCards.Save;

namespace EmpireOfCards.Bootstrap
{
    public class ManagerBundle
    {
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
    }

    public static class ManagerFactory
    {
        public static ManagerBundle CreateAll()
        {
            var root = new GameObject("[Managers]");
            Object.DontDestroyOnLoad(root);

            var bundle = new ManagerBundle
            {
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
                saveManager    = Create<SaveManager>(root)
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
