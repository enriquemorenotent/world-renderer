using DigitalRuby.Threading;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Extinction.Renderer
{
    public class DataPreloader : MonoBehaviour
    {
        public Dictionary<Vector3, ChunkData> chunkDataCache = new Dictionary<Vector3, ChunkData>();

        bool dirty;
        int loadRadius;
        int chunkSize;

        // We write here transform.position, because Transform
        // can only be read from the main thread
        Vector3 transformPosition;

        public void Launch(int loadRadius, int chunkSize)
        {
            this.chunkSize = chunkSize;
            this.loadRadius = loadRadius;
            EZThread.BeginThread(UpdateThread, false);
        }

        public void SetDirty()
        {
            dirty = true;
            transformPosition = transform.position;
        }

        void UpdateThread()
        {
            if (dirty)
            {
                dirty = false;
                RemoveUselessData();
                CollectData();
            }
        }

        void RemoveUselessData()
        {
            var toRemove = this.chunkDataCache.Where(pair =>
                Mathf.Abs(pair.Key.x - transformPosition.x) > loadRadius * (chunkSize * 2 + 1) ||
                Mathf.Abs(pair.Key.z - transformPosition.z) > loadRadius * (chunkSize * 2 + 1)
            ).ToList();

            foreach (var pair in toRemove)
            {
                this.chunkDataCache.Remove(pair.Key);
            }
        }

        void CollectData()
        {
            for (int z = -loadRadius; z <= loadRadius; z++)
                for (int x = -loadRadius; x <= loadRadius; x++)
                {
                    if (dirty) return;

                    Vector3 position = new Vector3(transformPosition.x + x * (chunkSize * 2 + 1), 0, transformPosition.z + z * (chunkSize * 2 + 1));

                    if (!this.chunkDataCache.ContainsKey(position))
                    {
                        ChunkData chunkData = ChunkData.LoadDataAt(position, chunkSize);
                        this.chunkDataCache.Add(position, chunkData);
                    }
                }
        }
    }
}