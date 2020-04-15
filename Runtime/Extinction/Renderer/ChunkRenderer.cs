using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Extinction.Data;

namespace Extinction.Renderer
{
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ChunkRenderer : MonoBehaviour
    {
        private Config.IWorld config;
        private Config.MapRenderConfig mapRenderConfig;
        private Vector3 renderPosition;
        private bool needsToBeRendered = false;
        private MeshData meshData;

        private MeshCollider meshCollider;
        private MeshFilter meshFilter;

        void Awake()
        {
            meshCollider = GetComponent<MeshCollider>();
            meshFilter = GetComponent<MeshFilter>();
        }

        void OnEnable()
        {
            meshFilter.mesh = new Mesh();
            meshCollider.sharedMesh = new Mesh();
        }

        void Update()
        {
            if (!needsToBeRendered) return;
            if (meshData == null) return;

            RenderMesh();
            needsToBeRendered = false;
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

        public bool IsRendered() => needsToBeRendered;

        void GenerateMeshData()
        {
            meshData = Utils.MeshGenerator.LoadDataAt(renderPosition, mapRenderConfig.chunkSize, config);
        }

        void RenderMesh()
        {
            Mesh mesh = new Mesh();

            mesh.SetVertices(meshData.vertices);
            mesh.SetTriangles(meshData.triangles, 0);
            mesh.SetUVs(0, meshData.uvs);
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public void ToPool() { WorldRenderer.singleton.chunkPool.Return(gameObject); }
    }
}