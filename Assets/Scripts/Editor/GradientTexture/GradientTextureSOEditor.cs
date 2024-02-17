using System.IO;
using UnityEditor;
using UnityEngine;

namespace fpsRed.Graphics.GradientTextures
{
    [CustomEditor(typeof(GradientTextureSO))]
    public class GradientTextureSOEditor : Editor
    {
        private GradientTextureSO so;

        private SerializedProperty sizeProperty;
        private SerializedProperty gradientProperty;
        private SerializedProperty textureProperty;

        private Vector2Int size;

        private void OnEnable()
        {
            so = (GradientTextureSO)target;

            sizeProperty = serializedObject.FindProperty("size");
            gradientProperty = serializedObject.FindProperty("gradient");
            textureProperty = serializedObject.FindProperty("texture");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            DrawTextureField();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            DrawUpdateTextureButton();
            DrawCreateTextureButton();
        }

        private void DrawTextureField()
        {
            Texture2D texture = (Texture2D)EditorGUILayout.ObjectField("Reference Texture", textureProperty.objectReferenceValue, typeof(Texture2D), false);

            if (texture == textureProperty.objectReferenceValue)
            {
                return;
            }

            if (texture != null && !texture.isReadable)
            {
                return;
            }

            textureProperty.objectReferenceValue = texture;
            _ = serializedObject.ApplyModifiedProperties();
        }

        private void DrawUpdateTextureButton()
        {
            if (!GUILayout.Button("Update Reference Texture"))
            {
                return;
            }

            Texture2D texture = (Texture2D)textureProperty.objectReferenceValue;

            if (texture == null || !texture.isReadable)
            {
                return;
            }

            ApplyGradient(texture);
        }

        private void DrawCreateTextureButton()
        {
            if (!GUILayout.Button("Create New Texture"))
            {
                return;
            }

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

        private void ApplyGradient(Texture2D texture)
        {
            Gradient gradient = gradientProperty.gradientValue;
            if (gradient.mode == GradientMode.Fixed)
            {
                texture.filterMode = FilterMode.Point;
            }
            else
            {
                texture.filterMode = FilterMode.Bilinear;
            }

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