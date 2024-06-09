using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectHand : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectHandArea = null;

    [SerializeField]
    private Transform parentCards = null;

    [SerializeField]
    private ItemCard prefabItemCard = null;

    private List<ItemCard> cards = new List<ItemCard>();

    [SerializeField]
    private Image imgTargeting = null;

    [SerializeField]
    private Transform lineTargeting = null;

    // 계산 최소화하기 위해 카드 수에 변동이 생겼을 때만 위치, 각도, 스케일 등 계산
    private int lastCardCount = 0;

    [Header("카드 규격")]
    [SerializeField]
    private float CARD_SIZE_WIDTH = 320;
    [SerializeField]
    private float CARD_SIZE_HEIGHT = 480f;

    [Header("카드 손패가 둥글게 있으므로, RADIUS만큼 y축 아래 지점을 중점으로 하는 원의 둘레로 일정한 각도 차이로 카드 배치")]
    [SerializeField]
    private float RADIUS = 4000f;
    // 손패 전체 각도
    [SerializeField]
    private float TOTAL_ROTATION = 18f;
    // 카드 간의 최대 각도 차이
    [SerializeField]
    private float ROT_INTERVAL_MAX = 3f;

    // 카드의 위치를 결정해주는 각도이자, 카드 자체가 돌아간 각도
    private float rotInterval = 0f;
    // 카드 최소 각도 (맨 오른쪽 카드 각도)
    private float rotMin = 0f;
    // 카드 최대 각도 (맨 왼쪽 카드 각도)
    private float rotMax = 0f;

    [Header("Scale")]
    // 카드 스케일
    private float scale = 0f;
    [SerializeField]
    private float SCALE_HAND = 0.8f;
    [SerializeField]
    private int COUNT_MAINTAIN_SCALE = 6;
    [SerializeField]
    private float SCALE_INTERVAL = 0.02f;

    [Header("적용 속도")]
    [SerializeField]
    private float SPEED_UPDATE_SCALE = 1f;
    [SerializeField]
    private float SPEED_UPDATE_ROTATION = 1f;
    [SerializeField]
    private float SPEED_UPDATE_POSITION = 2f;
    [SerializeField]
    private float SPEED_UPDATE_POSITION_MOUSE = 10f;

    // 걍 표시하기 위한 임시 -> objectHand 스크립트 없애고 PopupBattle로 다 옮길까 생각중이라서 일단 쓰기 편하게 여기다 둠
    [SerializeField]
    private Graphic[] graphicsAnnouncement = null;
    [SerializeField]
    private TMP_Text textAnnouncement = null;
    private Coroutine coroutine = null;


    private void Update()
    {
        if (cards.Count != lastCardCount)
        {
            OnChangedCardCount();
            lastCardCount = cards.Count;
        }

        int indexCardLookingAt = IndexCardLookingAt();

        for (int i = 0; i < cards.Count; i++)
        {
            // 타겟팅중이라면
            if (cards[i].GetTargeting() == true)
            {
                cards[i].transform.SetSiblingIndex(cards.Count);
                UpdateCardScale(i, scale);
                cards[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                UpdateCardPosition(i, 0f, CARD_SIZE_HEIGHT * 0.5f * scale);
                UpdateTargetingObjectPosition();
                UpdateLineTargetingPosition(cards[i].transform.position);

                // 마우스 우클릭 시 Deselect
                if (Input.GetMouseButtonDown(1))
                {
                    DeselectCard(cards[i]);
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    // popupbattle로 옮긴 후에 에너미 리스트 순회하면서 Utils.mouse rect 검사 후 cards[i].Use(target);
                    List<Character> enemyList = BattleManager.Instance.GetEnemyList();
                    for (int j = 0; j < enemyList.Count; j++)
                    {
                        if (Utils.IsMouseOverRecttTransform(enemyList[j].GetCharacterInfo().GetComponent<RectTransform>()) == true)
                        {
                            UseCard(cards[i], enemyList[j]);
                            break;
                        }
                    }
                }
                continue;
            }
            // 정보를 보고 있는 중이고
            else if (i == indexCardLookingAt)
            {
                // 마우스 포인터가 얹어진 카드를 앞으로 꺼내서 정보 보기
                cards[i].transform.SetSiblingIndex(cards.Count);

                // 선택중인 카드라면
                if (cards[i].GetSelected() == true)
                {
                    // 마우스 우클릭 시 Deselect
                    if (Input.GetMouseButtonDown(1))
                    {
                        DeselectCard(cards[i]);
                    }
                    else
                    {
                        // 마우스가 hand 범위를 벗어났을 때,
                        if (!Utils.IsMouseOverRecttTransform(rectHandArea))
                        {
                            // 에너지가 카드를 사용하기에 부족하면 Deselect
                            if (cards[i].GetUsable() == false)
                            {
                                Announce("에너지가 부족해");
                                DeselectCard(cards[i]);
                            }
                            else
                            {
                                if (cards[i].GetData().GetType() == typeof(AttackCard) && ((AttackCard)cards[i].GetData()).atkAllEnemies == false)
                                {
                                    // 공격카드 타겟팅 시작
                                    StartTargeting(cards[i]);
                                }
                            }
                        }

                        cards[i].transform.localScale = Vector3.one;
                        cards[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                        // 카드 마우스 위치 따라 이동
                        UpdateCardPositionToMouse(i);

                        continue;
                    }
                }
                cards[i].transform.localScale = Vector3.one;
                cards[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                cards[i].transform.localPosition = new Vector3(GetCardNextPosX(i, indexCardLookingAt), CARD_SIZE_HEIGHT * 0.5f, 0f);
                continue;
            }

            // 보고 있는 카드가 있으면, 그 다음 카드부터 순서 하나씩 당기기
            cards[i].transform.SetSiblingIndex(indexCardLookingAt >= 0 && i > indexCardLookingAt ? i - 1 : i);

            UpdateCardScale(i, scale);
            UpdateCardRotation(i, GetCardRot(i));
            UpdateCardPosition(i, GetCardNextPosX(i, indexCardLookingAt), GetCardNextPosY(i));
        }
    }

    // 계산 최소화하기 위해서 카드 갯수 바귀었을 때만 실행
    private void OnChangedCardCount()
    {
        if (cards.Count > 0)
        {
            // 스케일 조정
            scale = cards.Count <= COUNT_MAINTAIN_SCALE ? SCALE_HAND : SCALE_HAND - (SCALE_INTERVAL * (cards.Count - 5));
            // 옆카드와 비교했을 때, rotInterval만큼 차이
            // 최대 각도 차이는 3으로 일단 설정
            rotInterval = TOTAL_ROTATION / (cards.Count - 1);
            rotInterval = rotInterval > ROT_INTERVAL_MAX ? ROT_INTERVAL_MAX : rotInterval;
            // 최대각 (가장 좌측 카드 각도)
            rotMax = rotInterval * (cards.Count - 1) / 2;
            // 최소각 (가장 우측 카드 각도)
            rotMin = -rotMax;
        }
    }

    private float GetCardRot(int _index)
    {
        return rotMax - rotInterval * _index;
    }
    private float GetCardNextPosX(int _index, int _IndexCardLookingAt)
    {
        float nextPosX = -Mathf.Sin(GetCardRot(_index) / 180f * Mathf.PI) * RADIUS;
        // 선택된 카드가 타겟팅 중이면 벌어진 간격 제자리로
        if (_IndexCardLookingAt >= 0 && cards[_IndexCardLookingAt].GetTargeting() == false)
        {
            int diff = _IndexCardLookingAt - _index;
            nextPosX += _index == _IndexCardLookingAt ? 0 : -CARD_SIZE_WIDTH * 0.22f / (Mathf.Sign(diff) * Mathf.Pow(diff, 2));
        }
        return nextPosX;
    }

    private float GetCardNextPosY(int _index)
    {
        return -RADIUS + Mathf.Cos(GetCardRot(_index) / 180f * Mathf.PI) * RADIUS - (Mathf.Abs(_index - (cards.Count - 1) / 2f) * (30f - 2f * cards.Count) - 90f);
    }

    private void UpdateCardScale(int _index, float _scale)
    {
        float currScale = cards[_index].transform.localScale.x;
        cards[_index].transform.localScale = Mathf.Abs(currScale - _scale) < 0.01f ?
            Vector3.one * _scale : Vector3.one * Mathf.Lerp(currScale, _scale, Time.deltaTime * 10f * SPEED_UPDATE_SCALE);
    }

    private void UpdateCardRotation(int _index, float _nextRotation)
    {
        float currRot = cards[_index].transform.localRotation.eulerAngles.z;
        currRot = currRot > 180 ? currRot - 360 : currRot;
        cards[_index].transform.localRotation = Mathf.Abs(currRot - _nextRotation) < 1 ?
            Quaternion.Euler(0f, 0f, _nextRotation) : Quaternion.Euler(0f, 0f, Mathf.Lerp(currRot, _nextRotation, Time.deltaTime * 10f * SPEED_UPDATE_ROTATION));
    }
    private void UpdateCardPosition(int _index, float _nextPosX, float _nextPosY)
    {
        float currPosX = cards[_index].transform.localPosition.x;
        float posX = Mathf.Abs(currPosX - _nextPosX) < 1 ? _nextPosX : Mathf.Lerp(currPosX, _nextPosX, Time.deltaTime * 10f * SPEED_UPDATE_POSITION);
        float currPosY = cards[_index].transform.localPosition.y;
        float posY = Mathf.Abs(currPosY - _nextPosY) < 1 ? _nextPosY : Mathf.Lerp(currPosY, _nextPosY, Time.deltaTime * 10f * SPEED_UPDATE_POSITION);
        cards[_index].transform.localPosition = new Vector3(posX, posY, 0f);
    }

    private void UpdateCardPositionToMouse(int _index)
    {
        float posX = Mathf.Lerp(cards[_index].transform.position.x, Input.mousePosition.x, SPEED_UPDATE_POSITION_MOUSE);
        float posY = Mathf.Lerp(cards[_index].transform.position.y, Input.mousePosition.y, SPEED_UPDATE_POSITION_MOUSE);
        cards[_index].transform.position = new Vector3(posX, posY, 0f);
    }

    private int IndexCardLookingAt()
    {
        int indexCardSelected = IndexCardSelected();
        if (indexCardSelected >= 0)
        {
            return indexCardSelected;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].GetLookingAt() == true)
            {
                return i;
            }
        }

        return -1;
    }

    private int IndexCardSelected()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].GetSelected() == true)
            {
                return i;
            }
        }

        return -1;
    }

    public void DrawCard(CardData _data, Transform _transform)
    {
        ItemCard newCard = Instantiate(prefabItemCard, parentCards);
        newCard.transform.position = _transform.position;
        newCard.transform.localScale = Vector3.one * SCALE_HAND;
        newCard.Set(_data);
        newCard.SetActionClicked(
            () =>
            {
                if (newCard.GetSelected() == false)
                {
                    SelectCard(newCard);
                    return;
                }

                // hand 범위 안에서 카드 재클릭 시 Deselect
                if (Utils.IsMouseOverRecttTransform(rectHandArea))
                {
                    Announce("에너지가 부족해");
                    DeselectCard(newCard);
                }
                else
                {
                    if (newCard.GetData().GetType() == typeof(AttackCard) && ((AttackCard)newCard.GetData()).atkAllEnemies == true)
                    {
                        StartTargeting(newCard);
                    }
                    else
                    {
                        UseCard(newCard);
                    }
                }
            });
        cards.Add(newCard);
    }

    public void StartTargeting(ItemCard _itemCard)
    {
        if (_itemCard.GetUsable() == false)
        {
            Announce("에너지가 부족해");
            DeselectCard(_itemCard);
            return;
        }

        _itemCard.SetTargeting(true);
        imgTargeting.gameObject.SetActive(true);
        lineTargeting.gameObject.SetActive(true);
        UpdateTargetingObjectPosition();
        UpdateLineTargetingPosition(_itemCard.transform.position);
    }

    public void StopTargeting(ItemCard _itemCard)
    {
        _itemCard.SetTargeting(false);
        imgTargeting.gameObject.SetActive(false);
        lineTargeting.gameObject.SetActive(false);
    }

    private void UpdateTargetingObjectPosition()
    {
        imgTargeting.transform.position = Input.mousePosition;
    }

    private void UpdateLineTargetingPosition(Vector3 _posCard)
    {
        float lerp = 0f;
        Vector3 lastPos = _posCard;
        float posDiffX = Input.mousePosition.x - _posCard.x;
        float midPosX = _posCard.x - (Mathf.Abs(posDiffX) < 120f ? posDiffX : Mathf.Sign(posDiffX) * 120f);
        float midPosY = _posCard.y + (Input.mousePosition.y - _posCard.y) * 1.4f;
        Vector3 midPos = new Vector3(midPosX, midPosY, 0f);
        for (int i = 0; i < lineTargeting.childCount; i++)
        {
            lerp = (float)i / (lineTargeting.childCount - 1);
            Vector3 newPos = Utils.BezierCurvesVector3(_posCard, midPos, Input.mousePosition, lerp);
            lineTargeting.GetChild(i).position = newPos;
            Vector3 dir = (newPos - lastPos).normalized;
            lineTargeting.GetChild(i).rotation = Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f, Vector3.forward);
            lastPos = newPos;
        }
    }

    public void SelectCard(ItemCard _itemCard)
    {
        _itemCard.Select();
    }

    public void DeselectCard(ItemCard _itemCard)
    {
        StopTargeting(_itemCard);
        _itemCard.Deselect();
    }

    public void UseCard(ItemCard _itemCard, Character _target = null)
    {
        DeselectCard(_itemCard);
        _itemCard.Use(_target);
        _itemCard.SetClickable(false);
    }

    public void DiscardCard(ItemCard _itemCard)
    {
        _itemCard.SetClickable(false);
        _itemCard.gameObject.SetActive(false);
        cards.Remove(_itemCard);
    }

    public void Announce(string _str)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(CoAnnounce(_str));
    }

    private IEnumerator CoAnnounce(string _str)
    {
        textAnnouncement.text = _str;
        float albedo = 1f;
        bool loop = true;
        while (loop)
        {
            albedo -= Time.deltaTime;
            if (albedo < 0f)
            {
                albedo = 0f;
                loop = false;
            }

            for (int i = 0; i < graphicsAnnouncement.Length; i++)
            {
                Color color = graphicsAnnouncement[i].color;
                color.a = albedo;
                graphicsAnnouncement[i].color = color;
            }
            yield return null;
        }
    }
}
