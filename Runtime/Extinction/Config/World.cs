using System.Collections.Generic;
using UnityEngine;
using Extinction.Renderer;
using Extinction.Utils;

namespace Extinction.Config
{
    public interface IWorld
    {
        int GetHeight(float x, float z);

        List<TileID> GetTileIDs(float x, float z);

        List<Vector2> GetUVsFor(TileID tileID);
        List<Vector2> GetUVsFor(int biomeIndex, int terrainIndex, int nscode);
        List<Vector2> GetUVsFor(int col, int row);

        bool HasPropAt(Vector3 position);
        bool HasPropAt(float x, float z);

        GameObject GetProp(float x, float z);

        float GetPropVerticalOffset();
    }

    [CreateAssetMenu(fileName = "Config", menuName = "Extinction/Config/World", order = 1)]
    public class World : ScriptableObject, IWorld
    {
        Noise heightMap, biomeMap, hasPropMap;
        Cache<Vector2, TerrainID> terrainMap;
        public Cache<Vector2, Color> colorMap;

        [Header("Noise scale")]
        public float heightScale;
        public float propsScale;
        public float biomeScale;
        public float terrainScale;
        public float hasPropScale = 10f;

        [Header("Main configuration")]
        [Range(0, 9999)] public int masterSeed;
        public bool useRandomSeed;
        [Range(0, 100)] public int propSparsity = 70;
        [Range(3, 15)] public int maxHeight = 20;
        public float propVerticalOffset = 0.5f;
        public int propsOnlyAboveHeight = 2;


        [Header("Textures")]
        public Sprite sprite;
        public int columns, rows;

        [Header("Ecosystem")]
        public List<Biome> biomes;

        // Methods

        public void Setup()
        {
            masterSeed = useRandomSeed ? Random.Range(1, 9999) : masterSeed;
            biomeMap = new Noise(biomeScale, biomes.Count - 1, masterSeed + 1);
            hasPropMap = new Noise(hasPropScale, 100, masterSeed);
            terrainMap = new Cache<Vector2, TerrainID>(GenerateTerrainID);
            colorMap = new Cache<Vector2, Color>(GenerateTerrainColor);
        }

        // Getters

        Noise HeightMap()
        {
            if (heightMap == null) heightMap = new Noise(heightScale, maxHeight, masterSeed);
            return heightMap;
        }

        public int GetHeight(float x, float z) => HeightMap().At(x, z);

        public Vector3 GetPoint(float x, float z) => new Vector3(x, GetHeight(x, z), z);

        public Vector3 GetPoint(Vector3 point) => GetPoint(point.x, point.z);

        public int GetBiomeID(float x, float z) => biomeMap.At(x, z);

        public TerrainID GetTerrainID(float x, float z) => terrainMap.At(new Vector2(x, z));

        public Biome GetBiome(float x, float z) => biomes[GetBiomeID(x, z)];

        public List<Extinction.Data.Terrain> GetTerrains(float x, float z) => GetBiome(x, z).terrains;

        public Extinction.Data.Terrain GetTerrain(float x, float z) => GetTerrains(x, z)[GetTerrainID(x, z).terrain];

        public Color GetTerrainColor(float x, float z) => colorMap.At(new Vector2(x, z));

        public bool HasPropAt(Vector3 position) => HasPropAt(position.x, position.z);

        public bool HasPropAt(float x, float z) => GetHeight(x, z) >= propsOnlyAboveHeight && hasPropMap.At(x, z) > propSparsity;

        public GameObject GetProp(float x, float z) => GetBiome(x, z).GetProp(x, z);

        public float GetPropVerticalOffset() => propVerticalOffset;

        public List<TileID> GetTileIDs(float x, float z)
        {
            TerrainID t0 = GetTerrainID(x + 0, z + 1);
            TerrainID t1 = GetTerrainID(x + 1, z + 1);
            TerrainID t2 = GetTerrainID(x + 0, z + 0);
            TerrainID t3 = GetTerrainID(x + 1, z + 0);

            return TileID.CreateList(t0, t1, t2, t3);
        }

        // Terrain ID

        public TerrainID GenerateTerrainID(Vector2 position) => GenerateTerrainID(position.x, position.y);

        public TerrainID GenerateTerrainID(float x, float z) =>
            new TerrainID(
                GetBiomeID(x, z),
                IsFlat(x, z) ?
                    Noise.GetValue(x, z, terrainScale, (GetTerrains(x, z).Count - 1), (masterSeed + 2)) :
                    0
            );

        // Color

        public Color GenerateTerrainColor(Vector2 position) => GenerateTerrainColor(position.x, position.y);

        public Color GenerateTerrainColor(float x, float z) => GetTerrain(x, z).color;

        // Other

        bool IsFlat(float x, float z)
        {
            int t00 = GetHeight(x - 0, z - 0);
            int t10 = GetHeight(x - 1, z - 0);
            int t01 = GetHeight(x - 0, z - 1);
            int t11 = GetHeight(x - 1, z - 1);
            return t00 == t10 && t00 == t01 && t00 == t11;
        }

        // UVs

        public List<Vector2> GetUVsFor(TileID tt) => GetUVsFor(tt.terrainID.biome, tt.terrainID.terrain, tt.nscode);

        public List<Vector2> GetUVsFor(int biomeIndex, int terrainIndex, int nscode)
        {
            int row = biomes[biomeIndex].terrains[terrainIndex].row;
            return GetUVsFor(nscode, row);
        }

        public List<Vector2> GetUVsFor(int col, int row)
        {
            List<Vector2> list = new List<Vector2>();

            float xStep = 1f / (float)columns;
            float yStep = 1f / (float)rows;

            list.Add(new Vector2((col + 0) * xStep, (row + 0) * yStep));
            list.Add(new Vector2((col + 1) * xStep, (row + 0) * yStep));
            list.Add(new Vector2((col + 0) * xStep, (row + 1) * yStep));
            list.Add(new Vector2((col + 1) * xStep, (row + 1) * yStep));

            return list;
        }
    }
}
