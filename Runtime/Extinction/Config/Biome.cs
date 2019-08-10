using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Extinction.Renderer;
using Extinction.Utils;

namespace Extinction.Config
{
    [System.Serializable]
    public class WeightedProp
    {
        public string name = "not-named";
        public GameObject prefab;
        public int weight = 100;
    }

    [System.Serializable]
    public class Biome
    {
        // Attributes

        public string name;
        public List<int> terrains;
        public List<WeightedProp> props;

        Noise propDistribution;

        [Header("Which prop?")]
        [Range(1f, 200.0f)] public float propsScale = 10f;

        public int TotalPropWeight()
        {
            int total = 0;

            foreach (var prop in this.props)
            {
                total += prop.weight;
            }

            return total;
        }

        public GameObject GetPropForWeight(int index)
        {
            foreach (var prop in this.props)
            {
                if (prop.weight > index) return prop.prefab;
                index -= prop.weight;
            }

            return this.props[0].prefab;
        }

        public GameObject GetProp(float x, float z)
        {
            if (this.propDistribution == null)
                this.propDistribution = new Noise(propsScale, this.TotalPropWeight() - 1, 666);

            return this.GetPropForWeight(this.propDistribution.At(x, z));
        }
    }
}
