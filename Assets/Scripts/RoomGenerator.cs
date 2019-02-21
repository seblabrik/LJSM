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

    private List<bool> hasExit;// [Nord, Sud, Est, Ouest]

    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    
    private Tilemap ground;
    private Tilemap wall;
    private Tilemap outerWall;

    public GameObject player;

    public GameObject exit;

    public Tile[] groundTiles;
    public Tile[] wallTiles;
    public Tile[] outerWallTiles;
    public GameObject[] enemyTiles;

    private Transform roomObjects;
    private List<Vector3Int> gridPositions = new List<Vector3Int>();

    public Vector3 playerSpawnPosition;

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
        roomObjects = new GameObject("RoomObjects").transform;

        ground = GameObject.Find("Ground").GetComponent<Tilemap>();
        wall = GameObject.Find("Wall").GetComponent<Tilemap>();
        outerWall = GameObject.Find("OuterWall").GetComponent<Tilemap>();

        for (int x = -1; x < width + 1; x++)
        {
            for (int y = -1; y < height + 1; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                //if (isExit(x, y))
                //{
                //    Tile tile = groundTiles[Random.Range(0, groundTiles.Length)];
                //    ground.SetTile(pos, tile);
                //    GameObject instance = Instantiate(exit, pos + new Vector3(0.5f,0.5f, 0), Quaternion.identity);
                //    instance.transform.SetParent(roomObjects);
                //}
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

        GameObject instance = Instantiate(exit, new Vector3(width/2 + 0.5f, height + 1.5f, 0), Quaternion.identity);//North
        instance.transform.SetParent(roomObjects);
        instance = Instantiate(exit, new Vector3(width/2 + 0.5f, -1.5f, 0), Quaternion.identity);//South
        instance.transform.SetParent(roomObjects);
        instance = Instantiate(exit, new Vector3(width + 1.5f, height/2 + 0.5f, 0), Quaternion.identity);//East
        instance.transform.SetParent(roomObjects);
        instance = Instantiate(exit, new Vector3(-1.5f, height/2 + 0.5f, 0), Quaternion.identity);//West
        instance.transform.SetParent(roomObjects);
    }

    private bool isOuterWall(int x, int y)
    {
        if ((x == -1) && !((y == (height - 1) / 2) && hasExit[3])) { return true; }
        if ((x == width) && !((y == (height - 1) / 2) && hasExit[2])) { return true; }
        if ((y == -1) && !((x == (width - 1) / 2) && hasExit[1])) { return true; }
        if ((y == height) && !((x == (width - 1) / 2) && hasExit[0])) { return true; }
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
            GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
            instance.transform.SetParent(roomObjects);
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

    public void SetupRoom(RoomParameters param)
    {
        hasExit = param.hasExit;
        playerSpawnPosition = getPositionPlayer(param.playerSpawn);

        RoomSetup();
        InitialiseList();
        LayoutTileAtRandom(wallTiles, wallCount.minimum, wallCount.maximum, wall);
        LayoutObjectAtRandom(enemyTiles, 1, 2);//entre 1 et 2 enemies, provisoirement
        GameObject instance = Instantiate(player, playerSpawnPosition, Quaternion.identity);
        instance.transform.SetParent(roomObjects);
    }

    private Vector3 getPositionPlayer(string playerSpawn)
    {
        Vector3 pos = new Vector3();
        if (playerSpawn == "North") { pos = new Vector3(width/2 + 0.5f, height, 0f); }
        else if (playerSpawn == "South") { pos = new Vector3(width/2 + 0.5f, 0f, 0f); }
        else if (playerSpawn == "East") { pos = new Vector3(width - 0.5f, height/2 +0.5f, 0f); }
        else if (playerSpawn == "West") { pos = new Vector3(0.5f, height/2 +0.5f, 0f); }
        else if (playerSpawn == "Start") { pos = new Vector3(0.25f, 0.25f, 0f); }
        return pos;
    }
}
