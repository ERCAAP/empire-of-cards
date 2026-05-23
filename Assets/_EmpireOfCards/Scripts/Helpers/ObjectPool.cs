using System.Collections.Generic;
using UnityEngine;

namespace EmpireOfCards.Helpers
{
    /// <summary>
    /// Generic object pool for reusing GameObjects instead of frequent Instantiate/Destroy calls.
    /// Useful for cards, VFX, projectiles, and other frequently spawned objects.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [Header("Pool Settings")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 10;
        [SerializeField] private Transform parent;

        private readonly Queue<GameObject> pool = new Queue<GameObject>();

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Creates the pool parent (if needed) and pre-warms the pool with the initial size.
        /// </summary>
        public void Initialize()
        {
            if (parent == null)
            {
                var container = new GameObject($"Pool_{prefab.name}");
                container.transform.SetParent(transform);
                parent = container.transform;
            }

            PreWarm(initialSize);
        }

        /// <summary>
        /// Gets an object from the pool, or creates a new one if the pool is empty.
        /// The returned object is set active.
        /// </summary>
        public GameObject Get()
        {
            GameObject obj;

            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else
            {
                obj = Instantiate(prefab, parent);
            }

            obj.SetActive(true);
            return obj;
        }

        /// <summary>
        /// Returns an object to the pool. The object is deactivated and re-parented.
        /// </summary>
        public void Return(GameObject obj)
        {
            if (obj == null)
                return;

            obj.SetActive(false);
            obj.transform.SetParent(parent);
            pool.Enqueue(obj);
        }

        /// <summary>
        /// Pre-instantiates the given number of objects into the pool.
        /// </summary>
        public void PreWarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = Instantiate(prefab, parent);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }
    }
}
