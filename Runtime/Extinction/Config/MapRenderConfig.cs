using UnityEngine;

namespace Extinction.Config
{
    [System.Serializable]
    public class MapRenderConfig
    {
        [Range(1, 20)] public int radius = 2;
        [Range(1, 30)] public int chunkSize = 10;
        [Range(1, 20)] public int cacheRadius = 5;

        public int ChunkDiameter { get => chunkSize * 2 + 1; }

        public Vector3 ClosestChunkPosition(Vector3 position)
        {
            int x = (int)(position.x / ChunkDiameter) * ChunkDiameter;
            int z = (int)(position.z / ChunkDiameter) * ChunkDiameter;
            return new Vector3Int(x, 0, z);
        }

        public Vector3 GetPositionOfChunk(int x, int z) =>
            new Vector3(x, 0, z) * ChunkDiameter;
    }
}