using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using LJSM.Models;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public NavMeshSurface2d surfacePrefab;
    private NavMeshSurface2d surface;
    private RoomGenerator roomGenerator;
    private LevelGenerator levelGenerator;
    private MapController mapController;
    private GameObject map;

    public Dictionary<RoomIndex, RoomParam> levelRooms;
    public RoomParam currentRoom;
    public RoomIndex currentRoomIndex;

    public bool pauseGame = false;

    public string playerSpawn = "Start";

    public int width = 11;//doit toujours être impaire
    public int height = 11;//doit toujours être impaire

    private List<List<RoomIndex>> connectedRooms;

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

        mapController = GetComponent<MapController>();

        InitGame();
    }

    void Update()
    {
        if (Input.GetButtonDown("Map"))
        {
            map.SetActive(!map.activeSelf);
        }

        if (Input.GetButtonDown("Pause"))
        {
            if (pauseGame) { Time.timeScale = 1; }
            else { Time.timeScale = 0; }
            pauseGame = !pauseGame;
        }

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
        (levelRooms, connectedRooms) = levelGenerator.GenerateLevel();
        currentRoomIndex = new RoomIndex { abs = 0, ord = 0 };
        currentRoom = levelRooms[currentRoomIndex];
        map = mapController.CreateMap(levelRooms, connectedRooms);
        map.SetActive(false);
        roomGenerator.SetupRoom(currentRoom, playerSpawn);
        surface.BuildNavMesh();
    }

    void InitRoom()
    {
        surface = Instantiate(surfacePrefab);
        currentRoom = GetNextRoom();
        mapController.UpdateMap(currentRoom);
        map.SetActive(false);
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