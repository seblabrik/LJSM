
using LJSM.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{

    public float mapSizeParam = 30f;

    public GameObject mapPrefab;
    public GameObject mapBackgroundPrefab;
    public GameObject mapGatePrefab;
    public GameObject mapRoomPrefab;
    public GameObject mapPlayerPrefab;
    public GameObject mapDragablePrefab;

    private GameObject canvas;
    private GameObject dragableMap;
    private GameObject mapPlayer;
    private GameObject map;

    private int width;
    private int height;

    private Dictionary<RoomIndex, mapComponent> dictRooms;
    private Dictionary<List<RoomIndex>, mapComponent> dictGates;


    public GameObject CreateMap(Dictionary<RoomIndex, RoomParam> levelRooms, List<List<RoomIndex>> connectedRooms)
    {
        canvas = GameObject.Find("Canvas");
        map = Instantiate(mapPrefab, canvas.transform);
        Instantiate(mapBackgroundPrefab, map.transform);
        dragableMap = Instantiate(mapDragablePrefab, map.transform);
        dictRooms = new Dictionary<RoomIndex, mapComponent>();
        dictGates = new Dictionary<List<RoomIndex>, mapComponent>();

        DontDestroyOnLoad(canvas);

        width = GameManager.instance.width;
        height = GameManager.instance.height;

        foreach (RoomParam room in levelRooms.Values)
        {
            float x = room.coordonates.abs * mapSizeParam + (width - 1) / 2;
            float y = room.coordonates.ord * mapSizeParam + (height - 1) / 2;
            GameObject roomMap = Instantiate(mapRoomPrefab, new Vector3(x, y, 0f), Quaternion.identity);
            roomMap.transform.SetParent(dragableMap.transform, false);
            dictRooms.Add(room.coordonates, new mapComponent { gameObject = roomMap, isKnown = false });
            roomMap.SetActive(false);
            if (room.coordonates.isStartingRoom()) { dictRooms[room.coordonates].isKnown = true; roomMap.SetActive(true); }
        }
        List<string> closedList = new List<string>();

        foreach (List<RoomIndex> connection in connectedRooms)
        {
            string id1 = connection[0].abs + "" + connection[0].ord;
            string id2 = connection[1].abs + "" + connection[1].ord;
            if (closedList.Contains(id1 + id2) || closedList.Contains(id2 + id1)) { continue; }
            closedList.Add(id1 + id2);
            closedList.Add(id2 + id1);
            float x = ((connection[0].abs + connection[1].abs) * mapSizeParam + (width - 1)) / 2;
            float y = ((connection[0].ord + connection[1].ord) * mapSizeParam + (height - 1)) / 2;
            GameObject gate = Instantiate(mapGatePrefab, new Vector3(x, y, 0f), Quaternion.identity);
            gate.transform.SetParent(dragableMap.transform, false);
            dictGates.Add(connection, new mapComponent { gameObject = gate, isKnown = false});
            gate.SetActive(false);
            if (connection[0].isStartingRoom() || connection[1].isStartingRoom()) { dictGates[connection].isKnown = true; gate.SetActive(true); }
        }
        mapPlayer = Instantiate(mapPlayerPrefab, new Vector3((width - 1) / 2, (height - 1) / 2, 0f), Quaternion.identity);
        mapPlayer.transform.SetParent(dragableMap.transform, false);
        mapPlayer.transform.SetAsLastSibling();

        return map;
    }

    public void UpdateMap(RoomParam currentRoom)
    {
        Destroy(mapPlayer);
        float x  = currentRoom.coordonates.abs * mapSizeParam + (width - 1) / 2;
        float y = currentRoom.coordonates.ord * mapSizeParam + (height - 1) / 2;
        mapPlayer = Instantiate(mapPlayerPrefab, new Vector3(x, y, 0f), Quaternion.identity);
        mapPlayer.transform.SetParent(dragableMap.transform, false);
        mapPlayer.transform.SetAsLastSibling();
        UpdateMapComponent(currentRoom.coordonates);
        DisplayMapComponents();
    }

    private void DisplayMapComponents()
    {
        foreach (RoomIndex index in dictRooms.Keys)
        {
            if (dictRooms[index].isKnown) { dictRooms[index].gameObject.SetActive(true); }
        }
        foreach (List<RoomIndex> connection in dictGates.Keys)
        {
            if (dictGates[connection].isKnown) { dictGates[connection].gameObject.SetActive(true); }
        }
    }

    private void UpdateMapComponent(RoomIndex index)
    {
        foreach (RoomIndex roomIndex in dictRooms.Keys)
        {
            if (roomIndex.Equals(index)) { dictRooms[index].isKnown = true; }
        }
        foreach (List<RoomIndex> connection in dictGates.Keys)
        {
            if (connection[0].Equals(index) || connection[1].Equals(index)) { dictGates[connection].isKnown = true; }
        }
    }
}

