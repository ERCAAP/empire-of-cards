using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Save
{
    // ── Save Data Structures ───────────────────────────────────────

    [Serializable]
    public class SaveData
    {
        // Meta progression
        public int totalRuns;
        public int totalExits;
        public int totalCashEarned;
        public List<string> unlockedSectors = new List<string>();
        public List<string> unlockedPerks = new List<string>();
        public float carryOverCash;

        // Active run (null if no run in progress)
        public RunSaveData activeRun;
    }

    [Serializable]
    public class RunSaveData
    {
        public string businessName;
        public string sectorType;
        public int currentTurn;
        public int money;
        public float demand;
        public float capacity;
        public float quality;
        public float rating;
        public float staffStability;
        public float legalRisk;
        public float marketShare;
        public float hygiene;

        // Deck state
        public List<string> boardCardIds = new List<string>();
        public List<string> handCardIds = new List<string>();
        public List<string> drawPileIds = new List<string>();
        public List<string> discardPileIds = new List<string>();

        // Rival state
        public float rivalRating;
        public float rivalShare;
        public int rivalCash;
    }

    // ── SaveManager ────────────────────────────────────────────────

    public class SaveManager : MonoBehaviour
    {
        GameManager _gameManager;
        string _savePath;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _savePath = Path.Combine(Application.persistentDataPath, "empire_save.json");
            Debug.Log($"[SaveManager] Initialized. Path: {_savePath}");
        }

        // ── Save ───────────────────────────────────────────────────

        public void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_savePath, json);
            Debug.Log("[SaveManager] Game saved.");
        }

        // ── Load ───────────────────────────────────────────────────

        public SaveData Load()
        {
            if (!HasSave())
            {
                Debug.LogWarning("[SaveManager] No save file found, returning default.");
                return new SaveData();
            }

            string json = File.ReadAllText(_savePath);
            var data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("[SaveManager] Game loaded.");
            return data;
        }

        // ── Queries ────────────────────────────────────────────────

        public bool HasSave()
        {
            return File.Exists(_savePath);
        }

        public bool HasActiveRun()
        {
            if (!HasSave()) return false;
            var data = Load();
            return data.activeRun != null;
        }

        // ── Delete ─────────────────────────────────────────────────

        public void DeleteSave()
        {
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
                Debug.Log("[SaveManager] Save file deleted.");
            }
        }

        // ── Checkpoint (save current run state) ────────────────────

        public void SaveCheckpoint()
        {
            if (_gameManager == null || !_gameManager.IsRunning)
            {
                Debug.LogWarning("[SaveManager] No active run to checkpoint.");
                return;
            }

            var res = _gameManager.Resources;

            // Load existing meta or create new
            SaveData save = HasSave() ? Load() : new SaveData();

            save.activeRun = new RunSaveData
            {
                businessName   = _gameManager.BusinessName,
                sectorType     = _gameManager.SelectedSector.ToString(),
                currentTurn    = _gameManager.CurrentTurn,
                money          = res.GetMoney(),
                demand         = res.GetDemand(),
                capacity       = res.GetCapacity(),
                quality        = res.GetQuality(),
                rating         = res.GetRating(),
                staffStability = res.GetStaffStability(),
                legalRisk      = res.GetLegalRisk(),
                marketShare    = res.GetMarketShare(),
                hygiene        = res.GetHygiene()
            };

            // Card lists would be populated by DeckManager/BoardManager
            // when they expose their state -- placeholder for now
            save.activeRun.boardCardIds  = new List<string>();
            save.activeRun.handCardIds   = new List<string>();
            save.activeRun.drawPileIds   = new List<string>();
            save.activeRun.discardPileIds = new List<string>();

            // Rival state from RivalAI via GameManager reference
            var rivalAI = _gameManager.RivalAI as Gameplay.RivalAI;
            if (rivalAI != null)
            {
                save.activeRun.rivalRating = rivalAI.RivalRating;
                save.activeRun.rivalShare  = rivalAI.RivalShare;
                save.activeRun.rivalCash   = rivalAI.RivalCash;
            }

            Save(save);
            Debug.Log($"[SaveManager] Checkpoint saved at turn {_gameManager.CurrentTurn}.");
        }

        // ── Load Checkpoint ────────────────────────────────────────

        public RunSaveData LoadCheckpoint()
        {
            if (!HasActiveRun())
            {
                Debug.LogWarning("[SaveManager] No active run checkpoint found.");
                return null;
            }

            var data = Load();
            Debug.Log($"[SaveManager] Checkpoint loaded: turn {data.activeRun.currentTurn}.");
            return data.activeRun;
        }

        // ── Clear Active Run ───────────────────────────────────────

        public void ClearActiveRun()
        {
            if (!HasSave()) return;

            var data = Load();
            data.activeRun = null;
            Save(data);
            Debug.Log("[SaveManager] Active run cleared.");
        }
    }
}
