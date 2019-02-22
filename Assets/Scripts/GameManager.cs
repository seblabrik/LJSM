using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using LJSM.Models;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public NavMeshSurface2d surfacePrefab;
    private NavMeshSurface2d surface;
    private RoomGenerator roomGenerator;
    private LevelGenerator levelGenerator;

    public Dictionary<RoomIndex, RoomParam> levelRooms;
    public RoomParam currentRoom;
    public RoomIndex currentRoomIndex;

    public string playerSpawn = "Start";

    public int width = 11;//doit toujours être impaire
    public int height = 11;//doit toujours être impaire

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        roomGenerator = GetComponent<RoomGenerator>();

        levelGenerator = GetComponent<LevelGenerator>();

        InitGame();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.InitRoom();
    }


    void InitGame()
    {
        surface = Instantiate(surfacePrefab);
        levelRooms = levelGenerator.GenerateLevel();
        currentRoomIndex = new RoomIndex { abs = 0, ord = 0 };
        currentRoom = levelRooms[currentRoomIndex];
        roomGenerator.SetupRoom(currentRoom, playerSpawn);
        surface.BuildNavMesh();
    }

    void InitRoom()
    {
        surface = Instantiate(surfacePrefab);
        currentRoom = GetNextRoom();
        roomGenerator.SetupRoom(currentRoom, playerSpawn);
        surface.BuildNavMesh();
    }

    public void GameOver()
    {
        enabled = false;
    }

    public RoomParam GetNextRoom()
    {
        if (playerSpawn == "North") { currentRoomIndex.ord -= 1; }
        if (playerSpawn == "South") { currentRoomIndex.ord += 1; }
        if (playerSpawn == "East") { currentRoomIndex.abs -= 1; }
        if (playerSpawn == "West") { currentRoomIndex.abs += 1; }
        return levelRooms[currentRoomIndex];
    }
}