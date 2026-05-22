using System;
using System.Collections.Generic;
using EmpireOfCards.Core;

namespace EmpireOfCards.Data
{
    [Serializable]
    public class StaffApplicant
    {
        public string applicantId;
        public string templateCardId;
        public string displayName;
        public VentureType ventureType;
        public StaffRole role;
        public int requestedSalary;
        public int skillScore;
        public int reliabilityScore;
        public int trialTurns;
    }

    [Serializable]
    public class HiringDecisionResult
    {
        public StaffApplicant applicant;
        public HiringDecision decision;
        public bool accepted;
        public int agreedSalary;
        public int moraleDelta;
        public int loyaltyDelta;
        public bool placedImmediately;
    }

    [Serializable]
    public class StaffRoleCoverage
    {
        public VentureType ventureType;
        public int requiredRoleCount;
        public int coveredRoleCount;
        public float coverageRatio;
        public List<StaffRole> missingRoles = new List<StaffRole>();
    }

    [Serializable]
    public class StaffWorkloadReport
    {
        public VentureType ventureType;
        public float demand;
        public float capacity;
        public float workloadPressure;
        public float capacityPenalty;
        public float qualityPenalty;
        public float ratingPenalty;
        public float staffStabilityPenalty;
        public StaffRoleCoverage coverage = new StaffRoleCoverage();
        public List<StaffRolePressure> rolePressures = new List<StaffRolePressure>();
    }

    [Serializable]
    public class StaffRolePressure
    {
        public StaffRole role;
        public float pressure;
        public bool covered;
    }

    [Serializable]
    public class StaffRuntimeSaveData
    {
        public string cardId;
        public StaffRole role;
        public EmploymentStatus employmentStatus;
        public int baseSalary;
        public int negotiatedSalary;
        public int trialTurnsRemaining;
        public float workload;
        public float burnoutRisk;
        public float resignRisk;
        public QuitReason lastQuitReason;
        public int moral;
        public int fatigue;
        public int loyalty;
        public int experience;
        public int turnsWorked;
        public int consecutiveOvertimeTurns;
    }

    [Serializable]
    public class StaffSystemRuntimeSaveData
    {
        public List<StaffApplicant> applicantPool = new List<StaffApplicant>();
        public int lastApplicantTurn;
        public string pendingPoachCardId;
        public int pendingPoachOffer;
        public int lastPoachTurn;
    }

    [Serializable]
    public class ActiveCreditSaveData
    {
        public CreditType type;
        public int principal;
        public float interestRate;
        public int turnsRemaining;
        public int accumulatedInterest;
    }

    [Serializable]
    public class LegalIncidentRuntimeData
    {
        public string incidentId;
        public string displayName;
        public int turnsRemaining;
        public int forcedExpensePerTurn;
        public float ratingPenaltyPerTurn;
        public float capacityPenalty;
        public float legalRiskDeltaPerTurn;
        public int closureTurnsRemaining;
    }

    [Serializable]
    public class EconomyRuntimeSaveData
    {
        public SalaryChoice lastSalaryChoice;
        public int consecutiveSalaryDelays;
        public InsuranceType insuranceType;
        public int payrollDebt;
        public int accumulatedNetProfit;
        public int unpaidTaxDebt;
        public int unpaidTaxTurns;
        public List<ActiveCreditSaveData> activeCredits = new List<ActiveCreditSaveData>();
        public LegalIncidentRuntimeData legalIncident;
    }
}
