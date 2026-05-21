using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay
{
    public class HygieneSystem : MonoBehaviour
    {
        BoardManager _board;

        public void Init(BoardManager board)
        {
            _board = board;
            Debug.Log("[HygieneSystem] Initialized.");
        }

        // ── Main Calculation ───────────────────────────────────────

        public float CalculateHygiene(int cleanerCount, float supplierQuality,
            bool overloaded, float currentHygiene)
        {
            float cleanerBoost = cleanerCount * 1.5f * 0.1f;
            float supplierEffect = supplierQuality * 0.1f;
            float overloadPenalty = overloaded ? Constants.HYGIENE_OVERLOAD_PENALTY : 0f;
            float naturalDecay = Constants.HYGIENE_NATURAL_DECAY;

            float newHygiene = currentHygiene
                             + cleanerBoost
                             + supplierEffect
                             - overloadPenalty
                             - naturalDecay;

            return Mathf.Clamp(newHygiene, Constants.STAT_MIN, Constants.STAT_MAX);
        }

        // ── Threshold Events ───────────────────────────────────────

        public void CheckThresholds(float hygiene)
        {
            if (hygiene <= Constants.HYGIENE_SHUTDOWN_THRESHOLD)
            {
                EventBus.HygieneWarning("KAPATIS: Hijyen kritik seviyede!");
                return;
            }

            if (hygiene <= Constants.HYGIENE_DANGER_THRESHOLD)
            {
                EventBus.HygieneWarning("TEHLIKE: Hijyen denetimi riski yuksek!");
                return;
            }

            if (hygiene <= Constants.HYGIENE_DIRTY_THRESHOLD)
            {
                EventBus.HygieneWarning("UYARI: Kirli mutfak yorumlari basliyor.");
                return;
            }
        }

        // ── Inspection Check ───────────────────────────────────────

        public bool ShouldTriggerInspection(float hygiene, float legalRisk)
        {
            if (hygiene > Constants.HYGIENE_DANGER_THRESHOLD
                && legalRisk < Constants.CRISIS_LEGAL_INSPECTION_THRESHOLD)
                return false;

            float chance = 0f;

            if (hygiene <= Constants.HYGIENE_DANGER_THRESHOLD)
                chance += Constants.HYGIENE_INSPECTION_CHANCE;

            if (legalRisk >= Constants.CRISIS_LEGAL_INSPECTION_THRESHOLD)
                chance += 0.15f;

            return Random.value < chance;
        }
    }
}
