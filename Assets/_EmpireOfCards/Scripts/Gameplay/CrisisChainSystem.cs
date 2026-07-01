using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public class CrisisChainSystem : MonoBehaviour
    {
        BoardManager _board;
        EconomyManager _economy;

        CardData[] _crisisPool;
        bool _crisisThisTurn;

        public void Init(BoardManager board, EconomyManager economy)
        {
            _board = board;
            _economy = economy;
            Debug.Log("[CrisisChainSystem] Initialized.");
        }

        public void SetCrisisPool(CardData[] pool)
        {
            _crisisPool = pool;
        }

        // ── EventBus subscriptions ─────────────────────────────────

        void OnEnable()
        {
            EventBus.OnPhaseStarted += HandlePhaseStarted;
            EventBus.OnTurnStarted += HandleTurnStarted;
        }

        void OnDisable()
        {
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
            EventBus.OnTurnStarted -= HandleTurnStarted;
        }

        void HandleTurnStarted(int turn)
        {
            _crisisThisTurn = false;
        }

        void HandlePhaseStarted(TurnPhase phase)
        {
            if (phase != TurnPhase.CrisisReactionPhase) return;
            if (_crisisThisTurn) return;

            EvaluateCrisis();
        }

        // ── Evaluate ───────────────────────────────────────────────

        public void EvaluateCrisis()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            var res = gm.Resources;

            CrisisType triggered = CheckConditions(res);

            if (triggered == CrisisType.None)
            {
                EventBus.CrisisTriggered(CrisisType.None);
                return;
            }

            _crisisThisTurn = true;
            EventBus.CrisisTriggered(triggered);
            Debug.Log($"[CrisisChain] Crisis triggered: {triggered}");
        }

        // ── Condition Checks ───────────────────────────────────────

        CrisisType CheckConditions(PlayerResources res)
        {
            if (CheckFoodPoisoning(res)) return CrisisType.FoodPoisoning;
            if (CheckHygieneInspection(res)) return CrisisType.HygieneInspection;
            if (CheckReviewBomb(res)) return CrisisType.ReviewBomb;
            if (CheckStaffQuit(res)) return CrisisType.StaffQuit;
            if (CheckSupplyShortage(res)) return CrisisType.SupplyShortage;
            if (CheckRentIncrease()) return CrisisType.RentIncrease;
            return CrisisType.None;
        }

        bool CheckReviewBomb(PlayerResources res)
        {
            return res.GetRating() < Constants.CRISIS_RATING_THRESHOLD;
        }

        bool CheckHygieneInspection(PlayerResources res)
        {
            if (res.GetHygiene() >= Constants.CRISIS_HYGIENE_THRESHOLD
                && res.GetLegalRisk() < Constants.CRISIS_LEGAL_INSPECTION_THRESHOLD)
                return false;

            float chance = 0f;
            if (res.GetHygiene() < Constants.CRISIS_HYGIENE_THRESHOLD)
                chance += Constants.HYGIENE_INSPECTION_CHANCE;
            if (res.GetLegalRisk() >= Constants.CRISIS_LEGAL_INSPECTION_THRESHOLD)
                chance += 0.15f;

            return Random.value < chance;
        }

        bool CheckStaffQuit(PlayerResources res)
        {
            return res.GetStaffStability() < Constants.CRISIS_STABILITY_THRESHOLD;
        }

        bool CheckFoodPoisoning(PlayerResources res)
        {
            return res.GetQuality() < Constants.CRISIS_QUALITY_THRESHOLD
                   && res.GetHygiene() < Constants.CRISIS_QUALITY_HYGIENE_THRESHOLD;
        }

        bool CheckSupplyShortage(PlayerResources res)
        {
            if (_board == null) return false;
            var suppliers = _board.GetCardsInSlot(SlotType.Storage);
            return suppliers.Count == 0 && res.GetQuality() < 4f;
        }

        bool CheckRentIncrease()
        {
            var gm = GameManager.Instance;
            if (gm == null) return false;
            return gm.CurrentTurn == 10;
        }

        // ── Crisis Card Lookup ─────────────────────────────────────

        public CardData GetCrisisCard(CrisisType type)
        {
            if (_crisisPool == null) return null;

            foreach (var card in _crisisPool)
                if (card.crisisType == type)
                    return card;

            return null;
        }
    }
}
