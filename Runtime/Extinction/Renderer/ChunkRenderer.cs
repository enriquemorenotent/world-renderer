using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Extinction.Data;

namespace Extinction.Renderer
{
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ChunkRenderer : MonoBehaviour
    {
        public Config.IWorld config;
        public Config.MapRenderConfig mapRenderConfig;
        public Vector3 renderPosition;
        public bool needsToBeRendered = false;

        MeshData meshData;

        private MeshCollider meshCollider;
        private MeshFilter meshFilter;

        // Other

        List<GameObject> propsRendered;

        // Unity methods

        void Awake()
        {
            meshCollider = GetComponent<MeshCollider>();
            meshFilter = GetComponent<MeshFilter>();
            propsRendered = new List<GameObject>();
        }

        void OnEnable()
        {
            meshFilter.mesh = new Mesh();
            meshFilter.mesh.name = "Chunk mesh";
            meshCollider.sharedMesh = new Mesh();
        }

        void Update()
        {
            if (!needsToBeRendered) return;
            if (meshData == null) return;

            RenderMesh(meshData);
            needsToBeRendered = true;
        }

        public void StartRendering(Config.IWorld _config, Config.MapRenderConfig _mapRenderConfig, Vector3 _renderPosition)
        {
            config = _config;
            mapRenderConfig = _mapRenderConfig;
            renderPosition = _renderPosition;

            meshData = null;
            needsToBeRendered = true;

            Task.Run(GenerateMeshData);
        }

        // Other

        public bool IsRendered() => needsToBeRendered;

        void GenerateMeshData()
        {
            meshData = Utils.MeshGenerator.LoadDataAt(renderPosition, mapRenderConfig.chunkSize, config);
        }

        void RenderMesh(MeshData data)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Chunk";

            mesh.SetVertices(data.vertices);
            mesh.SetTriangles(data.triangles, 0);
            mesh.SetUVs(0, data.uvs);
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public void ToPool()
        {
            foreach (GameObject prop in propsRendered)
                WorldRenderer.singleton.propsPoolDeliverer.GetPool(prop.name).Return(prop);
            propsRendered = new List<GameObject>();
            WorldRenderer.singleton.chunkPool.Return(gameObject);
        }
    }
}