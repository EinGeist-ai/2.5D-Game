using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TilemapToMeshGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Material meshMaterial;

    void Start()
    {
        Mesh mesh = GenerateMesh();

        if (mesh != null)
        {
            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshRenderer>().material = meshMaterial;

#if UNITY_EDITOR
            SaveMeshAsset(mesh);
#endif
        }
    }

    Mesh GenerateMesh()
    {
        if (!tilemap)
        {
            Debug.LogError("Tilemap not assigned!");
            return null;
        }

        Mesh mesh = new Mesh();
        BoundsInt bounds = tilemap.cellBounds;
        int tileCount = 0;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Tile tile = tilemap.GetTile<Tile>(new Vector3Int(x, y, 0));
                if (tile == null || tile.sprite == null) continue;
                tileCount++;
            }
        }

        Vector3[] vertices = new Vector3[tileCount * 4];
        Vector2[] uvs = new Vector2[tileCount * 4];
        int[] triangles = new int[tileCount * 6];

        int v = 0, t = 0;
        Texture2D texture = null;
        Vector3 cellSize = tilemap.cellSize;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                Tile tile = tilemap.GetTile<Tile>(pos);
                if (tile == null || tile.sprite == null) continue;

                Sprite sprite = tile.sprite;
                if (texture == null) texture = sprite.texture;

                Vector3 center = tilemap.GetCellCenterLocal(pos);
                float halfX = cellSize.x * 0.5f;
                float halfY = cellSize.y * 0.5f;

                vertices[v + 0] = center + new Vector3(-halfX, -halfY, 0);
                vertices[v + 1] = center + new Vector3(halfX, -halfY, 0);
                vertices[v + 2] = center + new Vector3(-halfX, halfY, 0);
                vertices[v + 3] = center + new Vector3(halfX, halfY, 0);

                Rect texRect = sprite.textureRect;
                Vector2 texSize = new Vector2(texture.width, texture.height);

                Vector2 uv00 = new Vector2(texRect.xMin / texSize.x, texRect.yMin / texSize.y);
                Vector2 uv11 = new Vector2(texRect.xMax / texSize.x, texRect.yMax / texSize.y);

                uvs[v + 0] = new Vector2(uv00.x, uv00.y);
                uvs[v + 1] = new Vector2(uv11.x, uv00.y);
                uvs[v + 2] = new Vector2(uv00.x, uv11.y);
                uvs[v + 3] = new Vector2(uv11.x, uv11.y);

                triangles[t + 0] = v + 0;
                triangles[t + 1] = v + 2;
                triangles[t + 2] = v + 1;
                triangles[t + 3] = v + 2;
                triangles[t + 4] = v + 3;
                triangles[t + 5] = v + 1;

                v += 4;
                t += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

#if UNITY_EDITOR
    void SaveMeshAsset(Mesh mesh)
    {
        string folderPath = "Assets/GeneratedMeshes";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        string meshName = gameObject.name + "_Mesh.asset";
        string assetPath = Path.Combine(folderPath, meshName);

        // Avoid overwriting existing mesh assets
        string uniquePath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
        AssetDatabase.CreateAsset(mesh, uniquePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Saved mesh to: {uniquePath}");
    }
#endif
}
