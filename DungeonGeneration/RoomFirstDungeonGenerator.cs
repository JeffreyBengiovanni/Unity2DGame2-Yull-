using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [Header("Room Settings")]
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0,10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;
    [SerializeField]
    private bool bigCorridors = true;
    [SerializeField]
    private bool veryBigCorridors = true;
    [Header("Light Settings")]
    [SerializeField]
    private int minStepsBeforeLight = 6;
    [SerializeField]
    private int currentStepForLight = 0;
    [SerializeField]
    private int lightOverlapDistance = 3;
    [Header("Enemy Settings")]
    [SerializeField]
    private int minEnemyPerRoom = 3;
    [SerializeField]
    private int maxEnemyPerRoom = 6;
    [SerializeField]
    private int maxExtraEnemiesInHallway = 4;
    [SerializeField]
    private int minStepsBeforeEnemy = 6;
    [SerializeField]
    private int maxStepsBeforeEnemy = 12;
    [SerializeField]
    private int instanceStepsBeforeEnemy = 6;
    [SerializeField]
    private int currentStepForEnemy = 0;
    [SerializeField]
    private float minStepsFromPlayerAndBoss = 10;
    [SerializeField]
    private float chanceForBreakable = 10f;
    [SerializeField]
    private float distanceFromPathForBreakable = 2.5f;
    [SerializeField]
    private int minBreakablePerRoom = 5;
    [SerializeField]
    private int maxBreakablePerRoom = 8;
    [SerializeField]
    private float chanceNotToBeBreakable = 95f;
    [Header("Other")]
    [SerializeField]
    private Vector3Int spawnLocation;
    [SerializeField]
    private Vector3Int bossLocation;
    [SerializeField]
    public bool doneLoading = true;

    public void GenerateDungeonRoomFirst()
    {
        doneLoading = false;
        tilemapVisualizer.Clear();
        RunProceduralGeneration();
        UpdatePlayerInfo();
    }

    private void UpdatePlayerInfo()
    {
        GameManager.instance.player.playerInfo.ResetHealth();
        GameManager.instance.player.playerHealthBar.UpdateHealthbar();
    }

    protected override void RunProceduralGeneration()
    {
        StartCoroutine(LoadingMap());
        CreateRooms();

        // Invoke this after loading is done, for now just delay 1 sec
        Invoke("ScanGrid", .4f);
    }

    private IEnumerator LoadingMap()
    {
        GameManager.instance.ToggleLoadingScreen(true);
        while(!doneLoading)
        {
            yield return null;
        }
        GameManager.instance.ToggleLoadingScreen(false);
        Debug.Log("Loading finished");
    }

    private void ScanGrid()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.pathfinder.Scan();
        }
        doneLoading = true;
    }

    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition,
                                        new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);
        HashSet<Vector2Int> enemySpots = new HashSet<Vector2Int>();
        List<Vector2Int> startAndEndSpots = new List<Vector2Int>();
        HashSet<Vector2Int> breakableSpots = new HashSet<Vector2Int>();

        HashSet<Vector2Int> floor;
        
        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList, enemySpots, breakableSpots);
        }
        else
        {
            floor = CreateSimpleRooms(roomsList, enemySpots, breakableSpots);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        HashSet<Vector2Int> lightSpots = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }
        lightSpots.UnionWith(roomCenters);
        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters, lightSpots, enemySpots, startAndEndSpots, breakableSpots);
        floor.UnionWith(corridors);

        TrimEnemySpots(enemySpots);
        tilemapVisualizer.PaintBaseFloor(dungeonWidth, dungeonHeight);
        tilemapVisualizer.PaintFloorTiles(floor);
        tilemapVisualizer.PaintLightSourceTiles(lightSpots);
        tilemapVisualizer.PaintStartAndEndSpots(startAndEndSpots);
        tilemapVisualizer.CreateEnemies(enemySpots);
        tilemapVisualizer.CreateBreakables(breakableSpots, chanceNotToBeBreakable);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

    }

    private void TrimEnemySpots(HashSet<Vector2Int> enemySpots)
    {
        HashSet<Vector2Int> toRemove = new HashSet<Vector2Int>();
        foreach (var spot in enemySpots)
        {
            if (Vector3Int.Distance((Vector3Int)spot, spawnLocation) < minStepsFromPlayerAndBoss ||
                Vector3Int.Distance((Vector3Int)spot, bossLocation) < minStepsFromPlayerAndBoss)
            {
                toRemove.Add(spot);
            }
        }
        foreach (var spot in toRemove)
        {
            enemySpots.Remove(spot);
        }
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList, HashSet<Vector2Int> enemySpots, HashSet<Vector2Int> breakableSpots)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            HashSet<Vector2Int> tempRoomFloor = new HashSet<Vector2Int>();
            // Preventing spawning in boss room and player spawn

            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) &&
                    position.y >= (roomBounds.yMin + offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                    tempRoomFloor.Add(position);
                }
            }
            if (i != 0 && i != roomsList.Count - 1)
            {
                GenerateEnemySpots(tempRoomFloor, enemySpots);
                GenerateBreakableSpots(tempRoomFloor, breakableSpots, enemySpots, roomCenter);
            }
        }
        return floor;
    }

    private void GenerateBreakableSpots(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> breakableSpots, 
                                        HashSet<Vector2Int> enemySpots, Vector2Int roomCenter)
    {
        // Adding one since max ints are exclusive for Range
        int amountToAdd = Random.Range(minBreakablePerRoom, maxBreakablePerRoom + 1);
        List<Vector2Int> asList = roomFloor.ToList();
        int stuckPrevention = 0;
        for (int currentCount = 0; currentCount < amountToAdd; stuckPrevention++)
        {
            // Prevents a long runtime or infinite loop depending on room settings
            if (stuckPrevention >= roomFloor.Count)
            {
                break;
            }
            Vector2Int temp = asList[Random.Range(0, asList.Count)];
            if (!breakableSpots.Contains(temp) && !enemySpots.Contains(temp) && 
                Vector2Int.Distance(roomCenter, temp) > distanceFromPathForBreakable)
            {
                breakableSpots.Add(temp);
                currentCount++;
            }
            else
            {
                continue;
            }
        }
    }

    private void GenerateEnemySpots(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> enemySpots)
    {
        // Adding one since max ints are exclusive for Range
        int amountToAdd = Random.Range(minEnemyPerRoom, maxEnemyPerRoom + 1);
        List<Vector2Int> asList = roomFloor.ToList();
        int stuckPrevention = 0;
        for (int currentCount = 0; currentCount < amountToAdd; stuckPrevention++)
        {
            // Prevents a long runtime or infinite loop depending on room settings
            if (stuckPrevention >= roomFloor.Count)
            {
                break;
            }
            Vector2Int temp = asList[Random.Range(0, asList.Count)];
            if (!enemySpots.Contains(temp))
            {
                enemySpots.Add(temp);
                currentCount++;
            }
            else
            {
                continue;
            }
        }
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters, HashSet<Vector2Int> lightSpots, 
                                                HashSet<Vector2Int> enemySpots, List<Vector2Int> startAndEndSpots, 
                                                HashSet<Vector2Int> breakableSpots)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[UnityEngine.Random.Range(0, roomCenters.Count)];

        // Make this room be the spawn room
        if (GameManager.instance != null)
        {
            spawnLocation = (Vector3Int)currentRoomCenter;
            startAndEndSpots.Add(currentRoomCenter);
            GameManager.instance.player.transform.position = spawnLocation;
            GameManager.instance.cameraMovement.transform.position = new Vector3(spawnLocation.x, spawnLocation.y, -20);
        }
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest, bigCorridors, veryBigCorridors, 
                                                            lightSpots, enemySpots, breakableSpots);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        bossLocation = (Vector3Int)currentRoomCenter;
        startAndEndSpots.Add(currentRoomCenter);

        return corridors;
    }

    // Creates the corridors and has an option to make the corridors twice as wide, also notes light positions
    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination, bool bigCorr,
                                                bool veryBigCorr, HashSet<Vector2Int> lightSpots, HashSet<Vector2Int> enemySpots, HashSet<Vector2Int> breakableSpots)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        var doubleLength1 = position;
        var doubleLength2 = position;
        var tripleLength1 = position;
        var tripleLength2 = position;
        corridor.Add(position);

        instanceStepsBeforeEnemy = Random.Range(minStepsBeforeEnemy, maxStepsBeforeEnemy + 1);

        float z;

        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
                doubleLength1 = position + Vector2Int.right;
                doubleLength2 = position + Vector2Int.left ;
                tripleLength1 = doubleLength1 + Vector2Int.right;
                tripleLength2 = doubleLength2 + Vector2Int.left;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
                doubleLength1 = position + Vector2Int.right;
                doubleLength2 = position + Vector2Int.left;
                tripleLength1 = doubleLength1 + Vector2Int.right;
                tripleLength2 = doubleLength2 + Vector2Int.left;
            }
            corridor.Add(position);
            currentStepForLight++;
            currentStepForEnemy++;
            if (bigCorr)
            {
                corridor.Add(doubleLength1);
                corridor.Add(doubleLength2);
            }
            if (veryBigCorr)
            {
                corridor.Add(tripleLength1);
                corridor.Add(tripleLength2);
                if((z = Random.Range(0f, 100f)) <= chanceForBreakable)
                {
                    if (z % 2 == 0)
                    {
                        breakableSpots.Add(tripleLength1);
                    }
                    else
                    {
                        breakableSpots.Add(tripleLength2);
                    }
                }
            }
            if ( currentStepForLight >= minStepsBeforeLight)
            {
                if (NoLightsNearby(position, lightSpots))
                {
                    lightSpots.Add(position);
                    currentStepForLight = 0;
                }
            }
            if (currentStepForEnemy >= instanceStepsBeforeEnemy)
            {
                enemySpots.Add(position);
                
                //Adds more enemies
                if (veryBigCorr)
                {
                    int amount = Random.Range(0, maxExtraEnemiesInHallway + 1);

                    for (int i = 0; i < amount; i++)
                    {
                        Vector2Int s = position + new Vector2Int(Random.Range(0, 3), Random.Range(0, 3));
                        if (!breakableSpots.Contains(s))
                        {
                            enemySpots.Add(s);
                        }
                    }
                }
                currentStepForEnemy = 0;
                instanceStepsBeforeEnemy = Random.Range(minStepsBeforeEnemy, maxStepsBeforeEnemy + 1);
            }
        }

        // In order to make square corners, add above and below the last position
        if (bigCorr)
        {
            corridor.Add(position + Vector2Int.up);
            corridor.Add(position + Vector2Int.up + Vector2Int.right);
            corridor.Add(position + Vector2Int.up + Vector2Int.left);
            corridor.Add(position + Vector2Int.down);
            corridor.Add(position + Vector2Int.down + Vector2Int.right);
            corridor.Add(position + Vector2Int.down + Vector2Int.left);
        }
        if (veryBigCorr)
        {
            corridor.Add(position + Vector2Int.up + Vector2Int.right + Vector2Int.right);
            corridor.Add(position + Vector2Int.up + Vector2Int.left + Vector2Int.left);
            corridor.Add(position + Vector2Int.down + Vector2Int.right + Vector2Int.right);
            corridor.Add(position + Vector2Int.down + Vector2Int.left + Vector2Int.left);

            corridor.Add(position + Vector2Int.up + Vector2Int.up);
            corridor.Add(position + Vector2Int.up + Vector2Int.up + Vector2Int.right);
            corridor.Add(position + Vector2Int.up  + Vector2Int.up + Vector2Int.left);
            corridor.Add(position + Vector2Int.up + Vector2Int.up + Vector2Int.right + Vector2Int.right);
            corridor.Add(position + Vector2Int.up + Vector2Int.up + Vector2Int.left + Vector2Int.left);
            corridor.Add(position + Vector2Int.down + Vector2Int.down);
            corridor.Add(position + Vector2Int.down + Vector2Int.down + Vector2Int.right);
            corridor.Add(position + Vector2Int.down + Vector2Int.down +Vector2Int.left);
            corridor.Add(position + Vector2Int.down + Vector2Int.down + Vector2Int.right + Vector2Int.right);
            corridor.Add(position + Vector2Int.down + Vector2Int.down + Vector2Int.left + Vector2Int.left);
        }


        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
                doubleLength1 = position + Vector2Int.up;
                doubleLength2 = position + Vector2Int.down;
                tripleLength1 = doubleLength1 + Vector2Int.up;
                tripleLength2 = doubleLength2 + Vector2Int.down;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
                doubleLength1 = position + Vector2Int.up;
                doubleLength2 = position + Vector2Int.down;
                tripleLength1 = doubleLength1 + Vector2Int.up;
                tripleLength2 = doubleLength2 + Vector2Int.down;
            }
            corridor.Add(position);
            currentStepForLight++;
            currentStepForEnemy++;
            if (bigCorr)
            {
                corridor.Add(doubleLength1);
                corridor.Add(doubleLength2);
            }
            if (veryBigCorr)
            {
                corridor.Add(tripleLength1);
                corridor.Add(tripleLength2);
                if ((z = Random.Range(0f, 100f)) <= chanceForBreakable)
                {
                    if (z % 2 == 0)
                    {
                        breakableSpots.Add(tripleLength1);
                    }
                    else
                    {
                        breakableSpots.Add(tripleLength2);
                    }
                }
            }
            if (currentStepForLight >= minStepsBeforeLight)
            {
                if (NoLightsNearby(position, lightSpots))
                {
                    lightSpots.Add(position);
                    currentStepForLight = 0;
                }
            }
            if (currentStepForEnemy >= instanceStepsBeforeEnemy)
            {
                enemySpots.Add(position);
                //Adds more enemies
                if (veryBigCorr)
                {
                    int amount = Random.Range(0, maxExtraEnemiesInHallway + 1);

                    for (int i = 0; i < amount; i++)
                    {
                        Vector2Int s = position + new Vector2Int(Random.Range(0, 3), Random.Range(0, 3));
                        if (!breakableSpots.Contains(s))
                        {
                            enemySpots.Add(s);
                        }
                    }
                }
                currentStepForEnemy = 0;
                instanceStepsBeforeEnemy = Random.Range(minStepsBeforeEnemy, maxStepsBeforeEnemy + 1);
            }
        }
        return corridor;
    }

    // To avoid lights overlapping one another
    private bool NoLightsNearby(Vector2Int position, HashSet<Vector2Int> lightSpots)
    {
        foreach (var location in lightSpots)
        {
            if (Mathf.Abs(Vector2Int.Distance(position, location)) < lightOverlapDistance)
            {
                return false;
            }
        }
        return true;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList, HashSet<Vector2Int> enemySpots, HashSet<Vector2Int> breakableSpots)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>();
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    roomFloor.Add(position);
                    floor.Add(position);
                }
            }
            // Preventing spawning in boss room and player spawn
            if (room != roomsList[0] && room != roomsList[roomsList.Count - 1])
            {
                GenerateEnemySpots(roomFloor, enemySpots);
            }
        }
        return floor;
    }

}
