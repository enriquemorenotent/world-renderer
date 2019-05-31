using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Extinction.Renderer;
using Extinction.Utils;

namespace Extinction.Config
{
    [System.Serializable]
    public class Biome
    {
        #region Attributes

        public string name;
        public List<int> terrains;
        public List<GameObject> props;

        Noise propDistribution;

        [Range(0.0f, 1.0f)]
        public float propsThreshold = 0.5f;

        [Range(1.0f, 200.0f)]
        public float propsScale = 10f;

        #endregion

        public GameObject GetProp(float x, float z)
        {
            if (this.propDistribution == null)
                this.propDistribution = new Noise(propsThreshold, propsScale, props.Count - 1, 666);

            return this.props[this.propDistribution.At(x, z)];
        }
    }
}
