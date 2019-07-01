using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        public float apMeleeAttackCost { get; set; }
        public float apRangeAttackCost { get; set; }
        public float apMovingCost { get; set; }
    }

    public class UnitAnimation
    {
        public bool SpriteFaceRight { get; set; }
        public string meleeAttackAnimation { get; set; }
        public string rangeAttackAnimation { get; set; }
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

        public GearItem getItem(Slot slot)
        {
            if (slot == Slot.rightHand) { return this.rightHand; }
            if (slot == Slot.leftHand) { return this.leftHand; }
            if (slot == Slot.head) { return this.head; }
            return null;
        }

        public void setItem(GearItem item)
        {
            if (item.slot == Slot.rightHand) { this.rightHand = item; }
            if (item.slot == Slot.leftHand) { this.leftHand = item; }
            if (item.slot == Slot.head) { this.head = item; }
        }

        public float getDamageBonus()
        {
            List<GearItem> items = getItems();
            float dmg = 0f;
            foreach (GearItem item in items) { dmg += item.damage; }
            return dmg;
        }

        public void removeItem(Slot slot)
        {
            if (slot == Slot.rightHand) { this.rightHand = null; }
            if (slot == Slot.leftHand) { this.leftHand = null; }
            if (slot == Slot.head) { this.head = null; }
        }
    }

    public class GearItem
    {
        public GameObject prefab { get; set; }
        public Color color { get; set; }
        public GameObject gameObject { get; set; }
        public GameObject owner { get; set; }
        public float damage { get; set; }
        public Slot slot { get; set; }
        public Projectile projectile { get; set; }


        public GearItem Clone()
        {
            GearItem clone = new GearItem { };
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                propertyInfo.SetValue(clone, propertyInfo.GetValue(this));
            }
            return clone;
        }
    }

    public enum Slot
    {
        rightHand,
        leftHand,
        head
    }

    public class Projectile
    {
        public GameObject prefab { get; set; }
        public GameObject gameObject { get; set; }
        public float damage { get; set; }
    }
}