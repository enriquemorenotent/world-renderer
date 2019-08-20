using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Extinction.Renderer
{
    public class DataPreloader
    {
        public Dictionary<Vector3, ChunkData> cache = new Dictionary<Vector3, ChunkData>();

        int loadRadius;
        int chunkSize;

        Vector3 dataPoint;

        public DataPreloader(int _loadRadius, int _chunkSize, Vector3 position)
        {
            chunkSize = _chunkSize;
            loadRadius = _loadRadius;
            LoadAround(position);
        }

        public void LoadAround(Vector3 position)
        {
            dataPoint = position;
            Task.Run(UpdateData);
        }

        void UpdateData()
        {
            lock (cache)
            {
                RemoveUselessData();
                CollectData();
            }
        }

        void RemoveUselessData()
        {
            var toRemove = cache.Where(pair =>
                Mathf.Abs(pair.Key.x - dataPoint.x) > loadRadius * (chunkSize * 2 + 1) ||
                Mathf.Abs(pair.Key.z - dataPoint.z) > loadRadius * (chunkSize * 2 + 1)
            ).ToList();

            foreach (var pair in toRemove) cache.Remove(pair.Key);
        }

        void CollectData()
        {
            int diameter = chunkSize * 2 + 1;
            Vector3 position;

            for (int z = -loadRadius; z <= loadRadius; z++)
            for (int x = -loadRadius; x <= loadRadius; x++)
            {
                position = new Vector3(dataPoint.x + x * diameter, 0, dataPoint.z + z * diameter);

                if (!cache.ContainsKey(position))
                    cache.Add(position, ChunkData.LoadDataAt(position, chunkSize));
            }
        }
    }
}