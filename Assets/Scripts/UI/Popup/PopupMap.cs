using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMap : Popup
{
    [SerializeField]
    private Button prefabRoom = null;
    [SerializeField]
    private ScrollRect scrollRect = null;

    [SerializeField]
    private LineUIDrawer prefabPathDrawer = null;
    [SerializeField]
    private Transform parentPaths = null;

    private Button[,] map = null;

    private Vector2 SIZE_ROOM = new Vector2(60f, 60f);
    private Vector2 SPACING = new Vector2(110f, 480f);

    private float scaleAccessible = 1f;
    private bool isGettingLarger = true;

    private bool isRepositioning = false;
    // content의 이동하려는 normalizedPosY 값
    private float destination;
    private float lerpReposition = 0f;
    [SerializeField]
    private float speedReposition = 1f;

    protected override void OnEnable()
    {
        var currRoom = MapManager.Instance.GetCurrRoom();
        if (currRoom != null)
        {
            List<MapManager.Room> accessibleRooms = MapManager.Instance.GetMap()[currRoom.Item1, currRoom.Item2].nextRooms;
            // 다음 방들의 평균 높이 구함
            float height = 0f;
            for (int i = 0; i < accessibleRooms.Count; i++)
            {
                height += map[accessibleRooms[i].location.Item1, accessibleRooms[i].location.Item2].transform.localPosition.y;
            }
            height /= accessibleRooms.Count;

            // 바로 normalize하면 content의 가운데에서 가장자리로 갈 수록 쏠림현상 발생
            // normalizedPosition_y => (viewport_height * 0.5f)일 때 0 / (content_height - viewport_height * 0.5f)일 때를 1로 설정, 그보다 작으면 0 / 크면 1로 조정
            height = scrollRect.content.rect.height * 0.5f + (height - scrollRect.content.rect.height * 0.5f) * ((scrollRect.content.rect.height * 0.5f) / (scrollRect.content.rect.height * 0.5f - scrollRect.viewport.rect.height * 0.5f));
            if (height < 0f)
            {
                height = 0f;
            }
            else if (height > scrollRect.content.rect.height)
            {
                height = scrollRect.content.rect.height;
            }

            // 올라가는 연출
            StartReposition(height / scrollRect.content.rect.height);
            
            // 바로 이동
            //scrollRect.normalizedPosition = new Vector2(0f, height / scrollRect.content.rect.height);


            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] != null)
                    {
                        map[i, j].transform.localScale = Vector3.one;
                    }
                }
            }
        }

        base.OnEnable();
    }

    protected override void Update()
    {
        if (isRepositioning)
        {
            // 귀찮아서 걍 있는 거 씀
            if (Input.GetMouseButton(0) && Utils.IsMouseOverRecttTransform(scrollRect.viewport))
            {
                StopReposition();
            }
            else
            {
                UpdateReposition();
            }
        }

        if (isGettingLarger)
        {
            scaleAccessible += Time.deltaTime;
            if (scaleAccessible > 1.6f)
            {
                scaleAccessible = 1.6f;
                isGettingLarger = false;
            }
        }
        else
        {
            scaleAccessible -= Time.deltaTime;
            if (scaleAccessible < 1f)
            {
                scaleAccessible = 1f;
                isGettingLarger = true;
            }
        }

        // 선택 가능한 맵버튼 연출
        var currRoom = MapManager.Instance.GetCurrRoom();
        if (currRoom == null)
        {
            for (int i = 0; i < map.GetLength(1); i++)
            {
                if (map[0, i] != null)
                {
                    map[0, i].transform.localScale = scaleAccessible * Vector3.one;
                }
            }
        }
        else
        {
            List<MapManager.Room> accessibleRooms = MapManager.Instance.GetMap()[currRoom.Item1, currRoom.Item2].nextRooms;  
            for (int i = 0; i < accessibleRooms.Count; i++)
            {
                map[accessibleRooms[i].location.Item1, accessibleRooms[i].location.Item2].transform.localScale = scaleAccessible * Vector3.one;
            }
        }

        base.Update();
    }

    public void Set()
    {
        var rooms = MapManager.Instance.GetMap();
        map = new Button[StaticData.MAP_FLOORS, StaticData.MAP_ROOMS_FLOOR];

        for (int i = 0; i < StaticData.MAP_FLOORS; i++)
        {
            for (int j = 0; j < StaticData.MAP_ROOMS_FLOOR; j++)
            {
                if (rooms[i, j] == null)
                {
                    continue;
                }

                Button roomObject = Instantiate(prefabRoom, scrollRect.content);

                switch (rooms[i, j].type)
                {
                    case MapType.Monster:
                        roomObject.image.color = Color.gray;
                        break;
                    case MapType.Event:
                        roomObject.image.color = Color.white;
                        break;
                    case MapType.Elite:
                        roomObject.image.color = Color.magenta;
                        break;
                    case MapType.RestSite:
                        roomObject.image.color = Color.red;
                        break;
                    case MapType.Merchant:
                        roomObject.image.color = Color.green;
                        break;
                    case MapType.Treasure:
                        roomObject.image.color = Color.yellow;
                        break;
                    case MapType.Boss:
                        roomObject.image.color = Color.black;
                        break;
                }

                int floor = i;
                int room = j;
                roomObject.onClick.AddListener(
                    () =>
                    {
                        MapManager.Instance.EnterRoom(floor, room);
                    });
                Vector3 originalPos = new Vector3((j - (StaticData.MAP_ROOMS_FLOOR - 1) / 2f) * (SIZE_ROOM.x + SPACING.x), i * (SIZE_ROOM.y + SPACING.y) + 400f, 0f);
                // 그리드로 자리 배정 후 Random하게 퍼지도록
                Vector3 available = 0.5f * SPACING - SIZE_ROOM;
                Vector3 offSet = new Vector3(Random.Range(-available.x, available.x), Random.Range(-available.y, available.y), 0f);
                roomObject.transform.localPosition = originalPos + offSet;
                map[i, j] = roomObject;
            }
        }

        float lastRoomHeight = 0f;
        for (int i = 0; i < StaticData.MAP_ROOMS_FLOOR; i++)
        {
            if (map[StaticData.MAP_FLOORS - 1, i] == null)
            {
                continue;
            }
            float height = map[StaticData.MAP_FLOORS - 1, i].transform.localPosition.y;
            lastRoomHeight = lastRoomHeight > height ? lastRoomHeight : height;
        }

        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, lastRoomHeight + SIZE_ROOM.y + 200f);
        scrollRect.normalizedPosition = new Vector2(0.5f, 1f);

        DrawPaths();

        StartReposition(0f);
        //StartCoroutine(CoRepositionStartPoint());
    }

    private void DrawPaths()
    {
        var paths = MapManager.Instance.GetPaths();
        for (int i = 0; i < paths.GetLength(0); i++)
        {
            for (int j = 1; j < paths.GetLength(1); j++)
            {
                if (i > 0)
                {
                    // 경로 중첩되면 이전에 그렸기 때문에 다시 안 그림
                    if (paths[i, j - 1] == paths[i - 1, j - 1] &&
                        paths[i, j] == paths[i - 1, j])
                    {
                        continue;
                    }
                }

                LineUIDrawer lineDrawer = Instantiate(prefabPathDrawer, parentPaths);
                lineDrawer.points = new RectTransform[2];
                lineDrawer.points[0] = map[j, paths[i, j]].GetComponent<RectTransform>();
                lineDrawer.points[1] = map[j - 1, paths[i, j - 1]].GetComponent<RectTransform>();
            }
        }
    }

    private void StartReposition(float _normalizedPosY)
    {
        isRepositioning = true;
        lerpReposition = 0f;
        scrollRect.vertical = false;
        destination = _normalizedPosY;
    }

    private void StopReposition()
    {
        isRepositioning = false;
        scrollRect.vertical = true;
    }

    private void UpdateReposition()
    {
        lerpReposition += Time.deltaTime * 0.01f * speedReposition;
        float currY = Mathf.Lerp(scrollRect.normalizedPosition.y, destination, lerpReposition);
        if (Mathf.Abs(currY - destination) < 0.001f)
        {
            currY = destination;
            StopReposition();
        }
        scrollRect.normalizedPosition = new Vector2(0.5f, currY);
    }

    private void OnDrawGizmos()
    {
        var paths = MapManager.Instance?.GetPaths();
        if (paths == null)
        {
            return;
        }

        for (int i = 0; i < paths.GetLength(0); i++)
        {
            switch (i)
            {
                case 0:
                    Gizmos.color = Color.red;
                    break;
                case 1:
                    Gizmos.color = Color.grey;
                    break;
                case 2:
                    Gizmos.color = Color.white;
                    break;
                case 3:
                    Gizmos.color = Color.yellow;
                    break;
                case 4:
                    Gizmos.color = Color.green;
                    break;
                case 5:
                    Gizmos.color = Color.black;
                    break;
            }
            for (int j = 0; j < paths.GetLength(1); j++)
            {
                if (j > 0)
                {
                    Gizmos.DrawLine(map[j - 1, paths[i, j - 1]].transform.position, map[j, paths[i, j]].transform.position);
                }
            }
        }
    }
}
