using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;

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
            go.transform.localScale = new Vector3(0.3f, 0.5f, 0.3f);

            var rend = go.GetComponent<Renderer>();
            if (rend != null)
            {
                if (rend.material == null)
                    rend.material = new Material(Shader.Find("Standard"));
                rend.material.color = color;
            }

            float targetX = angry ? EXIT_X_RIGHT : TARGET_X + Random.Range(-2f, 2f);

            _active.Add(new CustomerCube
            {
                go = go,
                targetX = targetX,
                angry = angry,
                speed = WALK_SPEED + Random.Range(-0.3f, 0.5f)
            });
        }

        // ── Update walk ─────────────────────────────────────────────

        void Update()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                var cust = _active[i];
                if (cust.go == null) { _active.RemoveAt(i); continue; }

                Vector3 pos = cust.go.transform.position;
                pos.x = Mathf.MoveTowards(pos.x, cust.targetX, cust.speed * Time.deltaTime);
                cust.go.transform.position = pos;

                // Reached destination
                bool done = Mathf.Abs(pos.x - cust.targetX) < 0.1f;
                if (done && !cust.arrivedOnce)
                {
                    cust.arrivedOnce = true;
                    if (!cust.angry)
                    {
                        // Served customer: walk to exit after brief pause
                        cust.targetX = EXIT_X_RIGHT;
                        cust.waitTimer = 1.5f;
                    }
                }

                // Wait timer for served customers
                if (cust.arrivedOnce && cust.waitTimer > 0f)
                {
                    cust.waitTimer -= Time.deltaTime;
                    continue;
                }

                // Off screen -> recycle
                if (pos.x >= EXIT_X_RIGHT - 0.1f && cust.arrivedOnce)
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
        }
    }
}
