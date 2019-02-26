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

    public Dictionary<RoomIndex, RoomParam> levelRooms;
    public RoomParam currentRoom;
    public RoomIndex currentRoomIndex;

    public string playerSpawn = "Start";

    public int width = 11;//doit toujours être impaire
    public int height = 11;//doit toujours être impaire

    private GameObject map;
    private GameObject mapPlayer;
    private float mapSizeParam = 30f;
    private List<List<RoomIndex>> connectedRooms;
    public GameObject mapGatePrefab;
    public GameObject mapRoomPrefab;
    public GameObject mapPlayerPrefab;

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

    void Update()
    {
        if (Input.GetButtonDown("Map"))
        {
            map.SetActive(!map.activeSelf);
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
        CreateMap();
        map.SetActive(false);
        roomGenerator.SetupRoom(currentRoom, playerSpawn);
        surface.BuildNavMesh();
    }

    void InitRoom()
    {
        surface = Instantiate(surfacePrefab);
        currentRoom = GetNextRoom();
        CreateMap();
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

    private void CreateMap()
    {
        map = GameObject.Find("Map");

        foreach (RoomParam room in levelRooms.Values)
        {
            float x = room.coordonates.abs * mapSizeParam + (width - 1) / 2;
            float y = room.coordonates.ord * mapSizeParam + (height - 1) / 2;
            GameObject roomMap = Instantiate(mapRoomPrefab, new Vector3(x, y, 0f), Quaternion.identity);
            roomMap.transform.SetParent(map.transform, false);
            
        }
        List<string> closedList = new List<string>();

        foreach (List<RoomIndex> connection in connectedRooms)
        {
            string id1 = connection[0].abs + "" + connection[0].ord;
            string id2 = connection[1].abs + "" + connection[1].ord;
            if (closedList.Contains(id1 + id2) || closedList.Contains(id2 + id1)) { continue; }
            closedList.Add(id1 + id2);
            closedList.Add(id2 + id1);
            float x = ((connection[0].abs + connection[1].abs) * mapSizeParam + (width - 1)) / 2;
            float y = ((connection[0].ord + connection[1].ord) * mapSizeParam + (height - 1)) / 2;
            GameObject gate = Instantiate(mapGatePrefab, new Vector3(x, y, 0f), Quaternion.identity);
            gate.transform.SetParent(map.transform, false);
        }
        float x_ = currentRoom.coordonates.abs * mapSizeParam + (width - 1) / 2;
        float y_ = currentRoom.coordonates.ord * mapSizeParam + (height - 1) / 2;
        mapPlayer = Instantiate(mapPlayerPrefab, new Vector3(x_, y_, 0f), Quaternion.identity);
        mapPlayer.transform.SetParent(map.transform, false);
        mapPlayer.transform.SetAsLastSibling();
    }
}