using UnityEngine;
using TMPro;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string locKey;
        private TMP_Text _text;

        private void Awake() { _text = GetComponent<TMP_Text>(); }

        private void OnEnable()
        {
            Refresh();
            LocalizationManager.OnLanguageChanged += Refresh;
        }

        private void OnDisable()
        {
            LocalizationManager.OnLanguageChanged -= Refresh;
        }

        public void SetKey(string key)
        {
            locKey = key;
            Refresh();
        }

        private void Refresh()
        {
            if (_text != null && !string.IsNullOrEmpty(locKey))
                _text.text = LocalizationManager.Get(locKey);
        }
    }
}
