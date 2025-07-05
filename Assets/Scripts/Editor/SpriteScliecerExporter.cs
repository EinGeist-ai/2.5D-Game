#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class RandomSpriteScatterer : EditorWindow
{
    [SerializeField] private Terrain targetTerrain;
    [SerializeField] private List<Sprite> spriteList = new List<Sprite>();
    [SerializeField] private int spriteCount = 100;
    [SerializeField] private float fixedHeight = 0f;
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float maxSize = 1.5f;
    [SerializeField] private int seed = 12345;

    [MenuItem("Tools/2D Sprite Scatterer")]
    public static void ShowWindow()
    {
        GetWindow<RandomSpriteScatterer>("2D Sprite Scatterer");
    }

    void OnGUI()
    {
        targetTerrain = (Terrain)EditorGUILayout.ObjectField("Terrain", targetTerrain, typeof(Terrain), true);
        spriteCount = EditorGUILayout.IntField("Sprite Count", spriteCount);
        fixedHeight = EditorGUILayout.FloatField("Fixed Sprite Height (Y)", fixedHeight);

        EditorGUILayout.LabelField("Sprite Size Range");
        minSize = EditorGUILayout.FloatField("Min Size", minSize);
        maxSize = EditorGUILayout.FloatField("Max Size", maxSize);
        if (maxSize < minSize) maxSize = minSize; // Ensure max >= min

        seed = EditorGUILayout.IntField("Random Seed", seed);

        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty listProperty = serializedObject.FindProperty("spriteList");
        EditorGUILayout.PropertyField(listProperty, true);
        serializedObject.ApplyModifiedProperties();

        GUI.enabled = targetTerrain != null && spriteList.Count > 0;

        if (GUILayout.Button("Scatter Sprites"))
        {
            ScatterSprites();
        }

        GUI.enabled = true;

        GUILayout.Space(10);

        if (GUILayout.Button("Remove All Sprites"))
        {
            RemoveAllSprites();
        }
    }

    void ScatterSprites()
    {
        if (targetTerrain == null || spriteList.Count == 0)
        {
            Debug.LogWarning("Assign a terrain and at least one sprite.");
            return;
        }

        TerrainData terrainData = targetTerrain.terrainData;
        Vector3 terrainPos = targetTerrain.transform.position;
        Vector3 terrainSize = terrainData.size;

        GameObject container = new GameObject("ScatteredSprites");
        container.transform.position = Vector3.zero;

        System.Random rng = new System.Random(seed);

        for (int i = 0; i < spriteCount; i++)
        {
            float x = (float)rng.NextDouble() * terrainSize.x + terrainPos.x;
            float z = (float)rng.NextDouble() * terrainSize.z + terrainPos.z;

            // Fixed Y height relative to terrain base
            float y = terrainPos.y + fixedHeight;

            Vector3 position = new Vector3(x, y, z);

            GameObject spriteGO = new GameObject("Sprite_" + i);
            spriteGO.transform.position = position;

            // Random scale between minSize and maxSize
            float scale = Mathf.Lerp(minSize, maxSize, (float)rng.NextDouble());
            spriteGO.transform.localScale = Vector3.one * scale;

            spriteGO.transform.SetParent(container.transform);

            SpriteRenderer sr = spriteGO.AddComponent<SpriteRenderer>();
            sr.sprite = spriteList[rng.Next(spriteList.Count)];
            sr.sortingOrder = 10;
        }

        Debug.Log($"Placed {spriteCount} sprites at fixed height {fixedHeight} with size range [{minSize}, {maxSize}].");
    }

    void RemoveAllSprites()
    {
        GameObject container = GameObject.Find("ScatteredSprites");
        if (container != null)
        {
            DestroyImmediate(container);
            Debug.Log("Removed all scattered sprites.");
        }
        else
        {
            Debug.LogWarning("No scattered sprites container found to remove.");
        }
    }
}
#endif
