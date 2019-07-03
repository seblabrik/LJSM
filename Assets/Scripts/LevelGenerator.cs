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
    
    private int width;
    private int height;

    private int unitIdIterator;

    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);

    public Tile[] groundTiles;
    public Tile[] wallTiles;
    public Tile[] outerWallTiles;
    public GameObject zombie1;
    public GameObject zombie2;
    public GameObject zombie3;
    public GameObject wizard;

    public int numberOfRooms = 5;//Dans un premier temps on impose le nombre de salle


    public (Dictionary<RoomIndex, RoomParam>, List<List<RoomIndex>>) GenerateLevel()
    {
        width = GameManager.instance.width;
        height = GameManager.instance.height;
        unitIdIterator = 0;

        Dictionary<RoomIndex, RoomParam> rooms = new Dictionary<RoomIndex, RoomParam>();
        List<RoomIndex> openList = new List<RoomIndex>();
        List<RoomIndex> closedList = new List<RoomIndex>();
        List<List<RoomIndex>> connectedRooms = new List<List<RoomIndex>>();//on garde en memoire les salles connectees entre elles par une porte
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
                        List<RoomIndex> list1 = new List<RoomIndex>();
                        list1.Add(coord);
                        list1.Add(coordonates);
                        List<RoomIndex> list2 = new List<RoomIndex>();
                        list2.Add(coord);
                        list2.Add(coordonates);
                        connectedRooms.Add(list1);
                        connectedRooms.Add(list2);
                    }
                    continue;
                }
                if (openList.Contains(coord) || closedList.Contains(coord)) { continue; }
                openList.Add(coord);
            }
        }
        return (rooms, connectedRooms);
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

    private List<Vector3Int> InitialiseList(List<Vector3Int> gridPositions)
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

        return gridPositions;
    }

    private Vector3Int RandomPositionAvailable(List<Vector3Int> gridPositions)
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3Int randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }


    private List<UnitParam> LayoutUnitAtRandom(List<Vector3Int> gridPositions, GameObject tile, int minimum, int maximum, UnitNature unitNature)
    {
        List<ObjectParam> list_objectParam = LayoutObjectAtRandom(gridPositions, tile, minimum, maximum);
        List<UnitParam> list_unitParam = new List<UnitParam>();

        foreach (ObjectParam objectParam in list_objectParam)
        {
            UnitParam param = new UnitParam { id = unitIdIterator, objectParam = objectParam, unitNature = unitNature };
            unitIdIterator++;
            list_unitParam.Add(param);
        }

        return list_unitParam;
    }


    private List<ObjectParam> LayoutObjectAtRandom(List<Vector3Int> gridPositions, GameObject tile, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        List<ObjectParam> list_objectParam = new List<ObjectParam>();


        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPositionAvailable(gridPositions);
            ObjectParam param = new ObjectParam { tileChoice = tile, position = randomPosition };
            list_objectParam.Add(param);
        }
        return list_objectParam;
    }

    private List<TileParam> LayoutTileAtRandom(List<Vector3Int> gridPositions, Tile[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        List<TileParam> tilesParam = new List<TileParam>();

        for (int i = 0; i < objectCount; i++)
        {
            Vector3Int randomPosition = RandomPositionAvailable(gridPositions);
            Tile tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            TileParam param = new TileParam { tileChoice = tileChoice, position = randomPosition };
            tilesParam.Add(param);
        }
        return tilesParam;
    }

    RoomParam CreateRoom(RoomIndex coordonates)
    {
        (List<TileParam> groundTilesParam, List<TileParam> outerWallTilesParam) = CreateBoard();
        List<Vector3Int> gridPositions = new List<Vector3Int>();
        InitialiseList(gridPositions);
        List<TileParam> wallTilesParam = LayoutTileAtRandom(gridPositions, wallTiles, wallCount.minimum, wallCount.maximum);
        List<UnitParam> unitsParam;

        if (coordonates.isStartingRoom())//Dans la première starting room on ne met pas d'ennemies
        {
            unitsParam = LayoutUnitAtRandom(gridPositions, wizard, 1, 1, UnitNature.Wizard);
            return new RoomParam
            {
                coordonates = coordonates,
                wallTilesParam = wallTilesParam,
                outerWallTilesParam = outerWallTilesParam,
                groundTilesParam = groundTilesParam,
                unitsParam = unitsParam
            };
        }

        unitsParam = LayoutUnitAtRandom(gridPositions, zombie1, 0, 1, UnitNature.Zombie1);//entre 0 et 2 enemies, provisoirement
        unitsParam.AddRange(LayoutUnitAtRandom(gridPositions, zombie2, 0, 1, UnitNature.Zombie2));
        unitsParam.AddRange(LayoutUnitAtRandom(gridPositions, zombie3, 0, 1, UnitNature.Zombie3));

        RoomParam roomParam = new RoomParam
        {
            coordonates = coordonates,
            wallTilesParam = wallTilesParam,
            outerWallTilesParam = outerWallTilesParam,
            groundTilesParam = groundTilesParam,
            unitsParam = unitsParam
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
                SpecificSpot entry = isEntry(x, y);

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

    private SpecificSpot isEntry(int x, int y)
    {
        if ((x == -1) && (y == (height - 1) / 2)) { return SpecificSpot.West; }
        if ((x == width) && (y == (height - 1) / 2)) { return SpecificSpot.East; }
        if ((y == -1) && (x == (width - 1) / 2)) { return SpecificSpot.South; }
        if ((y == height) && (x == (width - 1) / 2)) { return SpecificSpot.North; }
        return SpecificSpot.Null;
    }

    //Cette fonction modifie le plan rooms pour y ajouter une ouverture entre les 2 salles de coordonnées coord1 et coord2
    Dictionary<RoomIndex, RoomParam> ConnectRooms(Dictionary<RoomIndex, RoomParam> rooms, RoomIndex coord1, RoomIndex coord2)
    {
        if (coord1.Equals(coord2)) { return rooms; }//petite sécurité...

        List<TileParam> entries1 = new List<TileParam>();
        foreach (TileParam param in rooms[coord1].outerWallTilesParam)
        {
            if (param.entry != SpecificSpot.Null) { entries1.Add(param); }
        }

        List<TileParam> entries2 = new List<TileParam>();
        foreach (TileParam param in rooms[coord2].outerWallTilesParam)
        {
            if (param.entry != SpecificSpot.Null) { entries2.Add(param); }
        }

        TileParam entry1 = new TileParam();
        TileParam entry2 = new TileParam();

        if (coord1.abs != coord2.abs)
        {
            if (coord1.abs < coord2.abs)
            {
                foreach (TileParam param in entries1) { if (param.entry == SpecificSpot.East) { entry1 = param; } }
                foreach (TileParam param in entries2) { if (param.entry == SpecificSpot.West) { entry2 = param; } }
            }
            else
            {
                foreach (TileParam param in entries1) { if (param.entry == SpecificSpot.West) { entry1 = param; } }
                foreach (TileParam param in entries2) { if (param.entry == SpecificSpot.East) { entry2 = param; } }
            }
        }
        else
        {
            if (coord1.ord < coord2.ord)
            {
                foreach (TileParam param in entries1) { if (param.entry == SpecificSpot.North) { entry1 = param; } }
                foreach (TileParam param in entries2) { if (param.entry == SpecificSpot.South) { entry2 = param; } }
            }
            else
            {
                foreach (TileParam param in entries1) { if (param.entry == SpecificSpot.South) { entry1 = param; } }
                foreach (TileParam param in entries2) { if (param.entry == SpecificSpot.North) { entry2 = param; } }
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