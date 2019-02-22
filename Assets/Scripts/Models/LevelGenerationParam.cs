﻿using System.Collections.Generic;
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
    }

    public class ObjectParam
    {
        public GameObject tileChoice { get; set; }
        public Vector3 position { get; set; }
    }

    public class TileParam
    {
        public Tile tileChoice { get; set; }
        public Vector3Int position { get; set; }
        public string entry { get; set; }//uniquement utilise par les outerwalls positionnes sur les entrees. Vaut "North", "South", East" ou "West" le cas echeant.
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
    }
}