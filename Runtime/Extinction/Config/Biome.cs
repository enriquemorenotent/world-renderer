using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extinction.Renderer;
using Extinction.Utils;

namespace Extinction.Config
{
    [System.Serializable]
    public class Biome
    {
        // Attributes

        public string name;
        public List<Extinction.Data.Terrain> terrains;
        public List<Extinction.Data.WeightedProp> props;
        [Range(1f, 200.0f)] public float propDistributionScale = 10f;

        Noise propDistribution;

        int TotalPropWeight => this.props.Aggregate(0, (accum, item) => accum + item.weight);

        GameObject GetPropForWeight(int index)
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
                this.propDistribution = new Noise(propDistributionScale, TotalPropWeight - 1, 666);

            return this.GetPropForWeight(this.propDistribution.At(x, z));
        }
    }
}
