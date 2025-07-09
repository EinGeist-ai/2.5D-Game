using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSpriteMeshUpdater : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private MeshFilter meshFilter;
    private Sprite lastSprite;
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Find the child MeshFilter (assumes child named "ShadowMesh")
        meshFilter = transform.Find("ShadowMesh")?.GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            Debug.LogError("Child with MeshFilter not found! Please create a child GameObject named 'ShadowMesh' with a MeshFilter.");
        }
    }

    void LateUpdate()
    {
        if (spriteRenderer.sprite == null || meshFilter == null) return;

        if (spriteRenderer.sprite != lastSprite)
        {
            // Destroy old mesh to free GPU memory
            if (meshFilter.mesh != null)
            {
                Destroy(meshFilter.mesh);
            }
            // Generate and assign new mesh based on current sprite
            meshFilter.mesh = spriteRenderer.sprite.GetAccurateMesh();
            lastSprite = spriteRenderer.sprite;
        }
    }
}

public static class SpriteExtensions
{
    public static Mesh GetAccurateMesh(this Sprite sprite, float alphaThreshold = 0.1f)
    {
        Texture2D tex = sprite.texture;
        Vector2[] uv = sprite.uv;
        Vector3[] originalVertices = Array.ConvertAll(sprite.vertices, v => (Vector3)v);
        ushort[] originalTriangles = sprite.triangles;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        // Map from original vertex index to new vertex index
        int[] vertexMap = new int[originalVertices.Length];
        for (int i = 0; i < vertexMap.Length; i++) vertexMap[i] = -1;

        // Filter vertices by checking alpha at UV coordinate
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector2 uvCoord = uv[i];
            Color pixelColor = tex.GetPixelBilinear(uvCoord.x, uvCoord.y);
            if (pixelColor.a >= alphaThreshold)
            {
                vertexMap[i] = vertices.Count;
                vertices.Add(originalVertices[i]);
                uvs.Add(uvCoord);
            }
        }

        // Rebuild triangles only if all 3 vertices are visible
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            int i0 = originalTriangles[i];
            int i1 = originalTriangles[i + 1];
            int i2 = originalTriangles[i + 2];

            if (vertexMap[i0] != -1 && vertexMap[i1] != -1 && vertexMap[i2] != -1)
            {
                triangles.Add(vertexMap[i0]);
                triangles.Add(vertexMap[i1]);
                triangles.Add(vertexMap[i2]);
            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }
}
