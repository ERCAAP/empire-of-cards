using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay
{
    public class RivalAI : MonoBehaviour
    {
        EconomyManager _economy;

        float _rivalRating;
        float _rivalShare;
        int _rivalCash;
        RivalStrategy _currentStrategy;

        public float RivalRating => _rivalRating;
        public float RivalShare => _rivalShare;
        public int RivalCash => _rivalCash;
        public RivalStrategy CurrentStrategy => _currentStrategy;

        public void Init(EconomyManager economy)
        {
            _economy = economy;
            _rivalRating = Constants.RIVAL_STARTING_RATING;
            _rivalShare = Constants.RIVAL_STARTING_SHARE;
            _rivalCash = Constants.RIVAL_STARTING_CASH;
            _currentStrategy = RivalStrategy.Defensive;
            Debug.Log("[RivalAI] Initialized.");
        }

        // ── EventBus subscriptions ─────────────────────────────────

        void OnEnable()
        {
            EventBus.OnPhaseStarted += HandlePhaseStarted;
        }

        void OnDisable()
        {
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
        }

        void HandlePhaseStarted(TurnPhase phase)
        {
            if (phase != TurnPhase.RivalPhase) return;
            DecideStrategy();
            ExecuteTurn();
        }

        // ── Strategy Decision ──────────────────────────────────────

        public void DecideStrategy()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            var res = gm.Resources;

            float playerShare = res.GetMarketShare();
            float playerRating = res.GetRating();

            if (playerShare > _rivalShare + 10f)
            {
                _currentStrategy = RivalStrategy.AggressiveMarketing;
            }
            else if (playerRating > _rivalRating + 0.5f)
            {
                _currentStrategy = RivalStrategy.PremiumQuality;
            }
            else if (_rivalCash < 100)
            {
                _currentStrategy = RivalStrategy.Defensive;
            }
            else if (playerShare < _rivalShare - 10f)
            {
                _currentStrategy = PickOffensiveStrategy();
            }
            else
            {
                _currentStrategy = RivalStrategy.CheapExpansion;
            }
        }

        // ── Turn Execution ─────────────────────────────────────────

        public void ExecuteTurn()
        {
            int actions = GetActionCount();

            for (int i = 0; i < actions; i++)
            {
                RivalMove move = PickMove(_currentStrategy);
                ApplyAction(move);
            }
        }

        // ── Action Application ─────────────────────────────────────

        void ApplyAction(RivalMove move)
        {
            switch (move)
            {
                case RivalMove.PriceWar:
                    ApplyPriceWar();
                    break;
                case RivalMove.MarketingBlitz:
                    ApplyMarketingBlitz();
                    break;
                case RivalMove.QualityPush:
                    ApplyQualityPush();
                    break;
                case RivalMove.StaffPoach:
                    ApplyStaffPoach();
                    break;
                case RivalMove.Expand:
                    ApplyExpand();
                    break;
                case RivalMove.Stabilize:
                    ApplyStabilize();
                    break;
                case RivalMove.Sabotage:
                    ApplySabotage();
                    break;
            }
        }

        // ── Move Implementations ───────────────────────────────────

        void ApplyPriceWar()
        {
            _rivalShare += 1.5f;
            _rivalCash -= 20;
            _rivalRating = Mathf.Max(_rivalRating - 0.1f, Constants.RATING_MIN);

            var gm = GameManager.Instance;
            if (gm != null) gm.Resources.AdjustMarketShare(-1f);

            EventBus.RivalAction(RivalMove.PriceWar, "Rakip fiyat kirdi!");
        }

        void ApplyMarketingBlitz()
        {
            _rivalShare += 2f;
            _rivalCash -= 30;

            var gm = GameManager.Instance;
            if (gm != null) gm.Resources.AdjustMarketShare(-1.5f);

            EventBus.RivalAction(RivalMove.MarketingBlitz, "Rakip agresif marketing baslattti!");
        }

        void ApplyQualityPush()
        {
            _rivalRating = Mathf.Min(_rivalRating + 0.3f, Constants.RATING_MAX);
            _rivalCash -= 25;
            _rivalShare += 0.5f;

            EventBus.RivalAction(RivalMove.QualityPush, "Rakip kaliteyi arttirdi.");
        }

        void ApplyStaffPoach()
        {
            _rivalRating = Mathf.Min(_rivalRating + 0.2f, Constants.RATING_MAX);
            _rivalCash -= 30;

            var gm = GameManager.Instance;
            if (gm != null) gm.Resources.AdjustStaffStability(-0.5f);

            EventBus.RivalAction(RivalMove.StaffPoach, "Rakip calisaniniza teklif verdi!");
        }

        void ApplyExpand()
        {
            _rivalShare += 1f;
            _rivalCash -= 40;

            EventBus.RivalAction(RivalMove.Expand, "Rakip genisledi.");
        }

        void ApplyStabilize()
        {
            _rivalRating = Mathf.Min(_rivalRating + 0.1f, Constants.RATING_MAX);
            _rivalCash += 10;

            EventBus.RivalAction(RivalMove.Stabilize, "Rakip savunmaya gecti.");
        }

        void ApplySabotage()
        {
            _rivalCash -= 15;

            var gm = GameManager.Instance;
            if (gm != null)
            {
                gm.Resources.AdjustRating(-0.2f);
                gm.Resources.AdjustLegalRisk(3f);
            }

            EventBus.RivalAction(RivalMove.Sabotage, "Rakip kirli oynuyor!");
        }

        // ── Helpers ────────────────────────────────────────────────

        int GetActionCount()
        {
            if (_currentStrategy == RivalStrategy.AggressiveMarketing
                || _currentStrategy == RivalStrategy.DirtyPlay)
                return Constants.RIVAL_ACTIONS_AGGRESSIVE;
            return Constants.RIVAL_ACTIONS_BASE;
        }

        RivalMove PickMove(RivalStrategy strategy)
        {
            switch (strategy)
            {
                case RivalStrategy.AggressiveMarketing:
                    return Random.value > 0.5f
                        ? RivalMove.MarketingBlitz
                        : RivalMove.PriceWar;
                case RivalStrategy.PremiumQuality:
                    return RivalMove.QualityPush;
                case RivalStrategy.CheapExpansion:
                    return Random.value > 0.5f
                        ? RivalMove.Expand
                        : RivalMove.PriceWar;
                case RivalStrategy.Defensive:
                    return RivalMove.Stabilize;
                case RivalStrategy.DirtyPlay:
                    return Random.value > 0.5f
                        ? RivalMove.Sabotage
                        : RivalMove.StaffPoach;
                default:
                    return RivalMove.Stabilize;
            }
        }

        RivalStrategy PickOffensiveStrategy()
        {
            float roll = Random.value;
            if (roll < 0.2f) return RivalStrategy.DirtyPlay;
            if (roll < 0.5f) return RivalStrategy.AggressiveMarketing;
            return RivalStrategy.CheapExpansion;
        }
    }
}
