using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager s_Instance = null;
    public static MapManager Instance
    {
        get
        {
            return s_Instance;
        }
    }

    public class Room
    {
        public MapType type = MapType.None;
        public Tuple<int, int> location;
        public List<Room> lastRooms = new List<Room>();
        public List<Room> nextRooms = new List<Room>();
        public Vector2 pos;
    }

    private Room[,] map = null;
    private int[,] paths = null;
    private Tuple<int, int> currRoom = null;

    private PopupMap popupMap = null;


    private void Awake()
    {
        s_Instance = this;

        StartCoroutine(UIManager.Instance.routineLoadPopup());
        DataTableManager.LoadAll();
        PlayerDataManager.Instance.SetDeckIds(StaticData.startDeckIronclad.ToList());
        CardManager.Instance.SetDeck(PlayerDataManager.Instance.GetCardIds());

        GenerateMap();

        popupMap = UIManager.Instance.MakePopup<PopupMap>();
        popupMap.Set();
    }

    private void OnDestroy()
    {
        s_Instance = null;
    }

    public Room[,] GetMap() => map;
    public int[,] GetPaths() => paths;
    public Tuple<int, int> GetCurrRoom() => currRoom;

    private void GenerateMap()
    {
        CalculatePath();

        map = new Room[StaticData.MAP_FLOORS, StaticData.MAP_ROOMS_FLOOR];
        for (int i = 0; i < StaticData.MAP_PATHS; i++)
        {
            for (int j = 0; j < StaticData.MAP_FLOORS; j++)
            {
                if (map[j, paths[i,j]] == null)
                {
                    map[j, paths[i, j]] = new Room();
                    map[j, paths[i, j]].location = new Tuple<int, int>(j, paths[i, j]);
                }

                if (j > 0)
                {
                    map[j, paths[i, j]].lastRooms.Add(map[j - 1, paths[i, j - 1]]);
                    map[j - 1, paths[i, j - 1]].nextRooms.Add(map[j, paths[i, j]]);
                }
            }
        }

        for (int i = 0; i < StaticData.MAP_FLOORS; i++)
        {
            for (int j = 0; j < StaticData.MAP_ROOMS_FLOOR; j++)
            {
                if (map[i, j] == null)
                {
                    continue;
                }

                switch (i)
                {
                    // 1���� ���� �Ϲ� ����
                    case 0:
                        {
                            map[i, j].type = MapType.Monster;
                        }
                        break;
                    // 9���� ���� ������
                    case 8:
                        {
                            map[i, j].type = MapType.Treasure;
                        }
                        break;
                    // 15���� ���� �޽�ó
                    case 14:
                        {
                            map[i, j].type = MapType.RestSite;
                        }
                        break;
                    default:
                        {
                            bool isValidRoom = false;
                            while (isValidRoom == false)
                            {
                                isValidRoom = true;
                                SetRandomRoom(map[i, j]);
                                MapType type = map[i, j].type;

                                // 6�� ���Ͽ����� ����Ʈ, �ްԼ� �Ұ�
                                if (i < 5)
                                {
                                    if (type == MapType.Elite || type == MapType.RestSite)
                                    {
                                        isValidRoom = false;
                                        continue;
                                    }
                                }

                                // ����Ʈ, ����, �޽�ó�� �����ؼ� ���� �� ����
                                if (type == MapType.Elite || type == MapType.Merchant || type == MapType.RestSite)
                                {
                                    if (map[i, j].lastRooms.Find((room) => room.type == type) != null)
                                    {
                                        isValidRoom = false;
                                        continue;
                                    }
                                }

                                // ���� ��忡�� 2�� �̻��� Path�� �������, ���� ������ �ٸ� Ÿ���� ���;� ��
                                if (i > 0)
                                {
                                    var lastRooms = map[i, j].lastRooms;
                                    for (int k = 0; k < lastRooms.Count; k++)
                                    {
                                        if (lastRooms[k].nextRooms.Find((room) => room != map[i,j] && room.type == type) != null)
                                        {
                                            isValidRoom = false;
                                            continue;
                                        }
                                    }
                                }

                                // 14�������� �޽�ó �Ұ�
                                if (i == 14)
                                {
                                    if (type == MapType.RestSite)
                                    {
                                        isValidRoom = false;
                                        continue;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }
    }

    private void SetRandomRoom(Room _room)
    {
        TableMapRandomType table = DataTableManager.GetTable<TableMapRandomType>();
        var books = table.GetBooks();
        float randomRate = UnityEngine.Random.Range(0f, 1f);
        MapType type = MapType.Monster;
        for (int i = books.Count - 1; i >= 0; i--)
        {
            if (books[i].TYPE == (int)MapType.Boss ||
                books[i].TYPE == (int)MapType.Treasure)
            {
                continue;
            }

            randomRate -= books[i].RATE;
            if (randomRate < 0f)
            {
                type = (MapType)books[i].TYPE;
                break;
            }
        }

        _room.type = type;
    }

    private void CalculatePath()
    {
        paths = new int[StaticData.MAP_PATHS, StaticData.MAP_FLOORS];
        for (int i = 0; i < StaticData.MAP_PATHS; i++)
        {
            for (int j = 0; j < StaticData.MAP_FLOORS; j++)
            {
                if (j == 0)
                {
                    if (i == StaticData.MAP_PATHS - 1)
                    {
                        bool isAtLeastTwoStartPoints = false;
                        while (isAtLeastTwoStartPoints == false)
                        {
                            paths[i, j] = UnityEngine.Random.Range(0, StaticData.MAP_ROOMS_FLOOR);
                            for (int k = 0; k < i; k++)
                            {
                                if (paths[k, 0] != paths[k + 1, 0])
                                {
                                    isAtLeastTwoStartPoints = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        paths[i, j] = UnityEngine.Random.Range(0, StaticData.MAP_ROOMS_FLOOR);
                    }
                }
                else
                {
                    int lastRoom = paths[i, j - 1];
                    int min = lastRoom - 1;
                    if (lastRoom == 0)
                    {
                        min = lastRoom;
                    }
                    else
                    {
                        for (int k = 0; k < i; k++)
                        {
                            if (k == i)
                            {
                                continue;
                            }
                            if (paths[k, j - 1] == lastRoom - 1 && paths[k, j] == lastRoom)
                            {
                                min = lastRoom;
                            }
                        }
                    }
                    int max = lastRoom + 1;
                    if (lastRoom == StaticData.MAP_ROOMS_FLOOR - 1)
                    {
                        max = lastRoom;
                    }
                    else
                    {
                        for (int k = 0; k < i; k++)
                        {
                            if (k == i)
                            {
                                continue;
                            }
                            if (paths[k, j - 1] == lastRoom + 1 && paths[k, j] == lastRoom)
                            {
                                max = lastRoom;
                            }
                        }
                    }

                    paths[i, j] = UnityEngine.Random.Range(min, max + 1);
                }
            }
        }
    }

    public void EnterRoom(int _floor, int _room)
    {
        if (currRoom == null)
        {
            if (_floor > 0)
            {
                LogManager.LogError("currRoom�� ���� ������ 1���� ���� ����", this, "EnterRoom");
                return;
            }
        }
        else if (map[currRoom.Item1, currRoom.Item2].nextRooms.Find((room) => room.location.Item1 == _floor && room.location.Item2 == _room) == null)
        {
            LogManager.LogError("���� ��ο� ���� �� ���� �õ�", this, "EnterRoom");
            return;
        }

        switch (map[_floor, _room].type)
        {
            case MapType.Monster:
                {
                    BattleManager.Instance.gameObject.SetActive(true);
                    BattleManager.Instance.ChangeState(BattleManager.BATTLE_STATE.INIT);
                }
                break;
            case MapType.Event:
                break;
            case MapType.Elite:
                {
                    BattleManager.Instance.gameObject.SetActive(true);
                    BattleManager.Instance.ChangeState(BattleManager.BATTLE_STATE.INIT);
                }
                break;
            case MapType.RestSite:
                {
                    PopupRestSite popupRestSite = UIManager.Instance.MakePopup<PopupRestSite>();
                    popupRestSite.SetAction(
                        () =>
                        {
                            MapManager.Instance.ExitRoom();
                        });
                }
                break;
            case MapType.Merchant:
                break;
            case MapType.Treasure:
                break;
            case MapType.Boss:
                {
                    BattleManager.Instance.gameObject.SetActive(true);
                    BattleManager.Instance.ChangeState(BattleManager.BATTLE_STATE.INIT);
                }
                break;
        }

        popupMap.gameObject.SetActive(false);
        currRoom = new Tuple<int, int>(_floor, _room);
    }

    public void ExitRoom()
    {
        popupMap.gameObject.SetActive(true);
    }
}
