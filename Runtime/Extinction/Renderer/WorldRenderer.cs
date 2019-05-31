using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extinction.Config;
using Extinction.Utils;

namespace Extinction.Renderer
{
    [AddComponentMenu("Extinction/World renderer")]
    [RequireComponent(typeof(DataPreloader))]
    [RequireComponent(typeof(DistanceDetector))]
    public class WorldRenderer : MonoBehaviour
    {
        #region attributes

        [SerializeField]
        public Pool chunkPool;

        public World config;

        // Size of world meassured in Chunks
        [Range(2, 20)]
        public int radius = 2;

        // Size of a chunk meassured in world units
        [Range(2, 30)]
        public int chunkSize = 10;

        // Size of world meassured in Chunks
        [Range(2, 20)]
        public int cacheRadius = 5;

        public Dictionary<Vector3, GameObject> renderedChunks = new Dictionary<Vector3, GameObject>();

        #endregion

        #region Singleton

        public static WorldRenderer Instance { get; private set; }

        #endregion

        #region Static methods

        public static World Config()
        {
            return Instance.config;
        }

        public static Dictionary<Vector3, ChunkData> GetChunkData()
        {
            return Instance.dataPreloader.chunkDataCache;
        }

        #endregion

        #region Components

        DistanceDetector detector;
        DataPreloader dataPreloader;

        #endregion

        #region Unity methods

        void Awake()
        {
            this.config.Setup();
            this.detector = GetComponent<DistanceDetector>();
            this.dataPreloader = GetComponent<DataPreloader>();
            this.dataPreloader.Launch(this.radius + this.cacheRadius, this.chunkSize);
        }

        void Start()
        {
            Instance = this;

            UpdateRenderPoint(Vector3.zero);
        }

        void Update()
        {
            if (!detector.IsTargetTooFar()) return;

            Vector3 position = detector.TargetPosition();
            UpdateRenderPoint(ClosestRenderPoint(position));
        }

        #endregion

        #region Helpers

        // Diameter of the world, meassured in world units
        public int Diameter()
        {
            return chunkSize * 2 + 1;
        }

        int Abs(float floatNumber)
        {
            return System.Convert.ToInt32(floatNumber);
        }

        Vector3 ClosestRenderPoint(Vector3 position)
        {
            return new Vector3Int(
                Abs(position.x / Diameter()) * Diameter(),
                0,
                Abs(position.z / Diameter()) * Diameter()
            );
        }

        #endregion

        #region Chunks

        void DeleteDistantChunks()
        {
            var distantChunks = this.renderedChunks.Where(pair =>
                Mathf.Abs(pair.Key.x - this.transform.position.x) / Diameter() > radius ||
                Mathf.Abs(pair.Key.z - this.transform.position.z) / Diameter() > radius
            ).ToList();

            foreach (var pair in distantChunks)
            {
                pair.Value.GetComponent<ChunkRenderer>().ToPool();
                this.renderedChunks.Remove(pair.Key);
            }
        }

        bool isChunkRenderedAt(Vector3 position)
        {
            return this.renderedChunks.ContainsKey(position);
        }

        void TrackChunk(GameObject chunkInstance, Vector3 position)
        {
            this.renderedChunks.Add(position, chunkInstance);
        }

        void InstantiateChunk(int x, int z)
        {
            Vector3 position = new Vector3(transform.position.x + x * Diameter(), 0, transform.position.z + z * Diameter());

            if (isChunkRenderedAt(position)) return;

            GameObject chunkInstance = chunkPool.GetFromPool();
            chunkInstance.transform.position = position;
            chunkInstance.name = string.Format("Chunk {0}, {1}", position.x, position.z);

            // Do NOT do this. It will mess the positions of the chunks when moving around
            // chunkInstance.transform.SetParent(this.transform);

            TrackChunk(chunkInstance, position);
        }

        #endregion

        void UpdateRenderPoint(Vector3 drawPoint)
        {
            transform.position = drawPoint;
            dataPreloader.SetDirty();
            DeleteDistantChunks();

            for (int z = -radius; z <= radius; z++)
                for (int x = -radius; x <= radius; x++)
                    InstantiateChunk(x, z);
        }
    }
}
