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
    private FightManager fightManager;
    private StatManager statManager;
    private AnimatorManager animatorManager;

    public GameObject map;

    private GearManager gearManager;

    public Dictionary<RoomIndex, RoomParam> levelRooms;
    public RoomParam currentRoom;
    public RoomIndex currentRoomIndex;

    public bool fightMode = false;

    public bool pauseGame = false;

    public SpecificSpot playerSpawn = SpecificSpot.Start;

    public int width = 11;//doit toujours être impaire
    public int height = 11;//doit toujours être impaire

    private List<List<RoomIndex>> connectedRooms;

    public UnitParam playerParam;

    private Dictionary<string, bool> playerAnimatorParameters;

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

        fightManager = GetComponent<FightManager>();

        gearManager = GetComponent<GearManager>();

        statManager = GetComponent<StatManager>();

        animatorManager = GetComponent<AnimatorManager>();

        playerAnimatorParameters = new Dictionary<string, bool>();

        InitGame();
    }

    void Update()
    {
        if (Input.GetButtonDown("Map"))
        {
            if (!pauseGame)
            {
                if (!map.activeSelf) { Time.timeScale = 0; }
                else { Time.timeScale = 1; }
            }
            map.SetActive(!map.activeSelf);
        }

        if (Input.GetButtonDown("Pause") && !map.activeSelf)
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
        statManager.InitiateStats(levelRooms);
        gearManager.InitiateGears(levelRooms);
        animatorManager.InitiateAnimatorParams(levelRooms);
        currentRoomIndex = new RoomIndex { abs = 0, ord = 0 };
        currentRoom = levelRooms[currentRoomIndex];
        map = mapController.CreateMap(levelRooms, connectedRooms);
        map.SetActive(false);

        playerParam = new UnitParam
        {
            id = 0,
            unitNature = UnitNature.Player,
            stat = statManager.GenerateStat(UnitNature.Player),
            gear = new Gear { },
            animatorParam = new AnimatorParam { booleans = new Dictionary<string, bool>() }
        };

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
        Destroy(GameObject.Find("RoomObjects"));
    }

    public RoomParam GetNextRoom()
    {
        if (playerSpawn == SpecificSpot.North) { currentRoomIndex.ord -= 1; }
        if (playerSpawn == SpecificSpot.South) { currentRoomIndex.ord += 1; }
        if (playerSpawn == SpecificSpot.East) { currentRoomIndex.abs -= 1; }
        if (playerSpawn == SpecificSpot.West) { currentRoomIndex.abs += 1; }
        return levelRooms[currentRoomIndex];
    }

    public void EnterFightMode()
    {
        fightMode = true;
        fightManager.InitFight();
    }

    public void ExitFightMode()
    {
        fightMode = false;
        GameObject.FindWithTag("Player").transform.SendMessage("ExitFightMode");
    }

    public void ChangeTurn()
    {
        fightManager.ChangeTurn();
    }

    public void HasDied(Transform unit_transform)
    {
        fightManager.HasDied(unit_transform);
    }

    public void SaveUnitsParams(int unitId, FightingUnitStat unitStat, Gear gear, AnimatorParam animatorParam, Vector3 position)
    {
        if (unitId!=0) { currentRoom.unitsParam[unitId].objectParam.position = position; }
        statManager.SaveUnitStats(currentRoom, playerParam, unitId, unitStat);
        gearManager.SaveUnitGear(currentRoom, playerParam, unitId, gear);
        animatorManager.SaveUnitAnimatorParam(currentRoom, playerParam, unitId, animatorParam);
    }

    public void ChangeRoom()
    {
        fightMode = false;
        foreach (Transform unit in GameObject.Find("RoomUnits").transform)
        {
            if (unit == null) { continue; }
            unit.SendMessage("SaveParams");
        }
    }

    public void SaveAnimatorParameter(int id, string name, bool value)
    {
        if (id == 0)
        {
            if (playerParam.animatorParam.booleans.ContainsKey(name)) { playerParam.animatorParam.booleans[name] = value; }
            else { playerParam.animatorParam.booleans.Add(name, value); }
        }
        else
        {
            if (currentRoom.unitsParam[id].animatorParam.booleans.ContainsKey(name)) { currentRoom.unitsParam[id].animatorParam.booleans[name] = value; }
            else { currentRoom.unitsParam[id].animatorParam.booleans.Add(name, value); }
        }
    }

    public void ReloadAnimatorParameters(int id, Animator animator)
    {
        if (id == 0)
        {
            foreach (string name in playerParam.animatorParam.booleans.Keys)
            {
                animator.SetBool(name, playerParam.animatorParam.booleans[name]);
            }
        }
        else
        {
            foreach (string name in currentRoom.unitsParam[id].animatorParam.booleans.Keys)
            {
                animator.SetBool(name, currentRoom.unitsParam[id].animatorParam.booleans[name]);
            }
        }
    }
}