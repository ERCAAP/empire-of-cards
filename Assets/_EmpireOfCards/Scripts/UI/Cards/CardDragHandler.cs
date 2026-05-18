using UnityEngine;
using UnityEngine.EventSystems;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI.Cards
{
    /// <summary>
    /// Drag-and-drop handler for playing cards from the hand.
    /// Implements pointer and drag interfaces. All visual tweens
    /// are driven by Update() polling -- no coroutines.
    /// </summary>
    [RequireComponent(typeof(CardUI))]
    public class CardDragHandler : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Drag Settings")]
        [SerializeField] private float dragScale = 1.15f;
        [SerializeField] private float hoverScale = 1.08f;
        [SerializeField] private float scaleSpeed = 10f;

        // Runtime
        private CardUI cardUI;
        private RectTransform rectTransform;
        private Canvas parentCanvas;
        private CanvasGroup canvasGroup;

        private Vector2 originalPosition;
        private Quaternion originalRotation;
        private int originalSiblingIndex;
        private Transform originalParent;

        private float targetScale = 1f;
        private bool isDragging;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void Awake()
        {
            cardUI = GetComponent<CardUI>();
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        private void Start()
        {
            parentCanvas = GetComponentInParent<Canvas>();
        }

        private void Update()
        {
            // Smooth scale transition (Update polling, no coroutine)
            float currentScale = transform.localScale.x;
            if (!Mathf.Approximately(currentScale, targetScale))
            {
                float newScale = Mathf.MoveTowards(currentScale, targetScale, scaleSpeed * Time.deltaTime);
                transform.localScale = Vector3.one * newScale;
            }
        }

        // ------------------------------------------------------------------
        // Drag
        // ------------------------------------------------------------------

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (cardUI == null || !cardUI.IsInteractable)
            {
                eventData.pointerDrag = null;
                return;
            }

            // Check PlayPhase and available actions
            var gm = GameManager.Instance;
            if (gm == null || gm.PlayerActions <= 0)
            {
                eventData.pointerDrag = null;
                return;
            }

            isDragging = true;

            // Store original transform state for snap-back
            originalPosition = rectTransform.anchoredPosition;
            originalRotation = rectTransform.localRotation;
            originalSiblingIndex = transform.GetSiblingIndex();
            originalParent = transform.parent;

            // Move to top of hierarchy so the card renders above everything
            transform.SetParent(parentCanvas != null ? parentCanvas.transform : transform.parent);
            transform.SetAsLastSibling();

            // Scale up
            targetScale = dragScale;

            // Remove rotation so the card is upright while dragging
            rectTransform.localRotation = Quaternion.identity;

            // Let raycasts pass through the dragged card to hit drop zones
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.85f;

            // Notify drop zones to show valid highlights
            DropZone.BroadcastDragStarted(cardUI);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging)
                return;

            // Follow the pointer
            if (parentCanvas != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentCanvas.transform as RectTransform,
                    eventData.position,
                    parentCanvas.worldCamera,
                    out Vector2 localPoint);

                rectTransform.anchoredPosition = localPoint;
            }
            else
            {
                rectTransform.position += (Vector3)eventData.delta;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging)
                return;

            isDragging = false;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
            targetScale = 1f;

            // Check whether we landed on a valid drop zone
            bool accepted = false;

            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                DropZone dropZone = eventData.pointerCurrentRaycast.gameObject
                    .GetComponentInParent<DropZone>();

                if (dropZone != null && dropZone.CanAccept(cardUI))
                {
                    dropZone.AcceptCard(cardUI);
                    accepted = true;
                }
            }

            if (!accepted)
            {
                // Return card to hand
                transform.SetParent(originalParent);
                transform.SetSiblingIndex(originalSiblingIndex);
                rectTransform.anchoredPosition = originalPosition;
                rectTransform.localRotation = originalRotation;
            }

            // Notify drop zones to hide highlights
            DropZone.BroadcastDragEnded();
        }

        // ------------------------------------------------------------------
        // Hover
        // ------------------------------------------------------------------

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isDragging || cardUI == null || !cardUI.IsInteractable)
                return;

            targetScale = hoverScale;
            cardUI.ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isDragging)
                return;

            targetScale = 1f;

            if (cardUI != null)
                cardUI.HideTooltip();
        }
    }
}
