using UnityEngine;
using System.Collections.Generic;

namespace Extinction.Utils
{
    public class PoolDeliverer : MonoBehaviour
    {
        // The reason why we are using the name of the prefab, and not the prefab
        // itself, is because it is impossible to check if a GameObject is an
        // instance of a prefab. Therefore, returning an instance to its proper
        // pool would be much more difficult.
        //
        // TLDR: Use names, not prefabs, as keys for the dictionary

        Dictionary<string, Pool> catalogue = new Dictionary<string, Pool>();

        [SerializeField] public GameObject poolPrefab;
        [SerializeField] List<GameObject> prefabs;

        [SerializeField] private int poolIncreaseStep = 10;

        void Start() { prefabs.ForEach(InstantiatePool); }

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
            return catalogue.TryGetValue(prefabName, out Pool pool) ? pool : null;
        }
    }
}