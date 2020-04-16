using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Extinction.Config;
using Extinction.Utils;
using Extinction.Data;

namespace Extinction.Renderer
{
    [AddComponentMenu("Extinction/World renderer")]
    [RequireComponent(typeof(DistanceDetector))]
    public class WorldRenderer : MonoBehaviour
    {
        public static WorldRenderer singleton { get; private set; }

        [SerializeField] public Pool chunkPool = null;
        [SerializeField] public Pool propsPool = null;
        [SerializeField] public World config;
        [SerializeField] private MapRenderConfig mapRenderConfig = null;

        private DistanceDetector detector;

        private Dictionary<Vector3, GameObject> activeChunks = new Dictionary<Vector3, GameObject>();

        void Awake()
        {
            singleton = this;
            config.Setup();
            UpdateRenderPoint(Vector3.zero);

            detector = GetComponent<DistanceDetector>();
            detector.onEscape.AddListener(() => UpdateRenderPoint(detector.TargetPosition()));
        }

        void DeleteDistantChunks()
        {
            var distantChunks = activeChunks.Where(pair =>
                Mathf.Abs(pair.Key.x - transform.position.x) / mapRenderConfig.ChunkDiameter > mapRenderConfig.radius ||
                Mathf.Abs(pair.Key.z - transform.position.z) / mapRenderConfig.ChunkDiameter > mapRenderConfig.radius
            ).ToList();

            foreach (var pair in distantChunks)
            {
                pair.Value.GetComponent<ChunkPropsRenderer>().ReturnPropsToPool();
                pair.Value.GetComponent<ChunkRenderer>().ToPool();
                activeChunks.Remove(pair.Key);
            }
        }

        bool IsChunkActive(Vector3 position) => activeChunks.ContainsKey(position);

        void TrackChunk(Vector3 position, GameObject chunkInstance)
        {
            activeChunks.Add(position, chunkInstance);
        }

        void InstantiateChunk(int x, int z)
        {
            Vector3 position = transform.position + mapRenderConfig.GetPositionOfChunk(x, z);

            if (IsChunkActive(position)) return;

            GameObject chunkInstance = chunkPool.Deliver();
            chunkInstance.name = $"Chunk {position.x}, {position.z}";
            chunkInstance.transform.position = position;

            ChunkRenderer chunkRenderer = chunkInstance.GetComponent<ChunkRenderer>();
            chunkRenderer.StartRendering(config, mapRenderConfig, position);

            ChunkPropsRenderer chunkPropsRenderer = chunkInstance.GetComponent<ChunkPropsRenderer>();
            chunkPropsRenderer.StartRendering(config, mapRenderConfig, position);

            TrackChunk(position, chunkInstance);
        }

        void UpdateRenderPoint(Vector3 point)
        {
            transform.position = mapRenderConfig.ClosestChunkPosition(point); ;
            onRenderPointUpdated.Invoke();
            DeleteDistantChunks();

            for (int z = -mapRenderConfig.radius; z <= mapRenderConfig.radius; z++)
                for (int x = -mapRenderConfig.radius; x <= mapRenderConfig.radius; x++)
                    InstantiateChunk(x, z);
        }

        #region Events

        static UnityEvent onRenderPointUpdated = new UnityEvent();

        public static void OnRenderPointUpdated(UnityAction action)
        {
            onRenderPointUpdated.AddListener(action);
        }

        #endregion
    }
}
