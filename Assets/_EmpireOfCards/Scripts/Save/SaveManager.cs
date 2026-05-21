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
        public string lastPlayedRunId;
        public List<RunSaveData> runs = new List<RunSaveData>();
        public RunSaveData activeRun;
    }

    [Serializable]
    public class RunSaveData
    {
        public string slotId;
        public string runName;
        public string runCategoryId;
        public string runCategoryLabel;
        public long savedAtUnixSeconds;
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
            NormalizeRunData(cachedData);
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

            var gm = GameManager.Instance;
            cachedData.runsPlayed++;

            if (won)
                cachedData.runsWon++;

            string currentSlotId = gm != null ? gm.CurrentRunSlotId : null;
            RemoveRunInternal(currentSlotId);

            // Calculate score from GameManager
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
            return HasRunSave(null);
        }

        public bool HasRunSave(string slotId)
        {
            return LoadRun(slotId) != null;
        }

        public List<RunSaveData> ListRuns()
        {
            if (cachedData == null)
                cachedData = LoadFromDisk();

            NormalizeRunData(cachedData);

            return cachedData.runs
                .FindAll(run => run != null && run.HasData());
        }

        public RunSaveData LoadRun()
        {
            return LoadRun(null);
        }

        public RunSaveData LoadRun(string slotId)
        {
            if (cachedData == null)
                cachedData = LoadFromDisk();

            NormalizeRunData(cachedData);

            if (!string.IsNullOrWhiteSpace(slotId))
            {
                RunSaveData selectedRun = cachedData.runs.Find(run => run != null && run.slotId == slotId && run.HasData());
                if (selectedRun != null)
                {
                    cachedData.lastPlayedRunId = selectedRun.slotId;
                    cachedData.activeRun = selectedRun;
                    return selectedRun;
                }
            }

            if (!string.IsNullOrWhiteSpace(cachedData.lastPlayedRunId))
            {
                RunSaveData lastPlayed = cachedData.runs.Find(run => run != null && run.slotId == cachedData.lastPlayedRunId && run.HasData());
                if (lastPlayed != null)
                {
                    cachedData.activeRun = lastPlayed;
                    return lastPlayed;
                }
            }

            RunSaveData firstRun = cachedData.runs.Find(run => run != null && run.HasData());
            cachedData.activeRun = firstRun;
            if (firstRun != null)
                cachedData.lastPlayedRunId = firstRun.slotId;

            return firstRun;
        }

        public void SaveRun(string slotId, RunSaveData runData)
        {
            if (runData == null)
                return;

            if (cachedData == null)
                cachedData = new SaveData();

            NormalizeRunData(cachedData);

            string resolvedSlotId = string.IsNullOrWhiteSpace(slotId)
                ? (!string.IsNullOrWhiteSpace(runData.slotId) ? runData.slotId : CreateRunSlotId())
                : slotId;

            runData.slotId = resolvedSlotId;
            runData.savedAtUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            int existingIndex = cachedData.runs.FindIndex(run => run != null && run.slotId == resolvedSlotId);
            if (existingIndex >= 0)
                cachedData.runs[existingIndex] = runData;
            else
                cachedData.runs.Add(runData);

            cachedData.lastPlayedRunId = resolvedSlotId;
            cachedData.activeRun = runData;
            Save(cachedData);
        }

        public void SaveRun(RunSaveData runData)
        {
            SaveRun(null, runData);
        }

        public void DeleteRunSave()
        {
            DeleteRunSave(null);
        }

        public void DeleteRunSave(string slotId)
        {
            if (cachedData == null)
                cachedData = LoadFromDisk();

            if (cachedData == null)
                cachedData = new SaveData();

            NormalizeRunData(cachedData);
            RemoveRunInternal(slotId);
            Save(cachedData);
        }

        public string CreateRunSlotId()
        {
            return Guid.NewGuid().ToString("N");
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
                NormalizeRunData(data);
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

        private void NormalizeRunData(SaveData data)
        {
            if (data == null)
                return;

            if (data.unlockedCardIds == null)
                data.unlockedCardIds = new List<string>();

            if (data.runs == null)
                data.runs = new List<RunSaveData>();

            if (data.activeRun != null && data.activeRun.HasData())
            {
                if (string.IsNullOrWhiteSpace(data.activeRun.slotId))
                    data.activeRun.slotId = CreateRunSlotId();

                if (data.activeRun.savedAtUnixSeconds <= 0)
                    data.activeRun.savedAtUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (!data.runs.Exists(run => run != null && run.slotId == data.activeRun.slotId))
                    data.runs.Add(data.activeRun);
            }

            for (int i = data.runs.Count - 1; i >= 0; i--)
            {
                RunSaveData run = data.runs[i];
                if (run == null || !run.HasData())
                {
                    data.runs.RemoveAt(i);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(run.slotId))
                    run.slotId = CreateRunSlotId();

                if (run.savedAtUnixSeconds <= 0)
                    run.savedAtUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }

            if (!string.IsNullOrWhiteSpace(data.lastPlayedRunId))
            {
                RunSaveData lastPlayed = data.runs.Find(run => run != null && run.slotId == data.lastPlayedRunId);
                data.activeRun = lastPlayed;
            }

            if ((data.activeRun == null || !data.activeRun.HasData()) && data.runs.Count > 0)
            {
                data.activeRun = data.runs[0];
                data.lastPlayedRunId = data.activeRun.slotId;
            }
            else if (data.activeRun == null)
            {
                data.lastPlayedRunId = null;
            }
        }

        private void RemoveRunInternal(string slotId)
        {
            NormalizeRunData(cachedData);

            string resolvedSlotId = slotId;
            if (string.IsNullOrWhiteSpace(resolvedSlotId))
                resolvedSlotId = !string.IsNullOrWhiteSpace(cachedData.lastPlayedRunId) ? cachedData.lastPlayedRunId : null;

            if (string.IsNullOrWhiteSpace(resolvedSlotId))
            {
                cachedData.activeRun = null;
                cachedData.lastPlayedRunId = null;
                return;
            }

            cachedData.runs.RemoveAll(run => run != null && run.slotId == resolvedSlotId);
            cachedData.activeRun = null;
            cachedData.lastPlayedRunId = null;

            RunSaveData fallbackRun = cachedData.runs.Find(run => run != null && run.HasData());
            if (fallbackRun != null)
            {
                cachedData.activeRun = fallbackRun;
                cachedData.lastPlayedRunId = fallbackRun.slotId;
            }
        }
    }
}
