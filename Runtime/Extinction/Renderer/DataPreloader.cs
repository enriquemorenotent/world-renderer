using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Extinction.Utils;
using Extinction.Data;

namespace Extinction.Renderer
{
    public class DataPreloader
    {
        // Attributes

        public Cache<Vector3, List<PropData>> data;

        int loadRadius;
        int chunkSize;

        Vector3 dataPoint;

        Config.World config;

        // Constructor

        public DataPreloader(int _loadRadius, int _chunkSize, Config.World _config)
        {
            loadRadius = _loadRadius;
            chunkSize = _chunkSize;
            config = _config;
            data = new Cache<Vector3, List<PropData>>(LoadChunkData, CleanChunkData);
        }

        // Methods

        public List<PropData> LoadChunkData(Vector3 position) =>
            Utils.PropDataGenerator.LoadDataAt(position, chunkSize, config);

        public void LoadAround(Vector3 position)
        {
            dataPoint = position;
            Task.Run(UpdateData);
        }

        void UpdateData()
        {
            data.CleanUp();
            CollectData();
        }

        bool CleanChunkData(Vector3 position)
        {
            bool toDelete =
                Mathf.Abs(position.x - dataPoint.x) > loadRadius * (chunkSize * 2 + 1) ||
                Mathf.Abs(position.z - dataPoint.z) > loadRadius * (chunkSize * 2 + 1);

            return toDelete;
        }

        void CollectData()
        {
            int diameter = chunkSize * 2 + 1;
            Vector3 position;

            for (int z = -loadRadius; z <= loadRadius; z++)
                for (int x = -loadRadius; x <= loadRadius; x++)
                {
                    position = new Vector3(dataPoint.x + x * diameter, 0, dataPoint.z + z * diameter);
                    data.At(position);
                }
        }
    }
}