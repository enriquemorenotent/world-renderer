using UnityEngine;

namespace Extinction.Config
{
    [System.Serializable]
    public class MapRenderConfig
    {
        [Range(1, 20)] public int radius = 2;
        [Range(1, 30)] public int chunkSize = 10;
        [Range(1, 20)] public int cacheRadius = 5;
    }
}