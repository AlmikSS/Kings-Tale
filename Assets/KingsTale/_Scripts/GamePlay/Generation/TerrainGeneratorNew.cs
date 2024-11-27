using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class TerrainGeneratorNew : MonoBehaviour
{
    [Header("��������� ��������")]
    public GameObject goldPrefab; // ������ ������
    public GameObject[] treePrefabs; // ������� ��������
    public GameObject centralGoldPrefab; // ������ ������������ ������
    public GameObject playerPrefab; // ������ ������
    public GameObject[] stonePrefabs; // ������ �������� ������

    [Header("��������� �����������")]
    public GameObject riverColliderPrefab; // ������ ��� ���-���������� ����
    public GameObject pathColliderPrefab; // ������ ��� ���-���������� ��������
    public GameObject sandClearingColliderPrefab; // ������ ��� ���-���������� �������� �������

    [Header("��������� �������")]
    public TerrainLayer[] groundTerrainLayers; // ������ ������� �������
    public TerrainLayer riverTerrainLayer; // �������� ����
    public TerrainLayer sandClearingLayer; // �������� �������� �������
    public TerrainLayer pathTerrainLayer; // �������� ��������
    public TerrainLayer[] stoneTerrainLayers;    // ������ ������� ������ � ���������� �����
    public TerrainLayer[] leafTerrainLayers;     // ������ ������� ������� � ���������� �����

    [Header("��������� �������")]
    public bool enableBaseTexture = true; // �������� �������� ��������
    public bool enableRiverTexture = true; // �������� �������� ����
    public bool enableSandClearingTexture = true; // �������� �������� �������� �������
    public bool enablePathTexture = true; // �������� �������� ��������

    [Header("��������� ��������")]
    public int terrainWidth = 256; // ������ ��������
    public int terrainLength = 256; // ����� ��������
    public int terrainHeight = 100; // ������ ��������
    public int alphamapResolution = 512; // ���������� ����� ����� (������ �� ������������������ ��������� � ��������)

    [Header("��������� ������������ ��������")]
    public float resourceBalanceThresholdPercent = 1f; // ����� ������������ �������� � ���������
    public float treeValue = 1f; // �������� ������ ������
    public float goldValue = 5f; // �������� ������ ������

    [Header("��������� ���������� ����")]
    public float edgeNoiseScale = 0.01f; // ������� ���� ��� ���������� ����
    public float edgeNoiseIntensity = 10f; // ������������� ����������� ����
    public float edgeMinOffset = 10f; // ����������� ������ �� ���� �����

    [Header("��������� �������� �������")]
    public float riverTextureSize = 2f;         // ������ �������� ����
    public float sandClearingTextureSize = 2f;  // ������ �������� �������� �������
    public float pathTextureSize = 1f;          // ������ �������� ��������
    private float stoneTextureSize = 0.5f;          // ������ ������� ������
    public float leafTextureSize = 0.5f;           // ������ ������� �������

    [Header("��������� �������")]
    public float groundTextureSize = 60f; // ������ ������� �������
    public float groundNoiseScale = 8f; // ������� ���� ������� ��� ������� �����
    [Range(0f, 1f)]
    public float groundTextureSmoothing = 0.936f;

    [Header("��������� ������ � �������")]
    private bool enableRandomStones = false;       // �������� �����
    public bool enableRandomLeaves = true;       // �������� ������
    [Range(0f, 1f)]
    private float stoneDensity = 0.46f;            // ��������� ������
    [Range(0f, 1f)]
    public float leafDensity = 0.5f;             // ��������� �������
    private float minStonePatchSize = 2f; // ����������� ������ ������� ������
    private float maxStonePatchSize = 4f; // ������������ ������ ������� ������
    private float minStonePatchArea = 20f; // ����������� ������� ����� ������
    public float minLeafPatchSize = 5f; // ����������� ������ ������� �������
    public float maxLeafPatchSize = 7f; // ������������ ������ ������� �������
    public float minLeafPatchArea = 20f; // ����������� ������� ����� �������

    [Header("����������� �������")] // 0 - ��� �����������, ��� ������ ��������, ��� ������ �����������
    [Range(0, 1)]
    public float riverTextureSmoothing = 0.1f; // ����
    [Range(0, 1)]
    public float pathTextureSmoothing = 0f; // ��������
    [Range(0, 5)]
    public float sandClearingTextureSmoothing = 3.08f; // �������
    [Range(0, 5)]
    public float leafTextureSmoothing = 1f; // ������

    [Header("��������� ������")]
    public bool generateGold = true; // ������������ ������
    public int goldClustersPerSector = 4; // ���������� ��������� ������ �� ������
    public int goldPerCluster = 2; // ���������� ������ � ��������
    public float goldSpacing = 8f; // ���������� ����� ������� � ��������
    public float treeExclusionRadiusAroundGold = 7f; // ������ ���������� �������� ������ ������
    public float goldMinRotationY = 0f; // ����������� ���� �������� ������ �� ��� Y
    public float goldMaxRotationY = 360f; // ������������ ���� �������� ������ �� ��� Y

    [Header("��������� ������������ ������")]
    public bool generateCentralGold = true; // ������������ ����������� ������
    public int centralGoldClusters = 3; // ���������� ��������� ������������ ������
    public int centralGoldPerCluster = 1; // ���������� ������ � ����������� ��������
    public float centralGoldSpacing = 10f; // ��������� ���������� ����� ������������ �������� ���������
    public float centralGoldRadius = 30f; // ������ ��������� ������������ ������
    public float centralGoldExclusionRadius = 10f; // ������ ������ ������������ ������ ��� �������� � ���
    public int centralGoldSpawnAttempts = 15; // ���������� ������� ������ ������������ ������

    [Header("��������� ��������")]
    public bool generateTrees = true; // ������������ �������
    public int treeClustersPerSector = 20; // ���������� ��������� �������� �� ������
    public int treesPerCluster = 10; // ���������� �������� � ��������
    public float treeSpacing = 1.5f; // ���������� ����� ��������� � ��������
    public float treeMinRotationY = 0f; // ����������� ���� �������� �������� �� ��� Y
    public float treeMaxRotationY = 360f; // ������������ ���� �������� �������� �� ��� Y
    public float forestBorderRandomness = 0.5f; // ������������ ������ ����
    public float forestNoiseScale = 0.1f; // ������� ���� ��� ������ ����
    public float treeMinSize = 0.9f; // ����������� ������ ������
    public float treeMaxSize = 1.1f; // ������������ ������ ������

    [Header("��������� �������� �������")]
    public bool generateSandClearings = true;
    public int minSandClearingCount = 2; // ����������� ���������� �������� �������
    public int maxSandClearingCount = 4; // ������������ ���������� �������� �������
    public float sandClearingRandomness = 1f; // ������������ ����� �������� �������
    public float sandClearingNoiseScale = 1f; // ������� ���� ������� ��� �������� �������
    public float minSandClearingSize = 15f; // ����������� ������ �������� �������
    public float maxSandClearingSize = 18f; // ������������ ������ �������� �������
    public float treeExclusionRadiusAroundSandClearing = 16f; // ������ �������� �� �������� �������
    public float sandClearingOffsetFromPaths = 10f;  // ������ �������� ������� �� ��������
    public float sandClearingOffsetFromRivers = 10f; // ������ �������� ������� �� ���
    public float sandClearingMinDistanceBetween = 20f; // ����������� ���������� ����� ��������� ���������

    [Header("��������� ����")]
    public bool generateRivers = true; // ������������ ����
    public bool allowCornerRivers = true; // ��������� ������� ����
    public int minRiverCount = 1; // ����������� ���������� ���
    public int maxRiverCount = 3; // ������������ ���������� ���
    public float riverStepSize = 0.3f; // ��� ����
    public float riverMaxAngleChange = 90f; // ������������ ��������� ���� ����
    public float riverWidth = 8f; // ������ ����
    public float riverColliderWidth = 3f; // ��������� ������ ����������� ����
    public float riverCurvature = 1f; // ������������� �������� ����
    public float riverNoiseScale = 0.1f; // ������� ���� ������� ��� �������� ����
    public float riverResourceOffset = 7f; // ������ �������� �� ����
    public int maxRiverAttempts = 30; // ������������ ���������� ������� ��������� ����
    public float riverColliderAccuracy = 10f; // �������� ����������� ���� (��� ������, ��� ������� ����������)

    [Header("��������� ��������")]
    public bool generatePaths = true; // ������������ ��������
    public bool generatePathsBetweenPlayers = false; // ������������ �������� ����� ��������
    public float pathWidth = 2.5f; // ������ ��������
    public float pathMaxAngleChange = 90f; // ������������ ��������� ���� ��������
    public float pathStepSize = 0.5f; // ��� ��������
    public float pathCurvature = 0.5f; // ����� ��������
    public float treeExclusionRadiusAroundPaths = 6f; // ������ �������� �� ��������
    public float pathColliderAccuracy = 10f; // �������� ����������� (��� ������, ��� ������� ����������)

    [Header("��������� ������")]
    public bool generateStones = true; // �������� ��������� ������
    public int minStoneLineCount = 2; // ����������� ���������� ����� ������
    public int maxStoneLineCount = 3; // ������������ ���������� ����� ������
    public float stoneSpacing = 0.01f; // ���������� ����� ������� � �����
    public int minStoneLineLength = 8; // ����������� ����� ����� ������
    public int maxStoneLineLength = 12; // ������������ ����� ����� ������
    public float maxStoneAngleChange = 45f; // ������������ ��������� ���� ����� �������
    public float minDistanceBetweenStoneLines = 30f; // ����������� ���������� ����� ������� ������
    public float stoneMinSize = 3f; // ����������� ������ ������
    public float stoneMaxSize = 5f; // ������������ ������ ������
    public float stoneMinHeight = 7f; // ����������� ������ ������
    public float stoneMaxHeight = 9f; // ������������ ������ ������
    public float stoneMinRotationY = 0f; // ����������� ���� �������� ������ �� ��� Y
    public float stoneMaxRotationY = 360f; // ������������ ���� �������� ������ �� ��� Y
    public float stoneExclusionRadiusAroundStones = 5f; // ������ ���������� �������� ������ ������
    public float stoneExclusionRadiusAroundGold = 5f; // ������ ���������� ������ ������ ������
    public float stoneResourceOffset = 10f; // ������ ������ �� ��� � ������ ��������
    public float stoneExclusionRadiusAroundPaths = 25f; // ������ ���������� ������ ������ ��������


    [Header("��������� ������ �������")]
    public bool generatePlayers = true; // ������������ �������
    public float playerSafeRadius = 25f; // ������ ������������ ������ ������
    public int playerCount = 4; // ���������� �������
    public List<Vector3> spawnPoints = new List<Vector3>(); // ����� ������ �������

    [Header("��������� �������")]
    private GameObject debugSectorPrefab; // ������ ��� ������� ��������
    private bool showDebugSectors = false; // �������� ���������� �������

    private Terrain terrain; // ������ �� �������
    private List<Vector3> goldClusterCenters = new List<Vector3>(); // ������ ��������� ������
    private List<Vector3> goldPositions = new List<Vector3>(); // ������� ������
    private List<Vector3> treePositions = new List<Vector3>(); // ������� ��������
    private List<Vector3> stonePositions = new List<Vector3>(); // ������� ������
    private List<Vector3> stoneLinePositions = new List<Vector3>();  // ������� ������ (�����)
    private List<Vector3> treeClusterCenters = new List<Vector3>(); // ������ ��������� ��������
    private List<Vector3> riverPoints = new List<Vector3>(); // ����� ������� ����
    private List<Vector3> centralGoldPositions = new List<Vector3>(); // ������� ������������ ������
    private List<Vector3> sandClearingCenters = new List<Vector3>(); // ������ �������� �������
    private List<List<Vector3>> allPaths = new List<List<Vector3>>(); // ��� ��������
    private List<List<Vector3>> allRivers = new List<List<Vector3>>(); // ��� ����
    private List<SandClearing> sandClearings = new List<SandClearing>(); //�������� �������
    private GameObject generatedObjectsParent; // ������������ ������ ��� ��������������� ��������
    private List<Vector3> allRiverPoints = new List<Vector3>(); // ��� ����� ���� ���
    private List<StonePatch> stonePatches = new List<StonePatch>(); // ������� ������ (����������)
    private List<LeafPatch> leafPatches = new List<LeafPatch>(); // ������� ������� (������ �������)
    private float[,] treeDensityMap; // ����� ��������� ��������
    private float[,] pathWeightMap; // ����� ����� ��������
    private float[,] riverWeightMap; // ����� ����� ���
    private List<List<Vector2>> riverPolygons = new List<List<Vector2>>();
    private List<List<Vector2>> pathPolygons = new List<List<Vector2>>();
    private int alphaMapWidth;
    private int alphaMapHeight;
    private GameObject stonesParentObject;
    private GameObject centralGoldParentObject;
    private GameObject GoldParentObject;
    private GameObject treesParentObject;
    private Vector2 groundPerlinOffset;


    private class SectorInfo
    {
        public int goldCount = 0; // ���������� ������ � �������
        public int treeCount = 0; // ���������� �������� � �������
    }

    private SectorInfo[] sectors = new SectorInfo[4];

    [ContextMenu("������������� �����")]
    public void GenerateMap()
    {
        Debug.Log("������ ��������� �����...");

        // ������� ����� ��������������� ��������
        if (generatedObjectsParent != null)
        {
            DestroyImmediate(generatedObjectsParent);
        }
        generatedObjectsParent = new GameObject("GeneratedObjects");

        groundPerlinOffset = new Vector2(Random.Range(0f, 10000f), Random.Range(0f, 10000f));

        // ������� ������� ����� ������ � �������
        stonePatches.Clear();
        leafPatches.Clear();

        // ������������� ������������ ��������
        stonesParentObject = new GameObject("Stones");
        stonesParentObject.transform.parent = generatedObjectsParent.transform;

        centralGoldParentObject = new GameObject("CentralGold");
        centralGoldParentObject.transform.parent = generatedObjectsParent.transform;

        GoldParentObject = new GameObject("Gold");
        GoldParentObject.transform.parent = generatedObjectsParent.transform;

        treesParentObject = new GameObject("Trees");
        treesParentObject.transform.parent = generatedObjectsParent.transform;

        // ������� ��������
        if (terrain != null)
        {
            DestroyImmediate(terrain.gameObject);
            terrain = null;
        }

        InitializeSectors();
        GenerateTerrain();

        alphaMapWidth = terrain.terrainData.alphamapWidth;
        alphaMapHeight = terrain.terrainData.alphamapHeight;
        pathWeightMap = new float[alphaMapHeight, alphaMapWidth];
        riverWeightMap = new float[alphaMapHeight, alphaMapWidth];

        // ������� �������
        goldClusterCenters.Clear();
        goldPositions.Clear();

        sandClearings.Clear();
        sandClearingCenters.Clear();

        treePositions.Clear();
        treeClusterCenters.Clear();

        riverPoints.Clear();
        allRiverPoints.Clear();
        allRivers.Clear();

        allPaths.Clear();

        spawnPoints.Clear();

        stonePositions.Clear();
        stoneLinePositions.Clear();

        riverPolygons.Clear();
        pathPolygons.Clear();

        if (generatePlayers)
        {
            PlacePlayers();
        }

        if (generateCentralGold)
        {
            GenerateCentralGold();
        }

        if (generateRivers)
        {
            GenerateRivers();
        }

        if (generatePaths)
        {
            GeneratePaths();
            GeneratePathWeightMap();
        }

        if (generateStones)
        {
            GenerateStones();
        }

        if (generateSandClearings)
        {
            GenerateSandClearings();
        }

        if (generateGold)
        {
            GenerateNormalGold();
        }

        if (generateTrees)
        {
            GenerateTrees();
        }

        ApplyTerrainTextures();

        BalanceResources();

        if (showDebugSectors)
        {
            ShowDebugSectors();
        }

        Debug.Log("��������� ����� ���������.");
    }


    private void InitializeSectors()
    {
        for (int i = 0; i < sectors.Length; i++)
        {
            sectors[i] = new SectorInfo();
        }
    }

    private void GenerateTerrain()
    {
        Debug.Log("��������� ��������...");
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = terrainWidth + 1;
        terrainData.size = new Vector3(terrainWidth, terrainHeight, terrainLength);
        terrainData.alphamapResolution = alphamapResolution;

        List<TerrainLayer> terrainLayersList = new List<TerrainLayer>();
        terrainLayersList.AddRange(groundTerrainLayers);
        terrainLayersList.Add(riverTerrainLayer);
        terrainLayersList.Add(sandClearingLayer);
        terrainLayersList.Add(pathTerrainLayer);
        terrainData.terrainLayers = terrainLayersList.ToArray();


        terrain = Terrain.CreateTerrainGameObject(terrainData).GetComponent<Terrain>();
        terrain.transform.position = Vector3.zero;
    }

    private void ShowDebugSectors()
    {
        Debug.Log("����������� ��������...");
        float sectorWidth = terrainWidth / 2f;
        float sectorLength = terrainLength / 2f;

        for (int i = 0; i < 4; i++)
        {
            Vector3 position = Vector3.zero;
            switch (i)
            {
                case 0:
                    position = new Vector3(sectorWidth / 2, 0, sectorLength + sectorLength / 2);
                    break;
                case 1:
                    position = new Vector3(sectorWidth + sectorWidth / 2, 0, sectorLength + sectorLength / 2);
                    break;
                case 2:
                    position = new Vector3(sectorWidth / 2, 0, sectorLength / 2);
                    break;
                case 3:
                    position = new Vector3(sectorWidth + sectorWidth / 2, 0, sectorLength / 2);
                    break;
            }

            position.y = terrain.SampleHeight(position);

            GameObject sectorDebug = Instantiate(debugSectorPrefab, position, Quaternion.identity, generatedObjectsParent.transform);
            sectorDebug.transform.localScale = new Vector3(sectorWidth, 1, sectorLength);
            sectorDebug.name = $"Sector_{i + 1}";
        }
    }

    private void GenerateCentralGold()
    {
        Debug.Log("��������� ������������ ������...");

        if (!generateCentralGold)
            return;

        Vector3 centralPosition = new Vector3(terrainWidth / 2, 0, terrainLength / 2);
        centralPosition.y = terrain.SampleHeight(centralPosition);

        for (int clusterIndex = 0; clusterIndex < centralGoldClusters; clusterIndex++)
        {
            Vector3 clusterCenter = GetRandomPositionNear(centralPosition, centralGoldRadius);

            int goldSpawned = 0;
            int attempts = 0;
            int maxAttempts = centralGoldSpawnAttempts;

            while (goldSpawned < centralGoldPerCluster && attempts < maxAttempts)
            {
                attempts++;
                Vector3 position = GetRandomPositionNear(clusterCenter, centralGoldSpacing);
                if (!IsPositionAcceptableForCentralGold(position))
                {
                    Debug.Log($"������� {attempts}: ������� {position} �� �������� ��� ������������ ������.");
                    continue;
                }

                Quaternion rotation = Quaternion.Euler(0, Random.Range(goldMinRotationY, goldMaxRotationY), 0);
                GameObject gold = Instantiate(centralGoldPrefab, position, rotation, centralGoldParentObject.transform);
                goldPositions.Add(position);
                centralGoldPositions.Add(position);
                sectors[GetSectorIndex(position)].goldCount++;
                goldSpawned++;
                Debug.Log($"������ {goldSpawned} � �������� {clusterIndex + 1} ������� ���������� � ������� {position}.");

                // ��������� ��������� ResourceIdentifier
                ResourceIdentifier identifier = gold.AddComponent<ResourceIdentifier>();
                if (identifier != null)
                {
                    identifier.resourceType = ResourceType.CentralGold;
                }
            }

            if (goldSpawned < centralGoldPerCluster)
            {
                Debug.LogWarning($"�� ������� ������������� ��� ����������� ������ � �������� {clusterIndex + 1}. �������� {goldSpawned} �� {centralGoldPerCluster}.");
            }
        }
    }


    private bool IsPositionAcceptableForCentralGold(Vector3 position)
    {
        // ���������, �� ������� �� ������ � ������� ������
        foreach (var goldPos in goldPositions)
        {
            if (Vector3.Distance(position, goldPos) < centralGoldSpacing)
            {
                return false;
            }
        }
        // ���������, �� � ����
        if (IsPositionInRiver(position, riverResourceOffset))
        {
            return false;
        }
        // ���������, �� � �������� �������
        if (IsPositionInSandClearing(position))
        {
            return false;
        }
        // ���������, �� ������ ������
        foreach (var treePos in treePositions)
        {
            if (Vector3.Distance(position, treePos) < centralGoldSpacing)
            {
                return false;
            }
        }
        // ���������, �� ������� �� ������ � ���������
        if (IsPositionNearPath(position, centralGoldExclusionRadius))
        {
            return false;
        }
        // ���������, �� ������� �� ������ � ������
        if (IsPositionNearStone(position, stoneExclusionRadiusAroundGold))
        {
            return false;
        }
        return true;
    }


    private void GenerateNormalGold()
    {
        Debug.Log("��������� �������� ������...");

        for (int sectorIndex = 0; sectorIndex < 4; sectorIndex++)
        {
            for (int i = 0; i < goldClustersPerSector; i++)
            {
                Vector3 clusterCenter = GetRandomPositionInSector(sectorIndex);
                if (IsPositionNearPlayer(clusterCenter) || IsPositionInRiver(clusterCenter, riverResourceOffset) || !IsPositionWithinWavyEdge(clusterCenter))
                    continue;

                goldClusterCenters.Add(clusterCenter);

                for (int j = 0; j < goldPerCluster; j++)
                {
                    Vector3 position = GetRandomPositionNear(clusterCenter, goldSpacing * goldPerCluster);
                    if (IsPositionNearPlayer(position) || IsPositionInRiver(position, riverResourceOffset) || !IsPositionWithinWavyEdge(position))
                        continue;

                    if (!IsPositionAcceptableForGold(position))
                        continue;

                    Quaternion rotation = Quaternion.Euler(0, Random.Range(goldMinRotationY, goldMaxRotationY), 0);
                    GameObject gold = Instantiate(goldPrefab, position, rotation, GoldParentObject.transform);
                    goldPositions.Add(position);
                    sectors[sectorIndex].goldCount++;
                }
            }
        }
    }


    private bool IsPositionInRiver(Vector3 position, float distance)
    {
        Vector2 posXZ = new Vector2(position.x, position.z);
        foreach (var river in allRivers)
        {
            foreach (var riverPoint in river)
            {
                Vector2 riverPointXZ = new Vector2(riverPoint.x, riverPoint.z);
                if (Vector2.Distance(posXZ, riverPointXZ) < distance + riverWidth / 2f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void GenerateTrees()
    {
        if (!generateTrees)
            return;

        Debug.Log("��������� ��������...");

        for (int sectorIndex = 0; sectorIndex < 4; sectorIndex++)
        {
            int clustersInSector = treeClustersPerSector;

            int clustersSpawned = 0;
            int maxAttempts = clustersInSector * 10;
            int attempts = 0;

            while (clustersSpawned < clustersInSector && attempts < maxAttempts)
            {
                Vector3 clusterCenter = GetRandomPositionInSector(sectorIndex);
                clusterCenter.y = terrain.SampleHeight(clusterCenter);

                if (IsPositionNearPlayer(clusterCenter) ||
                    IsPositionInRiver(clusterCenter, riverResourceOffset) ||
                    !IsPositionWithinWavyEdge(clusterCenter) ||
                    IsPositionInSandClearing(clusterCenter, treeExclusionRadiusAroundSandClearing) ||
                    IsPositionNearPath(clusterCenter, treeExclusionRadiusAroundPaths) ||
                    IsPositionNearCentralGold(clusterCenter, centralGoldExclusionRadius))
                {
                    attempts++;
                    continue;
                }

                bool tooCloseToGold = false;
                foreach (Vector3 goldPosition in goldPositions)
                {
                    if (Vector3.Distance(clusterCenter, goldPosition) < treeExclusionRadiusAroundGold)
                    {
                        tooCloseToGold = true;
                        break;
                    }
                }

                if (!tooCloseToGold)
                {
                    treeClusterCenters.Add(clusterCenter); // ��������� ����� �������� ��������

                    HashSet<Vector2> occupiedPositions = new HashSet<Vector2>(); // ��� �������������� ��������� ��������

                    for (int j = 0; j < treesPerCluster; j++)
                    {
                        Vector3 position = GetRandomPositionNear(clusterCenter, treeSpacing * treesPerCluster);
                        if (IsPositionNearPlayer(position) ||
                            IsPositionInRiver(position, riverResourceOffset) ||
                            !IsPositionWithinWavyEdge(position) ||
                            IsPositionInSandClearing(position, treeExclusionRadiusAroundSandClearing) ||
                            IsPositionNearPath(position, treeExclusionRadiusAroundPaths) ||
                            IsPositionNearCentralGold(position, centralGoldExclusionRadius))
                            continue;

                        if (!IsPositionAcceptableForTree(position))
                            continue;

                        Vector2 gridPosition = new Vector2(Mathf.Round(position.x * 10f), Mathf.Round(position.z * 10f));
                        if (occupiedPositions.Contains(gridPosition))
                        {
                            continue; // ���������� ���� ������� ��� ������
                        }
                        occupiedPositions.Add(gridPosition);

                        GameObject selectedTreePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];

                        float randomSize = Random.Range(treeMinSize, treeMaxSize);
                        Vector3 randomScale = new Vector3(randomSize, randomSize, randomSize);

                        Quaternion rotation = Quaternion.Euler(0, Random.Range(treeMinRotationY, treeMaxRotationY), 0);
                        GameObject tree = Instantiate(selectedTreePrefab, position, rotation, treesParentObject.transform);
                        tree.transform.localScale = randomScale; // ��������� ��������� ������
                        treePositions.Add(position);
                        sectors[sectorIndex].treeCount++;
                    }
                    clustersSpawned++;
                }
                attempts++;
            }
        }
    }


    private bool IsPositionNearCentralGold(Vector3 position, float radius)
    {
        // �������� �� ���������� �� ���� ����������� ������� ��������
        foreach (var centralGoldPos in centralGoldPositions) // ��������������, ��� ���� ������ ����������� ������� �������
        {
            if (Vector3.Distance(position, centralGoldPos) < centralGoldSpacing)
            {
                return true;
            }
        }
        return false;
    }


    private Vector3 GetRandomPositionInIrregularShape(Vector3 center, float radius, float noiseScale, float randomness)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Abs(Mathf.PerlinNoise(center.x * noiseScale + Mathf.Cos(angle), center.z * noiseScale + Mathf.Sin(angle)) - 0.5f) * 2f * radius * randomness;
        distance += Random.Range(0f, radius * (1f - randomness));
        float x = center.x + Mathf.Cos(angle) * distance;
        float z = center.z + Mathf.Sin(angle) * distance;

        int attempts = 0;
        int maxAttempts = 10;
        Vector3 position = Vector3.zero;

        while (attempts < maxAttempts)
        {
            x = center.x + Mathf.Cos(angle) * distance;
            z = center.z + Mathf.Sin(angle) * distance;

            position = new Vector3(x, 0, z);
            if (IsPositionWithinWavyEdge(position))
            {
                break;
            }
            attempts++;
        }

        x = Mathf.Clamp(x, edgeMinOffset, terrainWidth - edgeMinOffset);
        z = Mathf.Clamp(z, edgeMinOffset, terrainLength - edgeMinOffset);

        float y = terrain.SampleHeight(new Vector3(x, 0, z));
        return new Vector3(x, y, z);
    }

    private Vector3 GetRandomPositionNear(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(0f, radius);
        float x = center.x + Mathf.Cos(angle) * distance;
        float z = center.z + Mathf.Sin(angle) * distance;

        int attempts = 0;
        int maxAttempts = 10;
        Vector3 position = Vector3.zero;

        while (attempts < maxAttempts)
        {
            position = new Vector3(x, 0, z);
            if (IsPositionWithinWavyEdge(position))
            {
                break;
            }
            attempts++;
            distance = Random.Range(0f, radius);
            x = center.x + Mathf.Cos(angle) * distance;
            z = center.z + Mathf.Sin(angle) * distance;
        }

        x = Mathf.Clamp(x, edgeMinOffset, terrainWidth - edgeMinOffset);
        z = Mathf.Clamp(z, edgeMinOffset, terrainLength - edgeMinOffset);

        float y = terrain.SampleHeight(new Vector3(x, 0, z));
        return new Vector3(x, y, z);
    }

    private bool IsPositionWithinWavyEdge(Vector3 position)
    {
        // ��� x ����������� (����� � ������ ����)
        float leftEdgeNoise = Mathf.PerlinNoise(position.z * edgeNoiseScale, 0f) * edgeNoiseIntensity;
        float leftEdgeBoundary = edgeMinOffset + leftEdgeNoise;
        if (position.x < leftEdgeBoundary)
            return false;

        float rightEdgeNoise = Mathf.PerlinNoise(position.z * edgeNoiseScale, 100f) * edgeNoiseIntensity;
        float rightEdgeBoundary = terrainWidth - (edgeMinOffset + rightEdgeNoise);
        if (position.x > rightEdgeBoundary)
            return false;

        // ��� z ����������� (������� � ������ ����)
        float bottomEdgeNoise = Mathf.PerlinNoise(position.x * edgeNoiseScale, 200f) * edgeNoiseIntensity;
        float bottomEdgeBoundary = edgeMinOffset + bottomEdgeNoise;
        if (position.z < bottomEdgeBoundary)
            return false;

        float topEdgeNoise = Mathf.PerlinNoise(position.x * edgeNoiseScale, 300f) * edgeNoiseIntensity;
        float topEdgeBoundary = terrainLength - (edgeMinOffset + topEdgeNoise);
        if (position.z > topEdgeBoundary)
            return false;

        return true;
    }

    private int GetSectorIndex(Vector3 position)
    {
        bool isLeft = position.x < terrainWidth / 2;
        bool isTop = position.z > terrainLength / 2;

        if (isLeft && isTop)
            return 0;
        else if (!isLeft && isTop)
            return 1;
        else if (isLeft && !isTop)
            return 2;
        else
            return 3;
    }

    private Vector3 GetRandomPositionInSector(int sector)
    {
        float minX = edgeMinOffset;
        float maxX = terrainWidth - edgeMinOffset;
        float minZ = edgeMinOffset;
        float maxZ = terrainLength - edgeMinOffset;

        switch (sector)
        {
            case 0:
                maxX = terrainWidth / 2 - edgeMinOffset;
                minZ = terrainLength / 2 + edgeMinOffset;
                break;
            case 1:
                minX = terrainWidth / 2 + edgeMinOffset;
                minZ = terrainLength / 2 + edgeMinOffset;
                break;
            case 2:
                maxX = terrainWidth / 2 - edgeMinOffset;
                maxZ = terrainLength / 2 - edgeMinOffset;
                break;
            case 3:
                minX = terrainWidth / 2 + edgeMinOffset;
                maxZ = terrainLength / 2 - edgeMinOffset;
                break;
        }

        int attempts = 0;
        int maxAttempts = 10;
        Vector3 position = Vector3.zero;

        while (attempts < maxAttempts)
        {
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);
            float y = terrain.SampleHeight(new Vector3(x, 0, z));
            position = new Vector3(x, y, z);

            if (IsPositionWithinWavyEdge(position))
            {
                return position;
            }
            attempts++;
        }
        return position;
    }

    private bool IsPositionAcceptableForGold(Vector3 position)
    {
        // ���������, �� ������� �� ������ � ������� ������
        foreach (var goldPos in goldPositions)
        {
            if (Vector3.Distance(position, goldPos) < goldSpacing)
            {
                return false;
            }
        }
        // ���������, �� � ����
        if (IsPositionInRiver(position, riverResourceOffset))
        {
            return false;
        }
        // ���������, �� � �������� �������
        if (IsPositionInSandClearing(position))
        {
            return false;
        }
        // ���������, �� ������ ������
        foreach (var treePos in treePositions)
        {
            if (Vector3.Distance(position, treePos) < treeExclusionRadiusAroundGold)
            {
                return false;
            }
        }
        // ���������, �� ������� �� ������ � ���������
        if (IsPositionNearPath(position, treeExclusionRadiusAroundPaths))
        {
            return false;
        }
        // ���������, �� ������� �� ������ � ������
        if (IsPositionNearStone(position, stoneExclusionRadiusAroundGold))
        {
            return false;
        }
        return true;
    }


    private bool IsPositionAcceptableForTree(Vector3 position)
    {
        // ���������, �� ������� �� ������ � �������� ��������
        foreach (var clearing in sandClearings)
        {
            float distance = DistanceToPolygonEdge(clearing.vertices, new Vector2(position.x, position.z));
            if (distance < treeExclusionRadiusAroundSandClearing)
            {
                return false;
            }
        }

        // ���������, �� ������� �� ������ � ������� ������
        foreach (var treePos in treePositions)
        {
            if (Vector3.Distance(position, treePos) < treeSpacing)
            {
                return false;
            }
        }

        // ���������, �� ������� �� ������ � ������
        foreach (var goldPos in goldPositions)
        {
            if (Vector3.Distance(position, goldPos) < treeExclusionRadiusAroundGold)
            {
                return false;
            }
        }

        // ���������, �� ������� �� ������ � ���������
        foreach (var path in allPaths)
        {
            foreach (var pathPoint in path)
            {
                if (Vector3.Distance(position, pathPoint) < treeExclusionRadiusAroundPaths)
                {
                    return false;
                }
            }
        }

        // ���������, �� � ����
        if (IsPositionInRiver(position, riverResourceOffset))
        {
            return false;
        }

        // ���������, �� ������� �� ������ � ������
        if (IsPositionNearStone(position, stoneExclusionRadiusAroundStones))
        {
            return false;
        }

        return true;
    }



    private void GenerateRivers()
    {
        Debug.Log("��������� ���...");

        GameObject riversParent = new GameObject("Rivers");
        riversParent.transform.parent = generatedObjectsParent.transform;

        int riverCount = Random.Range(minRiverCount, maxRiverCount + 1);
        for (int i = 0; i < riverCount; i++)
        {
            int attempts = 0;
            bool riverGenerated = false;
            while (!riverGenerated && attempts < maxRiverAttempts)
            {
                if (CreateRiver(riversParent))
                {
                    riverGenerated = true;
                    // ������� ������� ���� � ��������� ���
                    List<Vector2> riverPolygon = CreatePolygonFromLine(allRivers[allRivers.Count - 1], riverWidth);
                    riverPolygons.Add(riverPolygon);
                }
                attempts++;
            }

            if (!riverGenerated)
            {
                Debug.LogWarning($"�� ������� ������������� ���� {i + 1} ����� {maxRiverAttempts} �������.");
            }
        }

        // ����� ��������� ��� ���������� ����� ����� ����
        GenerateRiverWeightMap();
    }


    private bool CreateRiver(GameObject riversParent)
    {
        List<Vector3> localRiverPoints = new List<Vector3>();

        bool generateCornerRiver = allowCornerRivers && Random.value < 0.5f;

        Vector3 startPosition;
        Vector3 targetPosition;

        if (generateCornerRiver)
        {
            // ���������� ������� ����
            startPosition = new Vector3(Random.value < 0.5f ? 0 : terrainWidth, 0, Random.Range(0, terrainLength));
            targetPosition = new Vector3(Random.Range(0, terrainWidth), 0, Random.value < 0.5f ? 0 : terrainLength);
        }
        else
        {
            // ���������� ���� �� ������ ���� � �������
            if (Random.value < 0.5f)
            {
                startPosition = new Vector3(Random.Range(0, terrainWidth), 0, 0);
                targetPosition = new Vector3(Random.Range(0, terrainWidth), 0, terrainLength);
            }
            else
            {
                startPosition = new Vector3(0, 0, Random.Range(0, terrainLength));
                targetPosition = new Vector3(terrainWidth, 0, Random.Range(0, terrainLength));
            }
        }

        startPosition.y = terrain.SampleHeight(startPosition);
        targetPosition.y = terrain.SampleHeight(targetPosition);

        // ���������, �� ������� �� ������ � ������� ��� ������������ ������
        if (IsPositionNearPlayer(startPosition) ||
            IsPositionNearPlayer(targetPosition) ||
            IsPositionNearCentralGold(startPosition, centralGoldExclusionRadius) ||
            IsPositionNearCentralGold(targetPosition, centralGoldExclusionRadius))
        {
            return false; // ������� ������ � ������ ��� ������������ ������, ������� �����
        }

        Vector3 centralPosition = new Vector3(terrainWidth / 2, 0, terrainLength / 2);

        Vector3 currentPosition = startPosition;

        int maxSteps = 1000;
        int steps = 0;

        while (Vector3.Distance(currentPosition, targetPosition) > 5f && steps < maxSteps)
        {
            Vector3 direction = (targetPosition - currentPosition).normalized;

            float noiseValue = Mathf.PerlinNoise(currentPosition.x * riverNoiseScale, currentPosition.z * riverNoiseScale);
            float angleChange = (noiseValue - 0.5f) * 2f * riverMaxAngleChange * riverCurvature;
            direction = Quaternion.Euler(0, angleChange, 0) * direction;

            float stepSize = riverStepSize;

            Vector3 nextPosition = currentPosition + direction * stepSize;
            nextPosition.x = Mathf.Clamp(nextPosition.x, 0, terrainWidth);
            nextPosition.z = Mathf.Clamp(nextPosition.z, 0, terrainLength);
            nextPosition.y = terrain.SampleHeight(nextPosition);

            // ���������, �� ������� �� ������ � ������������ ������
            if (Vector3.Distance(nextPosition, centralPosition) < centralGoldExclusionRadius)
            {
                return false;
            }

            // ���������, �� ������� �� ������ � ������������ ������
            if (IsPositionNearCentralGold(nextPosition, centralGoldExclusionRadius))
            {
                return false;
            }

            // ���������, �� ������� �� ������ � �������
            if (IsPositionNearPlayer(currentPosition))
            {
                return false;
            }

            currentPosition = nextPosition;

            // �������������� ��������: ���� �� ������ ��������� ����� ����������� ����
            if (Vector3.Distance(currentPosition, centralPosition) < centralGoldExclusionRadius)
            {
                return false;
            }

            localRiverPoints.Add(currentPosition);

            steps++;
        }

        if (steps >= maxSteps)
        {
            return false;
        }

        // ���������, �� ������� �� ���� ������ � ������������
        if (IsRiverTooClose(localRiverPoints))
        {
            return false;
        }

        // ��������� ����� � ���������� ������
        riverPoints.AddRange(localRiverPoints);
        allRiverPoints.AddRange(localRiverPoints);
        allRivers.Add(localRiverPoints);

        // ������� ���������� ����� ����
        CreateRiverColliders(localRiverPoints, riversParent);

        return true;
    }

    private bool IsRiverTooClose(List<Vector3> newRiverPoints)
    {
        float minRiverDistance = riverWidth * 2f; // ����������� ���������� ����� ������

        foreach (var existingRiver in allRivers)
        {
            foreach (var existingPoint in existingRiver)
            {
                Vector2 existingPointXZ = new Vector2(existingPoint.x, existingPoint.z);
                foreach (var newPoint in newRiverPoints)
                {
                    Vector2 newPointXZ = new Vector2(newPoint.x, newPoint.z);
                    if (Vector2.Distance(existingPointXZ, newPointXZ) < minRiverDistance)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void CreateRiverColliders(List<Vector3> riverPoints, GameObject parent)
    {
        GameObject collidersParent = new GameObject("RiverColliders");
        collidersParent.transform.parent = parent.transform;

        List<GameObject> colliderSegments = new List<GameObject>();

        float colliderStepSize = riverStepSize * riverColliderAccuracy;

        for (int i = 0; i < riverPoints.Count - 1;)
        {
            Vector3 start = riverPoints[i];
            Vector3 end = start;
            float accumulatedLength = 0f;
            int segmentsMerged = 0;

            // ���������� ��������� ��������� ��� �������� ����� �������� ����������
            while (i + segmentsMerged < riverPoints.Count - 1 && accumulatedLength < colliderStepSize)
            {
                Vector3 nextPoint = riverPoints[i + segmentsMerged + 1];
                accumulatedLength += Vector3.Distance(end, nextPoint);
                end = nextPoint;
                segmentsMerged++;
            }

            Vector3 direction = (end - start).normalized;
            float segmentLength = Vector3.Distance(start, end);
            Vector3 midPoint = (start + end) / 2;

            // ����������: ���������� riverColliderWidth ������ riverWidth
            GameObject colliderSegment = Instantiate(riverColliderPrefab, midPoint, Quaternion.LookRotation(direction), collidersParent.transform);
            colliderSegment.transform.localScale = new Vector3(riverColliderWidth * 2f, colliderSegment.transform.localScale.y, segmentLength);

            colliderSegments.Add(colliderSegment);

            i += segmentsMerged;
        }

        // ���������� ���������� � ���� ���
        if (colliderSegments.Count > 0)
        {
            CombineRiverColliders(collidersParent, colliderSegments);
        }
        else
        {
            DestroyImmediate(collidersParent);
        }
    }

    private void CombineRiverColliders(GameObject parent, List<GameObject> colliderSegments)
    {
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        foreach (GameObject segment in colliderSegments)
        {
            MeshFilter meshFilter = segment.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                CombineInstance combineInstance = new CombineInstance();
                combineInstance.mesh = meshFilter.sharedMesh;
                combineInstance.transform = meshFilter.transform.localToWorldMatrix;
                combineInstances.Add(combineInstance);
            }
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        MeshFilter parentMeshFilter = parent.AddComponent<MeshFilter>();
        parentMeshFilter.mesh = combinedMesh;

        MeshCollider parentMeshCollider = parent.AddComponent<MeshCollider>();
        parentMeshCollider.sharedMesh = combinedMesh;

        // ������� �������� ��������
        foreach (GameObject segment in colliderSegments)
        {
            DestroyImmediate(segment);
        }
    }

    private void GenerateRiverWeightMap()
    {
        Debug.Log("��������� ����� ����� ���...");

        foreach (var river in allRivers)
        {
            for (int i = 0; i < river.Count - 1; i++)
            {
                Vector3 start = river[i];
                Vector3 end = river[i + 1];

                // ����������� ������� ���������� � ���������� �����-�����
                int startX = Mathf.RoundToInt((start.x / terrainWidth) * (alphaMapWidth - 1));
                int startZ = Mathf.RoundToInt((start.z / terrainLength) * (alphaMapHeight - 1));
                int endX = Mathf.RoundToInt((end.x / terrainWidth) * (alphaMapWidth - 1));
                int endZ = Mathf.RoundToInt((end.z / terrainLength) * (alphaMapHeight - 1));

                // ����������� ����� �� ����� �����
                RasterizeRiverSegment(startX, startZ, endX, endZ);
            }
        }
    }

    private void RasterizeRiverSegment(int startX, int startZ, int endX, int endZ)
    {
        int dx = Mathf.Abs(endX - startX);
        int dz = Mathf.Abs(endZ - startZ);
        int sx = startX < endX ? 1 : -1;
        int sz = startZ < endZ ? 1 : -1;
        int err = dx - dz;

        int x = startX;
        int z = startZ;

        while (true)
        {
            SetRiverWeightAt(x, z);

            if (x == endX && z == endZ)
                break;

            int e2 = 2 * err;
            if (e2 > -dz)
            {
                err -= dz;
                x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                z += sz;
            }
        }
    }

    private void SetRiverWeightAt(int x, int z)
    {
        float riverRadius = (riverWidth / 2f) + riverTextureSmoothing;
        int radius = Mathf.CeilToInt((riverRadius / terrainWidth) * alphaMapWidth);

        float innerRadius = (riverWidth / 2f) / terrainWidth * alphaMapWidth;

        for (int dzOffset = -radius; dzOffset <= radius; dzOffset++)
        {
            for (int dxOffset = -radius; dxOffset <= radius; dxOffset++)
            {
                int nx = x + dxOffset;
                int nz = z + dzOffset;

                if (nx >= 0 && nx < alphaMapWidth && nz >= 0 && nz < alphaMapHeight)
                {
                    float distance = Mathf.Sqrt(dxOffset * dxOffset + dzOffset * dzOffset);

                    float weight = 0f;

                    if (distance <= innerRadius)
                    {
                        weight = 1f;
                    }
                    else if (distance <= radius)
                    {
                        weight = 1f - ((distance - innerRadius) / (radius - innerRadius));
                    }
                    else
                    {
                        weight = 0f;
                    }

                    weight = Mathf.Clamp01(weight);

                    if (riverWeightMap[nz, nx] < weight)
                    {
                        riverWeightMap[nz, nx] = weight;
                    }
                }
            }
        }
    }


    private void GeneratePaths()
    {
        Debug.Log("��������� ��������...");

        GameObject pathsParent = new GameObject("Paths");
        pathsParent.transform.parent = generatedObjectsParent.transform;

        // ���������� ����
        foreach (Vector3 spawnPoint in spawnPoints)
        {
            List<Vector3> pathPoints = GeneratePathRiverStyle(spawnPoint, new Vector3(terrainWidth / 2, 0, terrainLength / 2));
            allPaths.Add(pathPoints);
            CreatePathColliders(pathPoints, pathsParent);

            // ������� ������� �������� � ��������� ���
            List<Vector2> pathPolygon = CreatePolygonFromLine(pathPoints, pathWidth);
            pathPolygons.Add(pathPolygon);
        }

        if (generatePathsBetweenPlayers)
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                for (int j = i + 1; j < spawnPoints.Count; j++)
                {
                    List<Vector3> pathPoints = GeneratePathRiverStyle(spawnPoints[i], spawnPoints[j]);
                    allPaths.Add(pathPoints);
                    CreatePathColliders(pathPoints, pathsParent);

                    // ������� ������� �������� � ��������� ���
                    List<Vector2> pathPolygon = CreatePolygonFromLine(pathPoints, pathWidth);
                    pathPolygons.Add(pathPolygon);
                }
            }
        }
    }


    private bool PolygonsIntersect(List<Vector2> polygonA, List<Vector2> polygonB)
    {
        for (int i = 0; i < polygonA.Count; i++)
        {
            Vector2 a1 = polygonA[i];
            Vector2 a2 = polygonA[(i + 1) % polygonA.Count];

            for (int j = 0; j < polygonB.Count; j++)
            {
                Vector2 b1 = polygonB[j];
                Vector2 b2 = polygonB[(j + 1) % polygonB.Count];

                if (LinesIntersect(a1, a2, b1, b2))
                {
                    return true;
                }
            }
        }

        // ����� ���������, ��������� �� ���� ������� ������ �������
        if (IsPointInPolygon(polygonA, polygonB[0]) || IsPointInPolygon(polygonB, polygonA[0]))
        {
            return true;
        }

        return false;
    }

    private bool LinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float denominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);
        if (denominator == 0)
            return false; // ����� �����������

        float ua = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denominator;
        float ub = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denominator;

        return ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1;
    }


    private void CreatePathColliders(List<Vector3> pathPoints, GameObject parent)
    {
        GameObject pathObject = new GameObject("PathCollider");
        pathObject.transform.parent = parent.transform;

        List<GameObject> colliderSegments = new List<GameObject>();

        float colliderStepSize = pathStepSize * pathColliderAccuracy; // ���������� ��� �����������

        for (int i = 0; i < pathPoints.Count - 1;)
        {
            Vector3 start = pathPoints[i];
            Vector3 end = start;
            float accumulatedLength = 0f;
            int segmentsMerged = 0;

            // ���������� ��������� ��������� ��� �������� ����� �������� ����������
            while (i + segmentsMerged < pathPoints.Count - 1 && accumulatedLength < colliderStepSize)
            {
                Vector3 nextPoint = pathPoints[i + segmentsMerged + 1];
                accumulatedLength += Vector3.Distance(end, nextPoint);
                end = nextPoint;
                segmentsMerged++;
            }

            if (IsSegmentInRiver(start, end))
            {
                i += segmentsMerged;
                continue;
            }

            Vector3 direction = (end - start).normalized;
            float segmentLength = Vector3.Distance(start, end);
            Vector3 midPoint = (start + end) / 2;

            GameObject colliderSegment = Instantiate(pathColliderPrefab, midPoint, Quaternion.LookRotation(direction), pathObject.transform);
            colliderSegment.transform.localScale = new Vector3(pathWidth, colliderSegment.transform.localScale.y, segmentLength);

            colliderSegments.Add(colliderSegment);

            i += segmentsMerged;
        }

        // ���������� �������� ����������� � ���� ���
        if (colliderSegments.Count > 0)
        {
            CombinePathColliders(pathObject, colliderSegments);
        }
        else
        {
            DestroyImmediate(pathObject);
        }
    }

    private void CombinePathColliders(GameObject parent, List<GameObject> colliderSegments)
    {
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        foreach (GameObject segment in colliderSegments)
        {
            MeshFilter meshFilter = segment.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                CombineInstance combineInstance = new CombineInstance();
                combineInstance.mesh = meshFilter.sharedMesh;
                combineInstance.transform = meshFilter.transform.localToWorldMatrix;
                combineInstances.Add(combineInstance);
            }
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        MeshFilter parentMeshFilter = parent.AddComponent<MeshFilter>();
        parentMeshFilter.mesh = combinedMesh;

        MeshCollider parentMeshCollider = parent.AddComponent<MeshCollider>();
        parentMeshCollider.sharedMesh = combinedMesh;

        // ������� �������� ��������
        foreach (GameObject segment in colliderSegments)
        {
            DestroyImmediate(segment);
        }
    }

    private List<Vector3> GeneratePathRiverStyle(Vector3 startPosition, Vector3 targetPosition)
    {
        List<Vector3> pathPoints = new List<Vector3>();
        Vector3 currentPosition = startPosition;
        targetPosition.y = terrain.SampleHeight(targetPosition);

        int maxAttempts = 10000;
        int attempts = 0;

        while (Vector2.Distance(new Vector2(currentPosition.x, currentPosition.z), new Vector2(targetPosition.x, targetPosition.z)) > pathStepSize && attempts < maxAttempts)
        {
            Vector3 direction = (targetPosition - currentPosition).normalized;

            float angle = (Mathf.PerlinNoise(currentPosition.x * pathCurvature, currentPosition.z * pathCurvature) - 0.5f) * 2 * pathMaxAngleChange;
            direction = Quaternion.Euler(0, angle, 0) * direction;

            Vector3 nextPosition = currentPosition + direction * pathStepSize;
            nextPosition.x = Mathf.Clamp(nextPosition.x, edgeMinOffset, terrainWidth - edgeMinOffset);
            nextPosition.z = Mathf.Clamp(nextPosition.z, edgeMinOffset, terrainLength - edgeMinOffset);
            nextPosition.y = terrain.SampleHeight(nextPosition);

            pathPoints.Add(currentPosition);
            currentPosition = nextPosition;

            attempts++;
        }

        // ��������� ��������� ��� � ����
        if (Vector3.Distance(currentPosition, targetPosition) > 1f)
        {
            pathPoints.Add(currentPosition);
            pathPoints.Add(targetPosition);
        }

        return pathPoints;
    }

    private bool IsSegmentInRiver(Vector3 start, Vector3 end)
    {
        int numChecks = Mathf.CeilToInt(Vector3.Distance(start, end) / (riverWidth / 2f));
        for (int i = 0; i <= numChecks; i++)
        {
            float t = (float)i / numChecks;
            Vector3 point = Vector3.Lerp(start, end, t);
            if (IsPositionInRiver(point, 0f))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPositionNearPlayer(Vector3 position)
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (Vector3.Distance(position, spawnPoint) < playerSafeRadius)
            {
                return true;
            }
        }
        return false;
    }

    private void PlacePlayers()
    {
        Debug.Log("����� �������...");

        // ����������� ����� ������ �� ����� �����
        spawnPoints.Add(new Vector3(edgeMinOffset, 0, terrainLength - edgeMinOffset)); // ������� ����� ����
        spawnPoints.Add(new Vector3(terrainWidth - edgeMinOffset, 0, terrainLength - edgeMinOffset)); // ������� ������ ����
        spawnPoints.Add(new Vector3(edgeMinOffset, 0, edgeMinOffset)); // ������ ����� ����
        spawnPoints.Add(new Vector3(terrainWidth - edgeMinOffset, 0, edgeMinOffset)); // ������ ������ ����

        for (int i = 0; i < playerCount; i++)
        {
            Vector3 spawnPosition = spawnPoints[i];
            spawnPosition.y = terrain.SampleHeight(spawnPosition);
            Instantiate(playerPrefab, spawnPosition, Quaternion.identity, generatedObjectsParent.transform);
        }
    }

    private void BalanceResources()
    {
        Debug.Log("������������ ��������...");

        int[] goldBefore = new int[4];
        int[] treesBefore = new int[4];
        float[] resourceValuesBefore = new float[4];

        float totalResourceValue = 0f;

        for (int i = 0; i < sectors.Length; i++)
        {
            var sector = sectors[i];
            goldBefore[i] = sector.goldCount;
            treesBefore[i] = sector.treeCount;
            resourceValuesBefore[i] = sector.goldCount * goldValue + (generateTrees ? sector.treeCount * treeValue : 0f);
            totalResourceValue += resourceValuesBefore[i];
        }

        if (totalResourceValue == 0f)
        {
            Debug.Log("��� �������� ��� ������������.");
            return;
        }

        float averageResourceValue = totalResourceValue / 4f;

        for (int i = 0; i < sectors.Length; i++)
        {
            var sector = sectors[i];
            float sectorResourceValue = sector.goldCount * goldValue + (generateTrees ? sector.treeCount * treeValue : 0f);
            float deviation = Mathf.Abs(sectorResourceValue - averageResourceValue) / averageResourceValue * 100f;

            if (deviation > resourceBalanceThresholdPercent)
            {
                int balanceAttempts = 0;
                int maxBalanceAttempts = 100; // ����������� �� ���������� ������� ������������

                while (Mathf.Abs(sectorResourceValue - averageResourceValue) > (averageResourceValue * resourceBalanceThresholdPercent / 100f) && balanceAttempts < maxBalanceAttempts)
                {
                    balanceAttempts++;

                    bool modifyGold = generateGold && (Random.value < 0.5f);
                    bool modifyTrees = generateTrees && (Random.value < 0.5f);

                    if (sectorResourceValue > averageResourceValue)
                    {
                        // ��������� �������
                        if (modifyGold && sector.goldCount > 0)
                        {
                            RemoveResourceInSector(i, ResourceType.Gold);
                            sector.goldCount--;
                        }
                        else if (modifyTrees && sector.treeCount > 0)
                        {
                            RemoveResourceInSector(i, ResourceType.Tree);
                            sector.treeCount--;
                        }
                    }
                    else
                    {
                        // ����������� �������
                        if (modifyGold && generateGold)
                        {
                            AddResourceInSector(i, goldPrefab);
                            sector.goldCount++;
                        }
                        else if (modifyTrees && generateTrees)
                        {
                            // �������� ��������� ������ ������
                            GameObject randomTreePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                            AddResourceInSector(i, randomTreePrefab);
                            sector.treeCount++;
                        }
                    }

                    // ��������� �������� ������� ����� ���������
                    sectorResourceValue = sector.goldCount * goldValue + (generateTrees ? sector.treeCount * treeValue : 0f);

                    // ��������� �������� �� ������, ���� averageResourceValue ���� ����� ����
                    if (averageResourceValue == 0f)
                    {
                        break;
                    }

                    if (balanceAttempts >= maxBalanceAttempts)
                    {
                        Debug.LogWarning($"������������ ���������� ������� ������������ ({maxBalanceAttempts}) ���������� ��� ������� {i + 1}.");
                        break;
                    }
                }
            }
        }

        string balanceInfo = "�������� ������������ �������� �� ��������:\n";
        for (int i = 0; i < sectors.Length; i++)
        {
            var sector = sectors[i];
            float resourceValueAfter = sector.goldCount * goldValue + (generateTrees ? sector.treeCount * treeValue : 0f);
            balanceInfo += $"������ {i + 1}: ������ ���� = {goldBefore[i]}, ����� = {sector.goldCount}; " +
                           $"������� ���� = {(generateTrees ? treesBefore[i].ToString() : "N/A")}, ����� = {(generateTrees ? sector.treeCount.ToString() : "N/A")}; " +
                           $"�������� �������� ���� = {resourceValuesBefore[i]:F1}, ����� = {resourceValueAfter:F1}\n";
        }
        Debug.Log(balanceInfo);

        Debug.Log("������������ �������� ���������.");
    }



    private void RemoveResourceInSector(int sectorIndex, ResourceType resourceType)
    {
        foreach (Transform child in generatedObjectsParent.transform)
        {
            ResourceIdentifier identifier = child.GetComponent<ResourceIdentifier>();
            if (identifier != null && identifier.resourceType == resourceType)
            {
                if (resourceType == ResourceType.CentralGold)
                    continue;

                if (GetSectorIndex(child.position) == sectorIndex)
                {
                    if (IsPositionNearPlayer(child.position) || IsPositionInRiver(child.position, riverResourceOffset))
                        continue;

                    if (resourceType == ResourceType.Gold)
                    {
                        goldPositions.Remove(child.position);
                    }
                    else if (resourceType == ResourceType.Tree && generateTrees)
                    {
                        treePositions.Remove(child.position);
                    }

                    DestroyImmediate(child.gameObject);
                    break;
                }
            }
        }
    }


    private void AddResourceInSector(int sectorIndex, GameObject prefab)
    {
        // ���������, �������� �� ��������� ���������������� �������
        if ((prefab == goldPrefab && !generateGold) || (IsTreePrefab(prefab) && !generateTrees))
            return;

        int maxAttempts = 10;
        int attempts = 0;
        bool resourceAdded = false;

        while (attempts < maxAttempts && !resourceAdded)
        {
            attempts++;
            Vector3 position = GetRandomPositionInSector(sectorIndex);
            position.y = terrain.SampleHeight(position) + terrain.transform.position.y;

            if (IsPositionNearPlayer(position) ||
                IsPositionInRiver(position, riverResourceOffset + stoneResourceOffset) ||
                !IsPositionWithinWavyEdge(position) ||
                IsPositionInSandClearing(position, treeExclusionRadiusAroundSandClearing) ||
                IsPositionNearPath(position, treeExclusionRadiusAroundPaths) ||
                IsPositionNearCentralGold(position, centralGoldExclusionRadius))
            {
                Debug.Log($"������� {attempts}: ������� {position} �� �������� ��� ���������� �������.");
                continue;
            }

            if (prefab == goldPrefab)
            {
                if (!IsPositionAcceptableForGold(position))
                {
                    Debug.Log($"������� {attempts}: ������� {position} �� �������� ��� ������.");
                    continue;
                }
                Quaternion rotation = Quaternion.Euler(0, Random.Range(goldMinRotationY, goldMaxRotationY), 0);
                GameObject resource = Instantiate(prefab, position, rotation, GoldParentObject.transform);
                goldPositions.Add(position);
                sectors[sectorIndex].goldCount++;

                // ��������� ��������� ResourceIdentifier
                ResourceIdentifier identifier = resource.AddComponent<ResourceIdentifier>();
                if (identifier != null)
                {
                    identifier.resourceType = ResourceType.Gold;
                }

                resourceAdded = true;
                Debug.Log($"������ ������� ��������� � ������ {sectorIndex + 1} � ������� {position}.");
            }
            else if (IsTreePrefab(prefab) && generateTrees)
            {
                if (!IsPositionAcceptableForTree(position))
                {
                    Debug.Log($"������� {attempts}: ������� {position} �� �������� ��� ������.");
                    continue;
                }
                // �������� ��������� ������ ������ �� �������
                GameObject selectedTreePrefab = prefab;

                // ������������� ��������� ������ ������ � �������� ��������
                float randomSize = Random.Range(treeMinSize, treeMaxSize);
                Vector3 randomScale = new Vector3(randomSize, randomSize, randomSize);

                Quaternion rotation = Quaternion.Euler(0, Random.Range(treeMinRotationY, treeMaxRotationY), 0);
                GameObject resource = Instantiate(selectedTreePrefab, position, rotation, treesParentObject.transform);
                resource.transform.localScale = randomScale; // ��������� ��������� ������
                treePositions.Add(position);
                sectors[sectorIndex].treeCount++;

                // ��������� ��������� ResourceIdentifier
                ResourceIdentifier identifier = resource.AddComponent<ResourceIdentifier>();
                if (identifier != null)
                {
                    identifier.resourceType = ResourceType.Tree;
                }

                resourceAdded = true;
                Debug.Log($"������ ������� ��������� � ������ {sectorIndex + 1} � ������� {position}.");
            }
        }
    }



    private bool IsTreePrefab(GameObject prefab)
    {
        foreach (var treePrefab in treePrefabs)
        {
            if (prefab == treePrefab)
                return true;
        }
        return false;
    }

    private void ApplyTerrainTextures()
    {
        Debug.Log("���������� ������� � ��������...");

        // �������� ��� ���� �������
        List<TerrainLayer> terrainLayersList = new List<TerrainLayer>();

        // ������� ����
        int baseLayerStartIndex = 0;
        int riverLayerIndex = -1;
        int sandClearingLayerIndex = -1;
        int pathLayerIndex = -1;
        int stoneTerrainStartIndex = -1;
        int leafTerrainStartIndex = -1;

        // ��������� ������� �������� �����
        terrainLayersList.AddRange(groundTerrainLayers);
        baseLayerStartIndex = 0; // ������� �������� ���������� � ������� 0

        // ��������� �������� ����
        if (enableRiverTexture && riverTerrainLayer != null)
        {
            riverLayerIndex = terrainLayersList.Count;
            terrainLayersList.Add(riverTerrainLayer);
        }

        // ��������� �������� �������� �������
        if (enableSandClearingTexture && sandClearingLayer != null)
        {
            sandClearingLayerIndex = terrainLayersList.Count;
            terrainLayersList.Add(sandClearingLayer);
        }

        // ��������� �������� ��������
        if (enablePathTexture && pathTerrainLayer != null)
        {
            pathLayerIndex = terrainLayersList.Count;
            terrainLayersList.Add(pathTerrainLayer);
        }

        // ��������� �������� ������
        if (stoneTerrainLayers != null && stoneTerrainLayers.Length > 0)
        {
            stoneTerrainStartIndex = terrainLayersList.Count;
            terrainLayersList.AddRange(stoneTerrainLayers);
        }

        // ��������� �������� �������
        if (leafTerrainLayers != null && leafTerrainLayers.Length > 0)
        {
            leafTerrainStartIndex = terrainLayersList.Count;
            terrainLayersList.AddRange(leafTerrainLayers);
        }

        // ��������� ������� ������� ��� ������� ���� �����
        foreach (var groundLayer in groundTerrainLayers)
        {
            groundLayer.tileSize = new Vector2(groundTextureSize, groundTextureSize);
        }

        // ��������� ������� ������� ��� ��������� ����
        if (riverTerrainLayer != null)
            riverTerrainLayer.tileSize = new Vector2(riverTextureSize, riverTextureSize);

        if (sandClearingLayer != null)
            sandClearingLayer.tileSize = new Vector2(sandClearingTextureSize, sandClearingTextureSize);

        if (pathTerrainLayer != null)
            pathTerrainLayer.tileSize = new Vector2(pathTextureSize, pathTextureSize);

        foreach (var stoneLayer in stoneTerrainLayers)
        {
            stoneLayer.tileSize = new Vector2(stoneTextureSize, stoneTextureSize);
        }

        foreach (var leafLayer in leafTerrainLayers)
        {
            leafLayer.tileSize = new Vector2(leafTextureSize, leafTextureSize);
        }

        // ������������� ���� ������� �� �������
        terrain.terrainData.terrainLayers = terrainLayersList.ToArray();
        int numTextures = terrain.terrainData.alphamapLayers;

        // �������� ������� �����-�����
        alphaMapWidth = terrain.terrainData.alphamapWidth;
        alphaMapHeight = terrain.terrainData.alphamapHeight;

        // �������������� ����� ����� ���� � ��������, ���� ��� �� ����������������
        if (riverWeightMap == null)
            riverWeightMap = new float[alphaMapHeight, alphaMapWidth];
        if (pathWeightMap == null)
            pathWeightMap = new float[alphaMapHeight, alphaMapWidth];

        float[,,] alphaMaps = terrain.terrainData.GetAlphamaps(0, 0, alphaMapWidth, alphaMapHeight);

        // ������� ������ �����
        stonePatches.Clear();
        leafPatches.Clear();

        // ���������� ����� ������ � �������
        if (enableRandomStones)
        {
            GenerateStonePatches();
        }

        if (enableRandomLeaves)
        {
            GenerateLeafPatches();
        }

        // ������� ����� �������� ��� ���������
        int[,] stoneCoverageMap = new int[alphaMapWidth, alphaMapHeight];
        int[,] leafCoverageMap = new int[alphaMapWidth, alphaMapHeight];

        // �������������� ����� �������� ��������� -1
        for (int z = 0; z < alphaMapHeight; z++)
        {
            for (int x = 0; x < alphaMapWidth; x++)
            {
                stoneCoverageMap[x, z] = -1;
                leafCoverageMap[x, z] = -1;
            }
        }

        // ��������� ����� �������� ������
        foreach (var patch in stonePatches)
        {
            FillCoverageMap(patch.vertices, stoneCoverageMap, patch.textureIndex);
        }

        // ��������� ����� �������� �������
        foreach (var patch in leafPatches)
        {
            FillCoverageMap(patch.vertices, leafCoverageMap, patch.textureIndex);
        }

        for (int z = 0; z < alphaMapHeight; z++)
        {
            for (int x = 0; x < alphaMapWidth; x++)
            {
                float[] textureWeights = new float[numTextures];

                Vector3 worldPos = new Vector3(
                    (x / (float)(alphaMapWidth - 1)) * terrainWidth,
                    0,
                    (z / (float)(alphaMapHeight - 1)) * terrainLength
                );

                // �������� � ������� �����
                for (int i = 0; i < numTextures; i++)
                {
                    textureWeights[i] = 0f;
                }

                // �������� ���� �������� �������
                float riverWeight = (riverLayerIndex >= 0 && enableRiverTexture) ? riverWeightMap[z, x] : 0f;
                riverWeight = Mathf.Clamp01(riverWeight);

                float pathWeight = (pathLayerIndex >= 0 && enablePathTexture) ? pathWeightMap[z, x] : 0f;
                pathWeight = Mathf.Clamp01(pathWeight);
                pathWeight *= (1f - riverWeight); // �������� �� ����������� ����

                float sandClearingWeight = (sandClearingLayerIndex >= 0 && enableSandClearingTexture) ? GetSandClearingTextureWeight(worldPos) : 0f;
                sandClearingWeight = Mathf.Clamp01(sandClearingWeight);
                sandClearingWeight *= (1f - riverWeight) * (1f - pathWeight); // �������� ������� �� ����������� ���� � ��������

                // ��������� ��� ������� ������� �����
                float baseWeight = Mathf.Clamp01(1f - (riverWeight + pathWeight + sandClearingWeight));

                // ��������� ����������� ��� ������� ���� �����
                if (enableBaseTexture && baseWeight > 0f)
                {
                    float perlinValue = Mathf.PerlinNoise(
                        (worldPos.x) / groundNoiseScale,
                        (worldPos.z) / groundNoiseScale
                    );

                    float textureBlend = perlinValue * (groundTerrainLayers.Length - 1);
                    int textureIndex = Mathf.FloorToInt(textureBlend);
                    float blend = textureBlend - textureIndex;

                    for (int i = 0; i < groundTerrainLayers.Length; i++)
                    {
                        int layerIndex = baseLayerStartIndex + i;

                        if (i == textureIndex)
                        {
                            textureWeights[layerIndex] += baseWeight * (1f - blend * groundTextureSmoothing);
                        }
                        else if (i == textureIndex + 1 && i < groundTerrainLayers.Length)
                        {
                            textureWeights[layerIndex] += baseWeight * blend * groundTextureSmoothing;
                        }
                        else
                        {
                            textureWeights[layerIndex] += 0f;
                        }
                    }
                }

                // ������������� ���� ��� ����
                if (riverLayerIndex >= 0 && riverWeight > 0f)
                {
                    textureWeights[riverLayerIndex] += riverWeight;
                }

                // ������������� ���� ��� �������� �������
                if (sandClearingLayerIndex >= 0 && sandClearingWeight > 0f)
                {
                    textureWeights[sandClearingLayerIndex] += sandClearingWeight;
                }

                // ������������� ���� ��� ��������
                if (pathLayerIndex >= 0 && pathWeight > 0f)
                {
                    textureWeights[pathLayerIndex] += pathWeight;
                }

                // ����� ��� ��������, ��������� �� ����� � ����� ������ ��� �������
                bool isInStonePatch = false;

                // ���������, ��������� �� ������� ����� ������ ����� ������
                if (enableRandomStones && stoneCoverageMap[x, z] >= 0 && pathWeight < 0.5f)
                {
                    int stoneTextureIndexAtPoint = stoneCoverageMap[x, z];
                    if (stoneTextureIndexAtPoint >= 0)
                    {
                        int stoneLayerIndex = stoneTerrainStartIndex + stoneTextureIndexAtPoint;
                        textureWeights[stoneLayerIndex] += 1f;
                        // ������� �������� ������� ��� ������
                        if (sandClearingLayerIndex >= 0)
                            textureWeights[sandClearingLayerIndex] *= (1f - 1f); // ������������� 0
                        isInStonePatch = true;
                    }
                }

                // ���������, ��������� �� ������� ����� ������ ����� �������
                if (enableRandomLeaves && baseWeight > 0.5f && !isInStonePatch && pathWeight < 0.5f)
                {
                    float leafWeight = GetLeafPatchTextureWeight(worldPos);
                    if (leafWeight > 0f)
                    {
                        int leafLayerIndex = leafTerrainStartIndex; // ��������������, ��� � ��� ���� �������� ������
                        textureWeights[leafLayerIndex] += leafWeight;
                        // ��������� ��� ������� ������� ��� ��������
                        for (int i = 0; i < groundTerrainLayers.Length; i++)
                        {
                            int groundLayerIndex = baseLayerStartIndex + i;
                            textureWeights[groundLayerIndex] *= (1f - leafWeight);
                        }
                    }
                }

                // ����������� ���� �������
                float totalWeight = 0f;
                for (int i = 0; i < numTextures; i++)
                {
                    totalWeight += textureWeights[i];
                }

                if (totalWeight > 0f)
                {
                    for (int i = 0; i < numTextures; i++)
                    {
                        alphaMaps[z, x, i] = textureWeights[i] / totalWeight;
                    }
                }
                else
                {
                    // ���� ��� ���� ����� 0, ������������� ������ ������� ��������
                    alphaMaps[z, x, baseLayerStartIndex] = 1f;
                }
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, alphaMaps);
    }



    private float GetLeafPatchTextureWeight(Vector3 position)
    {
        float maxWeight = 0f;
        Vector2 posXZ = new Vector2(position.x, position.z);

        foreach (var leafPatch in leafPatches)
        {
            if (IsPointInPolygon(leafPatch.vertices, posXZ))
            {
                float distance = DistanceToPolygonEdge(leafPatch.vertices, posXZ);

                float smoothingDistance = leafTextureSmoothing;

                float weight = 1f;
                if (smoothingDistance > 0f)
                {
                    weight = Mathf.Clamp01(distance / smoothingDistance);
                }

                if (weight > maxWeight)
                {
                    maxWeight = weight;
                }
            }
        }

        return maxWeight;
    }




    private void GenerateStonePatches()
    {
        Debug.Log("��������� ����� ������...");

        float totalArea = terrainWidth * terrainLength;
        float targetCoverageArea = totalArea * stoneDensity;

        float averagePatchArea = Mathf.PI * Mathf.Pow((minStonePatchSize + maxStonePatchSize) / 4f, 2);
        int estimatedPatchCount = Mathf.CeilToInt(targetCoverageArea / averagePatchArea);

        for (int i = 0; i < estimatedPatchCount; i++)
        {
            // ���������� ��������� ������ �����
            float patchSize = Random.Range(minStonePatchSize, maxStonePatchSize);

            // ��������� ����������� ������
            if (patchSize < minStonePatchSize)
                continue; // ���������� ������� ��������� �����

            // ���������� ��������� ������� ������ �������� �������
            Vector3 position = new Vector3(
                Random.Range(0, terrainWidth),
                0,
                Random.Range(0, terrainLength)
            );

            position.y = terrain.SampleHeight(position) + terrain.transform.position.y;

            // ���������, ��������� �� ������� ������ �������� �������
            if (!IsPositionInSandClearing(position))
                continue;

            // ������� ��������� �������� �������
            List<Vector2> polygon = GenerateRandomConvexPolygon(position, patchSize);

            // ��������� ������� ��������
            float area = CalculatePolygonArea(polygon);
            if (area < minStonePatchArea)
                continue; // ���������� ����� � ��������� ��������

            // �������� ��������� ��������
            int textureIndex = Random.Range(0, stoneTerrainLayers.Length);

            StonePatch newPatch = new StonePatch()
            {
                vertices = polygon,
                textureIndex = textureIndex
            };

            stonePatches.Add(newPatch);
        }
    }




    private void GenerateLeafPatches()
{
    Debug.Log("��������� ����� �������...");

    float totalArea = terrainWidth * terrainLength;
    float targetCoverageArea = totalArea * leafDensity;

    float averagePatchArea = Mathf.PI * Mathf.Pow((minLeafPatchSize + maxLeafPatchSize) / 4f, 2);
    int estimatedPatchCount = Mathf.CeilToInt(targetCoverageArea / averagePatchArea);

    for (int i = 0; i < estimatedPatchCount; i++)
    {
        // ���������� ��������� ������ �����
        float patchSize = Random.Range(minLeafPatchSize, maxLeafPatchSize);

        // ��������� ����������� ������
        if (patchSize < minLeafPatchSize)
            continue; // ���������� ������� ��������� �����

        // ���������� ��������� ������� �� �����
        Vector3 position = new Vector3(
            Random.Range(0, terrainWidth),
            0,
            Random.Range(0, terrainLength)
        );

        position.y = terrain.SampleHeight(position) + terrain.transform.position.y;

        // ���������, ��� ������� �� ��������� � ����, �� �������� ��� �������� �������
        if (IsPositionInRiver(position, 0f) || IsPositionNearPath(position, 0f) || IsPositionInSandClearing(position))
            continue;

        // ������� ��������� �������� �������
        List<Vector2> polygon = GenerateRandomConvexPolygon(position, patchSize);

        // ��������� ������� ��������
        float area = CalculatePolygonArea(polygon);
        if (area < minLeafPatchArea)
            continue; // ���������� ����� � ��������� ��������

        // �������� ��������� ��������
        int textureIndex = Random.Range(0, leafTerrainLayers.Length);

        LeafPatch newPatch = new LeafPatch()
        {
            vertices = polygon,
            textureIndex = textureIndex
        };

        leafPatches.Add(newPatch);
    }
}



    private void FillCoverageMap(List<Vector2> polygon, int[,] coverageMap, int textureIndex)
    {
        // ����������� ���������� �������� � ���������� �����-�����
        int minX = alphaMapWidth - 1;
        int maxX = 0;
        int minZ = alphaMapHeight - 1;
        int maxZ = 0;

        List<Vector2> pixelPolygon = new List<Vector2>();

        foreach (var vertex in polygon)
        {
            float x = (vertex.x / terrainWidth) * (alphaMapWidth - 1);
            float z = (vertex.y / terrainLength) * (alphaMapHeight - 1);

            // ������� �������� x � z
            x = Mathf.Clamp(x, 0, alphaMapWidth - 1);
            z = Mathf.Clamp(z, 0, alphaMapHeight - 1);

            int xIntFloor = Mathf.FloorToInt(x);
            int xIntCeil = Mathf.CeilToInt(x);
            int zIntFloor = Mathf.FloorToInt(z);
            int zIntCeil = Mathf.CeilToInt(z);

            minX = Mathf.Min(minX, xIntFloor);
            maxX = Mathf.Max(maxX, xIntCeil);
            minZ = Mathf.Min(minZ, zIntFloor);
            maxZ = Mathf.Max(maxZ, zIntCeil);

            pixelPolygon.Add(new Vector2(x, z));
        }

        // ������� minX, maxX, minZ, maxZ
        minX = Mathf.Clamp(minX, 0, alphaMapWidth - 1);
        maxX = Mathf.Clamp(maxX, 0, alphaMapWidth - 1);
        minZ = Mathf.Clamp(minZ, 0, alphaMapHeight - 1);
        maxZ = Mathf.Clamp(maxZ, 0, alphaMapHeight - 1);

        // �������� �� ������� �������� � �������� �������� �������
        for (int z = minZ; z <= maxZ; z++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                Vector2 point = new Vector2(x + 0.5f, z + 0.5f);
                if (IsPointInPolygon(pixelPolygon, point))
                {
                    // ���������, ��� ������� ��������� � �������� �������
                    if (x >= 0 && x < alphaMapWidth && z >= 0 && z < alphaMapHeight)
                    {
                        coverageMap[x, z] = textureIndex;
                    }
                }
            }
        }
    }




    // ������� ��������� ���������� ��������� ��������
    private List<Vector2> GenerateRandomConvexPolygon(Vector3 center, float size)
    {
        int numPoints = Random.Range(3, 7); // ��������� ���������� ������ �� 3 �� 6
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < numPoints; i++)
        {
            float angle = i * Mathf.PI * 2f / numPoints + Random.Range(-Mathf.PI / numPoints, Mathf.PI / numPoints);
            float radius = size * 0.5f * Random.Range(0.7f, 1.0f);
            float x = center.x + Mathf.Cos(angle) * radius;
            float z = center.z + Mathf.Sin(angle) * radius;

            points.Add(new Vector2(x, z));
        }

        // ������� �������� ��������
        List<Vector2> convexHull = ComputeConvexHull(points);

        return convexHull;
    }

    // ������� ���������� �������� �������� (�������� �������)
    private List<Vector2> ComputeConvexHull(List<Vector2> points)
    {
        if (points.Count <= 1)
            return new List<Vector2>(points);

        // ��������� ����� �� X, ����� �� Y
        points.Sort((a, b) =>
        {
            int compareX = a.x.CompareTo(b.x);
            if (compareX == 0)
                return a.y.CompareTo(b.y);
            else
                return compareX;
        });

        List<Vector2> lower = new List<Vector2>();
        foreach (var p in points)
        {
            while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], p) <= 0)
                lower.RemoveAt(lower.Count - 1);
            lower.Add(p);
        }

        List<Vector2> upper = new List<Vector2>();
        for (int i = points.Count - 1; i >= 0; i--)
        {
            Vector2 p = points[i];
            while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], p) <= 0)
                upper.RemoveAt(upper.Count - 1);
            upper.Add(p);
        }

        lower.RemoveAt(lower.Count - 1);
        upper.RemoveAt(upper.Count - 1);

        lower.AddRange(upper);
        return lower;
    }

    private float Cross(Vector2 O, Vector2 A, Vector2 B)
    {
        return (A.x - O.x) * (B.y - O.y) - (A.y - O.y) * (B.x - O.x);
    }

    // ������� ���������� ������� ��������
    private float CalculatePolygonArea(List<Vector2> polygon)
    {
        float area = 0f;
        int count = polygon.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[(i + 1) % count];
            area += (a.x * b.y - b.x * a.y);
        }
        return Mathf.Abs(area) / 2f;
    }


    private void GeneratePathWeightMap()
    {
        Debug.Log("��������� ����� ����� ��������...");
        foreach (var path in allPaths)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 start = path[i];
                Vector3 end = path[i + 1];

                // ����������� ������� ���������� � ���������� �����-�����
                int startX = Mathf.RoundToInt((start.x / terrainWidth) * (alphaMapWidth - 1));
                int startZ = Mathf.RoundToInt((start.z / terrainLength) * (alphaMapHeight - 1));
                int endX = Mathf.RoundToInt((end.x / terrainWidth) * (alphaMapWidth - 1));
                int endZ = Mathf.RoundToInt((end.z / terrainLength) * (alphaMapHeight - 1));

                // ����������� ����� �� ����� �����
                RasterizePathSegment(startX, startZ, endX, endZ);
            }
        }
    }

    private void RasterizePathSegment(int startX, int startZ, int endX, int endZ)
    {
        int dx = Mathf.Abs(endX - startX);
        int dz = Mathf.Abs(endZ - startZ);
        int sx = startX < endX ? 1 : -1;
        int sz = startZ < endZ ? 1 : -1;
        int err = dx - dz;

        int x = startX;
        int z = startZ;

        while (true)
        {
            SetPathWeightAt(x, z);

            if (x == endX && z == endZ)
                break;

            int e2 = 2 * err;
            if (e2 > -dz)
            {
                err -= dz;
                x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                z += sz;
            }
        }
    }

    private void SetPathWeightAt(int x, int z)
    {
        // ������������ �������� ������ �������� � �������� �����-�����
        float pathHalfWidthPixels = (pathWidth / terrainWidth) * alphaMapWidth / 2f;

        // ������������ �������������� ����������� � ��������
        float smoothingPixels = pathTextureSmoothing * (pathWidth / terrainWidth) * alphaMapWidth;

        // ����� ������ ������� ��������
        float radius = pathHalfWidthPixels + smoothingPixels;

        for (int dzOffset = -Mathf.CeilToInt(radius); dzOffset <= Mathf.CeilToInt(radius); dzOffset++)
        {
            for (int dxOffset = -Mathf.CeilToInt(radius); dxOffset <= Mathf.CeilToInt(radius); dxOffset++)
            {
                int nx = x + dxOffset;
                int nz = z + dzOffset;

                // ��������� ������� �����
                if (nx >= 0 && nx < alphaMapWidth && nz >= 0 && nz < alphaMapHeight)
                {
                    // ��������� ���������� �� �������� ������� �� ������ ��������
                    float distance = Mathf.Sqrt(dxOffset * dxOffset + dzOffset * dzOffset);

                    float weight = 0f;

                    // ���� ������ �������� ������ ��������
                    if (distance <= pathHalfWidthPixels)
                    {
                        weight = 1f;
                    }
                    // ���� ������ ���� �����������
                    else if (pathTextureSmoothing > 0f && distance <= radius)
                    {
                        weight = 1f - ((distance - pathHalfWidthPixels) / smoothingPixels);
                    }

                    weight = Mathf.Clamp01(weight);

                    if (pathWeightMap[nz, nx] < weight)
                    {
                        pathWeightMap[nz, nx] = weight;
                    }
                }
            }
        }
    }



    private float GetSandClearingTextureWeight(Vector3 position)
    {
        float maxWeight = 0f;
        Vector2 posXZ = new Vector2(position.x, position.z);

        foreach (var clearing in sandClearings)
        {
            if (IsPointInPolygon(clearing.vertices, posXZ))
            {
                float distance = DistanceToPolygonEdge(clearing.vertices, posXZ);

                float smoothingDistance = sandClearingTextureSmoothing;

                float weight = 1f;
                if (smoothingDistance > 0f)
                {
                    weight = Mathf.Clamp01(distance / smoothingDistance);
                }

                if (weight > maxWeight)
                {
                    maxWeight = weight;
                }
            }
        }

        return maxWeight;
    }



    private bool IsPointInPolygon(List<Vector2> polygon, Vector2 point)
    {
        int crossings = 0;
        int count = polygon.Count;

        for (int i = 0; i < count; i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[(i + 1) % count];

            if (((a.y > point.y) != (b.y > point.y)) &&
                (point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y + Mathf.Epsilon) + a.x))
            {
                crossings++;
            }
        }

        return (crossings % 2) != 0;
    }




    private float DistanceToPolygonEdge(List<Vector2> polygon, Vector2 point)
    {
        float minDist = float.MaxValue;
        int count = polygon.Count;

        for (int i = 0; i < count; i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[(i + 1) % count];
            float dist = DistancePointToLineSegment(point, a, b);
            if (dist < minDist)
            {
                minDist = dist;
            }
        }

        return minDist;
    }

    private float DistancePointToLineSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ap = p - a;
        Vector2 ab = b - a;
        float ab2 = ab.x * ab.x + ab.y * ab.y;
        if (ab2 == 0)
            return Vector2.Distance(p, a);
        float ap_ab = ap.x * ab.x + ap.y * ab.y;
        float t = ap_ab / ab2;
        t = Mathf.Clamp01(t);
        Vector2 closest = a + ab * t;
        return Vector2.Distance(p, closest);
    }



    private bool IsPositionInSandClearing(Vector3 position, float offset = 0f)
    {
        Vector2 posXZ = new Vector2(position.x, position.z);

        foreach (var clearing in sandClearings)
        {
            // ��������� ���������� �� ������� �������
            float distanceToEdge = DistanceToPolygonEdge(clearing.vertices, posXZ);

            if (IsPointInPolygon(clearing.vertices, posXZ))
            {
                if (distanceToEdge >= offset)
                {
                    return true;
                }
            }
            else
            {
                if (distanceToEdge <= offset)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void GenerateSandClearings()
    {
        Debug.Log("��������� �������� �������...");

        int sandClearingCount = Random.Range(minSandClearingCount, maxSandClearingCount + 1);
        int attempts = 0;
        int maxAttempts = sandClearingCount * 10; // ������������ ���������� �������

        int clearingsGenerated = 0;

        while (clearingsGenerated < sandClearingCount && attempts < maxAttempts)
        {
            attempts++;

            Vector3 position = new Vector3(
                Random.Range(edgeMinOffset, terrainWidth - edgeMinOffset),
                0,
                Random.Range(edgeMinOffset, terrainLength - edgeMinOffset)
            );
            position.y = terrain.SampleHeight(position) + terrain.transform.position.y;

            float baseRadius = Random.Range(minSandClearingSize, maxSandClearingSize);

            if (IsPositionNearPlayer(position) || !IsPositionWithinWavyEdge(position))
            {
                Debug.Log($"������� {attempts}: ������� {position} �� �������� ��� �������� �������.");
                continue;
            }

            // �������� ���������� �� ������������ �������� ������� � ������ ��������
            bool tooCloseToOtherClearing = false;
            foreach (var existingClearing in sandClearings)
            {
                float distance = Vector3.Distance(position, existingClearing.center);
                float minDistance = existingClearing.baseRadius + baseRadius + sandClearingMinDistanceBetween;
                if (distance < minDistance)
                {
                    tooCloseToOtherClearing = true;
                    break;
                }
            }
            if (tooCloseToOtherClearing)
            {
                Debug.Log($"������� {attempts}: ������� {position} ������� ������ � ������ �������� �������.");
                continue; // ������� ������ � ������ �������, ������� �����
            }

            // ��������� ����������� ���������� �����
            SandClearing clearing = new SandClearing();
            clearing.center = position;
            clearing.baseRadius = baseRadius;

            int pointCount = 36; // ������ ����� ��� ����� �������� �����
            float angleStep = 360f / pointCount;

            clearing.vertices = new List<Vector2>();

            for (int j = 0; j < pointCount; j++)
            {
                float angle = j * angleStep;
                float noise = Mathf.PerlinNoise(
                    position.x * sandClearingNoiseScale + Mathf.Cos(Mathf.Deg2Rad * angle),
                    position.z * sandClearingNoiseScale + Mathf.Sin(Mathf.Deg2Rad * angle)
                );
                float radius = baseRadius + (noise - 0.5f) * sandClearingRandomness * baseRadius;

                float x = position.x + Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                float z = position.z + Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

                clearing.vertices.Add(new Vector2(x, z));
            }

            // �������� ������� �������
            if (baseRadius < minSandClearingSize)
            {
                Debug.Log($"������� {attempts}: ������ ������� {baseRadius} ������ ������������.");
                continue; // �� ��������� �������
            }

            // �������� ����������� � ������
            bool intersectsRiver = false;
            foreach (var riverPolygon in riverPolygons)
            {
                if (PolygonsIntersect(clearing.vertices, riverPolygon))
                {
                    intersectsRiver = true;
                    break;
                }
            }

            if (intersectsRiver)
            {
                Debug.Log($"������� {attempts}: �������� ������� ������������ � �����.");
                continue; // �� ��������� �������, ���� ��� ������������ � �����
            }

            // �������� ����������� � ����������
            bool intersectsPath = false;
            foreach (var pathPolygon in pathPolygons)
            {
                if (PolygonsIntersect(clearing.vertices, pathPolygon))
                {
                    intersectsPath = true;
                    break;
                }
            }

            if (intersectsPath)
            {
                Debug.Log($"������� {attempts}: �������� ������� ������������ � ���������.");
                continue; // �� ��������� �������, ���� ��� ������������ � ���������
            }

            // ���� ��� �������� ��������, ��������� �������
            sandClearings.Add(clearing);
            sandClearingCenters.Add(position);

            // ������� ��������� ��� �������� �������
            CreateSandClearingCollider(clearing);

            clearingsGenerated++;
            Debug.Log($"������� ������������� �������� ������� #{clearingsGenerated}.");
        }

        if (clearingsGenerated < sandClearingCount)
        {
            Debug.LogWarning($"������� ������������� ������ {clearingsGenerated} �� {sandClearingCount} �������� ������� ����� {attempts} �������.");
        }
        else
        {
            Debug.Log($"������� ������������� {clearingsGenerated} �� {sandClearingCount} �������� �������.");
        }
    }


    private void CreateSandClearingCollider(SandClearing clearing)
    {
        // ���������, ���������� �� ������
        if (sandClearingColliderPrefab == null)
        {
            Debug.LogError("������ sandClearingColliderPrefab �� ����������.");
            return;
        }

        // ������� ������ ��� �������� �������
        GameObject clearingObject = new GameObject("SandClearingCollider");
        clearingObject.transform.parent = generatedObjectsParent.transform;
        clearingObject.transform.position = terrain.transform.position; // ������������� ������� ������������ ��������

        // ������� MeshCollider �� ������ ������ �������
        MeshCollider meshCollider = clearingObject.AddComponent<MeshCollider>();
        Mesh mesh = new Mesh();

        // ������������ ������� �� Vector2 � Vector3
        Vector3[] vertices = new Vector3[clearing.vertices.Count];
        for (int i = 0; i < clearing.vertices.Count; i++)
        {
            Vector2 vertex = clearing.vertices[i];
            vertices[i] = new Vector3(vertex.x, terrain.SampleHeight(new Vector3(vertex.x, 0, vertex.y)), vertex.y);
        }

        // ������� ������������ ��� ����
        Triangulator triangulator = new Triangulator(clearing.vertices.ToArray());
        int[] indices = triangulator.Triangulate();

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshCollider.sharedMesh = mesh;
        meshCollider.convex = false; // ���������, ��� convex ���������� � false ��� ������� �����������
    }


    private List<Vector2> CreatePolygonFromLine(List<Vector3> linePoints, float width)
    {
        List<Vector2> polygon = new List<Vector2>();
        float halfWidth = width / 2f;

        for (int i = 0; i < linePoints.Count; i++)
        {
            Vector3 currentPoint = linePoints[i];
            Vector3 direction;

            if (i < linePoints.Count - 1)
            {
                direction = (linePoints[i + 1] - currentPoint).normalized;
            }
            else
            {
                direction = (currentPoint - linePoints[i - 1]).normalized;
            }

            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized * halfWidth;
            Vector2 leftPoint = new Vector2(currentPoint.x - perpendicular.x, currentPoint.z - perpendicular.z);
            Vector2 rightPoint = new Vector2(currentPoint.x + perpendicular.x, currentPoint.z + perpendicular.z);

            polygon.Add(leftPoint);
            polygon.Insert(0, rightPoint);
        }

        return polygon;
    }

    private void GenerateStones()
    {
        if (!generateStones)
            return;

        // �������� �� ������� �������� ������
        if (stonePrefabs == null || stonePrefabs.Length == 0)
        {
            Debug.LogError("������ stonePrefabs ����. ����������, �������� ������� ������ � ����������.");
            return;
        }

        Debug.Log("��������� ������...");

        // ������� ���������� �����
        if (stonesParentObject != null)
        {
            DestroyImmediate(stonesParentObject);
        }
        stonesParentObject = new GameObject("Stones");
        stonesParentObject.transform.parent = generatedObjectsParent.transform;

        stoneLinePositions.Clear(); // ������� ������ ������� ����� ������

        int stoneLineCount = Random.Range(minStoneLineCount, maxStoneLineCount + 1);
        int attempts = 0;
        int maxAttempts = stoneLineCount * 10;

        int linesGenerated = 0;

        while (linesGenerated < stoneLineCount && attempts < maxAttempts)
        {
            attempts++;

            Vector3 startPosition = new Vector3(
                Random.Range(edgeMinOffset, terrainWidth - edgeMinOffset),
                0,
                Random.Range(edgeMinOffset, terrainLength - edgeMinOffset)
            );
            startPosition.y = terrain.SampleHeight(startPosition) + terrain.transform.position.y;

            if (IsPositionNearPlayer(startPosition) ||
                IsPositionInRiver(startPosition, riverResourceOffset + stoneResourceOffset) ||
                IsPositionNearPath(startPosition, stoneExclusionRadiusAroundPaths) ||
                IsPositionNearStoneLine(startPosition, minDistanceBetweenStoneLines) ||
                IsPositionNearCentralGold(startPosition, centralGoldExclusionRadius) ||
                IsPositionNearGold(startPosition, stoneExclusionRadiusAroundGold) ||
                !IsPositionWithinWavyEdge(startPosition))
            {
                Debug.Log($"������� {attempts}: ������� {startPosition} �� �������� ��� ������ ����� ������.");
                continue;
            }

            int lineLength = Random.Range(minStoneLineLength, maxStoneLineLength + 1);

            List<Vector3> stoneLinePoints = new List<Vector3>();
            List<Vector3> stoneScales = new List<Vector3>();

            Vector3 currentPosition = startPosition;
            Vector3 direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

            float previousRandomSize = Random.Range(stoneMinSize, stoneMaxSize);
            float previousRandomHeight = Random.Range(stoneMinHeight, stoneMaxHeight);

            stoneLinePoints.Add(currentPosition);
            stoneScales.Add(new Vector3(previousRandomSize, previousRandomHeight, previousRandomSize));

            bool lineFailed = false;

            for (int i = 1; i < lineLength; i++)
            {
                float angleChange = Random.Range(-maxStoneAngleChange, maxStoneAngleChange);
                direction = Quaternion.Euler(0, angleChange, 0) * direction;

                float randomSize = Random.Range(stoneMinSize, stoneMaxSize);
                float randomHeight = Random.Range(stoneMinHeight, stoneMaxHeight);

                float distanceBetweenStones = (previousRandomSize / 2f) + stoneSpacing + (randomSize / 2f);

                Vector3 nextPosition = currentPosition + direction * distanceBetweenStones;
                nextPosition.x = Mathf.Clamp(nextPosition.x, edgeMinOffset, terrainWidth - edgeMinOffset);
                nextPosition.z = Mathf.Clamp(nextPosition.z, edgeMinOffset, terrainLength - edgeMinOffset);
                nextPosition.y = terrain.SampleHeight(nextPosition) + terrain.transform.position.y;

                if (IsPositionNearPlayer(nextPosition) ||
                    IsPositionInRiver(nextPosition, riverResourceOffset + stoneResourceOffset) ||
                    IsPositionNearPath(nextPosition, stoneExclusionRadiusAroundPaths) ||
                    IsPositionNearStoneLine(nextPosition, minDistanceBetweenStoneLines) ||
                    IsPositionNearCentralGold(nextPosition, centralGoldExclusionRadius) ||
                    IsPositionNearGold(nextPosition, stoneExclusionRadiusAroundGold) ||
                    !IsPositionWithinWavyEdge(nextPosition))
                {
                    Debug.Log($"������� {attempts}: ������� {nextPosition} �� �������� ��� �����.");
                    lineFailed = true;
                    break;
                }

                stoneLinePoints.Add(nextPosition);
                stoneScales.Add(new Vector3(randomSize, randomHeight, randomSize));

                currentPosition = nextPosition;
                previousRandomSize = randomSize;
                previousRandomHeight = randomHeight;
            }

            // �������� �� ����������� � ���������� ��� ������
            if (!lineFailed && DoesStoneLineIntersectPathsOrRivers(stoneLinePoints))
            {
                Debug.Log($"������� {attempts}: ����� ������ ���������� �������� ��� ����.");
                continue; // ����� ������������, ���������� ��
            }

            if (stoneLinePoints.Count >= minStoneLineLength)
            {
                CreateStoneLine(stoneLinePoints, stoneScales);
                linesGenerated++;
                Debug.Log($"������� ������������� ����� ������ #{linesGenerated}.");
            }
            else
            {
                Debug.Log($"������� {attempts}: ����� ����� ������ ������ �����������.");
            }
        }

        if (linesGenerated == 0)
        {
            Debug.LogWarning("�� ������� ������������� �� ����� ����� ������.");
        }
        else
        {
            Debug.Log($"������� ������������� {linesGenerated} �� {stoneLineCount} ����� ������.");
        }
    }


    private bool DoesStoneLineIntersectPathsOrRivers(List<Vector3> stoneLinePoints)
    {
        // ����������� �������� ����� � ������ ���������
        for (int i = 0; i < stoneLinePoints.Count - 1; i++)
        {
            Vector2 stoneStart = new Vector2(stoneLinePoints[i].x, stoneLinePoints[i].z);
            Vector2 stoneEnd = new Vector2(stoneLinePoints[i + 1].x, stoneLinePoints[i + 1].z);

            // ��������� ����������� � ���������� ��������
            foreach (var pathPolygon in pathPolygons)
            {
                for (int j = 0; j < pathPolygon.Count; j++)
                {
                    Vector2 pathStart = pathPolygon[j];
                    Vector2 pathEnd = pathPolygon[(j + 1) % pathPolygon.Count];

                    if (LinesIntersect(stoneStart, stoneEnd, pathStart, pathEnd))
                    {
                        return true;
                    }
                }
            }

            // ��������� ����������� � ���������� ���
            foreach (var riverPolygon in riverPolygons)
            {
                for (int j = 0; j < riverPolygon.Count; j++)
                {
                    Vector2 riverStart = riverPolygon[j];
                    Vector2 riverEnd = riverPolygon[(j + 1) % riverPolygon.Count];

                    if (LinesIntersect(stoneStart, stoneEnd, riverStart, riverEnd))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }



    private void CreateStoneLine(List<Vector3> stoneLinePoints, List<Vector3> stoneScales)
    {
        // ���������, ���� �� ������� ������
        if (stonePrefabs == null || stonePrefabs.Length == 0)
        {
            Debug.LogError("������ stonePrefabs ����. ����������, �������� ������� ������ � ����������.");
            return;
        }

        // ���������, ��� ����� ������� ���������
        if (stoneLinePoints.Count != stoneScales.Count)
        {
            Debug.LogError("����� ������� stoneLinePoints � stoneScales �� ���������.");
            return;
        }

        // ������� ������ ��� ����� ������ � ������������� ��� ��������� ��� ������
        GameObject stonesParent = new GameObject("StoneLine");
        stonesParent.transform.parent = stonesParentObject.transform;

        // ��������� ����� ����� ������ ��� �������� ������� �����
        Vector3 centroid = Vector3.zero;
        foreach (var point in stoneLinePoints)
        {
            centroid += point;
        }
        centroid /= stoneLinePoints.Count;

        // ��������� ����� ����� � ������ ������� ����� ������
        stoneLinePositions.Add(centroid);

        for (int i = 0; i < stoneLinePoints.Count; i++)
        {
            Vector3 position = stoneLinePoints[i];
            Vector3 randomScale = stoneScales[i];

            float randomRotationY = Random.Range(stoneMinRotationY, stoneMaxRotationY);

            // �������� ��������� ������ ����� �� �������
            GameObject selectedStonePrefab = stonePrefabs[Random.Range(0, stonePrefabs.Length)];

            // ������������ ������
            GameObject stone = Instantiate(selectedStonePrefab, position, Quaternion.Euler(0, randomRotationY, 0), stonesParent.transform);
            stone.transform.localScale = randomScale;

            stonePositions.Add(position);
        }
    }


    private bool IsPositionNearStoneLine(Vector3 position, float radius)
    {
        foreach (var linePos in stoneLinePositions)
        {
            if (Vector3.Distance(position, linePos) < radius)
            {
                return true;
            }
        }
        return false;
    }



    private bool IsPositionNearGold(Vector3 position, float radius)
    {
        foreach (var goldPos in goldPositions)
        {
            if (Vector3.Distance(position, goldPos) < radius)
            {
                return true;
            }
        }
        return false;
    }


    private bool IsPositionNearStone(Vector3 position, float radius)
    {
        foreach (var stonePos in stonePositions)
        {
            if (Vector3.Distance(position, stonePos) < radius)
            {
                return true;
            }
        }
        return false;
    }


    private bool IsPositionNearPath(Vector3 position, float distance)
    {
        Vector2 posXZ = new Vector2(position.x, position.z);
        foreach (var path in allPaths)
        {
            foreach (var pathPoint in path)
            {
                Vector2 pathPointXZ = new Vector2(pathPoint.x, pathPoint.z);
                if (Vector2.Distance(posXZ, pathPointXZ) < distance + pathWidth / 2f)
                {
                    return true;
                }
            }
        }
        return false;
    }


    private class SandClearing
    {
        public Vector3 center;
        public List<Vector2> vertices;
        public float baseRadius;
    }

    private class StonePatch
    {
        public List<Vector2> vertices;
        public int textureIndex;
    }

    private class LeafPatch
    {
        public List<Vector2> vertices;
        public int textureIndex;
    }
}
