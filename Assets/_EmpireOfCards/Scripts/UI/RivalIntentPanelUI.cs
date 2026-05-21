using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;
using EmpireOfCards.Presentation;

namespace EmpireOfCards.UI
{
    public class RivalIntentPanelUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text currentText;
        [SerializeField] private TMP_Text queueText;
        [SerializeField] private TMP_Text footerText;

        private readonly List<RivalQueuedAction> _plannedActions = new List<RivalQueuedAction>();
        private readonly List<RivalQueuedAction> _revealedActions = new List<RivalQueuedAction>();
        private bool _isRivalPhaseActive;

        public void Init(
            TMP_Text title,
            TMP_Text status,
            TMP_Text current,
            TMP_Text queue,
            TMP_Text footer)
        {
            titleText = title;
            statusText = status;
            currentText = current;
            queueText = queue;
            footerText = footer;
        }

        public void BeginPhase(GameManager gm)
        {
            _isRivalPhaseActive = true;
            _revealedActions.Clear();
            SyncPlannedActions(gm);
            Refresh(gm, null);
        }

        public void RevealAction(GameManager gm, RivalQueuedAction action)
        {
            _isRivalPhaseActive = true;
            SyncPlannedActions(gm);

            if (action != null && !Contains(_revealedActions, action))
                _revealedActions.Add(action);

            Refresh(gm, action);
        }

        public void SetIdle(GameManager gm)
        {
            _isRivalPhaseActive = false;
            Refresh(gm, null);
        }

        public void RefreshSnapshot(GameManager gm)
        {
            Refresh(gm, null);
        }

        private void Refresh(GameManager gm, RivalQueuedAction liveAction)
        {
            if (titleText != null)
                titleText.text = "RIVAL INTENT";

            RivalAI rival = gm != null ? gm.RivalAI : null;
            if (rival == null)
                return;

            if (statusText != null)
                statusText.text = _isRivalPhaseActive ? "LIVE  ·  RIVAL TURN" : "WAITING  ·  NEXT WINDOW";

            if (currentText != null)
            {
                if (_isRivalPhaseActive)
                {
                    RivalQueuedAction active = liveAction ?? GetNextPendingAction();
                    if (active != null)
                    {
                        currentText.text = $"{active.displayName.ToUpperInvariant()}\n{active.laneLabel.ToUpperInvariant()}  ·  {active.shortDescription.ToUpperInvariant()}";
                    }
                    else
                    {
                        currentText.text = "RIVAL IS RESOLVING\nWATCH THE RED LANES";
                    }
                }
                else
                {
                    string lastCard = string.IsNullOrWhiteSpace(rival.LastPlayedCardName) ? "WAITING" : rival.LastPlayedCardName.ToUpperInvariant();
                    string lane = string.IsNullOrWhiteSpace(rival.LastLaneLabel) ? "PRESSURE LANE" : rival.LastLaneLabel.ToUpperInvariant();
                    string style = string.IsNullOrWhiteSpace(rival.LastPressureStyle) ? "BALANCED" : rival.LastPressureStyle.ToUpperInvariant();
                    currentText.text = $"{lastCard}\n{lane}  ·  {style}";
                }
            }

            if (queueText != null)
            {
                if (_isRivalPhaseActive)
                {
                    string played = BuildActionRow("PLAYED", _revealedActions, 2);
                    string next = BuildPendingRow();
                    queueText.text = string.IsNullOrWhiteSpace(played)
                        ? next
                        : string.IsNullOrWhiteSpace(next) ? played : $"{played}\n{next}";
                }
                else
                {
                    string lastSequence = BuildActionRow("LAST SEQ", _revealedActions, 3);
                    queueText.text = string.IsNullOrWhiteSpace(lastSequence)
                        ? "WATCH THE RIVAL BAND FOR LIVE CARDS."
                        : lastSequence;
                }
            }

            if (footerText != null)
            {
                float playerShare = gm.EconomyManager != null && gm.EconomyManager.Snapshot != null
                    ? gm.EconomyManager.Snapshot.marketShare
                    : gm.PlayerCustomers;
                float rivalShare = Mathf.Clamp(100f - playerShare, 0f, 100f);
                footerText.text = $"SHARE {rivalShare:0.0}%  ·  INCOME ${rival.RivalIncome:N0}  ·  CUST {rival.RivalCustomers}  ·  THREAT {rival.RivalPressure:0.0}";
            }
        }

        private void SyncPlannedActions(GameManager gm)
        {
            _plannedActions.Clear();
            RivalAI rival = gm != null ? gm.RivalAI : null;
            if (rival == null || rival.QueuedActions == null)
                return;

            for (int i = 0; i < rival.QueuedActions.Count; i++)
            {
                if (rival.QueuedActions[i] != null)
                    _plannedActions.Add(rival.QueuedActions[i]);
            }
        }

        private RivalQueuedAction GetNextPendingAction()
        {
            for (int i = 0; i < _plannedActions.Count; i++)
            {
                if (!Contains(_revealedActions, _plannedActions[i]))
                    return _plannedActions[i];
            }

            return null;
        }

        private string BuildPendingRow()
        {
            var pending = new List<RivalQueuedAction>();
            for (int i = 0; i < _plannedActions.Count; i++)
            {
                if (!Contains(_revealedActions, _plannedActions[i]))
                    pending.Add(_plannedActions[i]);
            }

            return BuildActionRow("UP NEXT", pending, 2);
        }

        private static string BuildActionRow(string label, List<RivalQueuedAction> actions, int limit)
        {
            if (actions == null || actions.Count == 0)
                return string.Empty;

            string accent = ColorUtility.ToHtmlStringRGB(ControlDeskTheme.AccentAmber);
            var sb = new StringBuilder();
            sb.Append($"<color=#{accent}>{label}</color>  ");

            int count = Mathf.Min(limit, actions.Count);
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    sb.Append("  /  ");

                sb.Append(actions[i].displayName.ToUpperInvariant());
            }

            return sb.ToString();
        }

        private static bool Contains(List<RivalQueuedAction> actions, RivalQueuedAction candidate)
        {
            if (actions == null || candidate == null)
                return false;

            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i] == null)
                    continue;

                if (actions[i] == candidate)
                    return true;

                if (actions[i].displayName == candidate.displayName && actions[i].laneLabel == candidate.laneLabel)
                    return true;
            }

            return false;
        }
    }
}
