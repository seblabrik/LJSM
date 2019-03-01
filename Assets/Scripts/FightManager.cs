using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class FightManager : MonoBehaviour
{
    private List<Transform> fightingUnits;
    private int turnIndex;

    public void InitFight()
    {
        fightingUnits = new List<Transform>();
        Transform roomObjects = GameObject.Find("RoomUnits").transform;
        for (int i = 0; i < roomObjects.childCount; i++)
        {
            fightingUnits.Add(roomObjects.GetChild(i));
            roomObjects.GetChild(i).GetComponent<NavMeshAgent>().isStopped = true;
            roomObjects.GetChild(i).GetComponent<NavMeshAgent>().ResetPath();
        }
        turnIndex = Random.Range(0, fightingUnits.Count - 1);
        fightingUnits[turnIndex].SendMessage("InitTurn");
    }

    public void ChangeTurn()
    {
        turnIndex++;
        if (turnIndex >= fightingUnits.Count) { turnIndex -= fightingUnits.Count; }
        fightingUnits[turnIndex].SendMessage("InitTurn");
    }

    public void HasDied(Transform unit)
    {
        int i = fightingUnits.IndexOf(unit);
        fightingUnits.Remove(unit);
        if (fightingUnits.Count < 2) { GameManager.instance.ExitFightMode(); return; }
        if (turnIndex >= i)
        {
            turnIndex--;
            if (turnIndex < 0)
            {
                turnIndex += fightingUnits.Count;
            }
        }
    }
}
