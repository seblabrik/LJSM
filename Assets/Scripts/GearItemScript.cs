using LJSM.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearItemScript : MonoBehaviour
{
    public GearItem gearItem;

    public void SetGearItem(GearItem gearItem)
    {
        this.gearItem = gearItem;
        if (gearItem.owner == null)
        {
            GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        }
        else
        {
            GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        }
    }
}
