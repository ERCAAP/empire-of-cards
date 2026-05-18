using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "NewRival", menuName = "EmpireOfCards/Rival Data")]
    public class RivalData : ScriptableObject
    {
        [Header("--- Kimlik ---")]
        public string rivalId;              // "RIVAL_MegaCorp"
        public string rivalName;            // "MegaCorp"
        public RivalPersonality personality;
        public Sprite portrait;
        [TextArea(1, 2)]
        public string tagline;              // "Bu sektörde ikimize yer yok."

        [Header("--- Başlangıç ---")]
        public int startingMoney = 400;
        public int startingIncome = 80;     // İlk işletme geliri
        public int startingCustomers = 5;   // İlk işletme müşterisi
        public string startingBusinessName = "MegaCorp HQ";

        [Header("--- Davranış ---")]
        public int actionsPerTurn = 2;      // Normal: 2 hamle/tur
        [Range(0f, 1f)]
        public float aggressionThreshold = 0.5f;  // Player territories > threshold → agresif
        public int maxBusinesses = 4;
        public int maxEmployeesPerBusiness = 3;

        [Header("--- Büyüme Parametreleri ---")]
        public int businessCostThreshold = 200;
        public int hireCostThreshold = 80;
        public int baseBusinessIncome = 80;
        public int baseBusinessCustomers = 5;
        public int employeeIncomeBoost = 30;
        public int employeeCustomerBoost = 3;
        public int aggressiveCustomerBoost = 8;
        public int aggressiveIncomeBoost = 50;
        public int passiveCustomerGrowth = 2;
        public int passiveIncomeGrowth = 10;
        public string[] possibleBusinessNames;  // ["Teknoloji Mağazası", "Süpermarket", ...]

        [Header("--- Büyüme Takvimi (GDD Section 8.3) ---")]
        public RivalMilestone[] growthMilestones;

        [Header("--- Diyalog ---")]
        [TextArea(1, 2)]
        public string[] growingTaunts;      // Rakip büyürken: "Pazar payımız artıyor."
        [TextArea(1, 2)]
        public string[] playerGrowingTaunts;// Sen büyürken: "İlginç bir hamle..."
        [TextArea(1, 2)]
        public string[] aggressiveTaunts;   // Agresifken: "Bu sektörde ikimize yer yok."
        [TextArea(1, 2)]
        public string[] losingTaunts;       // Kaybederken: "Bu böyle bitmez."
        [TextArea(1, 2)]
        public string[] winningTaunts;      // Kazanırken: "Kaçınılmazdı."
    }

    [System.Serializable]
    public class RivalMilestone
    {
        public int turn;                    // Tur 5, 8, 12, 15 etc
        public int targetBusinesses;        // Hedef işletme sayısı
        public int targetEmployees;         // Hedef çalışan sayısı
        public int targetTerritories;       // Hedef bölge sayısı
        public bool enableAggression;       // Bu turdan sonra agresif olabilir mi
    }
}
