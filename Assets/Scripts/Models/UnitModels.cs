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
}