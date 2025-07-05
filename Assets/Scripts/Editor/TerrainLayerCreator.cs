using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TerrainLayerCreatorAndAssigner
{
    [MenuItem("Tools/Create and Assign Terrain Layers")]
    public static void CreateAndAssignTerrainLayers()
    {
        string texturesPath = "Assets/Terrain/Grass"; // Folder containing textures
        string outputPath = "Assets/Terrain/Grass";      // Folder to save TerrainLayer assets

        // Ensure the output folder exists
        if (!AssetDatabase.IsValidFolder(outputPath))
            AssetDatabase.CreateFolder("Assets", "TerrainLayers");

        // Get all Texture2D assets in the folder
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { texturesPath });
        if (textureGuids.Length == 0)
        {
            Debug.LogWarning("No textures found in folder: " + texturesPath);
            return;
        }

        // Try to get the selected terrain
        Terrain terrain = Selection.activeGameObject?.GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogWarning("Please select a Terrain in the scene before running this.");
            return;
        }

        List<TerrainLayer> terrainLayers = new List<TerrainLayer>();

        foreach (string guid in textureGuids)
        {
            string texturePath = AssetDatabase.GUIDToAssetPath(guid);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

            if (texture == null)
            {
                Debug.LogWarning("Could not load texture at path: " + texturePath);
                continue;
            }

            string layerName = Path.GetFileNameWithoutExtension(texturePath);
            string layerAssetPath = Path.Combine(outputPath, layerName + ".terrainlayer");

            // Check if the TerrainLayer already exists
            TerrainLayer terrainLayer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(layerAssetPath);
            if (terrainLayer == null)
            {
                // Create and save new TerrainLayer
                terrainLayer = new TerrainLayer();
                terrainLayer.diffuseTexture = texture;
                terrainLayer.tileSize = new Vector2(10, 10); // Change tiling if needed

                AssetDatabase.CreateAsset(terrainLayer, layerAssetPath);
            }

            terrainLayers.Add(terrainLayer);
        }

        // Assign layers to the selected terrain
        terrain.terrainData.terrainLayers = terrainLayers.ToArray();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created and assigned {terrainLayers.Count} terrain layers to '{terrain.name}'.");
    }
}
