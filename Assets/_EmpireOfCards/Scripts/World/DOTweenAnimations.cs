using UnityEngine;
using TMPro;
using EmpireOfCards.Core;
using DG.Tweening;

namespace EmpireOfCards.World
{
    /// <summary>
    /// Central animation helper. All DOTween animations in one place.
    /// Other scripts call static methods here. Subscribes to EventBus for
    /// auto-triggering turn banners, era transitions, and stat popups.
    /// </summary>
    public class DOTweenAnimations : MonoBehaviour
    {
        // ── Singleton-ish reference (set by bootstrap) ──────────────
        static DOTweenAnimations _instance;
        public static DOTweenAnimations Instance => _instance;

        // ── Config ──────────────────────────────────────────────────
        static readonly Color COL_POSITIVE = new Color(0.2f, 0.9f, 0.3f);
        static readonly Color COL_NEGATIVE = new Color(0.9f, 0.2f, 0.2f);
        static readonly Color COL_NEUTRAL  = new Color(0.9f, 0.9f, 0.3f);
        static readonly Color COL_MONEY    = new Color(1f, 0.85f, 0.2f);
        static readonly Color COL_CRISIS   = new Color(0.9f, 0.1f, 0.1f);

        void Awake()
        {
            _instance = this;
        }

        void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        // ── EventBus subscriptions ──────────────────────────────────

        void OnEnable()
        {
            EventBus.OnTurnStarted += HandleTurnStarted;
            EventBus.OnEraChanged += HandleEraChanged;
            EventBus.OnMoneyChanged += HandleMoneyChanged;
            EventBus.OnRatingChanged += HandleRatingChanged;
            EventBus.OnCrisisTriggered += HandleCrisis;
            EventBus.OnCustomersServed += HandleCustomersServed;
        }

        void OnDisable()
        {
            EventBus.OnTurnStarted -= HandleTurnStarted;
            EventBus.OnEraChanged -= HandleEraChanged;
            EventBus.OnMoneyChanged -= HandleMoneyChanged;
            EventBus.OnRatingChanged -= HandleRatingChanged;
            EventBus.OnCrisisTriggered -= HandleCrisis;
            EventBus.OnCustomersServed -= HandleCustomersServed;
        }

        // ── Auto-trigger handlers ───────────────────────────────────

        int _lastMoney;
        float _lastRating;

        void HandleTurnStarted(int turn)
        {
            string era = GameManager.GetEra(turn).ToString();
            string season = GameManager.GetSeason(turn).ToString();
            TurnStartBanner($"TUR {turn}  |  {era}  |  {season}");
        }

        void HandleEraChanged(Era era)
        {
            EraTransitionEffect(era.ToString());
        }

        void HandleMoneyChanged(int newMoney)
        {
            int delta = newMoney - _lastMoney;
            if (delta != 0 && _lastMoney != 0)
            {
                Color col = delta > 0 ? COL_POSITIVE : COL_NEGATIVE;
                string prefix = delta > 0 ? "+" : "";
                StatChangePopup(new Vector3(-5f, 1.5f, -5f), $"{prefix}${delta}", col);
            }
            _lastMoney = newMoney;
        }

        void HandleRatingChanged(float newRating)
        {
            float delta = newRating - _lastRating;
            if (Mathf.Abs(delta) > 0.01f && _lastRating > 0f)
            {
                Color col = delta > 0 ? COL_POSITIVE : COL_NEGATIVE;
                string prefix = delta > 0 ? "+" : "";
                StatChangePopup(new Vector3(5f, 1.5f, 7f), $"{prefix}{delta:F1} Rating", col);
            }
            _lastRating = newRating;
        }

        void HandleCrisis(CrisisType crisis)
        {
            CrisisScreenFlash();
        }

        void HandleCustomersServed(int served, int waited, int left)
        {
            if (served > 0)
                StatChangePopup(new Vector3(0f, 1.5f, 12f), $"{served} musteri hizmet edildi", COL_POSITIVE);
            if (left > 0)
                StatChangePopup(new Vector3(2f, 1.5f, 12f), $"{left} musteri gitti!", COL_NEGATIVE);
        }

        // ═══════════════════════════════════════════════════════════
        // ██  CARD ANIMATIONS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Card flies from hand to target slot with arc, scale punch on land.
        /// </summary>
        public static void CardPlaceAnimation(Transform card, Vector3 targetPos)
        {
            if (card == null) return;

            Vector3 startPos = card.position;
            Vector3 midPoint = (startPos + targetPos) * 0.5f + Vector3.up * 2f;

            var seq = DOTween.Sequence();

            // Arc movement via 3 waypoints
            Vector3[] path = { startPos, midPoint, targetPos };
            seq.Append(card.DOPath(path, 0.6f, PathType.CatmullRom)
                .SetEase(Ease.InOutQuad));

            // Scale punch on landing
            seq.Append(card.DOPunchScale(Vector3.one * 0.15f, 0.3f, 8, 0.5f));

            // Rotation flatten
            seq.Join(card.DORotate(Vector3.zero, 0.3f).SetEase(Ease.OutBack));
        }

        /// <summary>
        /// Card lifts slightly and scales up on hover.
        /// </summary>
        public static Tween CardHoverAnimation(Transform card)
        {
            if (card == null) return null;

            var seq = DOTween.Sequence();
            seq.Append(card.DOMoveY(card.position.y + 0.3f, 0.2f).SetEase(Ease.OutQuad));
            seq.Join(card.DOScale(card.localScale * 1.08f, 0.2f).SetEase(Ease.OutQuad));
            return seq;
        }

        /// <summary>
        /// Card returns to normal position and scale.
        /// </summary>
        public static Tween CardUnhoverAnimation(Transform card, Vector3 originalPos, Vector3 originalScale)
        {
            if (card == null) return null;

            var seq = DOTween.Sequence();
            seq.Append(card.DOMove(originalPos, 0.15f).SetEase(Ease.InQuad));
            seq.Join(card.DOScale(originalScale, 0.15f).SetEase(Ease.InQuad));
            return seq;
        }

        /// <summary>
        /// Card flies in from above with stagger delay per index.
        /// </summary>
        public static void CardDrawAnimation(Transform card, Vector3 handPos, int index)
        {
            if (card == null) return;

            Vector3 startPos = handPos + Vector3.up * 5f + Vector3.right * Random.Range(-1f, 1f);
            card.position = startPos;
            card.localScale = Vector3.zero;

            float delay = index * 0.12f;

            DOTween.Sequence()
                .SetDelay(delay)
                .Append(card.DOMove(handPos, 0.5f).SetEase(Ease.OutBack))
                .Join(card.DOScale(new Vector3(1f, 0.05f, 1.4f), 0.4f).SetEase(Ease.OutBack))
                .Append(card.DOPunchScale(Vector3.one * 0.05f, 0.2f, 6, 0.3f));
        }

        /// <summary>
        /// Card shrinks and fades out, then deactivates.
        /// </summary>
        public static void CardDiscardAnimation(Transform card)
        {
            if (card == null) return;

            DOTween.Sequence()
                .Append(card.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack))
                .Join(card.DOMoveY(card.position.y - 1f, 0.4f))
                .OnComplete(() =>
                {
                    if (card != null && card.gameObject != null)
                        card.gameObject.SetActive(false);
                });
        }

        /// <summary>
        /// Card grows slightly while being dragged (feedback).
        /// </summary>
        public static Tween CardDragStartAnimation(Transform card)
        {
            if (card == null) return null;
            return card.DOScale(card.localScale * 1.12f, 0.15f).SetEase(Ease.OutQuad);
        }

        /// <summary>
        /// Card returns to normal size after drag ends.
        /// </summary>
        public static Tween CardDragEndAnimation(Transform card, Vector3 normalScale)
        {
            if (card == null) return null;
            return card.DOScale(normalScale, 0.15f).SetEase(Ease.InQuad);
        }

        // ═══════════════════════════════════════════════════════════
        // ██  STAT ANIMATIONS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Floating text that rises and fades out (3D world space).
        /// </summary>
        public static void StatChangePopup(Vector3 worldPos, string text, Color color)
        {
            var go = new GameObject("StatPopup");
            go.transform.position = worldPos;
            go.transform.rotation = Quaternion.Euler(45f, 0f, 0f);

            var tmp = go.AddComponent<TextMeshPro>();
            tmp.text = text;
            tmp.fontSize = 4f;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.sortingOrder = 10;

            DOTween.Sequence()
                .Append(go.transform.DOMoveY(worldPos.y + 2f, 1.5f).SetEase(Ease.OutQuad))
                .Join(DOTween.To(() => tmp.color, c => tmp.color = c,
                    new Color(color.r, color.g, color.b, 0f), 1.5f).SetEase(Ease.InQuad))
                .Join(go.transform.DOScale(1.3f, 0.3f).SetEase(Ease.OutBack))
                .OnComplete(() => Object.Destroy(go));
        }

        /// <summary>
        /// Star rating pulse animation.
        /// </summary>
        public static void RatingStarPulse(Transform star, bool isUp)
        {
            if (star == null) return;

            Color pulseColor = isUp ? new Color(1f, 0.85f, 0f) : COL_NEGATIVE;
            var rend = star.GetComponent<Renderer>();
            if (rend != null && rend.material != null)
            {
                rend.material.DOColor(pulseColor, 0.2f)
                    .SetLoops(4, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }

            star.DOPunchScale(Vector3.one * 0.3f, 0.5f, 8, 0.3f);
        }

        /// <summary>
        /// Count-up / count-down number animation on a TMP text.
        /// </summary>
        public static Tween MoneyTickerAnimation(TMP_Text text, int from, int to)
        {
            if (text == null) return null;

            return DOTween.To(() => from, val =>
            {
                from = val;
                text.text = $"${val}";
            }, to, 0.8f).SetEase(Ease.OutQuad);
        }

        // ═══════════════════════════════════════════════════════════
        // ██  CRISIS ANIMATIONS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Red screen flash for crisis events.
        /// </summary>
        public void CrisisScreenFlash()
        {
            var flashGo = new GameObject("CrisisFlash");
            var canvas = flashGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;

            var imgGo = new GameObject("FlashImage");
            imgGo.transform.SetParent(flashGo.transform, false);

            var rt = imgGo.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var img = imgGo.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(0.8f, 0f, 0f, 0.5f);

            DOTween.Sequence()
                .Append(img.DOFade(0.6f, 0.1f))
                .Append(img.DOFade(0f, 0.5f).SetEase(Ease.InQuad))
                .OnComplete(() => Destroy(flashGo));
        }

        /// <summary>
        /// Crisis card drops from above with bounce.
        /// </summary>
        public static void CrisisCardDrop(Transform card, Vector3 center)
        {
            if (card == null) return;

            card.position = center + Vector3.up * 8f;
            card.localScale = Vector3.one * 1.5f;

            DOTween.Sequence()
                .Append(card.DOMove(center, 0.6f).SetEase(Ease.OutBounce))
                .Join(card.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack))
                .Append(card.DOPunchScale(Vector3.one * 0.1f, 0.3f, 6, 0.4f));
        }

        // ═══════════════════════════════════════════════════════════
        // ██  CUSTOMER ANIMATIONS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Customer cube walks from A to B.
        /// </summary>
        public static Tween CustomerWalkAnimation(Transform cube, Vector3 from, Vector3 to, float duration)
        {
            if (cube == null) return null;

            cube.position = from;

            // Slight bobbing while walking
            var seq = DOTween.Sequence();
            seq.Append(cube.DOMove(to, duration).SetEase(Ease.Linear));

            int steps = Mathf.Max(1, (int)(duration / 0.3f));
            for (int i = 0; i < steps; i++)
            {
                float delay = i * 0.3f;
                seq.Insert(delay, cube.DOMoveY(from.y + 0.05f, 0.15f)
                    .SetEase(Ease.OutQuad)
                    .SetLoops(2, LoopType.Yoyo));
            }

            return seq;
        }

        /// <summary>
        /// Customer enters shop: walks in, shrinks and disappears.
        /// </summary>
        public static void CustomerEnterShop(Transform cube, Vector3 doorPos)
        {
            if (cube == null) return;

            DOTween.Sequence()
                .Append(cube.DOMove(doorPos, 0.5f).SetEase(Ease.InQuad))
                .Append(cube.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack))
                .OnComplete(() =>
                {
                    if (cube != null && cube.gameObject != null)
                        cube.gameObject.SetActive(false);
                });
        }

        /// <summary>
        /// Angry customer leaves: walks fast, flashes red.
        /// </summary>
        public static void CustomerLeaveAngry(Transform cube, Vector3 exitPos)
        {
            if (cube == null) return;

            var rend = cube.GetComponent<Renderer>();

            var seq = DOTween.Sequence();
            seq.Append(cube.DOMove(exitPos, 0.8f).SetEase(Ease.InQuad));

            // Red flash while leaving
            if (rend != null && rend.material != null)
            {
                seq.Join(rend.material.DOColor(Color.red, 0.2f)
                    .SetLoops(4, LoopType.Yoyo));
            }

            seq.OnComplete(() =>
            {
                if (cube != null && cube.gameObject != null)
                    cube.gameObject.SetActive(false);
            });
        }

        // ═══════════════════════════════════════════════════════════
        // ██  MARKET BAR ANIMATIONS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Smooth bar resize for market share display.
        /// </summary>
        public static void MarketBarUpdate(Transform playerBar, Transform rivalBar, float playerShare, float rivalShare)
        {
            if (playerBar == null || rivalBar == null) return;

            float totalWidth = 8f;
            float playerWidth = Mathf.Max(0.1f, (playerShare / 100f) * totalWidth);
            float rivalWidth = Mathf.Max(0.1f, (rivalShare / 100f) * totalWidth);

            playerBar.DOScaleX(playerWidth, 0.5f).SetEase(Ease.OutQuad);
            rivalBar.DOScaleX(rivalWidth, 0.5f).SetEase(Ease.OutQuad);

            // Reposition so they stay adjacent
            float playerX = -totalWidth * 0.5f + playerWidth * 0.5f;
            float rivalX = totalWidth * 0.5f - rivalWidth * 0.5f;

            playerBar.DOLocalMoveX(playerX, 0.5f).SetEase(Ease.OutQuad);
            rivalBar.DOLocalMoveX(rivalX, 0.5f).SetEase(Ease.OutQuad);
        }

        // ═══════════════════════════════════════════════════════════
        // ██  TURN / ERA ANIMATIONS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Large text banner slides in from left, holds, slides out.
        /// </summary>
        public void TurnStartBanner(string text)
        {
            var canvasGo = new GameObject("TurnBannerCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 150;

            var tmpGo = new GameObject("BannerText");
            tmpGo.transform.SetParent(canvasGo.transform, false);

            var rt = tmpGo.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.4f);
            rt.anchorMax = new Vector2(1f, 0.6f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var tmp = tmpGo.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 48f;
            tmp.color = new Color(1f, 0.9f, 0.7f, 0f);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            // Background bar
            var bgGo = new GameObject("BannerBG");
            bgGo.transform.SetParent(canvasGo.transform, false);
            bgGo.transform.SetSiblingIndex(0);

            var bgRt = bgGo.AddComponent<RectTransform>();
            bgRt.anchorMin = new Vector2(0f, 0.38f);
            bgRt.anchorMax = new Vector2(1f, 0.62f);
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;

            var bgImg = bgGo.AddComponent<UnityEngine.UI.Image>();
            bgImg.color = new Color(0.05f, 0.05f, 0.08f, 0f);

            // Slide in -> hold -> slide out
            DOTween.Sequence()
                .Append(bgImg.DOFade(0.85f, 0.3f).SetEase(Ease.OutQuad))
                .Join(tmp.DOFade(1f, 0.3f).SetEase(Ease.OutQuad))
                .AppendInterval(1.2f)
                .Append(bgImg.DOFade(0f, 0.4f).SetEase(Ease.InQuad))
                .Join(tmp.DOFade(0f, 0.4f).SetEase(Ease.InQuad))
                .OnComplete(() => Destroy(canvasGo));
        }

        /// <summary>
        /// Screen flash + era name display for era transitions.
        /// </summary>
        public void EraTransitionEffect(string eraName)
        {
            // White flash
            var flashGo = new GameObject("EraFlash");
            var canvas = flashGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 250;

            var imgGo = new GameObject("FlashImg");
            imgGo.transform.SetParent(flashGo.transform, false);

            var rt = imgGo.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var img = imgGo.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(1f, 0.95f, 0.8f, 0f);

            // Era name text
            var textGo = new GameObject("EraText");
            textGo.transform.SetParent(flashGo.transform, false);

            var textRt = textGo.AddComponent<RectTransform>();
            textRt.anchorMin = new Vector2(0.2f, 0.35f);
            textRt.anchorMax = new Vector2(0.8f, 0.65f);
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = $"ERA: {eraName.ToUpper()}";
            tmp.fontSize = 72f;
            tmp.color = new Color(0.2f, 0.15f, 0.05f, 0f);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            DOTween.Sequence()
                // Flash in
                .Append(img.DOFade(0.7f, 0.2f))
                .Join(tmp.DOFade(1f, 0.3f))
                // Hold
                .AppendInterval(1.5f)
                // Fade out
                .Append(img.DOFade(0f, 0.5f).SetEase(Ease.InQuad))
                .Join(tmp.DOFade(0f, 0.5f).SetEase(Ease.InQuad))
                .OnComplete(() => Destroy(flashGo));
        }

        // ═══════════════════════════════════════════════════════════
        // ██  SLOT ANIMATIONS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Subtle pulse glow on empty slot markers.
        /// </summary>
        public static Tween SlotEmptyPulse(Transform slot)
        {
            if (slot == null) return null;

            return slot.DOScale(slot.localScale * 1.05f, 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        /// <summary>
        /// Slot highlight animation when a card hovers over it.
        /// </summary>
        public static void SlotHighlight(Transform slot, bool active)
        {
            if (slot == null) return;

            if (active)
            {
                slot.DOScale(new Vector3(0.9f, 0.15f, 1.1f), 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                slot.DOScale(new Vector3(0.8f, 0.1f, 1.0f), 0.15f).SetEase(Ease.InQuad);
            }
        }
    }
}
