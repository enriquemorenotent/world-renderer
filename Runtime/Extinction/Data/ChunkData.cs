using System.Collections.Generic;
using UnityEngine;

namespace Extinction.Data
{
    public class ChunkData
    {
        public MeshData meshData;
        public List<PropData> propDataList;

        public static ChunkData LoadDataAt(Vector3 position, int chunkSize, Config.World config)
        {
            ChunkData data = new ChunkData();

            data.meshData = Utils.MeshGenerator.LoadDataAt(position, chunkSize, config);
            data.propDataList = Utils.PropDataGenerator.LoadDataAt(position, chunkSize, config);

            return data;
        }
    }
}