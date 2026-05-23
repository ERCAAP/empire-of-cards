using TMPro;
using UnityEngine;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;
using EmpireOfCards.Presentation;

namespace EmpireOfCards.World
{
    [RequireComponent(typeof(BoxCollider))]
    public class QuestionDropZone3D : MonoBehaviour, IDropZoneVisual3D
    {
        [SerializeField] private int questionIndex;
        [SerializeField] private QuestionManager questionManager;

        private MeshRenderer _renderer;
        private TextMeshPro _label;
        private Color _baseColor;
        private Color _pulseColor;
        private bool _isPulsing;
        private string _currentText = "QUESTION";

        public int QuestionIndex => questionIndex;

        public void RuntimeInit(int index)
        {
            questionIndex = index;
        }

        public void SetQuestionManager(QuestionManager manager)
        {
            questionManager = manager;
        }

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _baseColor = _renderer != null ? _renderer.material.color : ControlDeskTheme.MidBand;
            _pulseColor = ControlDeskTheme.GuidedPulse;
            CreateLabel();
        }

        private void Update()
        {
            if (!_isPulsing || _renderer == null)
                return;

            float alpha = 0.5f + 0.25f * Mathf.Sin(Time.time * 7f);
            _renderer.material.color = Color.Lerp(_baseColor, _pulseColor, alpha);
        }

        public bool CanAccept(CardData card)
        {
            return questionManager != null && questionManager.CanCommitCardToQuestion(questionIndex, card);
        }

        public void SetHighlight(bool on, bool valid)
        {
            if (_renderer == null)
                return;

            if (!on)
            {
                _renderer.material.color = _baseColor;
                return;
            }

            _renderer.material.color = valid ? ControlDeskTheme.ValidHighlight : ControlDeskTheme.InvalidHighlight;
        }

        public void SetPulse(bool on)
        {
            _isPulsing = on;
            if (!on && _renderer != null)
                _renderer.material.color = _baseColor;
        }

        public void SetQuestionText(string text)
        {
            _currentText = string.IsNullOrWhiteSpace(text) ? "QUESTION" : text;
            if (_label != null)
                _label.text = _currentText;
        }

        public void ShowPreview(string text, bool valid)
        {
            if (_label == null)
                return;

            _label.text = string.IsNullOrWhiteSpace(text) ? _label.text : text;
            _label.color = valid ? ControlDeskTheme.TextPrimary : new Color(1f, 0.64f, 0.64f);
        }

        public void ClearPreview()
        {
            if (_label != null)
            {
                _label.text = _currentText;
                _label.color = ControlDeskTheme.TextPrimary;
            }
        }

        private void CreateLabel()
        {
            var go = new GameObject("QuestionLabel");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(0f, 0.34f, 0f);
            go.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            go.transform.localScale = Vector3.one * 0.055f;
            _label = go.AddComponent<TextMeshPro>();
            _label.fontSize = 4.8f;
            _label.alignment = TextAlignmentOptions.Center;
            _label.textWrappingMode = TextWrappingModes.Normal;
            _label.overflowMode = TextOverflowModes.Overflow;
            _label.color = ControlDeskTheme.TextPrimary;
            _label.text = "QUESTION";
        }
    }
}
