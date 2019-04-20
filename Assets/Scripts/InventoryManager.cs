using LJSM.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject swordPrefab;


    //Cette fonction est provisoire, son but est d'équiper le player pour faire des tests
    public Gear GetPlayerGear()
    {
        return new Gear { rightHand = new GearItem { gameObject = swordPrefab, damage = 10f, slot = Slot.RightHand } };
    }

}
