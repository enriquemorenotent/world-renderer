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
        #region Height noise

        [Header("Height noise configuration")]

        [Range(0.0f, 1.0f)]
        public float heightThreshold;

        [Range(0.0f, 200.0f)]
        public float heightScale;

        #endregion

        #region Props noise

        [Header("Props noise configuration")]

        [Range(0.0f, 1.0f)]
        public float propsThreshold;

        [Range(0.0f, 200.0f)]
        public float propsScale;

        #endregion

        #region Biome noise

        [Header("Biome noise configuration")]

        [Range(0.0f, 1.0f)]
        public float biomeThreshold;

        [Range(0.0f, 200.0f)]
        public float biomeScale;

        #endregion

        #region Other attributes

        Noise heightMap, biomeMap, propMap;
        Cache<Vector2, TerrainID> terrainMap;

        [Header("Main configuration")]
        public int masterSeed;

        [Range(3, 15)]
        public int maxHeight = 20;

        [Header("Texture")]
        public Sprite sprite;
        public int columns, rows;

        [Header("Biomes")]
        public List<Biome> biomes;

        #endregion

        #region Public methods

        public void Setup()
        {
            this.heightMap = new Noise(heightThreshold, heightScale, maxHeight, masterSeed);
            this.biomeMap = new Noise(biomeThreshold, biomeScale, biomes.Count - 1, masterSeed + 1);
            this.propMap = new Noise(propsThreshold, propsScale, maxHeight, masterSeed);
            this.terrainMap = new Cache<Vector2, TerrainID>(this.GenerateTerrainIdAt);
        }

        public int GetHeight(float x, float z)
        {
            return this.heightMap.At(x, z);
        }

        public Biome GetBiome(float x, float z)
        {
            return this.biomes[this.biomeMap.At(x, z)];
        }

        public bool HasPropAt(float x, float z)
        {
            return propMap.At(x, z) > 6;
        }

        public List<TileID> GetTileIDsAt(float x, float z)
        {
            TerrainID t0 = GetTerrainIDAt(x + 0, z + 1);
            TerrainID t1 = GetTerrainIDAt(x + 1, z + 1);
            TerrainID t2 = GetTerrainIDAt(x + 0, z + 0);
            TerrainID t3 = GetTerrainIDAt(x + 1, z + 0);
            return TileID.CreateList(t0, t1, t2, t3);
        }

        public GameObject GetProp(float x, float z)
        {
            return biomes[0].props[0];
        }

        #endregion

        #region Generate Terrain ID

        TerrainID GenerateTerrainIdAt(Vector2 position)
        {
            return GenerateTerrainIdAt(position.x, position.y);
        }

        TerrainID GenerateTerrainIdAt(float x, float z)
        {
            int biomeID, terrain;
            biomeID = biomeMap.At(x, z);
            if (IsFlat(x, z))
            {
                List<int> terrains = biomes[biomeID].terrains;
                terrain = Noise.GetValue(x, z, biomeThreshold, biomeScale, (terrains.Count - 1), (masterSeed + 2));
            }
            else
            {
                terrain = 0;
            }

            return new TerrainID(biomeID, terrain);
        }

        #endregion

        #region Other

        TerrainID GetTerrainIDAt(float x, float z)
        {
            return terrainMap.At(new Vector2(x, z));
        }

        // To refactor
        bool IsFlat(float x, float z)
        {
            return GetHeight(x - 0, z - 0) == GetHeight(x - 1, z - 0) &&
                GetHeight(x - 0, z - 0) == GetHeight(x - 0, z - 1) &&
                GetHeight(x - 0, z - 0) == GetHeight(x - 1, z - 1);
        }

        #endregion

        #region GetUVs

        public List<Vector2> GetUVsFor(TileID tt)
        {
            return GetUVsFor(tt.terrainID.biome, tt.terrainID.terrain, tt.nscode);
        }

        public List<Vector2> GetUVsFor(int biomeIndex, int terrainIndex, int nscode)
        {
            int row = biomes[biomeIndex].terrains[terrainIndex];
            List<Vector2> list = GetUVsFor(nscode, row);
            return list;
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

        #endregion
    }
}
