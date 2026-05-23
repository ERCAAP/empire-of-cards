using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public class DecisionHistoryManager : MonoBehaviour
    {
        [SerializeField] private List<DecisionRecord> ledger = new List<DecisionRecord>();
        [SerializeField] private int maxRecords = 20;

        public IReadOnlyList<DecisionRecord> Ledger => ledger;

        public void ResetState()
        {
            ledger.Clear();
        }

        public void Record(DecisionRecord record)
        {
            if (record == null)
                return;

            ledger.Add(record);
            while (ledger.Count > maxRecords)
                ledger.RemoveAt(0);

            EventBus.DecisionRecorded(record);
        }
    }
}
