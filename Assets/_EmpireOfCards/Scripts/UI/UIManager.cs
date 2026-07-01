using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.UI.Popups;

namespace EmpireOfCards.UI
{
    public class UIManager : MonoBehaviour
    {
        // ── References (set by HUDBuilder) ──────────────────────────
        TopBarUI _topBar;
        CrisisPopup _crisisPopup;
        TurnReportPanel _turnReport;
        GameSetupPanel _gameSetup;
        DashboardPanel _dashboard;
        StaffPromotionPopup _staffPromotion;
        GameOverScreen _gameOver;

        // ── Init ────────────────────────────────────────────────────

        public void Init(TopBarUI topBar, CrisisPopup crisisPopup, TurnReportPanel turnReport,
            GameSetupPanel gameSetup, DashboardPanel dashboard,
            StaffPromotionPopup staffPromotion, GameOverScreen gameOver)
        {
            _topBar = topBar;
            _crisisPopup = crisisPopup;
            _turnReport = turnReport;
            _gameSetup = gameSetup;
            _dashboard = dashboard;
            _staffPromotion = staffPromotion;
            _gameOver = gameOver;

            // Start with popups hidden
            if (_crisisPopup != null) _crisisPopup.Hide();
            if (_turnReport != null) _turnReport.Hide();
            if (_staffPromotion != null) _staffPromotion.Hide();
            if (_gameOver != null) _gameOver.Hide();
        }

        // ── EventBus subscriptions ──────────────────────────────────

        void OnEnable()
        {
            EventBus.OnCrisisTriggered += HandleCrisis;
            EventBus.OnCrisisResolved += HandleCrisisResolved;
            EventBus.OnPhaseStarted += HandlePhaseStarted;
            EventBus.OnPhaseEnded += HandlePhaseEnded;
            EventBus.OnTurnReport += HandleTurnReport;
            EventBus.OnGameOver += HandleGameOver;
            EventBus.OnStaffPromotionAvailable += HandleStaffPromotion;
            EventBus.OnPromotionChoiceMade += HandlePromotionChoice;
        }

        void OnDisable()
        {
            EventBus.OnCrisisTriggered -= HandleCrisis;
            EventBus.OnCrisisResolved -= HandleCrisisResolved;
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
            EventBus.OnPhaseEnded -= HandlePhaseEnded;
            EventBus.OnTurnReport -= HandleTurnReport;
            EventBus.OnGameOver -= HandleGameOver;
            EventBus.OnStaffPromotionAvailable -= HandleStaffPromotion;
            EventBus.OnPromotionChoiceMade -= HandlePromotionChoice;
        }

        // ── Public API ──────────────────────────────────────────────

        public void ShowSetup()
        {
            if (_gameSetup != null) _gameSetup.Show();
        }

        public void HideSetup()
        {
            if (_gameSetup != null) _gameSetup.Hide();
        }

        // ── Handlers ────────────────────────────────────────────────

        void HandleCrisis(CrisisType crisis)
        {
            if (_crisisPopup == null) return;

            string desc = GetCrisisDescription(crisis);
            _crisisPopup.Show(crisis, desc);
        }

        void HandleCrisisResolved(CrisisType crisis, string choiceId)
        {
            if (_crisisPopup != null)
                _crisisPopup.Hide();
        }

        void HandlePhaseStarted(TurnPhase phase)
        {
            // Show turn report at resolve end
            if (phase == TurnPhase.ResolvePhase && _turnReport != null)
                _turnReport.Hide(); // hide any old report before new resolve
        }

        void HandlePhaseEnded(TurnPhase phase)
        {
            // Crisis popup only during CrisisReactionPhase
            if (phase == TurnPhase.CrisisReactionPhase && _crisisPopup != null)
                _crisisPopup.Hide();
        }

        void HandleTurnReport(int income, int expense, int net, float ratingDelta, int served, int waited)
        {
            if (_turnReport != null)
                _turnReport.Show(income, expense, net, ratingDelta, served, waited);
        }

        void HandleStaffPromotion(CardData staff, StaffTier currentTier)
        {
            // StaffPromotionPopup handles its own EventBus subscription,
            // but UIManager ensures no conflicting popups are open
            if (_crisisPopup != null) _crisisPopup.Hide();
        }

        void HandlePromotionChoice(CardData staff, int choice)
        {
            // Popup hides itself; nothing extra needed here
        }

        void HandleGameOver(bool won, string reason)
        {
            // Hide everything, game over screen handles itself
            if (_crisisPopup != null) _crisisPopup.Hide();
            if (_turnReport != null) _turnReport.Hide();
            if (_staffPromotion != null) _staffPromotion.Hide();

            Debug.Log($"[UIManager] Game Over - {(won ? "WIN" : "LOSE")}: {reason}");
        }

        // ── Crisis description helper ───────────────────────────────

        static string GetCrisisDescription(CrisisType crisis)
        {
            switch (crisis)
            {
                case CrisisType.ReviewBomb:        return "Kotu yorumlar patladı! Rating dusecek.";
                case CrisisType.HygieneInspection: return "Hijyen denetimi! Yasal risk artacak.";
                case CrisisType.StaffQuit:         return "Personel istifa etti! Kapasite dusecek.";
                case CrisisType.SupplyShortage:    return "Malzeme krizi! Kalite dusecek.";
                case CrisisType.RentIncrease:      return "Kira zammi! Giderler artacak.";
                case CrisisType.ViralBadVideo:     return "Viral kotu video! Talep dusecek.";
                case CrisisType.FoodPoisoning:     return "Gida zehirlenmesi! Buyuk kriz!";
                case CrisisType.RivalPriceCut:     return "Rakip fiyat kırdi! Musteriler gidebilir.";
                case CrisisType.StaffTheft:        return "Calisan hirsizligi! Para kaybedildi.";
                case CrisisType.StreetConstruction: return "Sokak yapimi! Musteri akisi azalacak.";
                case CrisisType.Pandemic:          return "Pandemi! Tum istatistikler etkilenir.";
                default:                           return "Bilinmeyen kriz!";
            }
        }
    }
}
