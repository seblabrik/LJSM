using LJSM.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public void InitiateStats(Dictionary<RoomIndex, RoomParam> levelRooms)
    {
        foreach (RoomParam roomParam in levelRooms.Values)
        {
            if (roomParam.unitsParam != null)
            {
                foreach (UnitParam unitParam in roomParam.unitsParam)
                {
                    unitParam.stat = GenerateStat(unitParam.unitNature);
                }
            }
        }
    }

    public FightingUnitStat GenerateStat(UnitNature unitNature)
    {
        if (unitNature == UnitNature.Player)
        {
            return new FightingUnitStat
            {
                meleeRange = 1f,
                damage = 10f,
                hp = 100f,
                attackSpeed = 0.5f,
                apFull = 100f,
                apMeleeAttackCost = 50f,
                apRangeAttackCost = 50f,
                apMovingCost = 25f
            };
        }
        if (unitNature == UnitNature.Zombie1)
        {
            return new FightingUnitStat
            {
                meleeRange = 0.7f,
                damage = 10f,
                hp = 20f,
                attackSpeed = 1f,
                apFull = 75f,
                apMeleeAttackCost = 50f,
                apRangeAttackCost = 50f,
                apMovingCost = 25f
            };
        }
        if (unitNature == UnitNature.Zombie2)
        {
            return new FightingUnitStat
            {
                meleeRange = 0.7f,
                damage = 0f,
                hp = 50f,
                attackSpeed = 1f,
                apFull = 75f,
                apMeleeAttackCost = 50f,
                apRangeAttackCost = 50f,
                apMovingCost = 25f
            };
        }
        if (unitNature == UnitNature.Zombie3)
        {
            return new FightingUnitStat
            {
                meleeRange = 0.7f,
                damage = 10f,
                hp = 20f,
                attackSpeed = 1f,
                apFull = 75f,
                apMeleeAttackCost = 50f,
                apRangeAttackCost = 75f,
                apMovingCost = 25f
            };
        }
        if (unitNature == UnitNature.Wizard)
        {
            return new FightingUnitStat
            {
                meleeRange = 0.7f,
                damage = 10f,
                hp = 20f,
                attackSpeed = 1f,
                apFull = 75f,
                apMeleeAttackCost = 50f,
                apRangeAttackCost = 75f,
                apMovingCost = 25f
            };
        }
        return null;
    }

    public void SaveUnitStats(RoomParam currentRoom, UnitParam playerParam, int unitId, FightingUnitStat unitStat)
    {

        if (unitId != 0)
        {
            foreach (UnitParam unitParam in currentRoom.unitsParam)
            {
                if (unitParam.id == unitId) { unitParam.stat = unitStat; }
            }
        }
        else { playerParam.stat = unitStat; }
    }
}
