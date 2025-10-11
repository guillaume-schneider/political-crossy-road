using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public int x;
    public int y;
    public string biome;
    public bool isWalkable;
}


public class WorldGrid : MonoBehaviour
{
    [Header("Map Parameters")]
    public int width;
    public int height;
    public float tileSize;
    public Vector3 gridOffset = Vector3.zero;

    [Header("Affichage Gizmos")]
    public Color walkableColor = Color.green;
    public Color blockedColor = Color.red;
    public bool showLabels = false;

    private List<Tile> m_worldGrid;

    private float OffsetX => (width  - 1) * 0.5f;
    private float OffsetY => (height - 1) * 0.5f;

    public void OnValidate() => GenerateMap();

    public static WorldGrid Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void GenerateMap()
    {
        m_worldGrid = new List<Tile>();

        int halfWidth = width / 2;
        int halfHeight = height / 2;

        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            for (int y = -halfHeight; y <= halfHeight; y++)
            {
                Tile t = new Tile();
                t.x = x;
                t.y = y;
                t.biome = "test";
                t.isWalkable = true;

                m_worldGrid.Add(t);
            }
        }
    }

    public Vector3 NormalizeCoords(float gx, float gy)
    {
        return new Vector3(gx * tileSize, 0f, gy * tileSize);
    }

    public Vector3 GridToWorld(int ix, int iy)
    {
        // Position locale (pivot centré)
        var local = NormalizeCoords(ix, iy);

        // Applique offset + transform
        return transform.TransformPoint(gridOffset + local);
    }

    public bool WorldToGrid(Vector3 world, out int ix, out int iy)
    {
        // Revenir en local (annule pos/rot/scale)
        Vector3 local = transform.InverseTransformPoint(world) - gridOffset;

        float gx = local.x / tileSize + OffsetX; // coord. "grille" continue
        float gy = local.z / tileSize + OffsetY;

        // "Tuile qui contient le point" => floor
        ix = Mathf.FloorToInt(gx + 1e-6f);
        iy = Mathf.FloorToInt(gy + 1e-6f);

        // Bornes
        return ix >= 0 && ix < width && iy >= 0 && iy < height;
    }

    public bool WorldToGridSnap(Vector3 world, out int ix, out int iy)
    {
        Vector3 local = transform.InverseTransformPoint(world) - gridOffset;

        float gx = local.x / tileSize + OffsetX;
        float gy = local.z / tileSize + OffsetY;

        // "Tuile la plus proche" => round
        ix = Mathf.RoundToInt(gx);
        iy = Mathf.RoundToInt(gy);

        return ix >= 0 && ix < width && iy >= 0 && iy < height;
    }

    void OnDrawGizmos()
    {
        if (m_worldGrid == null || m_worldGrid.Count == 0)
            GenerateMap();

        foreach (Tile t in m_worldGrid)
        {
            Vector3 worldPos = GridToWorld(t.x, t.y);
            Vector3 pos = new Vector3(worldPos.x * tileSize, 0, worldPos.z * tileSize);

            Gizmos.color = t.isWalkable ? walkableColor : blockedColor;
            Gizmos.DrawWireCube(pos, Vector3.one * tileSize);

            if (showLabels)
            {
                var normalizedPos = NormalizeCoords(t.x, t.y);
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(new Vector3(pos.x - 0.25f, pos.y, pos.z), $"{normalizedPos.x},{normalizedPos.z}");
                #endif
            }
        }
    }
}
