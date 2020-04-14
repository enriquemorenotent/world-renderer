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
        // Fields

        [SerializeField] bool isChunkRendered = false;

        // Components

        MeshCollider meshCollider;
        MeshFilter meshFilter;

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

            isChunkRendered = false;
        }

        void Update()
        {
            if (!isChunkRendered) TryRenderChunk();
        }

        // Other

        public bool IsRendered() => isChunkRendered;

        void TryRenderChunk()
        {
            ChunkData chunkData;
            if (WorldRenderer.GetChunkData().TryGetValue(transform.position, out chunkData))
            {
                RenderMesh(chunkData.meshData);

                isChunkRendered = true;
            }
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