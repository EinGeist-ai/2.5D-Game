using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

public class SpriteToMeshTool : EditorWindow
{
    [MenuItem("Tools/Sprite To Mesh Generator")]
    public static void ShowWindow()
    {
        GetWindow<SpriteToMeshTool>("Sprite To Mesh");
    }

    void OnGUI()
    {
        GUILayout.Label("Convert selected Sprites or Textures into Mesh Prefabs", EditorStyles.boldLabel);

        if (GUILayout.Button("Convert Selected Sprites to Mesh Prefabs"))
        {
            ConvertSpritesToMeshes();
        }
    }

    void ConvertSpritesToMeshes()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        List<Sprite> sprites = new List<Sprite>();

        foreach (UnityEngine.Object obj in selection)
        {
            if (obj is Sprite sprite)
            {
                sprites.Add(sprite);
            }
            else if (obj is Texture2D texture)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                UnityEngine.Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (UnityEngine.Object sub in subAssets)
                {
                    if (sub is Sprite subSprite)
                        sprites.Add(subSprite);
                }
            }
        }

        if (sprites.Count == 0)
        {
            Debug.LogWarning("No Sprites found in selection.");
            return;
        }

        string folderPath = "Assets/SpriteMeshes";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        foreach (Sprite sprite in sprites)
        {
            CreateMeshFromSprite(sprite, folderPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Converted {sprites.Count} sprite(s) to mesh prefab(s) in '{folderPath}'.");
    }

    void CreateMeshFromSprite(Sprite sprite, string folderPath)
    {
        Mesh mesh = new Mesh();
        mesh.name = sprite.name;
        mesh.vertices = Array.ConvertAll(sprite.vertices, v => (Vector3)v);
        mesh.uv = sprite.uv;
        mesh.triangles = Array.ConvertAll(sprite.triangles, t => (int)t);
        mesh.RecalculateBounds();

        string meshPath = $"{folderPath}/{sprite.name}.mesh";
        AssetDatabase.CreateAsset(mesh, meshPath);

        GameObject go = new GameObject(sprite.name, typeof(MeshFilter), typeof(MeshRenderer));
        go.GetComponent<MeshFilter>().sharedMesh = mesh;

        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.mainTexture = sprite.texture;
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;

        string prefabPath = $"{folderPath}/{sprite.name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);

        GameObject.DestroyImmediate(go);
    }
}
