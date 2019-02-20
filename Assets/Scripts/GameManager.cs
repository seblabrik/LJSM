using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public NavMeshSurface2d surface;
    private RoomGenerator roomGenerator;

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
        
        surface = Instantiate(surface);

        InitGame();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.InitGame();
    }


    void InitGame()
    {
        roomGenerator.SetupRoom();
        surface.BuildNavMesh();
    }

    public void GameOver()
    {
        enabled = false;
    }
}
