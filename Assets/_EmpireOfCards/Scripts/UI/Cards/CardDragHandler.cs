using UnityEngine;
using UnityEngine.EventSystems;
using EmpireOfCards.Data;

namespace EmpireOfCards.UI.Cards
{
    /// <summary>
    /// Handles drag-and-drop behaviour for a card. Attach alongside CardUI.
    /// Implements pointer enter/exit for hover feedback.
    /// </summary>
    [RequireComponent(typeof(CardUI))]
    public class CardDragHandler : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        [Header("References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Settings")]
        [SerializeField] private float dragScale = 1.1f;
        [SerializeField] private float hoverScale = 1.05f;
        [SerializeField] private float snapBackSpeed = 15f;

        private Vector2 originalPosition;
        private Vector3 originalScale;
        private Transform originalParent;
        private bool isDragging;
        private CardUI cardUI;

        private void Awake()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            cardUI = GetComponent<CardUI>();
        }

        // --- Drag Handlers ---

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            originalPosition = rectTransform.anchoredPosition;
            originalScale = rectTransform.localScale;
            originalParent = transform.parent;

            // Scale up while dragging
            rectTransform.localScale = originalScale * dragScale;

            // Allow raycasts to pass through so drop zones can detect
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0.85f;
            }

            // Move to top of sibling order so card renders above others
            transform.SetAsLastSibling();

            ShowValidDropZones(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging || canvas == null)
                return;

            // Follow the mouse, accounting for canvas scale
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;

            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
            }

            ShowValidDropZones(false);

            // Check if we dropped on a valid drop zone
            bool droppedOnZone = false;

            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                DropZone dropZone = eventData.pointerCurrentRaycast.gameObject
                    .GetComponentInParent<DropZone>();

                if (dropZone != null && cardUI != null)
                {
                    CardData data = cardUI.GetCardData();
                    if (data != null && dropZone.IsValidDrop(data))
                    {
                        dropZone.OnCardDropped(data);
                        droppedOnZone = true;
                    }
                }
            }

            if (!droppedOnZone)
            {
                // Snap back to original position
                rectTransform.anchoredPosition = originalPosition;
            }

            rectTransform.localScale = originalScale;
        }

        // --- Hover Handlers ---

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isDragging)
                return;

            rectTransform.localScale = Vector3.one * hoverScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isDragging)
                return;

            rectTransform.localScale = Vector3.one;
        }

        // --- Helpers ---

        /// <summary>
        /// Highlights or un-highlights all valid drop zones in the scene for this card.
        /// </summary>
        private void ShowValidDropZones(bool show)
        {
            DropZone[] zones = FindObjectsByType<DropZone>(FindObjectsSortMode.None);
            CardData data = cardUI != null ? cardUI.GetCardData() : null;

            foreach (DropZone zone in zones)
            {
                if (show && data != null && zone.IsValidDrop(data))
                {
                    zone.ShowHighlight(true, Color.green);
                }
                else
                {
                    zone.ShowHighlight(false, Color.clear);
                }
            }
        }
    }
}
