using System.Collections.Generic;
using UnityEngine;
using Extinction.Data;

namespace Extinction.Renderer
{
    public class ChunkPropsRenderer : MonoBehaviour
    {
        private bool rendered = false;
        private List<GameObject> props = new List<GameObject>();

        void OnEnable() { rendered = false; }

        void OnDisable()
        {
            props.ForEach(ReturnPropToPool);
            props.Clear();
        }

        void Update()
        {
            if (rendered) return;
            TryRenderProps();
        }

        void TryRenderProps()
        {
            if (!WorldRenderer.GetChunkData().TryGetValue(transform.position, out List<PropData> data)) return;

            data.ForEach(RenderProp);
            rendered = true;
        }

        void RenderProp(PropData data)
        {
            GameObject instance = GetPool(data.prefab.name).Deliver();
            instance.name = data.prefab.name;
            instance.transform.position = data.position;
            props.Add(instance);
        }

        void ReturnPropToPool(GameObject instance)
        {
            if (instance == null) return;
            GetPool(instance.name).Return(instance);
        }

        Utils.Pool GetPool(string name) => WorldRenderer.singleton.propsPoolDeliverer.GetPool(name);
    }
}