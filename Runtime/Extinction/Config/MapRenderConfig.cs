using UnityEngine;

namespace Extinction.Config
{
    [System.Serializable]
    public class MapRenderConfig
    {
        [Range(2, 20)] public int radius = 2;
        [Range(2, 30)] public int chunkSize = 10;
        [Range(2, 20)] public int cacheRadius = 5;
    }
}