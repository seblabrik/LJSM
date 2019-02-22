using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

using LJSM.Models;

public class LevelGenerator : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    private List<Vector3Int> gridPositions = new List<Vector3Int>();
    private int width;
    private int height;


    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);

    public Tile[] groundTiles;
    public Tile[] wallTiles;
    public Tile[] outerWallTiles;
    public GameObject[] enemyTiles;

    public int numberOfRooms = 5;//Dans un premier temps on impose le nombre de salle
    public float roomConnectionProba = 0.1f;//proba entre 0 et 1 d'avoir une porte entre 2 salles


    public Dictionary<RoomIndex, RoomParam> GenerateLevel()
    {
        width = GameManager.instance.width;
        height = GameManager.instance.height;

        Dictionary<RoomIndex, RoomParam> rooms = new Dictionary<RoomIndex, RoomParam>();
        List<RoomIndex> openList = new List<RoomIndex>();
        List<RoomIndex> closedList = new List<RoomIndex>();
        List<(RoomIndex, RoomIndex)> connectedRooms = new List<(RoomIndex, RoomIndex)>();//on garde en memoire les salles connectees entre elles par une porte
        RoomIndex startRoomCoord = new RoomIndex { abs = 0, ord = 0 };
        openList.Add(startRoomCoord);

        for (int i = 0; i < numberOfRooms; i++)
        {
            int randomIndex = Random.Range(0, openList.Count - 1);
            RoomIndex coordonates = openList[randomIndex];
            openList.Remove(coordonates);
            closedList.Add(coordonates);
            RoomParam roomParam = CreateRoom(coordonates);
            rooms.Add(coordonates, roomParam);

            foreach (RoomIndex coord in GetNeighbouringRooms(coordonates))
            {
                if (closedList.Contains(coord))
                {
                    if (Math.Abs((coordonates.abs - coord.abs) + (coordonates.ord - coord.ord)) == 1)//ie salles voisines
                    {
                        rooms = ConnectRooms(rooms, coord, coordonates);//on ouvre les portes entre les 2 salles
                        connectedRooms.Add((coord, coordonates));
                        connectedRooms.Add((coordonates, coord));
                    }
                    continue;
                }
                if (openList.Contains(coord) || closedList.Contains(coord)) { continue; }
                openList.Add(coord);
            }
        }

        //foreach (RoomParam room_i in rooms.Values)
        //{
        //    foreach (RoomParam room_j in rooms.Values)
        //    {
        //        RoomIndex cord_i = room_i.coordonates;
        //        RoomIndex cord_j = room_j.coordonates;
        //        if (Math.Abs((cord_i.abs-cord_j.abs)+(cord_i.ord-cord_j.ord))!=1 || connectedRooms.Contains((cord_i, cord_j))) { continue; }
        //        float rand = Random.Range(0, roomConnectionProba);
        //        if (rand < 0.1f)
        //        {
        //            rooms = ConnectRooms(rooms, cord_i, cord_j);
        //            connectedRooms.Add((cord_i, cord_j));
        //            connectedRooms.Add((cord_j, cord_i));
        //        }
        //    }
        //}


        return rooms;
    }

    public List<RoomIndex> GetNeighbouringRooms(RoomIndex coord)
    {
        List<RoomIndex> neighbourList = new List<RoomIndex>(); int abs; int ord;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i * j != 0)//On veut pas de diagonale
                {
                    continue;
                }
                abs = coord.abs + i; ord = coord.ord + j;
                RoomIndex neighbour = new RoomIndex { abs = abs, ord = ord };
                neighbourList.Add(neighbour);

            }
        }
        return neighbourList;
    }

    void InitialiseList()
    {
        gridPositions.Clear();
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                gridPositions.Add(new Vector3Int(x, y, 0));
            }
        }
        //On retire les positions interdites (celles ou le player pourrait spawn)
        gridPositions.Remove(new Vector3Int(0, 0, 0));//Start
        gridPositions.Remove(new Vector3Int((width - 1) / 2 + 1, height - 1, 0));//Nord
        gridPositions.Remove(new Vector3Int((width - 1) / 2 + 1, 0, 0));//Sud
        gridPositions.Remove(new Vector3Int(width - 1, (height - 1) / 2 + 1, 0));//Est
        gridPositions.Remove(new Vector3Int(0, (height - 1) / 2 + 1, 0));//Ouest
    }

    private Vector3Int RandomPositionAvailable()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3Int randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }


    private List<ObjectParam> LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        List<ObjectParam> enemiesParam = new List<ObjectParam>();


        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPositionAvailable();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            ObjectParam param = new ObjectParam { tileChoice = tileChoice, position = randomPosition };
            enemiesParam.Add(param);
        }
        return enemiesParam;
    }

    private List<TileParam> LayoutTileAtRandom(Tile[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        List<TileParam> tilesParam = new List<TileParam>();

        for (int i = 0; i < objectCount; i++)
        {
            Vector3Int randomPosition = RandomPositionAvailable();
            Tile tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            TileParam param = new TileParam { tileChoice = tileChoice, position = randomPosition };
            tilesParam.Add(param);
        }
        return tilesParam;
    }

    RoomParam CreateRoom(RoomIndex coordonates)
    {
        (List<TileParam> groundTilesParam, List<TileParam> outerWallTilesParam) = CreateBoard();
        InitialiseList();
        List<TileParam> wallTilesParam = LayoutTileAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        List<ObjectParam> objectsParam = LayoutObjectAtRandom(enemyTiles, 1, 2);//entre 1 et 2 enemies, provisoirement

        RoomParam roomParam = new RoomParam
        {
            coordonates = coordonates,
            wallTilesParam = wallTilesParam,
            outerWallTilesParam = outerWallTilesParam,
            groundTilesParam = groundTilesParam,
            objectsParam = objectsParam
        };
        return roomParam;
    }

    (List<TileParam>, List<TileParam>) CreateBoard()
    {
        List<TileParam> outerWallTilesParam = new List<TileParam>();
        List<TileParam> groundTilesParam = new List<TileParam>();
        for (int x = -1; x < width + 1; x++)
        {
            for (int y = -1; y < height + 1; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                string entry = isEntry(x, y);

                if (x == -1 || x == width || y == -1 || y == height)
                {
                    Tile tile = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                    TileParam param = new TileParam { tileChoice = tile, position = pos, entry = entry };
                    outerWallTilesParam.Add(param);
                }
                else
                {
                    Tile tile = groundTiles[Random.Range(0, groundTiles.Length)];
                    TileParam param = new TileParam { tileChoice = tile, position = pos };
                    groundTilesParam.Add(param);
                }
            }
        }
        return (groundTilesParam, outerWallTilesParam);
    }

    private string isEntry(int x, int y)
    {
        if ((x == -1) && (y == (height - 1) / 2)) { return "West"; }
        if ((x == width) && (y == (height - 1) / 2)) { return "East"; }
        if ((y == -1) && (x == (width - 1) / 2)) { return "South"; }
        if ((y == height) && (x == (width - 1) / 2)) { return "North"; }
        return "";
    }

    Dictionary<RoomIndex, RoomParam> ConnectRooms(Dictionary<RoomIndex, RoomParam> rooms, RoomIndex coord1, RoomIndex coord2)
    {
        if (coord1.Equals(coord2)) { return rooms; }//petite sécurité...

        List<TileParam> entries1 = new List<TileParam>();
        foreach (TileParam param in rooms[coord1].outerWallTilesParam)
        {
            if (param.entry != "") { entries1.Add(param); }
        }

        List<TileParam> entries2 = new List<TileParam>();
        foreach (TileParam param in rooms[coord2].outerWallTilesParam)
        {
            if (param.entry != "") { entries2.Add(param); }
        }

        TileParam entry1 = new TileParam();
        TileParam entry2 = new TileParam();

        if (coord1.abs != coord2.abs)
        {
            if (coord1.abs < coord2.abs)
            {
                foreach (TileParam param in entries1) { if (param.entry == "East") { entry1 = param; } }
                foreach (TileParam param in entries2) { if (param.entry == "West") { entry2 = param; } }
            }
            else
            {
                foreach (TileParam param in entries1) { if (param.entry == "West") { entry1 = param; } }
                foreach (TileParam param in entries2) { if (param.entry == "East") { entry2 = param; } }
            }
        }
        else
        {
            if (coord1.ord < coord2.ord)
            {
                foreach (TileParam param in entries1) { if (param.entry == "North") { entry1 = param; } }
                foreach (TileParam param in entries2) { if (param.entry == "South") { entry2 = param; } }
            }
            else
            {
                foreach (TileParam param in entries1) { if (param.entry == "South") { entry1 = param; } }
                foreach (TileParam param in entries2) { if (param.entry == "North") { entry2 = param; } }
            }
        }

        rooms[coord1].outerWallTilesParam.Remove(entry1);
        entry1.tileChoice = groundTiles[Random.Range(0, groundTiles.Length)];
        rooms[coord1].groundTilesParam.Add(entry1);

        rooms[coord2].outerWallTilesParam.Remove(entry2);
        entry2.tileChoice = groundTiles[Random.Range(0, groundTiles.Length)];
        rooms[coord2].groundTilesParam.Add(entry2);

        return rooms;
    }
}