using System;
using System.Collections.Generic;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay.Venture
{
    public interface IVentureRuntime
    {
        VentureData Venture { get; }
        VenturePlaybook Playbook { get; }
        VentureBoardThemeProfile BoardTheme { get; }
        TurnScriptBeat GetBeatForTurn(int turnNumber);
        string[] GetPriorityCardIdsForTurn(int turnNumber);
        string[] GetPoolCardIds(int turnNumber, BoardPressureType pressure);
        CardData ResolveScriptedEvent(int turnNumber, BoardPressureType pressure);
        void OnTurnStarted(int turnNumber);
        void RegisterCardPlayed(CardData card, SlotType slotType);
        void RegisterEventFired(CardData card, int turnNumber);
        VentureRuntimeState CaptureRuntimeState();
        OpeningArcState CaptureOpeningArcState();
        EventChainState CaptureEventChainState();
        void RestoreState(VentureRuntimeState runtimeState, OpeningArcState openingArcState, EventChainState eventChainState);
    }

    public sealed class PlaybookVentureRuntime : IVentureRuntime
    {
        private readonly Dictionary<string, CardData> _lookup;
        private VentureRuntimeState _runtimeState = new VentureRuntimeState();
        private OpeningArcState _openingArcState = new OpeningArcState();
        private EventChainState _eventChainState = new EventChainState();

        public PlaybookVentureRuntime(VentureData venture, IReadOnlyDictionary<string, CardData> lookup)
        {
            Venture = venture;
            Playbook = venture != null ? venture.playbook : null;
            BoardTheme = venture != null ? venture.themeProfile : null;
            _lookup = new Dictionary<string, CardData>();

            if (lookup != null)
            {
                foreach (var item in lookup)
                    _lookup[item.Key] = item.Value;
            }
        }

        public VentureData Venture { get; }
        public VenturePlaybook Playbook { get; }
        public VentureBoardThemeProfile BoardTheme { get; }

        public TurnScriptBeat GetBeatForTurn(int turnNumber)
        {
            var beats = Playbook != null && Playbook.progressionArc != null
                ? Playbook.progressionArc.openingBeats
                : null;
            if (beats == null)
                return null;

            for (int i = 0; i < beats.Length; i++)
            {
                if (beats[i] != null && beats[i].turnNumber == turnNumber)
                    return beats[i];
            }

            return null;
        }

        public string[] GetPriorityCardIdsForTurn(int turnNumber)
        {
            TurnScriptBeat beat = GetBeatForTurn(turnNumber);
            if (beat != null && beat.priorityCardIds != null && beat.priorityCardIds.Length > 0)
                return beat.priorityCardIds;

            if (Playbook != null && Playbook.openingSequenceCardIds != null && Playbook.openingSequenceCardIds.Length > 0)
                return Playbook.openingSequenceCardIds;

            return Array.Empty<string>();
        }

        public string[] GetPoolCardIds(int turnNumber, BoardPressureType pressure)
        {
            var arc = Playbook != null ? Playbook.progressionArc : null;
            if (arc == null)
                return Array.Empty<string>();

            if (IsRecoveryPressure(pressure) && arc.recoverPoolCardIds != null && arc.recoverPoolCardIds.Length > 0)
                return arc.recoverPoolCardIds;

            if (IsCrisisPressure(pressure) && arc.crisisPoolCardIds != null && arc.crisisPoolCardIds.Length > 0)
                return arc.crisisPoolCardIds;

            if (turnNumber <= 2 && arc.openingPoolCardIds != null && arc.openingPoolCardIds.Length > 0)
                return arc.openingPoolCardIds;
            if (turnNumber <= 5 && arc.stabilizePoolCardIds != null && arc.stabilizePoolCardIds.Length > 0)
                return arc.stabilizePoolCardIds;
            if (turnNumber <= 8 && arc.scalePoolCardIds != null && arc.scalePoolCardIds.Length > 0)
                return arc.scalePoolCardIds;
            if (arc.latePoolCardIds != null && arc.latePoolCardIds.Length > 0)
                return arc.latePoolCardIds;

            return Array.Empty<string>();
        }

        public CardData ResolveScriptedEvent(int turnNumber, BoardPressureType pressure)
        {
            var chain = Playbook != null ? Playbook.eventChain : null;
            if (chain == null || chain.windows == null)
                return null;

            for (int i = 0; i < chain.windows.Length; i++)
            {
                VentureEventWindow window = chain.windows[i];
                if (window == null || string.IsNullOrWhiteSpace(window.windowId))
                    continue;
                if (_eventChainState.firedWindowIds.Contains(window.windowId))
                    continue;
                if (turnNumber < window.turnStart || turnNumber > window.turnEnd)
                    continue;
                if (window.requiredPressure != BoardPressureType.None && window.requiredPressure != pressure)
                    continue;
                if (window.preferredEventCardIds == null)
                    continue;

                for (int c = 0; c < window.preferredEventCardIds.Length; c++)
                {
                    string cardId = window.preferredEventCardIds[c];
                    if (!string.IsNullOrWhiteSpace(cardId) && _lookup.TryGetValue(cardId, out CardData card) && card != null)
                        return card;
                }
            }

            return null;
        }

        public void OnTurnStarted(int turnNumber)
        {
            _runtimeState.currentTurn = turnNumber;
            _runtimeState.buildStage = ResolveBuildStage(turnNumber);

            TurnScriptBeat beat = GetBeatForTurn(turnNumber);
            if (beat == null)
                return;

            _openingArcState.currentBeatTurn = turnNumber;
            _openingArcState.currentBeatId = beat.beatId;
            AddUnique(_runtimeState.seenBeatIds, beat.beatId);
        }

        public void RegisterCardPlayed(CardData card, SlotType slotType)
        {
            if (card == null)
                return;

            TurnScriptBeat beat = GetBeatForTurn(_runtimeState.currentTurn);
            if (beat != null && beat.priorityCardIds != null)
            {
                for (int i = 0; i < beat.priorityCardIds.Length; i++)
                {
                    if (beat.priorityCardIds[i] != card.cardId)
                        continue;

                    AddUnique(_openingArcState.completedBeatIds, beat.beatId);
                    if (beat.highlightPropIds != null)
                    {
                        for (int p = 0; p < beat.highlightPropIds.Length; p++)
                            AddUnique(_runtimeState.unlockedPropIds, beat.highlightPropIds[p]);
                    }
                    break;
                }
            }
        }

        public void RegisterEventFired(CardData card, int turnNumber)
        {
            _eventChainState.lastEventCardId = card != null ? card.cardId : null;

            var chain = Playbook != null ? Playbook.eventChain : null;
            if (chain == null || chain.windows == null)
                return;

            for (int i = 0; i < chain.windows.Length; i++)
            {
                VentureEventWindow window = chain.windows[i];
                if (window == null || string.IsNullOrWhiteSpace(window.windowId))
                    continue;
                if (turnNumber < window.turnStart || turnNumber > window.turnEnd)
                    continue;
                if (card == null || window.preferredEventCardIds == null || Array.IndexOf(window.preferredEventCardIds, card.cardId) < 0)
                    continue;
                AddUnique(_eventChainState.firedWindowIds, window.windowId);
            }
        }

        public VentureRuntimeState CaptureRuntimeState()
        {
            return new VentureRuntimeState
            {
                currentTurn = _runtimeState.currentTurn,
                buildStage = _runtimeState.buildStage,
                seenBeatIds = new List<string>(_runtimeState.seenBeatIds),
                unlockedPropIds = new List<string>(_runtimeState.unlockedPropIds)
            };
        }

        public OpeningArcState CaptureOpeningArcState()
        {
            return new OpeningArcState
            {
                currentBeatTurn = _openingArcState.currentBeatTurn,
                currentBeatId = _openingArcState.currentBeatId,
                completedBeatIds = new List<string>(_openingArcState.completedBeatIds)
            };
        }

        public EventChainState CaptureEventChainState()
        {
            return new EventChainState
            {
                firedWindowIds = new List<string>(_eventChainState.firedWindowIds),
                lastEventCardId = _eventChainState.lastEventCardId
            };
        }

        public void RestoreState(VentureRuntimeState runtimeState, OpeningArcState openingArcState, EventChainState eventChainState)
        {
            _runtimeState = runtimeState ?? new VentureRuntimeState();
            _openingArcState = openingArcState ?? new OpeningArcState();
            _eventChainState = eventChainState ?? new EventChainState();
        }

        private static bool IsCrisisPressure(BoardPressureType pressure)
        {
            return pressure == BoardPressureType.CapacityShortfall
                || pressure == BoardPressureType.HighLegalRisk
                || pressure == BoardPressureType.StaffInstability;
        }

        private static bool IsRecoveryPressure(BoardPressureType pressure)
        {
            return pressure == BoardPressureType.LowRating
                || pressure == BoardPressureType.WeakQuality
                || pressure == BoardPressureType.LowCash;
        }

        private static string ResolveBuildStage(int turnNumber)
        {
            if (turnNumber <= 2)
                return "opening";
            if (turnNumber <= 5)
                return "stabilize";
            if (turnNumber <= 8)
                return "scale";
            if (turnNumber <= 16)
                return "midgame";
            return "late";
        }

        private static void AddUnique(List<string> list, string value)
        {
            if (list == null || string.IsNullOrWhiteSpace(value) || list.Contains(value))
                return;

            list.Add(value);
        }
    }

    public static class VentureRuntimeFactory
    {
        public static IVentureRuntime Create(VentureData venture, IReadOnlyDictionary<string, CardData> lookup)
        {
            if (venture == null || venture.playbook == null)
                return null;

            return new PlaybookVentureRuntime(venture, lookup);
        }
    }
}
