using UnityEngine;
using UnityEditor;
using Extinction.Config;
using Extinction.Utils;

namespace Extinction.Renderer
{
    // [CustomEditor(typeof(WorldRenderer))]
    public class WorldRendererEditor : Editor
    {
        private SerializedProperty config, chunkPool, radius, chunkSize, cacheRadius, visitedChunkBufferRange, renderProps;

        public void OnEnable()
        {
            config = serializedObject.FindProperty("config");
            chunkPool = serializedObject.FindProperty("chunkPool");
            radius = serializedObject.FindProperty("radius");
            chunkSize = serializedObject.FindProperty("chunkSize");
            cacheRadius = serializedObject.FindProperty("cacheRadius");
            visitedChunkBufferRange = serializedObject.FindProperty("visitedChunkBufferRange");
            renderProps = serializedObject.FindProperty("renderProps");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(10);

            EditorGUILayout.PropertyField(config);
            EditorGUILayout.PropertyField(chunkPool);
            EditorGUILayout.PropertyField(visitedChunkBufferRange);

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("Chunks configuration", EditorStyles.boldLabel);

            GUILayout.Space(8);

            EditorGUILayout.PropertyField(radius, new GUIContent("Radius"));
            EditorGUILayout.LabelField("Defines the radius of the map rendered, meassured in chunks");

            GUILayout.Space(16);

            EditorGUILayout.PropertyField(chunkSize);
            EditorGUILayout.LabelField("Defines the radius of a chunk, meassured in world units");

            GUILayout.Space(16);

            EditorGUILayout.PropertyField(cacheRadius);
            EditorGUILayout.LabelField("Defines the radius that will be preloaded, even if it is not rendered");
            EditorGUILayout.LabelField("Example: 'Radius = 4' and 'Cache radius = 2' will cache a radius of 6 units");

            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(renderProps);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
