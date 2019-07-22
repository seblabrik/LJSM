using LJSM.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public void InitiateAnimatorParams(Dictionary<RoomIndex, RoomParam> levelRooms)
    {
        foreach (RoomParam roomParam in levelRooms.Values)
        {
            if (roomParam.unitsParam != null)
            {
                foreach (UnitParam unitParam in roomParam.unitsParam.Values)
                {
                    unitParam.animatorParam = new AnimatorParam { booleans = new Dictionary<string, bool>() };
                }
            }
        }
    }


    public void SaveUnitAnimatorParam(RoomParam currentRoom, UnitParam playerParam, int unitId, AnimatorParam animatorParam)
    {
        if (unitId != 0)
        {
            currentRoom.unitsParam[unitId].animatorParam = animatorParam;
        }
        else { playerParam.animatorParam = animatorParam; }
    }
}
