using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OptimizedMultiRaycast : MonoBehaviour
{
    [Header("Camera & Area Settings")]
    public Camera mainCamera;
    public Vector2 areaSize = new Vector2(200, 200);
    public int resolutionX = 10;
    public int resolutionY = 10;
    public float yOffset = 0f;

    [Header("Rotation Settings")]
    public Transform rotationPivot;
    public float rotationDegrees = 0f;

    [Header("Target & Raycast Settings")]
    public string targetTag = "ShadowReceiver";
    public float rayDistance = 100f;
    public LayerMask raycastLayers;
    [Range(0f, 1f)] public float alphaThreshold = 0.1f;

    [Header("Performance Settings")]
    public bool enableRaycasting = true;
    public float maxAllowedFrameTimeMs = 10f;
    public int warmupFrames = 10;

    [Header("Smoothing Settings")]
    [Range(0,5)] public int smoothingIterations = 1;

    [Header("Debug Settings")]
    public bool drawDebugRays = true;

    [Header("Mesh Settings")]
    public GameObject shadowMeshObject;

    // Internal buffers
    private const int MAX_HITS = 4;
    private RaycastHit[] hitsBuffer;
    private List<Vector3> vertices;
    private List<int> triangles;
    private Mesh shadowMesh;

    private int framesElapsed = 0;
    private float timeSinceLastUpdate = 0f;
    private readonly float updateInterval = 1f / 30f;
    private bool normalsDirty = true;
    private bool boundsDirty = true;

    void Start()
    {
        hitsBuffer = new RaycastHit[MAX_HITS];
        vertices = new List<Vector3>(resolutionX * resolutionY);
        triangles = new List<int>((resolutionX - 1) * (resolutionY - 1) * 6);

        if (shadowMeshObject != null)
        {
            shadowMesh = new Mesh { name = "Shadow Mesh" };
            shadowMesh.MarkDynamic();
            shadowMeshObject.GetComponent<MeshFilter>().mesh = shadowMesh;
        }
    }

    void Update()
    {
        if (!enableRaycasting || shadowMeshObject == null) return;

        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate < updateInterval) return;
        timeSinceLastUpdate = 0f;

        if (framesElapsed < warmupFrames)
        {
            framesElapsed++;
            ProcessMesh();
            return;
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        ProcessMesh();
        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > maxAllowedFrameTimeMs)
        {
            Debug.LogWarning($"⚠️ Raycasting disabled. Took {stopwatch.ElapsedMilliseconds}ms (max {maxAllowedFrameTimeMs}ms)");
            enableRaycasting = false;
        }
    }

    void ProcessMesh()
    {
        // Raycast grid
        var grid = CastRays();

        // Smooth results
        for (int i = 0; i < smoothingIterations; i++)
            grid = SmoothGrid(grid);

        BuildGridMesh(grid);
    }

    Vector3?[,] CastRays()
    {
        Vector2 center = new Vector2(Screen.width, Screen.height) * 0.5f;
        Vector2 start = center - areaSize * 0.5f;
        Vector2 step = new Vector2(
            resolutionX > 1 ? areaSize.x / (resolutionX - 1) : 0,
            resolutionY > 1 ? areaSize.y / (resolutionY - 1) : 0);

        Quaternion rot = Quaternion.Euler(rotationDegrees, 0, 0);
        Vector3?[,] grid = new Vector3?[resolutionX, resolutionY];

        for (int y = 0; y < resolutionY; y++)
        {
            for (int x = 0; x < resolutionX; x++)
            {
                Vector2 screenPos = start + new Vector2(x * step.x, y * step.y);
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(
                    new Vector3(screenPos.x, screenPos.y, mainCamera.nearClipPlane + 0.1f))
                    + mainCamera.transform.up * yOffset;

                Vector3 origin = rotationPivot.position + rot * (worldPos - rotationPivot.position);
                Vector3 dir = rot * mainCamera.transform.forward;

                if (drawDebugRays)
                    Debug.DrawRay(origin, dir * rayDistance, Color.green, updateInterval);

                int count = Physics.RaycastNonAlloc(origin, dir, hitsBuffer, rayDistance, raycastLayers);
                if (count == 0) continue;

                // Sort hits by distance
                for (int i = 1; i < count; i++)
                {
                    var key = hitsBuffer[i];
                    int j = i - 1;
                    while (j >= 0 && hitsBuffer[j].distance > key.distance)
                    {
                        hitsBuffer[j + 1] = hitsBuffer[j];
                        j--;
                    }
                    hitsBuffer[j + 1] = key;
                }

                bool passed = false;
                for (int i = 0; i < count; i++)
                {
                    var hit = hitsBuffer[i];
                    var sr = hit.collider.GetComponent<SpriteRenderer>();
                    if (!passed && sr != null && IsHitPixelVisible(sr, hit.point))
                        passed = true;
                    else if (passed && hit.collider.CompareTag(targetTag))
                    {
                        grid[x, y] = shadowMeshObject.transform.InverseTransformPoint(hit.point);
                        break;
                    }
                }
            }
        }
        return grid;
    }

    Vector3?[,] SmoothGrid(Vector3?[,] grid)
    {
        var result = new Vector3?[resolutionX, resolutionY];
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        for (int y = 0; y < resolutionY; y++)
        {
            for (int x = 0; x < resolutionX; x++)
            {
                if (!grid[x, y].HasValue) continue;
                Vector3 sum = grid[x, y].Value;
                int count = 1;
                for (int i = 0; i < 4; i++)
                {
                    int nx = x + dx[i], ny = y + dy[i];
                    if (nx >= 0 && nx < resolutionX && ny >= 0 && ny < resolutionY && grid[nx, ny].HasValue)
                    {
                        sum += grid[nx, ny].Value;
                        count++;
                    }
                }
                result[x, y] = sum / count;
            }
        }
        return result;
    }

    void BuildGridMesh(Vector3?[,] grid)
    {
        shadowMesh.Clear();
        vertices.Clear();
        triangles.Clear();

        int[,] idxMap = new int[resolutionX, resolutionY];
        int idx = 0;
        for (int y = 0; y < resolutionY; y++)
        {
            for (int x = 0; x < resolutionX; x++)
            {
                if (grid[x, y].HasValue)
                {
                    idxMap[x, y] = idx;
                    vertices.Add(grid[x, y].Value);
                    idx++;
                }
                else idxMap[x, y] = -1;
            }
        }

        for (int y = 0; y < resolutionY - 1; y++)
        {
            for (int x = 0; x < resolutionX - 1; x++)
            {
                int a = idxMap[x, y];
                int b = idxMap[x + 1, y];
                int c = idxMap[x, y + 1];
                int d = idxMap[x + 1, y + 1];
                if (a < 0 || b < 0 || c < 0 || d < 0) continue;
                triangles.Add(a); triangles.Add(c); triangles.Add(d);
                triangles.Add(a); triangles.Add(d); triangles.Add(b);
            }
        }

        shadowMesh.SetVertices(vertices);
        shadowMesh.SetTriangles(triangles, 0);
        if (normalsDirty) { shadowMesh.RecalculateNormals(); normalsDirty = false; }
        if (boundsDirty) { shadowMesh.RecalculateBounds(); boundsDirty = false; }
    }

    bool IsHitPixelVisible(SpriteRenderer sr, Vector3 hitPoint)
    {
        Texture2D tex = sr.sprite.texture;
        Vector2 local = sr.transform.InverseTransformPoint(hitPoint);
        Vector2 pivot = sr.sprite.pivot;
        Vector2 ppu = new Vector2(
            sr.sprite.rect.width / sr.sprite.bounds.size.x,
            sr.sprite.rect.height / sr.sprite.bounds.size.y);
        Vector2 coord = pivot + local * ppu;
        int px = Mathf.RoundToInt(sr.sprite.rect.x + coord.x);
        int py = Mathf.RoundToInt(sr.sprite.rect.y + coord.y);
        if (px < 0 || px >= tex.width || py < 0 || py >= tex.height) return false;
        return tex.GetPixel(px, py).a > alphaThreshold;
    }
}
