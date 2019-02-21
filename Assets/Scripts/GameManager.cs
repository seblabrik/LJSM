using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public NavMeshSurface2d surfacePrefab;
    private NavMeshSurface2d surface;
    private RoomGenerator roomGenerator;
    private RoomParameters param = new RoomParameters(true, true, true, true, "Start");
    public Vector3 enterPosition;

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

        InitGame();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.InitGame();
    }


    void InitGame()
    {
        surface = Instantiate(surfacePrefab);
        roomGenerator.SetupRoom(NextRoomParam());
        surface.BuildNavMesh();
    }

    public void GameOver()
    {
        enabled = false;
    }

    public RoomParameters NextRoomParam()
    {
        string playerSpawn = "Start";
        bool north = true;
        bool south = true;
        bool east = true;
        bool west = true;

        if (enterPosition != Vector3.zero)//valeur par default
        {
            if (enterPosition.y >= 3*roomGenerator.height / 4) { south = true; playerSpawn = "South"; }
            if (enterPosition.y <= roomGenerator.height / 4) { north = true; playerSpawn = "North"; }
            if (enterPosition.x >= 3*roomGenerator.width / 4) { west = true; playerSpawn = "West"; }
            if (enterPosition.x <= roomGenerator.width / 4) { east = true; playerSpawn = "East"; }
        }

        RoomParameters newParam = new RoomParameters(north, south, east, west, playerSpawn);

        return newParam;
    }
}

public class RoomParameters
{
    public List<bool> hasExit;// [Nord, Sud, Est, Ouest]

    public string playerSpawn;// 'North', 'South', 'East', 'West'

    public RoomParameters(bool north, bool south, bool east, bool west, string playerSpawn)
    {
        hasExit = new List<bool>();
        hasExit.Add(north);
        hasExit.Add(south);
        hasExit.Add(east);
        hasExit.Add(west);
        this.playerSpawn = playerSpawn;
    }

    public RoomParameters() { }
}
