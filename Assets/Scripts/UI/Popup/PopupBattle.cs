using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupBattle : Popup
{
    [SerializeField]
    private TMP_Text textEnergy = null;

    [SerializeField]
    private CharacterBattleUI playerInfo = null;

    [SerializeField]
    private List<CharacterBattleUI> enemyInfos = null;

    [SerializeField]
    private Transform drawPile = null;

    [SerializeField]
    private RectTransform rectHandArea = null;
    [SerializeField]
    private ItemCard prefabItemCard = null;
    private List<ItemCard> cardList = new List<ItemCard>();

    [SerializeField]
    private GameObject objectTargeting = null;

    [SerializeField]
    private Transform posUsingCard = null;

    [SerializeField]
    private Button btnTurnEnd = null;

    [SerializeField]
    private Graphic[] graphicsAnnouncement = null;
    [SerializeField]
    private TMP_Text textAnnouncement = null;
    private Coroutine coAnnounce = null;

    [Header("ī�并ġ ����")]
    [SerializeField]
    private TMP_Text textCountDrawpile = null;
    [SerializeField]
    private TMP_Text textCountDiscardpile = null;
    [SerializeField]
    private TMP_Text textCountExhaustpile = null;

    // ��� �ּ�ȭ�ϱ� ���� ī�� ���� ������ ������ ���� ��ġ, ����, ������ �� ���
    private int lastCardCount = 0;

    [Header("ī�� ���а� �ձ۰� �����Ƿ�, RADIUS��ŭ y�� �Ʒ� ������ �������� �ϴ� ���� �ѷ��� ������ ���� ���̷� ī�� ��ġ")]
    [SerializeField]
    private float RADIUS = 4000f;
    // ���� ��ü ����
    [SerializeField]
    private float TOTAL_ROTATION = 18f;
    // ī�� ���� �ִ� ���� ����
    [SerializeField]
    private float ROT_INTERVAL_MAX = 3f;

    // ī���� ��ġ�� �������ִ� ��������, ī�� ��ü�� ���ư� ����
    private float rotInterval = 0f;
    // ī�� �ּ� ���� (�� ������ ī�� ����)
    private float rotMin = 0f;
    // ī�� �ִ� ���� (�� ���� ī�� ����)
    private float rotMax = 0f;

    [Header("Scale")]
    // ī�� ������
    private float scale = 0f;
    [SerializeField]
    private float SCALE_HAND = 0.75f;
    [SerializeField]
    private int COUNT_MAINTAIN_SCALE = 6;
    [SerializeField]
    private float SCALE_INTERVAL = 0.02f;

    [Header("���� �ӵ�")]
    [SerializeField]
    private float SPEED_UPDATE_SCALE = 1f;
    [SerializeField]
    private float SPEED_UPDATE_ROTATION = 1f;
    [SerializeField]
    private float SPEED_UPDATE_POSITION = 1f;
    // ���콺 ����ٴϴ� �ӵ�
    [SerializeField]
    private float SPEED_UPDATE_POSITION_MOUSE = 10f;


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        if (cardList.Count != lastCardCount)
        {
            OnChangedCardCount();
            lastCardCount = cardList.Count;
        }

        int indexCardLookingAt = IndexCardLookingAt();

        for (int i = 0; i < cardList.Count; i++)
        {
            if (cardList[i].GetState() > ItemCard.CARD_STATE.NORMAL)
            {
                // ���콺 �����Ͱ� ����� ī�带 ������ ������ ���� ����
                cardList[i].transform.SetSiblingIndex(cardList.Count);
            }
            else
            {
                // ���� �ִ� ī�尡 ������, �� ���� ī����� ���� �ϳ��� ����
                cardList[i].transform.SetSiblingIndex(indexCardLookingAt >= 0 && i > indexCardLookingAt ? i - 1 : i);
            }

            switch (cardList[i].GetState())
            {
                case ItemCard.CARD_STATE.DRAW:
                    {
                        UpdateCardScale(i, scale);
                        UpdateCardRotation(i, GetCardRot(i));
                        Vector3 nextPos = new Vector3(GetCardNextPosX(i, indexCardLookingAt), GetCardNextPosY(i), 0f);
                        UpdateCardPosition(i, nextPos);

                        // ��ο� ������ ������ �� ������
                        if (Vector3.Distance(cardList[i].transform.localPosition, nextPos) < 5f)
                        {
                            cardList[i].SetState(ItemCard.CARD_STATE.NORMAL);
                        }
                    }
                    break;
                case ItemCard.CARD_STATE.NORMAL:
                    {
                        if (i == indexCardLookingAt)
                        {
                            cardList[i].transform.localScale = Vector3.one;
                            cardList[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                            cardList[i].transform.localPosition = new Vector3(GetCardNextPosX(i, indexCardLookingAt), StaticData.CARD_SIZE_HEIGHT * 0.5f, 0f);
                        }
                        else
                        {
                            UpdateCardScale(i, scale);
                            UpdateCardRotation(i, GetCardRot(i));
                            UpdateCardPosition(i, GetCardNextPosX(i, indexCardLookingAt), GetCardNextPosY(i));
                        }
                    }
                    break;
                case ItemCard.CARD_STATE.SELECTED:
                    {
                        cardList[i].transform.localScale = Vector3.one;
                        cardList[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                        // ī�� ���콺 ��ġ ���� �̵�
                        UpdateCardPositionToMouse(i);

                        // ���콺 ��Ŭ�� �� Deselect
                        if (Input.GetMouseButtonDown(1))
                        {
                            DeselectCard(cardList[i]);
                        }
                        else
                        {
                            // ���콺�� hand ������ ����� ��,
                            if (!Utils.IsMouseOverRecttTransform(rectHandArea))
                            {
                                int usable = cardList[i].GetUsable();
                                // �������� ī�带 ����ϱ⿡ �����ϸ� Deselect
                                if (usable != 100)
                                {
                                    switch (usable)
                                    {
                                        case 101:
                                            Announce("�������� ������");
                                            break;
                                        case 102:
                                            Announce("ī�� Ư�� - ���Ұ�");
                                            break;
                                    }
                                    DeselectCard(cardList[i]);
                                }
                                else
                                {
                                    if (cardList[i].GetData().IsTargetCard())
                                    {
                                        //LogManager.Log("IsTargetCard = " + cardList[i].GetData().IsTargetCard());
                                        // ����ī�� Ÿ���� ����
                                        StartTargeting(cardList[i]);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case ItemCard.CARD_STATE.TARGETING:
                    {
                        UpdateCardScale(i, scale);
                        cardList[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                        UpdateCardPosition(i, 0f, StaticData.CARD_SIZE_HEIGHT * 0.5f * scale);

                        // ���콺 ��Ŭ�� �� Deselect
                        if (Input.GetMouseButtonDown(1))
                        {
                            StopTargeting(cardList[i]);
                        }
                        else if (Input.GetMouseButtonDown(0))
                        {
                            // popupbattle�� �ű� �Ŀ� ���ʹ� ����Ʈ ��ȸ�ϸ鼭 Utils.mouse rect �˻� �� cards[i].Use(target);
                            List<Enemy> enemyList = BattleManager.Instance.GetEnemyList();
                            for (int j = 0; j < enemyList.Count; j++)
                            {
                                if (Utils.IsMouseOverRecttTransform(enemyInfos[j].GetComponent<RectTransform>()) == true) 
                                {
                                    UseCard(cardList[i], enemyList[j]);
                                    break;
                                }
                            }
                        }
                        // ���콺 �����Ϳ� �̾����� Ÿ���� ����
                        UpdateObjectTargeting(cardList[i].transform.position);
                    }
                    break;
                case ItemCard.CARD_STATE.USING:
                    {
                        UpdateCardScale(i, scale);
                        cardList[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                        UpdateCardPosition(i, posUsingCard.position, false);
                        if (Vector3.Distance(cardList[i].transform.position, posUsingCard.position) < 5f)
                        {
                            cardList[i].EndUsing();
                        }
                    }
                    break;
                case ItemCard.CARD_STATE.DISCARD:
                    DiscardCard(cardList[i]);
                    break;
                case ItemCard.CARD_STATE.EXHAUSTED:
                    break;
                default:
                    break;
            }
        }

        textCountDrawpile.text = BattleManager.Instance.GetDrawPile().Count.ToString();
        textCountDiscardpile.text = BattleManager.Instance.GetDiscardPile().Count.ToString();
        textCountExhaustpile.text = BattleManager.Instance.GetExhaustPile().Count.ToString();

        base.Update();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void SetEnergy(int _energy, int _refillEnergy)
    {
        textEnergy.text = string.Format(_energy + "/" +  _refillEnergy);
    }

    public void ActivateBtnTurnEnd(bool _active)
    {
        btnTurnEnd.interactable = _active;
    }

    public void TurnEnd()
    {
        BattleManager.Instance.TurnEnd();
    }

    public CharacterBattleUI GetPlayerInfo() => playerInfo;
    public List<CharacterBattleUI> GetEnemyInfos() => enemyInfos;

    public void ButtonDrawPile()
    {
        PopupDrawPile popupDrawPile = UIManager.Instance.MakePopup<PopupDrawPile>();
        popupDrawPile.SetCards(BattleManager.Instance.GetDrawPile());
    }

    public void ButtonDiscardPile()
    {
        PopupDiscardPile popupDiscardPile = UIManager.Instance.MakePopup<PopupDiscardPile>();
        popupDiscardPile.SetCards(BattleManager.Instance.GetDiscardPile());
    }

    public void ButtonExhaustPile()
    {
        PopupExhaustPile popupExhaustPile = UIManager.Instance.MakePopup<PopupExhaustPile>();
        popupExhaustPile.SetCards(BattleManager.Instance.GetExhaustPile());
    }
    
    // ��� �ּ�ȭ�ϱ� ���ؼ� ī�� ���� �ٱ;��� ���� ����
    private void OnChangedCardCount()
    {
        if (cardList.Count > 0)
        {
            // ������ ����
            scale = cardList.Count <= COUNT_MAINTAIN_SCALE ? SCALE_HAND : SCALE_HAND - (SCALE_INTERVAL * (cardList.Count - 5));
            // ��ī��� ������ ��, rotInterval��ŭ ����
            // �ִ� ���� ���̴� 3���� �ϴ� ����
            rotInterval = TOTAL_ROTATION / (cardList.Count - 1);
            rotInterval = rotInterval > ROT_INTERVAL_MAX ? ROT_INTERVAL_MAX : rotInterval;
            // �ִ밢 (���� ���� ī�� ����)
            rotMax = rotInterval * (cardList.Count - 1) / 2;
            // �ּҰ� (���� ���� ī�� ����)
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
        // ���õ� ī�尡 Ÿ���� ���̸� ������ ���� ���ڸ���
        if (_IndexCardLookingAt >= 0 && cardList[_IndexCardLookingAt].GetState() != ItemCard.CARD_STATE.TARGETING)
        {
            int diff = _IndexCardLookingAt - _index;
            nextPosX += _index == _IndexCardLookingAt ? 0 : -StaticData.CARD_SIZE_WIDTH * 0.22f / (Mathf.Sign(diff) * Mathf.Pow(diff, 2));
        }
        return nextPosX;
    }

    private float GetCardNextPosY(int _index)
    {
        return -RADIUS + Mathf.Cos(GetCardRot(_index) / 180f * Mathf.PI) * RADIUS - (Mathf.Abs(_index - (cardList.Count - 1) / 2f) * (30f - 2f * cardList.Count) - 90f);
    }

    private void UpdateCardScale(int _index, float _scale)
    {
        float currScale = cardList[_index].transform.localScale.x;
        cardList[_index].transform.localScale = Mathf.Abs(currScale - _scale) < 0.01f ?
            Vector3.one * _scale : Vector3.one * Mathf.Lerp(currScale, _scale, Time.deltaTime * 10f * SPEED_UPDATE_SCALE);
    }

    private void UpdateCardRotation(int _index, float _nextRotation)
    {
        float currRot = cardList[_index].transform.localRotation.eulerAngles.z;
        currRot = currRot > 180 ? currRot - 360 : currRot;
        cardList[_index].transform.localRotation = Mathf.Abs(currRot - _nextRotation) < 1 ?
            Quaternion.Euler(0f, 0f, _nextRotation) : Quaternion.Euler(0f, 0f, Mathf.Lerp(currRot, _nextRotation, Time.deltaTime * 10f * SPEED_UPDATE_ROTATION));
    }
    private void UpdateCardPosition(int _index, float _nextPosX, float _nextPosY, bool _isLocalPosition = true)
    {
        float currPosX = _isLocalPosition ? cardList[_index].transform.localPosition.x : cardList[_index].transform.position.x;
        float currPosY = _isLocalPosition ? cardList[_index].transform.localPosition.y : cardList[_index].transform.position.y;
        float posX = Mathf.Abs(currPosX - _nextPosX) < 1 ? _nextPosX : Mathf.Lerp(currPosX, _nextPosX, Time.deltaTime * 10f * SPEED_UPDATE_POSITION);
        float posY = Mathf.Abs(currPosY - _nextPosY) < 1 ? _nextPosY : Mathf.Lerp(currPosY, _nextPosY, Time.deltaTime * 10f * SPEED_UPDATE_POSITION);
        if (_isLocalPosition)
        {
            cardList[_index].transform.localPosition = new Vector3(posX, posY, 0f);
        }
        else
        {
            cardList[_index].transform.position = new Vector3(posX, posY, 0f);
        }
    }

    private void UpdateCardPosition(int _index, Vector3 _nextPos, bool _isLocalPosition = true)
    {
        UpdateCardPosition(_index, _nextPos.x, _nextPos.y, _isLocalPosition);
    }

    private void UpdateCardPositionToMouse(int _index)
    {
        float posX = Mathf.Lerp(cardList[_index].transform.position.x, Input.mousePosition.x, Time.deltaTime * 10f * SPEED_UPDATE_POSITION_MOUSE);
        float posY = Mathf.Lerp(cardList[_index].transform.position.y, Input.mousePosition.y, Time.deltaTime * 10f * SPEED_UPDATE_POSITION_MOUSE);
        cardList[_index].transform.position = new Vector3(posX, posY, 0f);
    }

    private int IndexCardLookingAt()
    {
        int indexCardSelected = IndexCardSelected();
        if (indexCardSelected >= 0)
        {
            return indexCardSelected;
        }

        for (int i = 0; i < cardList.Count; i++)
        {
            if (cardList[i].GetLookingAt() == true)
            {
                return i;
            }
        }

        return -1;
    }

    private int IndexCardSelected()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            if (cardList[i].GetState() > ItemCard.CARD_STATE.NORMAL)
            {
                return i;
            }
        }

        return -1;
    }

    public void DrawCard(CardData _data)
    {
        ItemCard newCard = Instantiate(prefabItemCard, rectHandArea);
        newCard.transform.position = drawPile.position;
        newCard.transform.localScale = Vector3.zero;
        newCard.Set(_data);
        newCard.SetState(ItemCard.CARD_STATE.DRAW);
        newCard.SetActionClicked(
            () =>
            {
                switch (newCard.GetState())
                {
                    case ItemCard.CARD_STATE.NORMAL:
                        SelectCard(newCard);
                        break;
                    case ItemCard.CARD_STATE.SELECTED:
                        {
                            // hand ���� �ȿ��� ī�� ��Ŭ�� �� Deselect
                            if (Utils.IsMouseOverRecttTransform(rectHandArea))
                            {
                                DeselectCard(newCard);
                            }
                            else
                            {
                                bool isTargetCard = false;
                                var actionList = newCard.GetData().actionList;
                                for (int i = 0; i < actionList.Count; i++)
                                {
                                    if (actionList[i].Target == CardAction.CardAction.TargetType.SingleEnemy)
                                    {
                                        isTargetCard = true;
                                        break;
                                    }
                                }

                                if (isTargetCard)
                                {
                                    StartTargeting(newCard);
                                }
                                else
                                {
                                    UseCard(newCard);
                                }
                            }
                        }
                        break;
                    case ItemCard.CARD_STATE.TARGETING:
                        break;
                    case ItemCard.CARD_STATE.USING:
                        break;
                    case ItemCard.CARD_STATE.DISCARD:
                        break;
                    case ItemCard.CARD_STATE.EXHAUSTED:
                        break;
                    default:
                        break;
                }
            });
        cardList.Add(newCard);
    }

    public void SelectCard(ItemCard _itemCard)
    {
        _itemCard.SetState(ItemCard.CARD_STATE.SELECTED);
    }

    public void DeselectCard(ItemCard _itemCard)
    {
        _itemCard.SetState(ItemCard.CARD_STATE.NORMAL);
    }

    public void UseCard(ItemCard _itemCard, Character _target = null)
    {
        StopTargeting(_itemCard);
        _itemCard.Use(_target);
    }

    public void DiscardCard(ItemCard _itemCard)
    {
        BattleManager.Instance.DiscardCard(cardList.IndexOf(_itemCard));
        cardList.Remove(_itemCard);
        Destroy(_itemCard.gameObject);
    }

    public void ExhaustCard(ItemCard _itemCard)
    {
        BattleManager.Instance.ExhaustCard(cardList.IndexOf(_itemCard));
        cardList.Remove(_itemCard);
        Destroy(_itemCard.gameObject);
    }

    // BattleManager���� PopupBattle�� Discard�� �䱸�ϴ� ������ ��� - �� ���� �����ϰ� PopupBattle �������� Discard����
    public void DiscardAll()
    {
        while (cardList.Count > 0)
        {
            if (cardList[0].GetData().featureList.Exists((feature) => feature.type == CARD_FEATURE.EXHAUST))
            {
                ExhaustCard(cardList[0]);
            }
            else
            {
                DiscardCard(cardList[0]);
            }
        }
    }

    public void StartTargeting(ItemCard _itemCard)
    {
        int usable = _itemCard.GetUsable();
        if (usable != 100)
        {
            switch (usable)
            {
                case 101:
                    Announce("�������� ������");
                    break;
                case 102:
                    Announce("ī�� Ư�� - ���Ұ�");
                    break;
            }
            DeselectCard(_itemCard);
            return;
        }

        _itemCard.SetState(ItemCard.CARD_STATE.TARGETING);
        objectTargeting.SetActive(true);
        UpdateObjectTargeting(_itemCard.transform.position);
    }

    public void StopTargeting(ItemCard _itemCard)
    {
        _itemCard.SetState(ItemCard.CARD_STATE.NORMAL);
        objectTargeting.SetActive(false);
    }

    private void UpdateObjectTargeting(Vector3 _posCard)
    {
        float lerp = 0f;
        Vector3 lastPos = _posCard;
        float posDiffX = Input.mousePosition.x - _posCard.x;
        float midPosX = _posCard.x - (Mathf.Abs(posDiffX) < 120f ? posDiffX : Mathf.Sign(posDiffX) * 120f);
        float midPosY = _posCard.y + (Input.mousePosition.y - _posCard.y) * 1.4f;
        Vector3 midPos = new Vector3(midPosX, midPosY, 0f);
        for (int i = 0; i < objectTargeting.transform.childCount; i++)
        {
            lerp = (float)i / (objectTargeting.transform.childCount - 1);
            Vector3 newPos = Utils.BezierCurvesVector3(_posCard, midPos, Input.mousePosition, lerp);
            objectTargeting.transform.GetChild(i).position = newPos;
            Vector3 dir = (newPos - lastPos).normalized;
            objectTargeting.transform.GetChild(i).rotation = Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f, Vector3.forward);
            lastPos = newPos;
        }
    }

    public void Announce(string _str)
    {
        if (coAnnounce != null)
        {
            StopCoroutine(coAnnounce);
        }
        coAnnounce = StartCoroutine(CoAnnounce(_str));
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
