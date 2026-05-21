using TMPro;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Gameplay;
using EmpireOfCards.UI.Clarity;
using EmpireOfCards.Presentation;

namespace EmpireOfCards.UI
{
    public class AnalyticsPanelUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text playerBodyText;
        [SerializeField] private TMP_Text rivalNameText;
        [SerializeField] private TMP_Text rivalBodyText;
        [SerializeField] private TMP_Text footerText;

        private float _lastPlayerShare = float.NaN;
        private float _lastDemand = float.NaN;
        private float _lastCapacity = float.NaN;
        private float _lastQuality = float.NaN;
        private float _lastRating = float.NaN;
        private float _lastRisk = float.NaN;
        private int _lastCash = int.MinValue;

        public void Init(
            TMP_Text title,
            TMP_Text playerName,
            TMP_Text playerBody,
            TMP_Text rivalName,
            TMP_Text rivalBody,
            TMP_Text footer)
        {
            titleText = title;
            playerNameText = playerName;
            playerBodyText = playerBody;
            rivalNameText = rivalName;
            rivalBodyText = rivalBody;
            footerText = footer;
        }

        public void Refresh(GameManager gm)
        {
            if (gm == null || gm.EconomyManager == null)
                return;

            var snapshot = gm.EconomyManager.Snapshot;
            var rival = gm.RivalAI;
            string buildIdentity = GameClarityFormatter.GetBuildIdentity(gm);
            string buildDetail = GameClarityFormatter.GetBuildIdentityDetail(buildIdentity);
            string pressureLabel = GameClarityFormatter.GetPressureLabel(gm.EconomyManager.CurrentPressure);
            string pressureDetail = GameClarityFormatter.GetPressureDetail(gm.EconomyManager.CurrentPressure);
            string playerName = string.IsNullOrWhiteSpace(gm.RunDisplayName) ? "Your Venture" : gm.RunDisplayName;
            string rivalName = rival != null ? rival.RivalDisplayName : "Rival";
            float playerShare = snapshot != null ? snapshot.marketShare : 0f;
            float rivalShare = Mathf.Clamp(100f - playerShare, 0f, 100f);

            if (titleText != null)
                titleText.text = $"CONTROL DESK  ·  {pressureLabel}";

            if (playerNameText != null)
                playerNameText.text = $"YOU  ·  {playerName.ToUpperInvariant()}";

            if (playerBodyText != null && snapshot != null)
            {
                string shareLine = BuildMetricLine("Share", FormatDelta(playerShare, ref _lastPlayerShare), ControlDeskTheme.PlayerBlock);
                string cashLine = BuildMetricLine("Cash", FormatMoney(gm.PlayerMoney, ref _lastCash), ControlDeskTheme.MoneyGold);
                string flowLine = BuildDualMetricLine("Demand", FormatDelta(snapshot.demand, ref _lastDemand), "Cap", FormatDelta(snapshot.capacity, ref _lastCapacity), ControlDeskTheme.AccentBlue);
                string qualityLine = BuildDualMetricLine("Qual", FormatDelta(snapshot.quality, ref _lastQuality), "Rate", FormatDelta(snapshot.rating, ref _lastRating), ControlDeskTheme.AccentAmber);
                string riskLine = BuildMetricLine("Risk", FormatDelta(snapshot.legalRisk, ref _lastRisk), ControlDeskTheme.AccentRed);
                string buildLine = BuildMetricLine("Build", buildIdentity, ControlDeskTheme.TextPrimary);
                playerBodyText.text =
                    $"{shareLine}\n{cashLine}\n{flowLine}\n{qualityLine}\n{riskLine}\n{buildLine}";
            }

            if (rivalNameText != null)
                rivalNameText.text = $"RIVAL  ·  {rivalName.ToUpperInvariant()}";

            if (rivalBodyText != null)
            {
                if (rival != null)
                {
                    string tactic = string.IsNullOrWhiteSpace(rival.LastPressureStyle) ? "Balanced" : rival.LastPressureStyle;
                    string lastCard = string.IsNullOrWhiteSpace(rival.LastPlayedCardName) ? "Waiting" : rival.LastPlayedCardName;
                    string lane = string.IsNullOrWhiteSpace(rival.LastLaneLabel) ? "Pressure Lane" : rival.LastLaneLabel;
                    string shareLine = BuildMetricLine("Share", $"{rivalShare:0.0}%", ControlDeskTheme.RivalBlock);
                    string economyLine = BuildDualMetricLine("Income", $"${rival.RivalIncome:N0}", "Cust", rival.RivalCustomers.ToString(), ControlDeskTheme.AccentAmber);
                    string qualityLine = BuildDualMetricLine("Qual", $"{rival.RivalQuality:0.0}", "Rate", $"{rival.RivalRating:0.0}", ControlDeskTheme.AccentBlue);
                    string threatLine = BuildDualMetricLine("Threat", $"{rival.RivalPressure:0.0}", "Lane", lane, ControlDeskTheme.AccentRed);
                    string focusLine = BuildMetricLine("Focus", tactic, ControlDeskTheme.AccentAmber);
                    string lastLine = BuildMetricLine("Last", lastCard, ControlDeskTheme.TextPrimary);
                    rivalBodyText.text =
                        $"{shareLine}\n{economyLine}\n{qualityLine}\n{threatLine}\n{focusLine}\n{lastLine}";
                    if (footerText != null)
                        footerText.text = $"Pressure read: {pressureDetail}\nBuild note: {buildDetail}";
                }
                else
                {
                    rivalBodyText.text = "No rival snapshot.";
                    if (footerText != null)
                        footerText.text = string.Empty;
                }
            }
        }

        private static string FormatMoney(int value, ref int previous)
        {
            string formatted = $"${value:N0}";
            if (previous == int.MinValue)
            {
                previous = value;
                return formatted;
            }

            int delta = value - previous;
            previous = value;
            if (delta == 0)
                return formatted;

            string color = delta > 0 ? ColorUtility.ToHtmlStringRGB(ControlDeskTheme.AccentGreen) : ColorUtility.ToHtmlStringRGB(ControlDeskTheme.AccentRed);
            string arrow = delta > 0 ? "↑" : "↓";
            return $"{formatted} <color=#{color}>{arrow}</color>";
        }

        private static string FormatDelta(float value, ref float previous)
        {
            string formatted = value.ToString("0.0");
            if (float.IsNaN(previous))
            {
                previous = value;
                return formatted;
            }

            float delta = value - previous;
            previous = value;
            if (Mathf.Abs(delta) < 0.05f)
                return formatted;

            string color = delta > 0f ? ColorUtility.ToHtmlStringRGB(ControlDeskTheme.AccentGreen) : ColorUtility.ToHtmlStringRGB(ControlDeskTheme.AccentRed);
            string arrow = delta > 0f ? "↑" : "↓";
            return $"{formatted} <color=#{color}>{arrow}</color>";
        }

        private static string BuildMetricLine(string label, string value, Color accent)
        {
            string color = ColorUtility.ToHtmlStringRGB(accent);
            return $"<color=#{color}>{label.ToUpperInvariant()}</color>  {value}";
        }

        private static string BuildDualMetricLine(string leftLabel, string leftValue, string rightLabel, string rightValue, Color accent)
        {
            string color = ColorUtility.ToHtmlStringRGB(accent);
            return $"<color=#{color}>{leftLabel.ToUpperInvariant()}</color> {leftValue}   <color=#{color}>{rightLabel.ToUpperInvariant()}</color> {rightValue}";
        }
    }
}
