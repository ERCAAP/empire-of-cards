using UnityEngine;
using EmpireOfCards.Core.TurnPhases;

namespace EmpireOfCards.Core
{
    public class TurnManager : MonoBehaviour
    {
        int currentTurn;
        TurnPhase currentPhase;
        ITurnPhase activePhaseHandler;
        bool turnInProgress;
        Era lastEra;
        SeasonType lastSeason;

        public int CurrentTurn => currentTurn;
        public TurnPhase CurrentPhase => currentPhase;
        public bool TurnInProgress => turnInProgress;

        // ── Turn lifecycle ──────────────────────────────────────────

        public void BeginTurn()
        {
            if (turnInProgress) return;

            currentTurn++;
            turnInProgress = true;

            CheckEraChange();
            CheckSeasonChange();

            EventBus.TurnStarted(currentTurn);

            TransitionTo(TurnPhase.DrawPhase);
        }

        public void CompleteCurrentPhase()
        {
            if (activePhaseHandler != null)
            {
                activePhaseHandler.Exit();
                EventBus.PhaseEnded(currentPhase);
            }

            TurnPhase? next = GetNextPhase(currentPhase);
            if (next.HasValue)
            {
                TransitionTo(next.Value);
            }
            else
            {
                FinishTurn();
            }
        }

        // ── Phase management ────────────────────────────────────────

        void TransitionTo(TurnPhase phase)
        {
            currentPhase = phase;
            activePhaseHandler = CreatePhaseHandler(phase);
            EventBus.PhaseStarted(phase);
            activePhaseHandler.Enter();
        }

        ITurnPhase CreatePhaseHandler(TurnPhase phase)
        {
            switch (phase)
            {
                case TurnPhase.DrawPhase:            return new DrawPhase(this);
                case TurnPhase.PlanningPhase:        return new PlanningPhase(this);
                case TurnPhase.PlayPhase:            return new PlayPhase(this);
                case TurnPhase.ResolvePhase:         return new ResolvePhase(this);
                case TurnPhase.CrisisReactionPhase:  return new CrisisReactionPhase(this);
                case TurnPhase.RivalPhase:           return new RivalPhase(this);
                case TurnPhase.MarketUpdatePhase:     return new MarketUpdatePhase(this);
                default:                             return new DrawPhase(this);
            }
        }

        TurnPhase? GetNextPhase(TurnPhase current)
        {
            switch (current)
            {
                case TurnPhase.DrawPhase:            return TurnPhase.PlanningPhase;
                case TurnPhase.PlanningPhase:        return TurnPhase.PlayPhase;
                case TurnPhase.PlayPhase:            return TurnPhase.ResolvePhase;
                case TurnPhase.ResolvePhase:         return TurnPhase.CrisisReactionPhase;
                case TurnPhase.CrisisReactionPhase:  return TurnPhase.RivalPhase;
                case TurnPhase.RivalPhase:           return TurnPhase.MarketUpdatePhase;
                case TurnPhase.MarketUpdatePhase:     return null;
                default:                             return null;
            }
        }

        // ── Turn finish ─────────────────────────────────────────────

        void FinishTurn()
        {
            turnInProgress = false;
            activePhaseHandler = null;

            EventBus.TurnEnded(currentTurn);

            if (currentTurn >= Constants.MAX_TURNS)
            {
                var gm = GameManager.Instance;
                if (gm != null)
                {
                    bool won = gm.Resources.GetMarketShare() >= Constants.WIN_MARKET_SHARE;
                    string reason = won ? "Market share target reached" : "Turn limit reached";
                    gm.EndRun(won, reason);
                }
                return;
            }

            BeginTurn();
        }

        // ── Era / Season change detection ───────────────────────────

        void CheckEraChange()
        {
            Era newEra = GameManager.GetEra(currentTurn);
            if (currentTurn > 1 && newEra != lastEra)
            {
                lastEra = newEra;
                EventBus.EraChanged(newEra);

                var gm = GameManager.Instance;
                if (gm != null)
                    gm.Resources.SetActionsPerTurn(GameManager.GetActionsForEra(newEra));
            }
            else if (currentTurn == 1)
            {
                lastEra = newEra;
            }
        }

        void CheckSeasonChange()
        {
            SeasonType newSeason = GameManager.GetSeason(currentTurn);
            if (currentTurn > 1 && newSeason != lastSeason)
            {
                lastSeason = newSeason;
                EventBus.SeasonChanged(newSeason);
            }
            else if (currentTurn == 1)
            {
                lastSeason = newSeason;
            }
        }

        // ── Update loop ─────────────────────────────────────────────

        void Update()
        {
            if (turnInProgress && activePhaseHandler != null)
                activePhaseHandler.Tick(Time.deltaTime);
        }
    }
}
