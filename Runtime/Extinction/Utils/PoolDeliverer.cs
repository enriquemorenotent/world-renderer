using UnityEngine;
using System.Collections.Generic;

namespace Extinction.Utils
{
    public class PoolDeliverer : MonoBehaviour
    {
        Dictionary<string, Pool> catalogue = new Dictionary<string, Pool>();

        [SerializeField] public GameObject poolPrefab;
        [SerializeField] GameObject[] prefabs;

        [SerializeField] private int poolIncreaseStep = 10;

        void Start()
        {
            foreach (var prefab in prefabs) InstantiatePool(prefab);
        }

        void InstantiatePool(GameObject prefab)
        {
            var instance = Instantiate(poolPrefab, Vector3.zero, Quaternion.identity);
            instance.transform.SetParent(transform);
            instance.name = "Pool " + prefab.name;
            Pool pool = instance.GetComponent<Pool>();
            pool.poolIncreaseStep = poolIncreaseStep;
            pool.SetPrefab(prefab);
            catalogue.Add(prefab.name, pool);
        }

        public Pool GetPool(string prefabName)
        {
            Pool pool;
            return catalogue.TryGetValue(prefabName, out pool) ? pool : null;
        }
    }
}