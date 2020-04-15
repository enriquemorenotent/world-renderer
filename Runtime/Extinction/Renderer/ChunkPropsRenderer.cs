using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Extinction.Data;

namespace Extinction.Renderer
{
    public class ChunkPropsRenderer : MonoBehaviour
    {
        private Config.IWorld config;
        private Config.MapRenderConfig mapRenderConfig;
        private Vector3 renderPosition;
        private bool needsToBeRendered = false;
        private List<PropData> data = new List<PropData>();
        private List<GameObject> props = new List<GameObject>();

        void Update()
        {
            if (!needsToBeRendered) return;
            if (data.Count == 0) return;

            data.ForEach(RenderProp);

            needsToBeRendered = false;
        }

        public void StartRendering(Config.IWorld _config, Config.MapRenderConfig _mapRenderConfig, Vector3 _renderPosition)
        {
            config = _config;
            mapRenderConfig = _mapRenderConfig;
            renderPosition = _renderPosition;

            data.Clear();
            props.Clear();
            needsToBeRendered = true;

            try { Task.Run(GenerateData); }
            catch (System.Exception e) { throw e; }

        }

        void GenerateData()
        {
            data = Utils.PropDataGenerator.LoadDataAt(renderPosition, mapRenderConfig.chunkSize, config);
        }

        void RenderProp(PropData data)
        {
            GameObject instance = GetPool(data.prefab.name).Deliver();
            instance.name = data.prefab.name;
            instance.transform.position = data.position;
            props.Add(instance);
        }

        public void ReturnPropsToPool()
        {
            props.ForEach(ReturnPropToPool);
        }

        void ReturnPropToPool(GameObject instance)
        {
            GetPool(instance.name).Return(instance);
        }

        Utils.Pool GetPool(string name) => WorldRenderer.singleton.propsPoolDeliverer.GetPool(name);
    }
}