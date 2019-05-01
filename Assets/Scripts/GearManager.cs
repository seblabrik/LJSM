using LJSM.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearManager : MonoBehaviour
{
    public GameObject swordPrefab;


    private Gear GetZombieGear()
    {
        int level = Random.Range(0, 5);
        Color color = GetColor(level);
        float dmg = GetDamage(level);
        return new Gear { rightHand = new GearItem { prefab = swordPrefab, color = color, damage = dmg, slot = Slot.rightHand } };
    }

    public void InitiateGears(Dictionary<RoomIndex, RoomParam> levelRooms)
    {
        foreach (RoomParam roomParam in levelRooms.Values)
        {
            foreach (UnitParam unitParam in roomParam.unitsParam)
            {
                if (unitParam.unitNature == UnitNature.Zombie2) { unitParam.gear = GetZombieGear(); }
                else { unitParam.gear = new Gear { }; }
            }
        }
    }

    public void SaveUnitGear(RoomParam currentRoom, UnitParam playerParam, int unitId, Gear gear)
    {
        if (unitId != 0)
        {
            foreach (UnitParam unitParam in currentRoom.unitsParam)
            {
                if (unitParam.id == unitId) { unitParam.gear = gear; }
            }
        }
        else { playerParam.gear = gear; }
    }

    private float GetDamage(int level)
    {
        float dmg_max = 50f;
        float dmg_min = 10f;
        return dmg_min + (dmg_max - dmg_min) * (level/5);
    }

    private Color GetColor(int level)
    {
        float shade_max = 1f;
        float shade_min = 0.5f;
        float shade = shade_max + (shade_min - shade_max) * (level / 5);
        return new Color(1f, shade, shade);
    }
}
