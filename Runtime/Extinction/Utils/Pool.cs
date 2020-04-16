using System.Collections.Generic;
using UnityEngine;

namespace Extinction.Utils
{
    public class Pool : MonoBehaviour
    {
        [SerializeField]
        GameObject prefab;

        Queue<GameObject> queue = new Queue<GameObject>();

        public int instancesOnStart = 100;
        public int instancesCreated = 0;
        public int instancesDelivered = 0;
        public int instancesReturned = 0;
        public int poolIncreaseStep = 10;

        public int active = 0;

        void Start()
        {
            if (prefab == null) return;

            GrowPoolByAmount(instancesOnStart);
            prefab.SetActive(false);

        }

        void Update()
        {
            if (instancesCreated - queue.Count < poolIncreaseStep)
                GrowPoolByOne();
        }

        public void SetPrefab(GameObject _prefab)
        {
            prefab = _prefab;
            prefab.SetActive(false);
            
            GrowPoolByDefaultStep();
        }

        void GrowPoolByOne()
        {
            var instance = Instantiate(prefab);
            instance.name = prefab.name;
            instance.transform.SetParent(transform);
            queue.Enqueue(instance);
            instancesCreated++;
        }

        void GrowPoolByDefaultStep() => GrowPoolByAmount(poolIncreaseStep);

        void GrowPoolByAmount(int amount)
            { for (int i = 0; i < amount; i++) GrowPoolByOne(); }

        public void Return(GameObject instance)
        {
            instance.SetActive(false);
            queue.Enqueue(instance);
            instancesReturned++;
            active--;
        }

        public GameObject Deliver()
        {
            if (queue.Count == 0) GrowPoolByDefaultStep();

            instancesDelivered++;
            active++;

            GameObject instance = queue.Dequeue();
            instance.SetActive(true);
            return instance;
        }
    }
}
