using System.IO;
using UnityEditor;
using UnityEngine;

namespace fpsRed.Graphics.GradientTextures
{
    [CustomEditor(typeof(GradientTextureSO))]
    public class GradientTextureSOEditor : Editor
    {
        private GradientTextureSO so;

        private SerializedProperty referenceTextureProperty;
        private SerializedProperty sizeProperty;
        private SerializedProperty gradientProperty;

        private Vector2Int size;

        private void OnEnable()
        {
            so = (GradientTextureSO)target;

            referenceTextureProperty = serializedObject.FindProperty("referenceTexture");
            sizeProperty = serializedObject.FindProperty("size");
            gradientProperty = serializedObject.FindProperty("gradient");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Create New Texture"))
            {
                size = sizeProperty.vector2IntValue;

                if (size.x < 1 || size.y < 1)
                {
                    Debug.LogError("Cannot create a texture with less than 1 pixels!");
                    return;
                }

                Texture2D newTexture = new(size.x, size.y, TextureFormat.ARGB32, false);
                ApplyGradient(newTexture);

                byte[] byteArray = newTexture.EncodeToPNG();

                string directoryPath = $"{Path.GetDirectoryName(AssetDatabase.GetAssetPath(so))}/{so.name}.png";
                string filePath = AssetDatabase.GenerateUniqueAssetPath(directoryPath);

                File.WriteAllBytes(filePath, byteArray);

                AssetDatabase.Refresh();

                _ = serializedObject.ApplyModifiedProperties();
            }
        }

        private void ApplyGradient(Texture2D texture)
        {
            Gradient gradient = gradientProperty.gradientValue;
            Color color;

            for (int x = 0; x < texture.width; x++)
            {
                color = gradient.Evaluate((float)x / texture.width);

                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
        }
    }
}