using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;

    public bool updateMapContinuously = false;

    [Header("Map Seed")]
    public int seed;

    [Header("Water Height")]
    public float waterHeight = 0.7f;

    [Header("Noise Values")]
    public float heightScale = 3f;
    public float frequency;
    public float amplitude;
    public float lacunarity;
    public float persistance;
    public int octaves;
    public bool useFalloff;
    public float fallOffValueA = 3;
    public float fallOffValueB = 2.2f;
    public AnimationCurve heightCurve;

    [Header("GameObjects")]
    public GameObject scene;
    public GameObject terrain;

    // Mesh generation
    public int mapWidth = 100;
    public int mapHeight = 100;
    private MeshCollider meshCollider;
    private PerlinNoise noise;
    public float[,] noiseValues;
    private float[,] falloffMap;
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uv;

    private bool updateQueued = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        meshCollider = terrain.GetComponent<MeshCollider>();

        mesh = new Mesh();
        terrain.GetComponent<MeshFilter>().mesh = mesh;
        uv = new Vector2[(mapWidth + 1) * (mapHeight + 1)];

        falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth + 1, mapHeight + 1, fallOffValueA, fallOffValueB);
        noise = new PerlinNoise(seed.GetHashCode(), frequency, amplitude, lacunarity, persistance, octaves);

        CreateTerrainMeshData();

        ApplyMesh();
    }

    
    void Update()
    {
        if (updateQueued)
        {
            UpdateTerrainMeshData();
            ApplyMesh();
        }

        if (updateMapContinuously)
        {
            noise = new PerlinNoise(seed.GetHashCode(), frequency, amplitude, lacunarity, persistance, octaves);
            CreateTerrainMeshData();
            ApplyMesh();
        }
    }

    public void UpdateMap()
    {
        updateQueued = true;
    }

    private void UpdateTerrainMeshData()
    {


        for (int i = 0, z = 0; z <= mapHeight; z++)
        {
            for (int x = 0; x <= mapWidth; x++)
            {
                float y = noiseValues[x, z] * heightScale;
                vertices[i] = new Vector3(x, y, z);
                uv[i] = new Vector2(x / (float)mapWidth, z / (float)mapHeight);
                i++;
            }
        }

        triangles = new int[mapWidth * mapHeight * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + mapWidth + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + mapWidth + 1;
                triangles[tris + 5] = vert + mapWidth + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        updateQueued = false;
    }

    private void CreateTerrainMeshData()
    {
        vertices = new Vector3[(mapWidth + 1) * (mapHeight + 1)];
        noiseValues = noise.GetNoiseValues(mapWidth + 1, mapHeight + 1);

        for (int x = 0; x <= mapWidth; x++)
        {
            for (int y = 0; y <= mapHeight; y++)
            {
                if (useFalloff)
                {
                    // SUBSTRACT FALLOFF MAP VALUES
                    noiseValues[x, y] = Mathf.Clamp01(noiseValues[x, y] - falloffMap[x, y]);
                }
            }
        }

        for (int i = 0, z = 0; z <= mapHeight; z++)
        {
            for (int x = 0; x <= mapWidth; x++)
            {
                float y = heightCurve.Evaluate(noiseValues[x, z]) * heightScale;
                vertices[i] = new Vector3(x, y, z);
                uv[i] = new Vector2(x / (float)mapWidth, z / (float)mapHeight);
                i++;
            }
        }

        triangles = new int[mapWidth * mapHeight * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + mapWidth + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + mapWidth + 1;
                triangles[tris + 5] = vert + mapWidth + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    private void ApplyMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        meshCollider.sharedMesh = mesh;

        mesh.RecalculateNormals();
    }
}
