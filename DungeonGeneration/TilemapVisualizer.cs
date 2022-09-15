using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;

    [SerializeField]
    private Transform sceneComponentsHolder;

    [SerializeField]
    private Transform sceneLightPrefab;

    [SerializeField]
    private Transform warpZonePrefab;

    [SerializeField]
    private Transform activeEnemies;

    [SerializeField]
    private List<TileBase> backDropTiles, floorTiles, wallTops, wallSideRights, wallSideLefts, wallBottoms, wallFulls,
        wallInnerCornerDownLefts, wallInnerCornerDownRights,
        wallDiagonalCornerDownRights, wallDiagonalCornerDownLefts, wallDiagonalCornerUpRights, wallDiagonalCornerUpLefts,
        lightTiles, startTiles, endTiles;

    public void Awake()
    {
        ClearLights();
        ClearEnemies();
    }

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTiles);
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if(WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = wallTops[Random.Range(0, wallTops.Count)];
        }
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = wallSideRights[Random.Range(0,wallSideRights.Count)];
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = wallSideLefts[Random.Range(0, wallSideLefts.Count)];
        }
        else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
        {
            tile = wallBottoms[Random.Range(0, wallBottoms.Count)];
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFulls[Random.Range(0, wallFulls.Count)];
        }

        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
        }
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, List<TileBase> tiles)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tiles[Random.Range(0,tiles.Count)], position);
        }
    }

    internal void PaintLightSourceTiles(HashSet<Vector2Int> lightSpots)
    {
        PaintTiles(lightSpots, floorTilemap, lightTiles);
        CreateLights(lightSpots);
    }

    private void CreateLights(HashSet<Vector2Int> positions)
    {
        Vector3 adjust = new Vector3(0.5f, 0.5f, 0);
        foreach (var position in positions)
        {            
            Instantiate(sceneLightPrefab, (Vector3Int)position + adjust, Quaternion.identity, sceneComponentsHolder);
        }
    }

    internal void CreateEnemies(HashSet<Vector2Int> positions)
    {
        if(GameManager.instance != null)
        {
            Vector3 adjust = new Vector3(0.5f, 0.5f, 0);
            foreach (var position in positions)
            {
                float roll = Random.Range(0, 100f);
                if (roll < 85)
                {
                    Instantiate(GameManager.instance.basicEnemyPrefabs[Random.Range(0,GameManager.instance.basicEnemyPrefabs.Count)], 
                                (Vector3Int)position + adjust, Quaternion.identity, activeEnemies);
                }
                else
                {
                    Instantiate(GameManager.instance.advancedEnemyPrefabs[Random.Range(0, GameManager.instance.advancedEnemyPrefabs.Count)],
                                                    (Vector3Int)position + adjust, Quaternion.identity, activeEnemies);
                }
            }
        }
    }

    internal void PaintStartAndEndSpots(List<Vector2Int> startAndEndSpots)
    {
        PaintSingleTile(floorTilemap, startTiles[Random.Range(0, startTiles.Count)], startAndEndSpots[0]);
        PaintSingleTile(floorTilemap, endTiles[Random.Range(0, endTiles.Count)], startAndEndSpots[1]);
        CreateWarpZone(startAndEndSpots);
    }

    internal void CreateWarpZone(List<Vector2Int> startAndEndSpots)
    {
        Vector3 adjust = new Vector3(0.5f, 0.5f, 0);
        Instantiate(warpZonePrefab, (Vector3Int)startAndEndSpots[1] + adjust, Quaternion.identity, sceneComponentsHolder);
    }

    internal void CreateBreakables(HashSet<Vector2Int> positions, float chanceToBeNormalBreakable)
    {
        if (GameManager.instance != null)
        {
            foreach (var position in positions)
            {
                Vector3 adjust = new Vector3(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), 0);
                float roll = Random.Range(0, 100f);
                if (roll < chanceToBeNormalBreakable)
                {
                    Instantiate(GameManager.instance.breakablePrefabs[Random.Range(0, GameManager.instance.breakablePrefabs.Count)],
                                                    (Vector3Int)position + adjust, Quaternion.identity, activeEnemies);
                }
                else
                {
                    Instantiate(GameManager.instance.chestPrefabs[Random.Range(0, GameManager.instance.chestPrefabs.Count)],
                                                    (Vector3Int)position + adjust, Quaternion.identity, activeEnemies);
                }
                
            }
        }
    }

    internal void PaintBaseFloor(int dungeonWidth, int dungeonHeight)
    {
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                var tilePosition = new Vector2Int(x, y);
                PaintSingleTile(floorTilemap, backDropTiles[UnityEngine.Random.Range(0, backDropTiles.Count)], tilePosition);
            }

        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        ClearLights();
        ClearEnemies();
    }

    private void ClearLights()
    {
        if(GameManager.instance != null)
        {
            foreach (Transform child in sceneComponentsHolder)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void ClearEnemies()
    {
        if (GameManager.instance != null)
        {
            foreach (Transform child in activeEnemies)
            {
                Destroy(child.gameObject);
            }
        }
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownLefts[Random.Range(0, wallInnerCornerDownLefts.Count)];
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownRights[Random.Range(0, wallInnerCornerDownRights.Count)];
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownLefts[Random.Range(0, wallDiagonalCornerDownLefts.Count)];
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownRights[Random.Range(0, wallDiagonalCornerDownRights.Count)];
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpRights[Random.Range(0, wallDiagonalCornerUpRights.Count)];
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpLefts[Random.Range(0, wallDiagonalCornerUpLefts.Count)];
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = wallFulls[Random.Range(0, wallFulls.Count)];
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
        {
            tile = wallBottoms[Random.Range(0, wallBottoms.Count)];
        }


        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
        }
    }
}
