using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using LJSM.Models;

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

    private int width;
    private int height;

    private Tilemap ground;
    private Tilemap wall;
    private Tilemap outerWall;

    public GameObject player;

    public GameObject exit;

    private Transform roomObjects;
    private Transform roomUnits;

    void BoardSetup(List<TileParam> groundTilesParam, List<TileParam> outerWallTilesParam)
    {

        foreach(TileParam param in groundTilesParam)
        {
            ground.SetTile(param.position, param.tileChoice);
        }

        foreach (TileParam param in outerWallTilesParam)
        {
            outerWall.SetTile(param.position, param.tileChoice);
        }

        GameObject instance = Instantiate(exit, new Vector3((width - 1) / 2 + 1f, height + 1.5f, 0), Quaternion.identity);//North
        instance.transform.SetParent(roomObjects);
        instance = Instantiate(exit, new Vector3((width - 1) / 2 + 1f, -1.5f, 0), Quaternion.identity);//South
        instance.transform.SetParent(roomObjects);
        instance = Instantiate(exit, new Vector3(width + 1.5f, (height - 1) / 2 + 1f, 0), Quaternion.identity);//East
        instance.transform.SetParent(roomObjects);
        instance = Instantiate(exit, new Vector3(-1.5f, (height - 1) / 2 + 1f, 0), Quaternion.identity);//West
        instance.transform.SetParent(roomObjects);
    }

    private void LayoutObjects(List<ObjectParam> objectsParam)
    {
        foreach (ObjectParam param in objectsParam)
        {
            GameObject instance = Instantiate(param.tileChoice, param.position, Quaternion.identity);
            instance.transform.SetParent(roomUnits);
        }
    }

    private void LayoutTiles(List<TileParam> wallTilesParam, Tilemap tilemap)
    {
        foreach (TileParam param in wallTilesParam)
        {
            tilemap.SetTile(param.position, param.tileChoice);
        }
    }

    public void SetupRoom(RoomParam param, string playerSpawn)
    {
        roomObjects = new GameObject("RoomObjects").transform;
        roomUnits = new GameObject("RoomUnits").transform;
        roomUnits.SetParent(roomObjects);

        ground = GameObject.Find("Ground").GetComponent<Tilemap>();
        wall = GameObject.Find("Wall").GetComponent<Tilemap>();
        outerWall = GameObject.Find("OuterWall").GetComponent<Tilemap>();

        width = GameManager.instance.width;
        height = GameManager.instance.height;

        Vector3 playerSpawnPosition = getPositionPlayer(playerSpawn);

        BoardSetup(param.groundTilesParam, param.outerWallTilesParam);
        LayoutTiles(param.wallTilesParam, wall);
        LayoutObjects(param.objectsParam);
        GameObject instance = Instantiate(player, playerSpawnPosition, Quaternion.identity);
        instance.transform.SetParent(roomUnits);
    }

    private Vector3 getPositionPlayer(string playerSpawn)
    {
        Vector3 pos = new Vector3();
        if (playerSpawn == "North") { pos = new Vector3((width - 1) / 2 + 1f, height, 0f); }
        else if (playerSpawn == "South") { pos = new Vector3((width - 1) / 2 + 1f, 0f, 0f); }
        else if (playerSpawn == "East") { pos = new Vector3(width - 0.5f, (height - 1) / 2 + 1f, 0f); }
        else if (playerSpawn == "West") { pos = new Vector3(0.5f, (height - 1) / 2 + 1f, 0f); }
        else if (playerSpawn == "Start") { pos = new Vector3(0.25f, 0.25f, 0f); }
        return pos;
    }
}