using System;
using System.Collections;
using UnityEngine;

namespace EmpireOfCards.Core
{
    public class TurnManager : MonoBehaviour
    {
        [Header("Phase Timing")]
        [SerializeField] private float eventPhaseDuration = 1.5f;
        [SerializeField] private float drawPhaseDuration = 1.0f;
        [SerializeField] private float resolvePhaseDuration = 1.5f;
        [SerializeField] private float rivalPhaseDuration = 2.0f;

        [Header("State")]
        [SerializeField] private TurnPhase currentPhase;

        private bool playerEndedPlayPhase;
        private Coroutine turnCoroutine;

        // --- Events ---
        public event Action<TurnPhase> OnPhaseStarted;
        public event Action<TurnPhase> OnPhaseEnded;

        // --- Properties ---
        public TurnPhase CurrentPhase => currentPhase;

        /// <summary>
        /// Begins a new turn by kicking off the phase sequence coroutine.
        /// </summary>
        public void StartTurn()
        {
            if (turnCoroutine != null)
            {
                StopCoroutine(turnCoroutine);
            }

            GameManager gm = GameManager.Instance;
            if (gm == null)
                return;

            gm.ResetActions();
            playerEndedPlayPhase = false;
            turnCoroutine = StartCoroutine(TurnSequence());
        }

        /// <summary>
        /// Called by the End Turn button during the Play Phase.
        /// </summary>
        public void EndPlayPhase()
        {
            playerEndedPlayPhase = true;
        }

        /// <summary>
        /// Ends the current turn, advances the counter, checks win/lose, then starts the next turn.
        /// </summary>
        public void EndTurn()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null)
                return;

            gm.AdvanceTurn();

            if (gm.CheckWinCondition() || gm.CheckLoseCondition())
                return;

            if (gm.CurrentTurn > gm.MaxTurns)
            {
                gm.EndRun(false);
                return;
            }

            StartTurn();
        }

        // ----------------------------------------------------------------
        // Phase Coroutines
        // ----------------------------------------------------------------

        private IEnumerator TurnSequence()
        {
            yield return StartCoroutine(ExecuteEventPhase());
            yield return StartCoroutine(ExecuteDrawPhase());
            yield return StartCoroutine(ExecutePlayPhase());
            yield return StartCoroutine(ExecuteResolvePhase());
            yield return StartCoroutine(ExecuteRivalPhase());

            SetPhase(TurnPhase.TurnEnd);
            OnPhaseStarted?.Invoke(TurnPhase.TurnEnd);

            EndTurn();

            OnPhaseEnded?.Invoke(TurnPhase.TurnEnd);
        }

        private IEnumerator ExecuteEventPhase()
        {
            SetPhase(TurnPhase.EventPhase);
            OnPhaseStarted?.Invoke(TurnPhase.EventPhase);

            // TODO: Draw and resolve a random event card
            yield return new WaitForSeconds(eventPhaseDuration);

            OnPhaseEnded?.Invoke(TurnPhase.EventPhase);
        }

        private IEnumerator ExecuteDrawPhase()
        {
            SetPhase(TurnPhase.DrawPhase);
            OnPhaseStarted?.Invoke(TurnPhase.DrawPhase);

            // TODO: Draw cards up to hand size via DeckManager
            yield return new WaitForSeconds(drawPhaseDuration);

            OnPhaseEnded?.Invoke(TurnPhase.DrawPhase);
        }

        private IEnumerator ExecutePlayPhase()
        {
            SetPhase(TurnPhase.PlayPhase);
            OnPhaseStarted?.Invoke(TurnPhase.PlayPhase);

            // Wait until the player presses the End Turn button
            playerEndedPlayPhase = false;

            while (!playerEndedPlayPhase)
            {
                yield return null;
            }

            OnPhaseEnded?.Invoke(TurnPhase.PlayPhase);
        }

        private IEnumerator ExecuteResolvePhase()
        {
            SetPhase(TurnPhase.ResolvePhase);
            OnPhaseStarted?.Invoke(TurnPhase.ResolvePhase);

            // TODO: Calculate income, apply effects, resolve combos
            yield return new WaitForSeconds(resolvePhaseDuration);

            OnPhaseEnded?.Invoke(TurnPhase.ResolvePhase);
        }

        private IEnumerator ExecuteRivalPhase()
        {
            SetPhase(TurnPhase.RivalPhase);
            OnPhaseStarted?.Invoke(TurnPhase.RivalPhase);

            // TODO: Execute rival AI turn via RivalAI
            yield return new WaitForSeconds(rivalPhaseDuration);

            OnPhaseEnded?.Invoke(TurnPhase.RivalPhase);
        }

        private void SetPhase(TurnPhase phase)
        {
            currentPhase = phase;
        }
    }
}
