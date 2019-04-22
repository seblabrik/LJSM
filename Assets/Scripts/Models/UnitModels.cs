using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LJSM.Models
{
    public class FightingUnitStat
    {
        public float meleeRange { get; set; }
        public float damage { get; set; }
        public float hp { get; set; }
        public float attackSpeed { get; set; }
        public float apFull { get; set; }
        public float apAttackCost { get; set; }
        public float apMovingCost { get; set; }
    }

    public class UnitAnimation
    {
        public bool SpriteFaceRight { get; set; }
        public string meleeAttackAnimation { get; set; }
    }

    public class Gear
    {
        public GearItem rightHand { get; set; }
        public GearItem leftHand { get; set; }
        public GearItem head { get; set; }

        public List<GearItem> getItems()
        {
            List<GearItem> items = new List<GearItem>();
            if (rightHand != null) { items.Add(rightHand); }
            if (leftHand != null) { items.Add(leftHand); }
            if (head != null) { items.Add(head); }
            return items;
        }

        public float getDamageBonus()
        {
            List<GearItem> items = getItems();
            float dmg = 0f;
            foreach (GearItem item in items) { dmg += item.damage; }
            return dmg;
        }
    }

    public class GearItem
    {
        public GameObject gameObject { get; set; }
        public float damage { get; set; }
        public Slot slot { get; set; }
    }

    public enum Slot
    {
        rightHand,
        leftHand,
        head
    }
}