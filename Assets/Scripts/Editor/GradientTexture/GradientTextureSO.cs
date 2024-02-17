using UnityEngine;

namespace fpsRed.Graphics.GradientTextures
{
    [CreateAssetMenu(fileName = "Gradient Texture", menuName = "Scriptable Object/Gradient Texture")]
    public class GradientTextureSO : ScriptableObject
    {
        [Header("Gradient")]
        [SerializeField] private Gradient gradient;

        [Space]

        [Header("Texture")]
        [SerializeField] private Vector2Int size;
        [SerializeField, HideInInspector] private Texture2D texture;
    }
}