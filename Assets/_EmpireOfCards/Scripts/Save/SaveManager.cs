using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Save
{
    /// <summary>
    /// Meta-progression save data persisted between runs.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int totalXP;
        public int currentAscension;
        public int runsPlayed;
        public int runsWon;
        public List<string> unlockedCardIds = new List<string>();
        public int bestScore;
        public bool tutorialCompleted;
        public RunSaveData activeRun;
    }

    [Serializable]
    public class RunSaveData
    {
        public string runName;
        public int ventureType;
        public int currentTurn;
        public int playerMoney;
        public int playerActions;
        public int playerMaxActions;
        public int playerBusinessSlots;
        public int playerCustomers;
        public int rivalCustomers;
        public int playerMarketBlocks;
        public int rivalMarketBlocks;
        public float fbiRisk;
        public int redrawsRemaining;
        public EmpireOfCards.Data.VentureBoardSnapshot economySnapshot;
        public List<string> drawPileIds = new List<string>();
        public List<string> handIds = new List<string>();
        public List<string> discardPileIds = new List<string>();
        public List<string> operationSlotIds = new List<string>();
        public List<string> staffSlotIds = new List<string>();
        public List<string> marketingSlotIds = new List<string>();
        public List<string> supplierSlotIds = new List<string>();
        public List<string> tempEffectSlotIds = new List<string>();

        public bool HasData()
        {
            return !string.IsNullOrWhiteSpace(runName);
        }
    }

    /// <summary>
    /// JSON save/load for meta progression. Subscribes to EventBus.OnGameOver
    /// to automatically persist progress at the end of a run.
    /// Uses Application.persistentDataPath for cross-platform compatibility.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        [SerializeField] private string saveFileName = "empire_save.json";

        // Cached data loaded on Awake
        private SaveData cachedData;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load existing save or create fresh
            cachedData = LoadFromDisk();
        }

        private void OnEnable()
        {
            EventBus.OnGameOver += OnGameOver;
        }

        private void OnDisable()
        {
            EventBus.OnGameOver -= OnGameOver;
        }

        // ------------------------------------------------------------------
        // EventBus callback
        // ------------------------------------------------------------------

        private void OnGameOver(bool won)
        {
            if (cachedData == null)
                cachedData = new SaveData();

            cachedData.runsPlayed++;

            if (won)
                cachedData.runsWon++;

            cachedData.activeRun = null;

            // Calculate score from GameManager
            var gm = GameManager.Instance;
            if (gm != null)
            {
                int score = CalculateRunScore(gm, won);
                if (score > cachedData.bestScore)
                    cachedData.bestScore = score;

                // XP earned from the run (simplified: score / 10)
                int xpEarned = Mathf.Max(score / 10, 10);
                cachedData.totalXP += xpEarned;
            }

            Save(cachedData);
        }

        // ------------------------------------------------------------------
        // Public API
        // ------------------------------------------------------------------

        /// <summary>
        /// Serializes the save data to JSON and writes it to disk.
        /// </summary>
        public void Save(SaveData data)
        {
            if (data == null)
            {
                Debug.LogWarning("[SaveManager] Attempted to save null data.");
                return;
            }

            cachedData = data;

            try
            {
                string json = JsonUtility.ToJson(data, prettyPrint: true);
                string path = GetSavePath();
                File.WriteAllText(path, json);
                Debug.Log($"[SaveManager] Saved to {path}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Save failed: {e.Message}");
            }
        }

        /// <summary>
        /// Returns the cached save data (loaded on Awake).
        /// </summary>
        public SaveData Load()
        {
            if (cachedData == null)
                cachedData = LoadFromDisk();

            return cachedData;
        }

        /// <summary>
        /// Deletes the save file from disk and resets cached data.
        /// </summary>
        public void DeleteSave()
        {
            string path = GetSavePath();

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    Debug.Log("[SaveManager] Save file deleted.");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SaveManager] Delete failed: {e.Message}");
                }
            }

            cachedData = new SaveData();
        }

        /// <summary>
        /// Returns the full path to the save file.
        /// </summary>
        public string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, saveFileName);
        }

        /// <summary>
        /// Returns true if a save file exists on disk.
        /// </summary>
        public bool HasSave()
        {
            return File.Exists(GetSavePath());
        }

        public bool HasRunSave()
        {
            return cachedData != null && cachedData.activeRun != null && cachedData.activeRun.HasData();
        }

        public RunSaveData LoadRun()
        {
            if (cachedData == null)
                cachedData = LoadFromDisk();

            return cachedData != null && cachedData.activeRun != null && cachedData.activeRun.HasData()
                ? cachedData.activeRun
                : null;
        }

        public void SaveRun(RunSaveData runData)
        {
            if (runData == null)
                return;

            if (cachedData == null)
                cachedData = new SaveData();

            cachedData.activeRun = runData;
            Save(cachedData);
        }

        public void DeleteRunSave()
        {
            if (cachedData == null)
                cachedData = LoadFromDisk();

            if (cachedData == null)
                cachedData = new SaveData();

            cachedData.activeRun = null;
            Save(cachedData);
        }

        /// <summary>
        /// Unlocks a card by ID if not already unlocked.
        /// </summary>
        public void UnlockCard(string cardId)
        {
            if (cachedData == null)
                cachedData = new SaveData();

            if (!cachedData.unlockedCardIds.Contains(cardId))
            {
                cachedData.unlockedCardIds.Add(cardId);
                Save(cachedData);
            }
        }

        /// <summary>
        /// Returns true if the card with the given ID has been unlocked.
        /// </summary>
        public bool IsCardUnlocked(string cardId)
        {
            if (cachedData == null)
                return false;

            return cachedData.unlockedCardIds.Contains(cardId);
        }

        // ------------------------------------------------------------------
        // Internal
        // ------------------------------------------------------------------

        private SaveData LoadFromDisk()
        {
            string path = GetSavePath();

            if (!File.Exists(path))
            {
                Debug.Log("[SaveManager] No save file found, returning default data.");
                return new SaveData();
            }

            try
            {
                string json = File.ReadAllText(path);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log("[SaveManager] Save loaded successfully.");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Load failed: {e.Message}");
                return new SaveData();
            }
        }

        /// <summary>
        /// Calculates the total score for the run based on GDD scoring rules.
        /// </summary>
        private int CalculateRunScore(GameManager gm, bool won)
        {
            int score = 0;

            score += gm.PlayerCustomers * Constants.SCORE_CUSTOMER_SHARE;
            score += gm.PlayerMoney * Constants.SCORE_MONEY;

            // Active combos score (GDD Section 10.3)
            if (gm.ComboSystem != null)
                score += gm.ComboSystem.ActiveCombos.Count * Constants.SCORE_COMBO;

            // Active businesses score (GDD Section 10.3)
            if (gm.BoardManager != null)
                score += gm.BoardManager.GetActiveBusinessCount() * Constants.SCORE_BUSINESS;

            // Early finish bonus
            int turnsRemaining = Constants.MAX_TURNS - gm.CurrentTurn;
            if (turnsRemaining > 0 && won)
                score += turnsRemaining * Constants.SCORE_EARLY_FINISH;

            // FBI evasion bonus (low risk = higher bonus)
            if (gm.FBIRisk < 0.1f)
                score += Constants.SCORE_FBI_EVASION * 3;
            else if (gm.FBIRisk < 0.3f)
                score += Constants.SCORE_FBI_EVASION;

            // Win bonus
            if (won)
                score += Constants.SCORE_WIN_BONUS;

            return score;
        }
    }
}
