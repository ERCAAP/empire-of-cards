using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public class QuestionManager : MonoBehaviour
    {
        [SerializeField] private List<QuestionRuntimeState> activeQuestions = new List<QuestionRuntimeState>();
        [SerializeField] private List<CardData> economyCardsForResolve = new List<CardData>();

        private DecisionHistoryManager _historyManager;

        public IReadOnlyList<QuestionRuntimeState> ActiveQuestions => activeQuestions;

        public void Init(DecisionHistoryManager historyManager)
        {
            _historyManager = historyManager;
        }

        public void ResetState()
        {
            activeQuestions.Clear();
            economyCardsForResolve.Clear();
            EventBus.QuestionsGenerated(activeQuestions.ToArray());
        }

        public void BeginTurnQuestions(int turnNumber, VentureType ventureType, BoardPressureType pressure, TurnBriefData brief)
        {
            activeQuestions.Clear();
            economyCardsForResolve.Clear();

            activeQuestions.Add(CreateRuntimeState(BuildPressureQuestion(turnNumber, ventureType, pressure)));
            activeQuestions.Add(CreateRuntimeState(BuildVentureQuestion(turnNumber, ventureType)));

            EventBus.QuestionsGenerated(activeQuestions.ToArray());
            EventBus.QuestionFocusRequested(new CameraFocusRequest
            {
                state = BoardCameraState.QuestionFocus,
                questionIndex = 0,
                reason = "QuestionsGenerated",
                blendDuration = 0.45f
            });
        }

        public bool CanCommitCardToQuestion(int questionIndex, CardData card)
        {
            if (questionIndex < 0 || questionIndex >= activeQuestions.Count || card == null)
                return false;
            if (!QuestionTagUtility.IsResponseCard(card))
                return false;

            var question = activeQuestions[questionIndex];
            if (question == null || question.definition == null)
                return false;

            if (!question.HasPrimary)
                return QuestionTagUtility.Matches(card, question.definition.primaryTag);

            if (!question.definition.supportAllowed || question.HasSupport)
                return false;

            return QuestionTagUtility.Matches(card, question.definition.optionalSupportTag);
        }

        public bool CommitCardToQuestion(int questionIndex, CardData card)
        {
            if (!CanCommitCardToQuestion(questionIndex, card))
                return false;

            var question = activeQuestions[questionIndex];
            if (!question.HasPrimary)
                question.committedPrimaryCard = card;
            else
                question.committedSupportCard = card;

            EventBus.QuestionAnswered(questionIndex, question);
            return true;
        }

        public void PrepareResolve(int turnNumber)
        {
            economyCardsForResolve.Clear();

            for (int i = 0; i < activeQuestions.Count; i++)
            {
                var question = activeQuestions[i];
                if (question == null || question.definition == null)
                    continue;

                if (question.committedPrimaryCard != null)
                {
                    economyCardsForResolve.Add(question.committedPrimaryCard);
                    if (question.committedSupportCard != null)
                        economyCardsForResolve.Add(question.committedSupportCard);

                    question.resolutionState = ResolveStateFromCard(question.committedPrimaryCard);
                    question.outcomeLabel = BuildOutcomeLabel(question);
                }
                else
                {
                    question.resolutionState = QuestionResolutionState.Ignored;
                    question.outcomeLabel = "Ignored";
                    economyCardsForResolve.Add(BuildPenaltyCard(question.definition));
                }

                var record = new DecisionRecord
                {
                    turnNumber = turnNumber,
                    questionId = question.definition.questionId,
                    questionHeadline = question.definition.headline,
                    primaryCardId = question.committedPrimaryCard != null ? question.committedPrimaryCard.cardId : null,
                    supportCardId = question.committedSupportCard != null ? question.committedSupportCard.cardId : null,
                    placementType = "QuestionResponse",
                    laneId = question.definition.primaryTag,
                    questionZoneId = $"QuestionTray_{i + 1}",
                    outcomeLabel = question.outcomeLabel
                };
                record.resolvedEffects.Add(question.outcomeLabel);
                if (question.resolutionState == QuestionResolutionState.SolvedDangerously || !string.IsNullOrWhiteSpace(question.definition.riskWarning))
                    record.delayedRiskFlags.Add(string.IsNullOrWhiteSpace(question.definition.riskWarning) ? "Risk follow-up possible" : question.definition.riskWarning);

                _historyManager?.Record(record);
                EventBus.QuestionResolved(i, question);
            }
        }

        public IReadOnlyList<CardData> GetEconomyCardsForResolve()
        {
            return economyCardsForResolve;
        }

        private static QuestionRuntimeState CreateRuntimeState(QuestionDefinition definition)
        {
            return new QuestionRuntimeState
            {
                definition = definition,
                spawnedTurn = GameManager.Instance != null ? GameManager.Instance.CurrentTurn : 0,
                resolutionState = QuestionResolutionState.Pending,
                outcomeLabel = "Pending"
            };
        }

        private static QuestionResolutionState ResolveStateFromCard(CardData card)
        {
            if (card == null)
                return QuestionResolutionState.Ignored;

            return card.cardFamily switch
            {
                CardFamily.Risk => QuestionResolutionState.SolvedDangerously,
                CardFamily.Growth => QuestionResolutionState.SolvedExpensively,
                CardFamily.Reaction => QuestionResolutionState.SolvedCleanly,
                _ => QuestionResolutionState.PartiallySolved
            };
        }

        private static string BuildOutcomeLabel(QuestionRuntimeState question)
        {
            return question.resolutionState switch
            {
                QuestionResolutionState.SolvedCleanly => "Solved Cleanly",
                QuestionResolutionState.SolvedExpensively => "Solved Expensively",
                QuestionResolutionState.SolvedDangerously => "Solved Dangerously",
                QuestionResolutionState.PartiallySolved => "Partially Solved",
                _ => "Ignored"
            };
        }

        private static CardData BuildPenaltyCard(QuestionDefinition definition)
        {
            var penalty = ScriptableObject.CreateInstance<CardData>();
            penalty.cardId = $"QuestionPenalty_{definition.questionId}";
            penalty.cardName = $"{definition.headline} Penalty";
            penalty.cardFamily = CardFamily.Crisis;
            penalty.cardType = CardType.Event;
            penalty.targetSlotType = SlotType.TempEffect;
            penalty.demandDelta = definition.penaltyDemandDelta;
            penalty.capacityDelta = definition.penaltyCapacityDelta;
            penalty.qualityDelta = definition.penaltyQualityDelta;
            penalty.ratingDeltaPerTurn = definition.penaltyRatingDelta;
            penalty.legalRiskDeltaPerTurn = definition.penaltyRiskDelta;
            penalty.cashDeltaPerTurn = definition.penaltyCashDelta;
            penalty.entersTempEffectOnUse = true;
            penalty.hideFlags = HideFlags.DontSave;
            return penalty;
        }

        private static QuestionDefinition BuildPressureQuestion(int turnNumber, VentureType ventureType, BoardPressureType pressure)
        {
            return pressure switch
            {
                BoardPressureType.LowDemand => CreateQuestion(turnNumber, ventureType, "Q_DEMAND", "Mahalle seni fark etmiyor", "Bugun seni fark ettirecek hamle ne?", "Demand", "Reputation", true, -0.8f, 0f, 0f, -0.15f, 0f, 0f),
                BoardPressureType.CapacityShortfall => CreateQuestion(turnNumber, ventureType, "Q_CAPACITY", "Yogunluk geliyor", "Talep artisina neyle yetiseceksin?", "Capacity", "Staff", true, 0f, -1.1f, -0.3f, -0.05f, 0f, 0f),
                BoardPressureType.WeakQuality => CreateQuestion(turnNumber, ventureType, "Q_QUALITY", "Kalite kirilgani var", "Musteri deneyimini neyle koruyacaksin?", "Quality", "Supply", true, 0f, 0f, -0.8f, -0.20f, 0f, 0f),
                BoardPressureType.HighLegalRisk => CreateQuestion(turnNumber, ventureType, "Q_RISK", "Denetim baskisi artisiyor", "Riski neyle yatistiracaksin?", "Risk", "Staff", true, 0f, 0f, 0f, -0.10f, 1.25f, -15f),
                BoardPressureType.StaffInstability => CreateQuestion(turnNumber, ventureType, "Q_STAFF", "Ekip dagiliyor", "Motivasyonu neyle toplayacaksin?", "Staff", "Loyalty", true, 0f, -0.5f, -0.2f, -0.10f, 0.2f, 0f),
                BoardPressureType.LowCash => CreateQuestion(turnNumber, ventureType, "Q_CASH", "Nakit sikisiyor", "Bugunu neyle dondureceksin?", "Cash", "Supply", true, -0.2f, 0f, -0.2f, -0.05f, 0f, -25f),
                _ => CreateQuestion(turnNumber, ventureType, "Q_GENERIC", "Gunluk baski var", "Bugunun ana sorununa neyle cevap vereceksin?", "Capacity", "Demand", true, -0.2f, -0.2f, 0f, -0.05f, 0f, 0f)
            };
        }

        private static QuestionDefinition BuildVentureQuestion(int turnNumber, VentureType ventureType)
        {
            return ventureType switch
            {
                VentureType.FastFood => CreateQuestion(turnNumber, ventureType, "Q_FF", "Ogle yogunlugu geliyor", "Mutfak mi servis mi once guclenecek?", "Speed", "Capacity", true, -0.4f, -0.5f, -0.3f, -0.08f, 0f, 0f),
                VentureType.Cafe => CreateQuestion(turnNumber, ventureType, "Q_CAFE", "Ilk guven testi", "Kaliteyi mi gorunurlugu mu once tasiyacaksin?", "Reputation", "Quality", true, -0.5f, 0f, -0.4f, -0.15f, 0f, 0f),
                VentureType.TechApp => CreateQuestion(turnNumber, ventureType, "Q_TECH", "Urun hazir mi", "Bug mi growth mu once gelecek?", "Quality", "Demand", true, -0.2f, -0.3f, -0.6f, -0.12f, 0.2f, 0f),
                VentureType.ClothingStore => CreateQuestion(turnNumber, ventureType, "Q_CLOTH", "Vitrin karari", "Algiyi mi fiyati mi one cikaracaksin?", "Reputation", "Cash", true, -0.3f, 0f, -0.3f, -0.10f, 0f, 0f),
                VentureType.GroceryStore => CreateQuestion(turnNumber, ventureType, "Q_GROCERY", "Raf sagligi bozuluyor", "Depoyu mu iliskiyi mi once koruyacaksin?", "Supply", "Loyalty", true, -0.3f, 0f, -0.5f, -0.08f, 0f, -10f),
                _ => CreateQuestion(turnNumber, ventureType, "Q_GENERIC_2", "Bugunun ikinci baskisi", "Hangi hattin acigi var?", "Demand", "Capacity", true, -0.2f, -0.2f, -0.2f, -0.05f, 0f, 0f)
            };
        }

        private static QuestionDefinition CreateQuestion(
            int turnNumber,
            VentureType ventureType,
            string id,
            string headline,
            string detail,
            string primaryTag,
            string supportTag,
            bool supportAllowed,
            float penaltyDemand,
            float penaltyCapacity,
            float penaltyQuality,
            float penaltyRating,
            float penaltyRisk,
            float penaltyCash)
        {
            return new QuestionDefinition
            {
                questionId = $"{id}_{turnNumber}",
                ventureType = ventureType,
                headline = headline,
                detail = detail,
                primaryTag = primaryTag,
                optionalSupportTag = supportTag,
                riskWarning = penaltyRisk > 0f ? "Risk may create a follow-up pressure if ignored or answered dangerously." : string.Empty,
                questionFamily = id,
                followUpIds = Array.Empty<string>(),
                cameraFocusHint = "question_stage",
                presentationStyle = "pressure_tray",
                supportAllowed = supportAllowed,
                penaltyDemandDelta = penaltyDemand,
                penaltyCapacityDelta = penaltyCapacity,
                penaltyQualityDelta = penaltyQuality,
                penaltyRatingDelta = penaltyRating,
                penaltyRiskDelta = penaltyRisk,
                penaltyCashDelta = penaltyCash
            };
        }
    }
}
