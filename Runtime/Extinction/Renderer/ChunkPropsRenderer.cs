using System.Collections.Generic;
using UnityEngine;
using Extinction.Data;

namespace Extinction.Renderer
{
    public class ChunkPropsRenderer : MonoBehaviour
    {
        private bool arePropsRendered = false;
        private List<GameObject> props = new List<GameObject>();

        void OnEnable() { arePropsRendered = false; }

        void OnDisable()
        {
            props.Clear();
        }

        void Update()
        {
            if (arePropsRendered) return;
            TryRenderProps();
        }

        void TryRenderProps()
        {
            ChunkData chunkData;
            if (WorldRenderer.GetChunkData().TryGetValue(transform.position, out chunkData))
            {
                RenderProps(chunkData);
                arePropsRendered = true;
            }
        }

        void RenderProps(ChunkData chunkData)
        {
            foreach (PropData data in chunkData.propDataList)
            {
                GameObject instance = WorldRenderer.singleton.propsPoolDeliverer.GetPool(data.prefab.name).Deliver();
                instance.transform.position = data.position;
                props.Add(instance);
            }
        }
    }
}