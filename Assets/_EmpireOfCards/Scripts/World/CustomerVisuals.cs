using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Helpers;
using DG.Tweening;

namespace EmpireOfCards.World
{
    public class CustomerVisuals : MonoBehaviour
    {
        // ── Config ──────────────────────────────────────────────────
        const int MAX_VISIBLE      = 10;
        const float WALK_SPEED     = 2f;
        const float SPAWN_X_LEFT   = -8f;
        const float TARGET_X       = 0f;
        const float EXIT_X_RIGHT   = 8f;
        const float STREET_Y       = 0.3f;
        const float STREET_Z       = 4f;
        const float DOOR_Z         = -2f; // restaurant entrance z

        // ── Customer type colors ────────────────────────────────────
        static readonly Color COL_BARGAIN    = new Color(0.85f, 0.20f, 0.20f); // red
        static readonly Color COL_GOURMET    = new Color(1.00f, 0.84f, 0.00f); // gold
        static readonly Color COL_LOYAL      = new Color(0.25f, 0.50f, 0.90f); // blue
        static readonly Color COL_INFLUENCER = new Color(0.65f, 0.25f, 0.75f); // purple
        static readonly Color COL_FAMILY     = new Color(0.20f, 0.75f, 0.30f); // green
        static readonly Color COL_RANDOM     = new Color(0.55f, 0.55f, 0.55f); // gray

        // ── Pool ────────────────────────────────────────────────────
        readonly List<CustomerCube> _active = new();
        readonly Queue<GameObject> _pool = new();

        // ── Track last served data for coloring ─────────────────────
        int _lastServed;
        int _lastLeft;

        // ── EventBus ────────────────────────────────────────────────

        void OnEnable()
        {
            EventBus.OnCustomersServed += HandleCustomersServed;
        }

        void OnDisable()
        {
            EventBus.OnCustomersServed -= HandleCustomersServed;
        }

        void HandleCustomersServed(int served, int waited, int left)
        {
            _lastServed = served;
            _lastLeft = left;

            int total = Mathf.Min(served + left, MAX_VISIBLE);

            // Spawn served customers (walk to shop)
            for (int i = 0; i < Mathf.Min(served, MAX_VISIBLE); i++)
                SpawnCustomer(GetRandomCustomerColor(), false);

            // Spawn angry/leaving customers
            int angryCount = Mathf.Min(left, MAX_VISIBLE - _active.Count);
            for (int i = 0; i < angryCount; i++)
                SpawnCustomer(COL_RANDOM, true);
        }

        // ── Spawn ───────────────────────────────────────────────────

        void SpawnCustomer(Color color, bool angry)
        {
            if (_active.Count >= MAX_VISIBLE) return;

            GameObject go = GetFromPool();
            go.SetActive(true);

            float randomZ = STREET_Z + Random.Range(-0.4f, 0.4f);
            go.transform.position = new Vector3(SPAWN_X_LEFT, STREET_Y, randomZ);
            go.transform.localScale = Vector3.zero; // start invisible for spawn animation

            var rend = go.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = MaterialHelper.Create(color);
            }

            // Pop-in scale animation
            go.transform.DOScale(new Vector3(0.3f, 0.5f, 0.3f), 0.25f)
                .SetEase(Ease.OutBack);

            // Build cube person: add head on top of body
            BuildCubePerson(go, color);

            float targetX = angry ? EXIT_X_RIGHT : TARGET_X + Random.Range(-2f, 2f);

            var cust = new CustomerCube
            {
                go = go,
                targetX = targetX,
                angry = angry,
                speed = WALK_SPEED + Random.Range(-0.3f, 0.5f)
            };

            _active.Add(cust);

            // Use DOTween for walk if angry (fast leave)
            if (angry)
            {
                Vector3 exitPos = new Vector3(EXIT_X_RIGHT, STREET_Y, randomZ);
                DOTweenAnimations.CustomerLeaveAngry(go.transform, exitPos);
                cust.useDOTween = true;
            }
        }

        // ── Build cube person ───────────────────────────────────────

        void BuildCubePerson(GameObject bodyGo, Color bodyColor)
        {
            // Check if head already exists
            var existingHead = bodyGo.transform.Find("Head");
            if (existingHead != null) return;

            // Head cube on top of body
            var head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.name = "Head";
            head.transform.SetParent(bodyGo.transform);
            head.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            head.transform.localScale = new Vector3(0.7f, 0.5f, 0.7f);

            var headRend = head.GetComponent<Renderer>();
            if (headRend != null)
            {
                headRend.material = MaterialHelper.Create(Color.Lerp(bodyColor, Color.white, 0.25f));
            }

            // Remove head collider so raycasts hit body
            var headCol = head.GetComponent<Collider>();
            if (headCol != null) Object.Destroy(headCol);
        }

        // ── Update walk ─────────────────────────────────────────────

        void Update()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                var cust = _active[i];
                if (cust.go == null || !cust.go.activeInHierarchy)
                {
                    if (cust.go != null) ReturnToPool(cust.go);
                    _active.RemoveAt(i);
                    continue;
                }

                // Skip DOTween-driven customers
                if (cust.useDOTween) continue;

                Vector3 pos = cust.go.transform.position;
                pos.x = Mathf.MoveTowards(pos.x, cust.targetX, cust.speed * Time.deltaTime);

                // Walking bob animation (manual)
                if (!cust.arrivedOnce || cust.waitTimer <= 0f)
                {
                    float bob = Mathf.Sin(Time.time * 8f + i * 1.3f) * 0.03f;
                    pos.y = STREET_Y + bob;
                }

                cust.go.transform.position = pos;

                // Reached destination
                bool done = Mathf.Abs(pos.x - cust.targetX) < 0.1f;
                if (done && !cust.arrivedOnce)
                {
                    cust.arrivedOnce = true;
                    if (!cust.angry)
                    {
                        // Served customer: animate entering shop, then walk to exit
                        Vector3 doorPos = new Vector3(pos.x, STREET_Y, DOOR_Z);
                        cust.useDOTween = true;

                        var go = cust.go;
                        DOTween.Sequence()
                            .Append(go.transform.DOMoveZ(DOOR_Z, 0.6f).SetEase(Ease.InQuad))
                            .Append(go.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack))
                            .AppendInterval(1f)
                            .AppendCallback(() =>
                            {
                                if (go != null)
                                {
                                    go.transform.position = new Vector3(pos.x, STREET_Y, STREET_Z);
                                    go.transform.localScale = new Vector3(0.3f, 0.5f, 0.3f);
                                }
                            })
                            .Append(go.transform.DOScale(new Vector3(0.3f, 0.5f, 0.3f), 0.15f))
                            .Append(go.transform.DOMoveX(EXIT_X_RIGHT, 2f).SetEase(Ease.Linear))
                            .OnComplete(() =>
                            {
                                if (go != null) ReturnToPool(go);
                            });
                    }
                    else
                    {
                        cust.targetX = EXIT_X_RIGHT;
                        cust.waitTimer = 0f;
                    }
                }

                // Wait timer for served customers
                if (cust.arrivedOnce && cust.waitTimer > 0f)
                {
                    cust.waitTimer -= Time.deltaTime;
                    continue;
                }

                // Off screen -> recycle
                if (pos.x >= EXIT_X_RIGHT - 0.1f && cust.arrivedOnce && !cust.useDOTween)
                {
                    ReturnToPool(cust.go);
                    _active.RemoveAt(i);
                }
            }
        }

        // ── Pool management ─────────────────────────────────────────

        GameObject GetFromPool()
        {
            if (_pool.Count > 0)
                return _pool.Dequeue();

            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "CustomerCube";
            go.transform.SetParent(transform);
            return go;
        }

        void ReturnToPool(GameObject go)
        {
            go.SetActive(false);
            _pool.Enqueue(go);
        }

        // ── Color helper ────────────────────────────────────────────

        Color GetRandomCustomerColor()
        {
            int idx = Random.Range(0, 6);
            switch (idx)
            {
                case 0: return COL_BARGAIN;
                case 1: return COL_GOURMET;
                case 2: return COL_LOYAL;
                case 3: return COL_INFLUENCER;
                case 4: return COL_FAMILY;
                default: return COL_RANDOM;
            }
        }

        // ── Internal data ───────────────────────────────────────────

        class CustomerCube
        {
            public GameObject go;
            public float targetX;
            public bool angry;
            public float speed;
            public bool arrivedOnce;
            public float waitTimer;
            public bool useDOTween;
        }
    }
}
