using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Extinction.Renderer
{
    public class MeshData
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();
        public List<Vector2> uvs = new List<Vector2>();

        public static MeshData LoadDataAt(Vector3 position, int chunkSize)
        {
            MeshData data = new MeshData();

            int offset = 0;
            for (float z = -chunkSize - 0.5f; z <= chunkSize - 0.5f; z++)
                for (float x = -chunkSize - 0.5f; x <= chunkSize - 0.5f; x++)
                {
                    var height = WorldRenderer.Config().GetHeight(position.x + x, position.z + z);
                    List<TileID> tileIdList = WorldRenderer.Config().GetTileIDsAt(position.x + x, position.z + z);

                    tileIdList = tileIdList.OrderBy(tile => tile.terrainID.biome * 10 + tile.terrainID.terrain).ToList();

                    foreach (TileID tileID in tileIdList)
                    {
                        data.vertices.Add(new Vector3(x + 0, height, z + 0));
                        data.vertices.Add(new Vector3(x + 1, height, z + 0));
                        data.vertices.Add(new Vector3(x + 0, height, z + 1));
                        data.vertices.Add(new Vector3(x + 1, height, z + 1));

                        data.triangles.Add(offset + 0);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 3);
                        offset += 4;

                        List<Vector2> uvs = WorldRenderer.Config().GetUVsFor(tileID);
                        data.uvs.AddRange(uvs);
                    }

                    var upperHeight = WorldRenderer.Config().GetHeight(position.x + x, position.z + z + 1);

                    while (upperHeight > height)
                    {
                        data.vertices.Add(new Vector3(x + 0, upperHeight - 1, z + 1));
                        data.vertices.Add(new Vector3(x + 1, upperHeight - 1, z + 1));
                        data.vertices.Add(new Vector3(x + 0, upperHeight + 0, z + 1));
                        data.vertices.Add(new Vector3(x + 1, upperHeight + 0, z + 1));

                        data.triangles.Add(offset + 0);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 3);
                        offset += 4;

                        data.uvs.AddRange(WorldRenderer.Config().GetUVsFor(0, 28));

                        upperHeight--;
                    }

                    while (upperHeight < height)
                    {
                        data.vertices.Add(new Vector3(x + 0, upperHeight + 0, z + 1));
                        data.vertices.Add(new Vector3(x + 1, upperHeight + 0, z + 1));
                        data.vertices.Add(new Vector3(x + 0, upperHeight + 1, z + 1));
                        data.vertices.Add(new Vector3(x + 1, upperHeight + 1, z + 1));

                        data.triangles.Add(offset + 0);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 3);
                        data.triangles.Add(offset + 2);
                        offset += 4;

                        data.uvs.AddRange(WorldRenderer.Config().GetUVsFor(0, 28));

                        upperHeight++;
                    }

                    var rightHeight = WorldRenderer.Config().GetHeight(position.x + x + 1, position.z + z);

                    while (rightHeight > height)
                    {
                        data.vertices.Add(new Vector3(x + 1, rightHeight - 1, z + 1));
                        data.vertices.Add(new Vector3(x + 1, rightHeight - 1, z + 0));
                        data.vertices.Add(new Vector3(x + 1, rightHeight + 0, z + 1));
                        data.vertices.Add(new Vector3(x + 1, rightHeight + 0, z + 0));

                        data.triangles.Add(offset + 0);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 3);
                        offset += 4;

                        data.uvs.AddRange(WorldRenderer.Config().GetUVsFor(0, 28));

                        rightHeight--;
                    }

                    while (rightHeight < height)
                    {
                        data.vertices.Add(new Vector3(x + 1, rightHeight + 0, z + 0));
                        data.vertices.Add(new Vector3(x + 1, rightHeight + 0, z + 1));
                        data.vertices.Add(new Vector3(x + 1, rightHeight + 1, z + 0));
                        data.vertices.Add(new Vector3(x + 1, rightHeight + 1, z + 1));

                        data.triangles.Add(offset + 0);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 3);
                        offset += 4;

                        data.uvs.AddRange(WorldRenderer.Config().GetUVsFor(0, 28));

                        rightHeight++;
                    }
                }

            return data;
        }
    }
}