using UnityEngine;
using UnityEditor;

public class RandomTerrainPainter : EditorWindow
{
    [SerializeField] private Terrain terrain;
    [SerializeField] private int seed = 12345;
    [SerializeField, Range(0f, 1f)] private float layer0Weight = 0.5f;

    [MenuItem("Tools/Random Terrain Painter")]
    public static void ShowWindow()
    {
        GetWindow<RandomTerrainPainter>("Random Terrain Painter");
    }

    void OnGUI()
    {
        terrain = (Terrain)EditorGUILayout.ObjectField("Terrain", terrain, typeof(Terrain), true);
        seed = EditorGUILayout.IntField("Random Seed", seed);
        layer0Weight = EditorGUILayout.Slider("Layer 0 Probability", layer0Weight, 0f, 1f);

        if (terrain != null && GUILayout.Button("Apply Random Terrain Layers"))
        {
            ApplyRandomLayers();
        }
    }

    void ApplyRandomLayers()
    {
        TerrainData data = terrain.terrainData;
        int width = data.alphamapWidth;
        int height = data.alphamapHeight;
        int layerCount = data.alphamapLayers;

        if (layerCount < 2)
        {
            Debug.LogError("You need at least 2 terrain layers assigned to the terrain.");
            return;
        }

        float[,,] map = new float[width, height, layerCount];
        System.Random rng = new System.Random(seed);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int chosen;

                // Favor Layer 0 (index 0)
                if (rng.NextDouble() < layer0Weight)
                {
                    chosen = 0;
                }
                else
                {
                    // Pick another random layer that's not 0
                    do {
                        chosen = rng.Next(layerCount);
                    } while (chosen == 0 && layerCount > 1);
                }

                // Zero all layers
                for (int i = 0; i < layerCount; i++)
                {
                    map[x, y, i] = 0f;
                }

                // Set only chosen layer to 1
                map[x, y, chosen] = 1f;
            }
        }

        data.SetAlphamaps(0, 0, map);
        Debug.Log("Random terrain layers applied with Layer 0 favored.");
    }
}
