using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public int numberOfWalls;

    public Tilemap wall;
    public GameObject player;
    public GameObject Enemy1;
    public Tile tile;

    public NavMeshSurface2d surface2d;  


    void Start()
    {
        GenerateLevel();
        surface2d.BuildNavMesh();
    }

    // Create a grid based level
    void GenerateLevel()
    {
        bool playerSpawned = false;
        bool Enemy1Spawned = false;
        int numberOfWallsPlaced = 0;

        // Loop over the grid
        for (int x = 0; x <= width; x += 2)
        {
            for (int y = 0; y <= height; y += 2)
            {
                // Should we place a wall?
                if (UnityEngine.Random.value > .6f && numberOfWallsPlaced< numberOfWalls)
                {
                    // Spawn a wall
                    Vector3Int pos = new Vector3Int((int)(x - width / 2f), (int)(y - height / 2f), 0);
                    wall.SetTile(pos, tile);
                    numberOfWallsPlaced++;
                }
                else if (!playerSpawned) // Should we spawn a player?
                {
                    // Spawn the player
                    Vector3 pos = new Vector3(x - width / 2f, y - height / 2f, 0f);
                    Instantiate(player, pos, Quaternion.identity);
                    playerSpawned = true;
                }
                else if (!Enemy1Spawned) // Should we spawn an Enemy1?
                {
                    // Spawn the Enemy1
                    Vector3 pos = new Vector3(x - width / 2f, y - height / 2f, 0f);
                    Instantiate(Enemy1, pos, Quaternion.identity);
                    Enemy1Spawned = true;
                }
            }
        }
    }
}
