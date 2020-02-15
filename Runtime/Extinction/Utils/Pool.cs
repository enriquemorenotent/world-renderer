using System.Collections.Generic;
using UnityEngine;

namespace Extinction.Utils
{
    public class Pool : MonoBehaviour
    {
        [SerializeField]
        GameObject prefab;

        Queue<GameObject> queue = new Queue<GameObject>();

        public int instancesCreated = 0;
        public int instancesDelivered = 0;
        public int instancesReturned = 0;
        public int poolIncreaseStep = 10;

        void Start()
        {
            if (prefab != null) GrowPool();
        }

        public void SetPrefab(GameObject _prefab)
        {
            prefab = _prefab;
            GrowPool();
        }

        void GrowPool()
        {
            for (int i = 0; i < poolIncreaseStep; i++)
            {
                var instance = Instantiate(prefab);
                instance.name = prefab.name;
                instance.transform.SetParent(transform);
                Return(instance);
            }
            instancesCreated += poolIncreaseStep;
        }

        public void Return(GameObject instance)
        {
            instance.SetActive(false);
            queue.Enqueue(instance);
            instancesReturned++;
        }

        public GameObject Deliver()
        {
            if (queue.Count == 0) GrowPool();

            instancesDelivered++;
            GameObject instance = queue.Dequeue();
            instance.SetActive(true);
            return instance;
        }
    }
}
