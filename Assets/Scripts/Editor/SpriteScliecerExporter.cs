#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class SpriteSlicerExporter : EditorWindow
{
    private Texture2D sourceTexture;

    [MenuItem("Tools/Sprite Slicer Exporter")]
    public static void ShowWindow()
    {
        GetWindow<SpriteSlicerExporter>("Sprite Slicer Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Export Sliced Sprites as PNGs", EditorStyles.boldLabel);
        sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Sliced Sprite Texture", sourceTexture, typeof(Texture2D), false);

        if (sourceTexture != null && GUILayout.Button("Export Slices"))
        {
            ExportSlices();
        }
    }

    private void ExportSlices()
    {
        if (sourceTexture == null)
        {
            Debug.LogError("No texture assigned.");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(sourceTexture);
        string directory = Path.GetDirectoryName(assetPath);
        string textureName = Path.GetFileNameWithoutExtension(assetPath);

        // Get all sub-sprites
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToArray();

        if (sprites.Length == 0)
        {
            Debug.LogWarning("No sliced sprites found in the selected texture.");
            return;
        }

        foreach (Sprite sprite in sprites)
        {
            Rect rect = sprite.textureRect;
            Texture2D newTex = new Texture2D((int)rect.width, (int)rect.height);
            Color[] pixels = sourceTexture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            newTex.SetPixels(pixels);
            newTex.Apply();

            byte[] pngData = newTex.EncodeToPNG();
            string outPath = Path.Combine(directory, textureName + "_" + sprite.name + ".png");

            File.WriteAllBytes(outPath, pngData);
            Debug.Log($"Saved slice: {outPath}");
        }

        AssetDatabase.Refresh();
    }
}
#endif