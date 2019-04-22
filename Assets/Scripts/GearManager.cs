﻿using LJSM.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearManager : MonoBehaviour
{
    public GameObject swordPrefab;


    //Cette fonction est provisoire, son but est d'équiper le player pour faire des tests
    public Gear GetPlayerGear()
    {
        return new Gear { rightHand = new GearItem { gameObject = swordPrefab, damage = 10f, slot = Slot.rightHand } };
    }

    public void InitiateGears(Dictionary<RoomIndex, RoomParam> levelRooms)
    {
        foreach (RoomParam roomParam in levelRooms.Values)
        {
            foreach (UnitParam unitParam in roomParam.unitsParam)
            {
                if (unitParam.unitNature == UnitNature.Zombie2) { unitParam.gear = GetPlayerGear(); }
                else { unitParam.gear = new Gear { }; }
            }
        }
    }
}