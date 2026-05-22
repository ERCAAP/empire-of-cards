using NUnit.Framework;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay.Economy;
using EmpireOfCards.Gameplay.Staff;

namespace EmpireOfCards.Tests.EditMode
{
    public class StaffScenarioSystemTests
    {
        private GameObject _go;
        private StaffStateSystem _staff;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("StaffStateSystemTests");
            _staff = _go.AddComponent<StaffStateSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_go);
        }

        [Test]
        public void DemandOverCapacity_WithMissingRoles_IncreasesWorkloadAndFatigue()
        {
            CardData cashier = MakeStaff("FF10", "Cashier", VentureType.FastFood, StaffRole.Cashier, 35);
            _staff.RegisterStaff(cashier);

            StaffWorkloadReport report = _staff.ResolveWorkload(VentureType.FastFood, 12f, 4f, 2, null);
            StaffState state = _staff.GetState(cashier);

            Assert.That(report.workloadPressure, Is.GreaterThan(8f));
            Assert.That(state.workload, Is.GreaterThan(0f));
            Assert.That(state.fatigue, Is.GreaterThan(0));

            Object.DestroyImmediate(cashier);
        }

        [Test]
        public void SustainedHighWorkload_LowersMoraleAndLoyalty()
        {
            CardData cashier = MakeStaff("FF10", "Cashier", VentureType.FastFood, StaffRole.Cashier, 35);
            _staff.RegisterStaff(cashier);

            for (int i = 0; i < 3; i++)
            {
                _staff.ResolveWorkload(VentureType.FastFood, 14f, 4f, 3, null);
                _staff.TickAll();
            }

            StaffState state = _staff.GetState(cashier);
            Assert.That(state.moral, Is.LessThan(Constants.STAFF_DEFAULT_MORAL));
            Assert.That(state.loyalty, Is.LessThanOrEqualTo(Constants.STAFF_DEFAULT_LOYALTY));

            Object.DestroyImmediate(cashier);
        }

        [Test]
        public void BurnedOutLowMoraleStaff_QuitsAndLeavesState()
        {
            CardData cashier = MakeStaff("FF10", "Cashier", VentureType.FastFood, StaffRole.Cashier, 35);
            _staff.RegisterStaff(cashier);
            StaffState state = _staff.GetState(cashier);
            state.fatigue = Constants.STAFF_FATIGUE_MAX;
            state.moral = 1;
            state.workload = 8f;

            bool quit = _staff.TryResolveQuitChecks();

            Assert.That(quit, Is.True);
            Assert.That(_staff.GetState(cashier), Is.Null);

            Object.DestroyImmediate(cashier);
        }

        [Test]
        public void AddingRequiredRoles_ImprovesCoverageAndReducesPressure()
        {
            CardData cashier = MakeStaff("FF10", "Cashier", VentureType.FastFood, StaffRole.Cashier, 35);
            _staff.RegisterStaff(cashier);
            float weakPressure = _staff.ResolveWorkload(VentureType.FastFood, 8f, 6f, 2, null).workloadPressure;

            CardData chef = MakeStaff("FF03", "Chef", VentureType.FastFood, StaffRole.Chef, 45);
            CardData cleaning = MakeStaff("FF11", "Cleaning", VentureType.FastFood, StaffRole.Cleaning, 32);
            _staff.RegisterStaff(chef);
            _staff.RegisterStaff(cleaning);

            StaffWorkloadReport covered = _staff.ResolveWorkload(VentureType.FastFood, 8f, 6f, 2, null);

            Assert.That(covered.coverage.coverageRatio, Is.EqualTo(1f));
            Assert.That(covered.workloadPressure, Is.LessThan(weakPressure));

            Object.DestroyImmediate(cashier);
            Object.DestroyImmediate(chef);
            Object.DestroyImmediate(cleaning);
        }

        [Test]
        public void SalaryChoices_ChangeMoraleAndLoyalty()
        {
            CardData cashier = MakeStaff("FF10", "Cashier", VentureType.FastFood, StaffRole.Cashier, 35);
            _staff.RegisterStaff(cashier);

            _staff.ApplySalaryResult(new SalaryResult
            {
                moraleChange = Constants.SALARY_DELAY_MORALE,
                loyaltyChange = -1,
                resignRiskIncrease = Constants.SALARY_DELAY_RESIGN_RISK
            });

            StaffState delayed = _staff.GetState(cashier);
            Assert.That(delayed.moral, Is.LessThan(Constants.STAFF_DEFAULT_MORAL));
            Assert.That(delayed.resignRisk, Is.GreaterThan(0f));

            _staff.ApplySalaryResult(new SalaryResult
            {
                moraleChange = Constants.SALARY_ADVANCE_MORALE,
                loyaltyChange = Constants.SALARY_ADVANCE_LOYALTY
            });

            StaffState advanced = _staff.GetState(cashier);
            Assert.That(advanced.loyalty, Is.GreaterThanOrEqualTo(Constants.STAFF_DEFAULT_LOYALTY));

            Object.DestroyImmediate(cashier);
        }

        [Test]
        public void ReactionTempCard_ReducesFatiguePressure()
        {
            CardData barista = MakeStaff("CF03", "Barista", VentureType.Cafe, StaffRole.Barista, 48);
            CardData reaction = ScriptableObject.CreateInstance<CardData>();
            reaction.cardId = "CF15";
            reaction.cardName = "Reset The Shift";
            reaction.cardType = CardType.Upgrade;
            reaction.cardFamily = CardFamily.Reaction;
            reaction.targetSlotType = SlotType.TempEffect;
            reaction.fatigueDeltaPerTurn = -2;
            reaction.moraleDeltaPerTurn = 1;
            reaction.burnoutRiskDeltaPerTurn = -0.12f;

            _staff.RegisterStaff(barista);
            StaffState state = _staff.GetState(barista);
            state.fatigue = 5;
            _staff.ResolveWorkload(VentureType.Cafe, 6f, 6f, 1, new[] { reaction });

            Assert.That(state.fatigue, Is.LessThan(5));
            Assert.That(state.moral, Is.GreaterThanOrEqualTo(Constants.STAFF_DEFAULT_MORAL));

            Object.DestroyImmediate(barista);
            Object.DestroyImmediate(reaction);
        }

        [Test]
        public void RuntimeState_CapturesAndRestoresStaffLifecycle()
        {
            CardData cashier = MakeStaff("FF10", "Cashier", VentureType.FastFood, StaffRole.Cashier, 35);
            _staff.RegisterStaff(cashier);
            StaffState state = _staff.GetState(cashier);
            state.moral = 3;
            state.fatigue = 8;
            state.workload = 6f;
            state.resignRisk = 0.4f;

            var saved = _staff.CaptureStaffRuntimeState();
            _staff.Reset();
            _staff.RegisterStaff(cashier);
            _staff.RestoreStaffRuntimeState(saved);

            StaffState restored = _staff.GetState(cashier);
            Assert.That(restored.moral, Is.EqualTo(3));
            Assert.That(restored.fatigue, Is.EqualTo(8));
            Assert.That(restored.workload, Is.EqualTo(6f));
            Assert.That(restored.resignRisk, Is.EqualTo(0.4f));

            Object.DestroyImmediate(cashier);
        }

        private static CardData MakeStaff(string id, string name, VentureType venture, StaffRole role, int salary)
        {
            CardData card = ScriptableObject.CreateInstance<CardData>();
            card.cardId = id;
            card.cardName = name;
            card.cardType = CardType.Employee;
            card.ventureType = venture;
            card.targetSlotType = SlotType.Staff;
            card.staffRole = role;
            card.salaryPerTurn = salary;
            card.upkeepCostPerTurn = salary;
            card.defaultTrialTurns = 2;
            return card;
        }
    }
}
