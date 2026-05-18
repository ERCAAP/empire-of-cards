using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        public float bestScore;
    }

    /// <summary>
    /// Handles JSON-based save/load for meta-progression data.
    /// Uses Application.persistentDataPath for cross-platform compatibility.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        [SerializeField] private string saveFileName = "empire_save.json";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

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
        /// Loads save data from disk. Returns a new SaveData if no file exists.
        /// </summary>
        public SaveData Load()
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
        /// Deletes the save file from disk.
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
    }
}
