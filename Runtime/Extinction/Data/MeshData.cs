using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extinction.Config;

namespace Extinction.Data
{
    public class MeshData
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();
        public List<Vector2> uvs = new List<Vector2>();
    }
}