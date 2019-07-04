using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace LJSM.Models
{
    public class RoomParam
    {
        public RoomIndex coordonates { get; set; }
        public List<TileParam> wallTilesParam { get; set; }
        public List<TileParam> outerWallTilesParam { get; set; }
        public List<TileParam> groundTilesParam { get; set; }
        public List<ObjectParam> objectsParam { get; set; }
        public List<UnitParam> unitsParam { get; set; }
    }

    public class ObjectParam
    {
        public GameObject tileChoice { get; set; }
        public Vector3 position { get; set; }
    }

    public class UnitParam
    {
        public int id { get; set; }
        public ObjectParam objectParam { get; set; }
        public UnitNature unitNature { get; set; }
        public FightingUnitStat stat { get; set; }
        public Gear gear { get; set; }
    }

    public class TileParam
    {
        public Tile tileChoice { get; set; }
        public Vector3Int position { get; set; }
        public SpecificSpot entry { get; set; }//uniquement utilise par les outerwalls positionnes sur les entrees.
    }

    public struct RoomIndex
    {
        public int abs { get; set; }
        public int ord { get; set; }

        public override bool Equals(System.Object obj)
        {
            if (obj == null || !(obj is RoomIndex))
                return false;
            else
                return abs == ((RoomIndex)obj).abs && ord == ((RoomIndex)obj).ord;
        }

        public override int GetHashCode()
        {
            return 1000*abs + ord;
        }

        public bool isStartingRoom()
        {
            if (abs == 0 && ord == 0) { return true; }
            return false;
        }

    }

    public enum SpecificSpot
    {
        Null,
        North,
        South,
        East,
        West,
        Start
    }

    public enum UnitNature
    {
        Player,
        Zombie1,
        Zombie2,
        Zombie3,
        Wizard
    }
}