using System;
using UnityEngine;

namespace EmpireOfCards.Core
{
    [Serializable]
    public class PlayerResources
    {
        // ── Backing fields ──────────────────────────────────────────
        [SerializeField] int money;
        [SerializeField] int actionsPerTurn;
        [SerializeField] int actionsRemaining;
        [SerializeField] float demand;
        [SerializeField] float capacity;
        [SerializeField] float quality;
        [SerializeField] float rating;
        [SerializeField] float staffStability;
        [SerializeField] float legalRisk;
        [SerializeField] float marketShare;
        [SerializeField] float hygiene;

        // ── Initialize with Constants defaults ──────────────────────

        public void Initialize()
        {
            money            = Constants.STARTING_CASH;
            actionsPerTurn   = Constants.ACTIONS_ERA_1;
            actionsRemaining = actionsPerTurn;
            demand           = Constants.STARTING_DEMAND;
            capacity         = Constants.STARTING_CAPACITY;
            quality          = Constants.STARTING_QUALITY;
            rating           = Constants.STARTING_RATING;
            staffStability   = Constants.STARTING_STABILITY;
            legalRisk        = Constants.STARTING_LEGAL_RISK;
            marketShare      = Constants.STARTING_MARKET_SHARE;
            hygiene          = Constants.STARTING_HYGIENE;
        }

        // ── Money ───────────────────────────────────────────────────

        public int GetMoney() => money;

        public void SetMoney(int value)
        {
            money = value;
            EventBus.MoneyChanged(money);
        }

        public void AdjustMoney(int delta)
        {
            money += delta;
            EventBus.MoneyChanged(money);
        }

        // ── Actions ─────────────────────────────────────────────────

        public int GetActionsRemaining() => actionsRemaining;
        public int GetActionsPerTurn() => actionsPerTurn;

        public void SetActionsPerTurn(int value)
        {
            actionsPerTurn = value;
        }

        public void ResetActions()
        {
            actionsRemaining = actionsPerTurn;
            EventBus.ActionsChanged(actionsRemaining);
        }

        public bool HasActions() => actionsRemaining > 0;

        public bool UseAction()
        {
            if (actionsRemaining <= 0) return false;
            actionsRemaining--;
            EventBus.ActionsChanged(actionsRemaining);
            return true;
        }

        // ── Demand ──────────────────────────────────────────────────

        public float GetDemand() => demand;

        public void SetDemand(float value)
        {
            demand = Mathf.Clamp(value, Constants.STAT_MIN, Constants.STAT_MAX);
            EventBus.DemandChanged(demand);
        }

        public void AdjustDemand(float delta)
        {
            SetDemand(demand + delta);
        }

        // ── Capacity ────────────────────────────────────────────────

        public float GetCapacity() => capacity;

        public void SetCapacity(float value)
        {
            capacity = Mathf.Clamp(value, Constants.STAT_MIN, Constants.STAT_MAX);
            EventBus.CapacityChanged(capacity);
        }

        public void AdjustCapacity(float delta)
        {
            SetCapacity(capacity + delta);
        }

        // ── Quality ─────────────────────────────────────────────────

        public float GetQuality() => quality;

        public void SetQuality(float value)
        {
            quality = Mathf.Clamp(value, Constants.STAT_MIN, Constants.STAT_MAX);
            EventBus.QualityChanged(quality);
        }

        public void AdjustQuality(float delta)
        {
            SetQuality(quality + delta);
        }

        // ── Rating ──────────────────────────────────────────────────

        public float GetRating() => rating;

        public void SetRating(float value)
        {
            rating = Mathf.Clamp(value, Constants.RATING_MIN, Constants.RATING_MAX);
            EventBus.RatingChanged(rating);
        }

        public void AdjustRating(float delta)
        {
            SetRating(rating + delta);
        }

        // ── Staff Stability ─────────────────────────────────────────

        public float GetStaffStability() => staffStability;

        public void SetStaffStability(float value)
        {
            staffStability = Mathf.Clamp(value, Constants.STAT_MIN, Constants.STAT_MAX);
            EventBus.StaffStabilityChanged(staffStability);
        }

        public void AdjustStaffStability(float delta)
        {
            SetStaffStability(staffStability + delta);
        }

        // ── Legal Risk ──────────────────────────────────────────────

        public float GetLegalRisk() => legalRisk;

        public void SetLegalRisk(float value)
        {
            legalRisk = Mathf.Clamp(value, Constants.STAT_MIN, Constants.LEGAL_RISK_MAX);
            EventBus.LegalRiskChanged(legalRisk);
        }

        public void AdjustLegalRisk(float delta)
        {
            SetLegalRisk(legalRisk + delta);
        }

        // ── Market Share ────────────────────────────────────────────

        public float GetMarketShare() => marketShare;

        public void SetMarketShare(float value)
        {
            marketShare = Mathf.Clamp(value, Constants.MARKET_SHARE_MIN, Constants.MARKET_SHARE_MAX);
            float rivalShare = Constants.MARKET_SHARE_MAX - marketShare;
            EventBus.MarketShareChanged(marketShare, rivalShare);
        }

        public void AdjustMarketShare(float delta)
        {
            SetMarketShare(marketShare + delta);
        }

        // ── Hygiene ─────────────────────────────────────────────────

        public float GetHygiene() => hygiene;

        public void SetHygiene(float value)
        {
            hygiene = Mathf.Clamp(value, Constants.STAT_MIN, Constants.STAT_MAX);
            EventBus.HygieneChanged(hygiene);
        }

        public void AdjustHygiene(float delta)
        {
            SetHygiene(hygiene + delta);
        }
    }
}
