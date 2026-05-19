using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmpireOfCards.Core
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        public enum Language { EN, TR }

        [SerializeField] private Language currentLanguage = Language.EN;

        // Key -> localized string
        private Dictionary<string, string> _strings = new Dictionary<string, string>();

        // Event for UI refresh when language changes
        public static event Action OnLanguageChanged;

        public Language CurrentLanguage => currentLanguage;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLanguage(currentLanguage);
        }

        public void SetLanguage(Language lang)
        {
            if (lang == currentLanguage) return;
            currentLanguage = lang;
            LoadLanguage(lang);
            OnLanguageChanged?.Invoke();
        }

        // Main lookup - returns key itself if not found (fail-safe)
        public static string Get(string key)
        {
            if (Instance == null) return key;
            return Instance._strings.TryGetValue(key, out string val) ? val : key;
        }

        // Lookup with explicit fallback - returns fallback if key not found
        public static string GetWithFallback(string key, string fallback)
        {
            if (Instance == null) return fallback;
            return Instance._strings.TryGetValue(key, out string val) ? val : fallback;
        }

        // Shorthand with format args
        public static string Get(string key, params object[] args)
        {
            string template = Get(key);
            try { return string.Format(template, args); }
            catch { return template; }
        }

        private void LoadLanguage(Language lang)
        {
            _strings.Clear();
            string fileName = $"Localization/{lang.ToString().ToLower()}";
            var textAsset = Resources.Load<TextAsset>(fileName);

            if (textAsset == null)
            {
                Debug.LogWarning($"[Localization] Missing file: Resources/{fileName}.json");
                return;
            }

            var data = JsonUtility.FromJson<LocalizationData>(textAsset.text);
            if (data?.entries == null) return;

            foreach (var entry in data.entries)
                _strings[entry.key] = entry.value;

            Debug.Log($"[Localization] Loaded {_strings.Count} strings for {lang}");
        }
    }

    [Serializable]
    public class LocalizationData
    {
        public LocalizationEntry[] entries;
    }

    [Serializable]
    public class LocalizationEntry
    {
        public string key;
        public string value;
    }
}
