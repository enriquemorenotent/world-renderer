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
        // Fields

        [SerializeField] public Pool chunkPool;
        [SerializeField] public PoolDeliverer propsPoolDeliverer;

        [SerializeField] public World config;

        [SerializeField] private MapRenderConfig mapRenderConfig;

        [Range(300, 2000)] public int visitedChunkBufferRange = 500;

        // Components

        DistanceDetector detector;

        // Other

        public Dictionary<Vector3, GameObject> renderedChunks = new Dictionary<Vector3, GameObject>();

        public List<Vector3> visitedChunks = new List<Vector3>();

        public int ChunkDiameter { get => mapRenderConfig.chunkSize * 2 + 1; }

        // Singleton

        public static WorldRenderer singleton { get; private set; }

        public static World Config() => singleton.config;

        [Header("Flags")]
        public bool renderProps = true;

        // Unity methods

        void Awake()
        {
            singleton = this;
            config.Setup();
            UpdateRenderPoint(Vector3.zero);

            detector = GetComponent<DistanceDetector>();
            detector.onEscape.AddListener(() => UpdateRenderPoint(detector.TargetPosition()));
        }

        void Update()
        {
            if (AreAllChunksRendered())
                detector.Reset();
        }

        // Helpers

        Vector3 ClosestRenderPoint(Vector3 position) => new Vector3Int(
            (int)(position.x / ChunkDiameter) * ChunkDiameter,
            0,
            (int)(position.z / ChunkDiameter) * ChunkDiameter
        );

        bool AreAllChunksRendered()
        {
            return renderedChunks.All(pair => pair.Value.GetComponent<ChunkRenderer>().IsRendered());
        }

        //
        // Chunk control
        //

        void DeleteDistantChunks()
        {
            var distantChunks = renderedChunks.Where(pair =>
                Mathf.Abs(pair.Key.x - transform.position.x) / ChunkDiameter > mapRenderConfig.radius ||
                Mathf.Abs(pair.Key.z - transform.position.z) / ChunkDiameter > mapRenderConfig.radius
            ).ToList();

            foreach (var pair in distantChunks)
            {
                pair.Value.GetComponent<ChunkPropsRenderer>().ReturnPropsToPool();
                pair.Value.GetComponent<ChunkRenderer>().ToPool();
                renderedChunks.Remove(pair.Key);
            }

            visitedChunks.RemoveAll(position => Vector3.Distance(transform.position, position) > visitedChunkBufferRange);
        }

        bool isChunkRenderedAt(Vector3 position) => renderedChunks.ContainsKey(position);

        void TrackChunk(GameObject chunkInstance, Vector3 position)
        {
            renderedChunks.Add(position, chunkInstance);
            if (!visitedChunks.Contains(position)) visitedChunks.Add(position);
        }

        void InstantiateChunk(int x, int z)
        {
            Vector3 position = new Vector3(transform.position.x + x * ChunkDiameter, 0, transform.position.z + z * ChunkDiameter);

            if (isChunkRenderedAt(position)) return;

            GameObject chunkInstance = chunkPool.Deliver();
            chunkInstance.name = string.Format("Chunk {0}, {1}", position.x, position.z);
            chunkInstance.transform.position = position;

            ChunkRenderer chunkRenderer = chunkInstance.GetComponent<ChunkRenderer>();
            chunkRenderer.StartRendering(config, mapRenderConfig, position);

            ChunkPropsRenderer chunkPropsRenderer = chunkInstance.GetComponent<ChunkPropsRenderer>();
            chunkPropsRenderer.StartRendering(config, mapRenderConfig, position);

            TrackChunk(chunkInstance, position);
        }

        void UpdateRenderPoint(Vector3 point)
        {
            Vector3 drawPoint = ClosestRenderPoint(point);
            transform.position = drawPoint;
            onRenderPointUpdated.Invoke();
            DeleteDistantChunks();

            for (int z = -mapRenderConfig.radius; z <= mapRenderConfig.radius; z++)
                for (int x = -mapRenderConfig.radius; x <= mapRenderConfig.radius; x++)
                    InstantiateChunk(x, z);
        }

        //
        // Events
        //

        static UnityEvent onRenderPointUpdated = new UnityEvent();

        public static void OnRenderPointUpdated(UnityAction action)
        {
            onRenderPointUpdated.AddListener(action);
        }
    }
}
