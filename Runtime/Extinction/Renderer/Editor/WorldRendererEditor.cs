using UnityEngine;
using UnityEditor;
using Extinction.Config;
using Extinction.Utils;

namespace Extinction.Renderer
{
    [CustomEditor(typeof(WorldRenderer))]
    public class WorldRendererEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            WorldRenderer worldRenderer = (WorldRenderer)target;

            worldRenderer.config = (World)EditorGUILayout.ObjectField("Configuration", worldRenderer.config, typeof(World), true);

            worldRenderer.chunkPool = (Pool)EditorGUILayout.ObjectField("Chunk pool", worldRenderer.chunkPool, typeof(Pool), true);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Map meassurements", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            worldRenderer.radius = EditorGUILayout.IntSlider("Radius:", worldRenderer.radius, 2, 50);
            EditorGUILayout.LabelField("Defines the radius of the map rendered, meassured in chunks");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            worldRenderer.chunkSize = EditorGUILayout.IntSlider("Chunk size", worldRenderer.chunkSize, 2, 50);
            EditorGUILayout.LabelField("Defines the radius of a chunk, meassured in world units");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            worldRenderer.cacheRadius = EditorGUILayout.IntSlider("Cache radius", worldRenderer.cacheRadius, 2, 50);
            EditorGUILayout.LabelField("Defines the radius that will be preloaded, even if it is not rendered");
            EditorGUILayout.LabelField("Example: 'Radius = 4' and 'Cache radius = 2' will cache a radius of 6 units");
            EditorGUILayout.EndVertical();

            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Chunks rendered: " + worldRenderer.renderedChunks.Count, MessageType.Info);
            }

            EditorUtility.SetDirty(worldRenderer);
        }
    }
}
