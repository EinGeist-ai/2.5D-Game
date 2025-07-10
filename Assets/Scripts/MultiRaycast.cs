using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiRaycast : MonoBehaviour
{
    [Header("Camera & Area Settings")]
    public Camera mainCamera;
    public Vector2 areaSize = new Vector2(200, 200); // Screen space area in pixels
    public int resolutionX = 10;
    public int resolutionY = 10;
    public float yOffset = 0f;

    [Header("Rotation Settings")]
    public Transform rotationPivot;         // World space pivot to rotate rays around
    public float rotationDegrees = 0f;      // Rotation angle in degrees (around X axis)

    [Header("Target & Raycast Settings")]
    public string targetTag = "ShadowReceiver";
    public GameObject prefabToSpawn;
    public float rayDistance = 100f;
    public LayerMask raycastLayers;
    [Range(0f, 1f)]
    public float alphaThreshold = 0.1f;

    [Header("Performance Settings")]
    public bool enableRaycasting = true;
    public float maxAllowedFrameTimeMs = 10f;

    [Header("Warmup Settings")]
    public int warmupFrames = 10;

    private List<GameObject> objectPool = new List<GameObject>();
    private int poolIndex = 0;
    private int framesElapsed = 0;

    void Start()
    {
        Application.targetFrameRate = 60;
    }
    void Update()
    {
        if (!enableRaycasting) return;

        foreach (var obj in objectPool)
            if (obj != null)
                obj.SetActive(false);
        poolIndex = 0;

        if (framesElapsed < warmupFrames)
        {
            framesElapsed++;
            CastRays();
            return;
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        CastRays();

        stopwatch.Stop();
        if (stopwatch.ElapsedMilliseconds > maxAllowedFrameTimeMs)
        {
            UnityEngine.Debug.LogWarning($"⚠️ Raycasting disabled to protect performance. Took {stopwatch.ElapsedMilliseconds}ms (max allowed: {maxAllowedFrameTimeMs}ms)");
            enableRaycasting = false;
        }
    }

    void CastRays()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 start = screenCenter - areaSize / 2f;
        Vector2 step = new Vector2(
            resolutionX > 1 ? areaSize.x / (resolutionX - 1) : 0,
            resolutionY > 1 ? areaSize.y / (resolutionY - 1) : 0);

        Quaternion rotation = Quaternion.Euler(rotationDegrees, 0, 0);

        for (int y = 0; y < resolutionY; y++)
        {
            for (int x = 0; x < resolutionX; x++)
            {
                Vector2 screenPos = start + new Vector2(x * step.x, y * step.y);

                // Base world position from screen pos near clip plane
                Vector3 baseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, mainCamera.nearClipPlane + 0.1f));
                baseWorldPos += mainCamera.transform.up * yOffset;

                // Rotate position offset around pivot on X axis
                Vector3 offset = baseWorldPos - rotationPivot.position;
                Vector3 rotatedOffset = rotation * offset;
                Vector3 rayOrigin = rotationPivot.position + rotatedOffset;

                // Option 2: Ray direction is fixed orthographic style, camera forward direction
                Vector3 direction = mainCamera.transform.forward;

                RaycastHit[] hits = Physics.RaycastAll(rayOrigin, direction, rayDistance, raycastLayers);
                if (hits.Length == 0) continue;

                hits = hits.OrderBy(h => h.distance).ToArray();

                bool spritePassed = false;

                foreach (RaycastHit hit in hits)
                {
                    var sr = hit.collider.GetComponent<SpriteRenderer>();
                    if (!spritePassed && sr != null && sr.sprite != null && sr.sprite.texture != null)
                    {
                        if (IsHitPixelVisible(sr, hit.point))
                        {
                            spritePassed = true;
                        }
                    }
                    else if (spritePassed)
                    {
                        if (hit.collider.CompareTag(targetTag))
                        {
                            GameObject spawned = GetPooledObject();
                            spawned.transform.position = hit.point;
                            spawned.transform.rotation = Quaternion.identity;
                            break;
                        }
                    }
                }

#if UNITY_EDITOR
                UnityEngine.Debug.DrawRay(rayOrigin, direction * rayDistance, Color.green, 0.5f);
#endif
            }
        }
    }

    private bool IsHitPixelVisible(SpriteRenderer sr, Vector3 hitPoint)
    {
        Texture2D tex = sr.sprite.texture;

        Vector2 localPos = sr.transform.InverseTransformPoint(hitPoint);
        Vector2 pivot = sr.sprite.pivot;
        Vector2 pixelsPerUnit = new Vector2(
            sr.sprite.rect.width / sr.sprite.bounds.size.x,
            sr.sprite.rect.height / sr.sprite.bounds.size.y);
        Vector2 texCoord = pivot + new Vector2(localPos.x, localPos.y) * pixelsPerUnit;

        int px = Mathf.RoundToInt(sr.sprite.rect.x + texCoord.x);
        int py = Mathf.RoundToInt(sr.sprite.rect.y + texCoord.y);

        if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
        {
            Color pixel = tex.GetPixel(px, py);
            return pixel.a > alphaThreshold;
        }

        return false;
    }

    private GameObject GetPooledObject()
    {
        if (poolIndex >= objectPool.Count)
        {
            GameObject newObj = Instantiate(prefabToSpawn);
            newObj.SetActive(false);
            objectPool.Add(newObj);
        }

        GameObject pooledObj = objectPool[poolIndex];
        pooledObj.SetActive(true);
        poolIndex++;
        return pooledObj;
    }
}
