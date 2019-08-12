using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Extinction.Renderer;
using Extinction.Utils;

namespace Extinction.Config
{
    [CreateAssetMenu(fileName = "Config", menuName = "Extinction/Config/World", order = 1)]
    public class World : ScriptableObject
    {
        Noise heightMap, biomeMap, hasPropMap;
        Cache<Vector2, TerrainID> terrainMap;

        [Header("Noise scale")]
        [Range(1.0f, 200.0f)] public float heightScale;
        [Range(1.0f, 200.0f)] public float propsScale;
        [Range(1.0f, 200.0f)] public float biomeScale;
        [Range(1.0f, 200.0f)] public float hasPropScale = 10f;

        [Header("Main configuration")]
        [Range(0, 9999)] public int masterSeed;
        [Range(0, 100)] public int propSparsity = 70;
        [Range(3, 15)] public int maxHeight = 20;
        public float propVerticalOffset = 0.5f;


        [Header("Textures")]
        public Sprite sprite;
        public int columns, rows;

        [Header("Ecosystem")]
        public List<Biome> biomes;

        // Methods

        public void Setup()
        {
            heightMap  = new Noise(heightScale, maxHeight, masterSeed);
            biomeMap   = new Noise(biomeScale, biomes.Count - 1, masterSeed + 1);
            hasPropMap = new Noise(hasPropScale, 100, masterSeed);
            terrainMap = new Cache<Vector2, TerrainID>(GenerateTerrainIdAt);
        }

        public int GetHeight(float x, float z) => heightMap.At(x, z);

        public Biome GetBiome(float x, float z) => biomes[biomeMap.At(x, z)];

        public List<int> GetTerrains(float x, float z) => GetBiome(x, z).terrains;

        public bool HasPropAt(float x, float z) => hasPropMap.At(x, z) > propSparsity;

        public GameObject GetProp(float x, float z) => biomes[0].props[0].prefab;

        public List<TileID> GetTileIDsAt(float x, float z)
        {
            TerrainID t0 = GetTerrainIDAt(x + 0, z + 1);
            TerrainID t1 = GetTerrainIDAt(x + 1, z + 1);
            TerrainID t2 = GetTerrainIDAt(x + 0, z + 0);
            TerrainID t3 = GetTerrainIDAt(x + 1, z + 0);
            return TileID.CreateList(t0, t1, t2, t3);
        }

        // Terrain ID

        public TerrainID GenerateTerrainIdAt(Vector2 position) => GenerateTerrainIdAt(position.x, position.y);

        public TerrainID GenerateTerrainIdAt(float x, float z)
        {
            int biomeID = biomeMap.At(x, z);
            int terrain = 0;

            if (IsFlat(x, z)) terrain = Noise.GetValue(x, z, biomeScale, (GetTerrains(x, z).Count - 1), (masterSeed + 2));

            return new TerrainID(biomeID, terrain);
        }

        // Other

        TerrainID GetTerrainIDAt(float x, float z) => terrainMap.At(new Vector2(x, z));

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
            int row = biomes[biomeIndex].terrains[terrainIndex];
            return GetUVsFor(nscode, row);
        }

        public List<Vector2> GetUVsFor(int col, int row)
        {
            List<Vector2> list = new List<Vector2>();

            float xStep = 1f / (float) columns;
            float yStep = 1f / (float) rows;

            list.Add(new Vector2((col + 0) * xStep, (row + 0) * yStep));
            list.Add(new Vector2((col + 1) * xStep, (row + 0) * yStep));
            list.Add(new Vector2((col + 0) * xStep, (row + 1) * yStep));
            list.Add(new Vector2((col + 1) * xStep, (row + 1) * yStep));

            return list;
        }
    }
}
