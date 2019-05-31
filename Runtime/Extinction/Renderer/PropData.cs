using UnityEngine;
using System.Collections.Generic;

namespace Extinction.Renderer
{
    public class PropData
    {
        public Vector3 position;
        public GameObject prefab;

        public static List<PropData> LoadDataAt(Vector3 chunkPosition, int chunkSize)
        {
            List<PropData> dataList = new List<PropData>();

            for (float z = -chunkSize - 0.5f; z <= chunkSize - 0.5f; z++)
                for (float x = -chunkSize - 0.5f; x <= chunkSize - 0.5f; x++)
                {
                    Vector3 position = chunkPosition + new Vector3(x, 20, z);

                    bool hasProp = WorldRenderer.Config().HasPropAt(position.x, position.z);

                    if (hasProp)
                    {
                        PropData propData = new PropData();
                        propData.position = position;
                        propData.position.y = WorldRenderer.Config().GetHeight(position.x, position.z);
                        propData.position += Vector3.one / 2;
                        propData.prefab = WorldRenderer.Config().GetBiome(position.x, position.z).GetProp(position.x, position.z);
                        dataList.Add(propData);
                    }
                }

            return dataList;
        }
    }
}