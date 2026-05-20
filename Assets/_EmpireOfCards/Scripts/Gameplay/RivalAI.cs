using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    [Serializable]
    public class RivalBusiness
    {
        public string name;
        public int income;
        public int customers;
        public int employeeCount;
        public int maxEmployees = 3;
        public VentureType ventureType;
        public float qualityScore = 5f;
        public float platformRating = 3f;
        public int legalRisk;
        public float priceScore = 5f;
    }

    public class RivalAI : MonoBehaviour
    {
        [SerializeField] private RivalData data;
        [SerializeField] private int rivalMoney;
        [SerializeField] private int rivalIncome;
        [SerializeField] private int rivalCustomers;
        [SerializeField] private int totalRivalEmployees;
        [SerializeField] private List<RivalBusiness> rivalBusinesses = new List<RivalBusiness>();
        [SerializeField] private VentureType activeVenture = VentureType.FastFood;
        [SerializeField] private float rivalRating = 3f;
        [SerializeField] private float rivalQuality = 5f;
        [SerializeField] private float rivalPressure = 0f;

        public int RivalMoney => rivalMoney;
        public int RivalCustomers => rivalCustomers;
        public int RivalIncome => rivalIncome;
        public IReadOnlyList<RivalBusiness> RivalBusinesses => rivalBusinesses;
        public string CurrentMood => "focused";
        public string MoodIcon => "!";
        public RivalData Data => data;

        public void Init(RivalData rivalData)
        {
            data = rivalData;
        }

        public void Initialize()
        {
            Initialize(VentureType.FastFood);
        }

        public void Initialize(VentureType playerVenture)
        {
            activeVenture = playerVenture;
            rivalMoney = data != null ? data.startingMoney : 450;
            rivalIncome = 60;
            rivalCustomers = 32;
            totalRivalEmployees = 2;
            rivalRating = 3.2f;
            rivalQuality = 5f;
            rivalPressure = 0f;

            rivalBusinesses.Clear();
            rivalBusinesses.Add(new RivalBusiness
            {
                name = GetRivalName(playerVenture),
                income = rivalIncome,
                customers = rivalCustomers,
                employeeCount = totalRivalEmployees,
                ventureType = playerVenture,
                qualityScore = rivalQuality,
                platformRating = rivalRating,
                priceScore = playerVenture == VentureType.GroceryStore ? 7f : 5f
            });
        }

        public void Initialize(RivalData rivalData)
        {
            data = rivalData;
            Initialize();
        }

        public void TakeTurn(int playerBlocks, int rivalBlocks, int currentTurn)
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            float playerShare = gm.EconomyManager != null && gm.EconomyManager.Snapshot != null
                ? gm.EconomyManager.Snapshot.marketShare
                : gm.PlayerCustomers;

            RivalMove move = DecideMove(playerShare, currentTurn);
            ApplyMove(move, playerShare);

            rivalCustomers = Mathf.Clamp(Mathf.RoundToInt(100f - playerShare + rivalPressure), 0, 100);
            rivalIncome = Mathf.RoundToInt(45f + rivalQuality * 6f + rivalRating * 8f);
            rivalMoney += Mathf.Max(10, rivalIncome / 4);

            if (rivalBusinesses.Count > 0)
            {
                var lead = rivalBusinesses[0];
                lead.customers = rivalCustomers;
                lead.income = rivalIncome;
                lead.platformRating = rivalRating;
                lead.qualityScore = rivalQuality;
            }

            EventBus.RivalActed(GetMoveDescription(move));
            EventBus.RivalMoodChanged(move.ToString());
        }

        public void RespondToPoach_Accept() { }
        public void RespondToPoach_CounterOffer() { }
        public void RespondToPoach_RejectWithBonus() { }

        public void CalculateRivalCustomers()
        {
            rivalCustomers = Mathf.Clamp(rivalCustomers, 0, 100);
        }

        public void CalculateRivalIncome()
        {
            rivalIncome = Mathf.RoundToInt(45f + rivalQuality * 6f + rivalRating * 8f);
        }

        public void ApplyCustomerPenalty(int penalty)
        {
            rivalCustomers = Mathf.Max(0, rivalCustomers - penalty);
        }

        public void ApplyNextPurchaseCostIncrease(float multiplier)
        {
            rivalPressure += multiplier * 2f;
        }

        public void CloseWeakestBusiness(int turns)
        {
            rivalCustomers = Mathf.Max(0, rivalCustomers - 8);
            rivalPressure -= 2f;
        }

        public int DisableProductionOneTurn()
        {
            var gm = GameManager.Instance;
            if (gm != null && gm.BoardManager != null)
                gm.BoardManager.SetProductionDisabledNextTurn(true);
            return 1;
        }

        public string GetTaunt(int playerBlocks, int rivalBlocks)
        {
            return GetMoveDescription(DecideMove(playerBlocks * 10f, GameManager.Instance != null ? GameManager.Instance.CurrentTurn : 1));
        }

        public void OnPlayerComboTriggered()
        {
            rivalPressure += 1.5f;
        }

        public void Reset()
        {
            rivalBusinesses.Clear();
            rivalMoney = 0;
            rivalIncome = 0;
            rivalCustomers = 0;
            totalRivalEmployees = 0;
            rivalPressure = 0f;
        }

        private RivalMove DecideMove(float playerShare, int currentTurn)
        {
            if (playerShare >= 58f)
                return RivalMove.MarketingBlitz;
            if (playerShare >= 50f)
                return activeVenture == VentureType.FastFood || activeVenture == VentureType.GroceryStore
                    ? RivalMove.PriceWar
                    : RivalMove.MarketingBlitz;
            if (currentTurn >= 15)
                return RivalMove.OpenBranch;
            if (activeVenture == VentureType.TechApp && rivalRating < 3.6f)
                return RivalMove.QualityImprove;
            if (activeVenture == VentureType.Cafe && currentTurn % 4 == 0)
                return RivalMove.StaffPoach;
            return RivalMove.QualityImprove;
        }

        private void ApplyMove(RivalMove move, float playerShare)
        {
            switch (move)
            {
                case RivalMove.PriceWar:
                    rivalPressure += 4f;
                    rivalCustomers += 5;
                    break;
                case RivalMove.MarketingBlitz:
                    rivalRating = Mathf.Clamp(rivalRating + 0.35f, 1f, 5f);
                    rivalPressure += 5f;
                    rivalCustomers += 4;
                    break;
                case RivalMove.QualityImprove:
                    rivalQuality = Mathf.Clamp(rivalQuality + 0.45f, 0f, 10f);
                    rivalRating = Mathf.Clamp(rivalRating + 0.15f, 1f, 5f);
                    break;
                case RivalMove.StaffPoach:
                    rivalPressure += 2f;
                    totalRivalEmployees++;
                    if (playerShare > 45f)
                        rivalCustomers += 3;
                    break;
                case RivalMove.SeekInvestment:
                    rivalMoney += 120;
                    rivalPressure += 1f;
                    break;
                case RivalMove.OpenBranch:
                    rivalCustomers += 6;
                    rivalIncome += 15;
                    rivalPressure += 3f;
                    break;
                case RivalMove.Sabotage:
                    var gm = GameManager.Instance;
                    if (gm != null && gm.BoardManager != null)
                        gm.BoardManager.SetProductionDisabledNextTurn(true);
                    rivalPressure += 2f;
                    break;
            }
        }

        private string GetRivalName(VentureType venture)
        {
            return venture switch
            {
                VentureType.FastFood => "Rival Kitchen",
                VentureType.Cafe => "Rival Cafe",
                VentureType.TechApp => "Rival App",
                VentureType.ClothingStore => "Rival Boutique",
                VentureType.GroceryStore => "Rival Market",
                _ => "Rival Co."
            };
        }

        private string GetMoveDescription(RivalMove move)
        {
            return move switch
            {
                RivalMove.PriceWar => "Rival starts a price war.",
                RivalMove.MarketingBlitz => "Rival launches a marketing blitz.",
                RivalMove.QualityImprove => "Rival improves product quality.",
                RivalMove.StaffPoach => "Rival pressures your staffing edge.",
                RivalMove.SeekInvestment => "Rival secures new funding.",
                RivalMove.OpenBranch => "Rival expands its footprint.",
                RivalMove.Sabotage => "Rival triggers operational pressure.",
                _ => "Rival adapts."
            };
        }
    }
}
