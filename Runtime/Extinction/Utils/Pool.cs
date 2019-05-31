using System.Collections.Generic;
using UnityEngine;

namespace Extinction.Utils
{
    public class Pool : MonoBehaviour
    {
        #region Attributes

        [SerializeField]
        GameObject prefab;

        Queue<GameObject> queue = new Queue<GameObject>();

        #endregion

        #region Stats

        public int instancesCreated = 0;
        public int instancesDelivered = 0;
        public int instancesReturned = 0;

        #endregion

        #region Unity methods

        void Start()
        {
            GrowPool();
        }

        #endregion

        #region Mehtods

        void GrowPool()
        {
            for (int i = 0; i < 10; i++)
            {
                var instance = Instantiate(this.prefab);
                instance.transform.SetParent(this.transform);
                this.AddToPool(instance);
            }
            instancesCreated += 10;
        }

        public void AddToPool(GameObject instance)
        {
            instance.SetActive(false);
            this.queue.Enqueue(instance);
            instancesReturned++;
        }

        public GameObject GetFromPool()
        {
            if (queue.Count == 0)
            {
                this.GrowPool();
            }

            instancesDelivered++;
            GameObject instance = queue.Dequeue();
            instance.SetActive(true);
            return instance;
        }

        #endregion
    }
}
