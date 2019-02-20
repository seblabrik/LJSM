using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class RoomGenerator : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int width = 11;//doit toujours être impaire
    public int height = 11;//doit toujours être impaire

    private bool entryNorth = true;
    private bool entrySouth = false;
    private bool entryEast = true;
    private bool entryWest = true;

    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);

    public Tilemap ground;
    public Tilemap wall;
    public Tilemap outerWall;
    public GameObject player;

    public NavMeshSurface2d surface2d;

    public Tile[] groundTiles;
    public Tile[] wallTiles;
    public Tile[] outerWallTiles;
    public GameObject[] enemyTiles;

    private Transform roomHolder;
    private List<Vector3Int> gridPositions = new List<Vector3Int>();

    public Vector3 playerSpawnPosition = new Vector3(0, 0, 0);


    void Start()
    {
        SetupRoom();
        surface2d.BuildNavMesh();
    }

    void InitialiseList()
    {
        gridPositions.Clear();

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                gridPositions.Add(new Vector3Int(x, y, 0));
            }
        }
        Vector3Int TileplayerSpawnPosition = new Vector3Int((int) playerSpawnPosition.x, (int) playerSpawnPosition.y, 0);
        gridPositions.Remove(TileplayerSpawnPosition);
    }

    void RoomSetup()
    {
        roomHolder = new GameObject("Room").transform;

        for (int x = -1; x < width + 1; x++)
        {
            for (int y = -1; y < height + 1; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (isOuterWall(x, y))
                {
                    Tile tile = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                    outerWall.SetTile(pos, tile);
                }
                else
                {
                    Tile tile = groundTiles[Random.Range(0, groundTiles.Length)];
                    ground.SetTile(pos, tile);
                }
            }
        }
    }

    private bool isOuterWall(int x, int y)
    {
        if ((x == -1) && !((y == (height - 1) / 2) && entryWest)) { return true; }
        if ((x == width) && !((y == (height - 1) / 2) && entryEast)) { return true; }
        if ((y == -1) && !((x == (width - 1) / 2) && entrySouth)) { return true; }
        if ((y == height) && !((x == (width - 1) / 2) && entryNorth)) { return true; }
        return false;
    }

    private Vector3Int RandomPositionAvailable()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3Int randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    private void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPositionAvailable();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    private void LayoutTileAtRandom(Tile[] tileArray, int minimum, int maximum, Tilemap tilemap)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3Int randomPosition = RandomPositionAvailable();
            Tile tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            tilemap.SetTile(randomPosition, tileChoice);
        }
    }

    public void SetupRoom()
    {
        RoomSetup();
        InitialiseList();
        LayoutTileAtRandom(wallTiles, wallCount.minimum, wallCount.maximum, wall);
        LayoutObjectAtRandom(enemyTiles, 1, 2);//entre 1 et 2 enemies, provisoirement
        Instantiate(player, playerSpawnPosition, Quaternion.identity);
    }
}
