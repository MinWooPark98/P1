using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectHand : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectHand = null;

    [SerializeField]
    private ItemCard prefabItemCard = null;

    [SerializeField]
    private Image imgTargeting = null;


    private List<ItemCard> cards = new List<ItemCard>();

    // ��� �ּ�ȭ�ϱ� ���� ī�� ���� ������ ������ ���� ��ġ, ����, ������ �� ���
    private int lastCardCount = 0;

    [Header("ī�� �԰�")]
    [SerializeField]
    private float CARD_SIZE_WIDTH = 320;
    [SerializeField]
    private float CARD_SIZE_HEIGHT = 480f;

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
    private float SCALE_HAND = 0.8f;
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
    private float SPEED_UPDATE_POSITION = 2f;
    [SerializeField]
    private float SPEED_UPDATE_POSITION_MOUSE = 10f;


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
            // Ÿ�������̶��
            if (cards[i].GetTargeting() == true)
            {
                cards[i].transform.SetSiblingIndex(cards.Count);
                UpdateCardScale(i, scale);
                cards[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                UpdateCardPosition(i, 0f, CARD_SIZE_HEIGHT * 0.5f * scale);
                UpdateTargetingObjectPosition();

                // ���콺 ��Ŭ�� �� Deselect
                if (Input.GetMouseButtonDown(1))
                {
                    DeselectCard(cards[i]);
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    // popupbattle�� �ű� �Ŀ� ���ʹ� ����Ʈ ��ȸ�ϸ鼭 Utils.mouse rect �˻� �� cards[i].Use(target);
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

            // ������ ���� �ִ� ���̰�
            if (i == indexCardLookingAt)
            {
                // ���콺 �����Ͱ� ����� ī�带 ������ ������ ���� ����
                cards[i].transform.SetSiblingIndex(cards.Count);

                // �������� ī����
                if (cards[i].GetSelected() == true)
                {
                    // ���콺 ��Ŭ�� �� Deselect
                    if (Input.GetMouseButtonDown(1))
                    {
                        DeselectCard(cards[i]);
                    }
                    else
                    {
                        // ���콺�� hand ������ ����� ��,
                        if (!Utils.IsMouseOverRecttTransform(rectHand))
                        {
                            // �������� ī�带 ����ϱ⿡ �����ϸ� Deselect
                            if (cards[i].GetUsable() == false)
                            {
                                DeselectCard(cards[i]);
                            }
                            else
                            {
                                if (cards[i].GetData().GetType() == typeof(AttackCard) && ((AttackCard)cards[i].GetData()).atkAllEnemies == false)
                                {
                                    // ����ī�� Ÿ���� ����
                                    StartTargeting(cards[i]);
                                }
                            }
                        }

                        cards[i].transform.localScale = Vector3.one;
                        cards[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                        // ī�� ���콺 ��ġ ���� �̵�
                        UpdateCardPositionToMouse(i);

                        continue;
                    }
                }
                cards[i].transform.localScale = Vector3.one;
                cards[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                cards[i].transform.localPosition = new Vector3(GetCardNextPosX(i, indexCardLookingAt), CARD_SIZE_HEIGHT * 0.5f, 0f);
                continue;
            }

            // ���� �ִ� ī�尡 ������, �� ���� ī����� ���� �ϳ��� ����
            cards[i].transform.SetSiblingIndex(indexCardLookingAt >= 0 && i > indexCardLookingAt ? i - 1 : i);

            UpdateCardScale(i, scale);
            UpdateCardRotation(i, GetCardRot(i));
            UpdateCardPosition(i, GetCardNextPosX(i, indexCardLookingAt), GetCardNextPosY(i));
        }
    }

    // ��� �ּ�ȭ�ϱ� ���ؼ� ī�� ���� �ٱ;��� ���� ����
    private void OnChangedCardCount()
    {
        if (cards.Count > 0)
        {
            // ������ ����
            scale = cards.Count <= COUNT_MAINTAIN_SCALE ? SCALE_HAND : SCALE_HAND - (SCALE_INTERVAL * (cards.Count - 5));
            // ��ī��� ������ ��, rotInterval��ŭ ����
            // �ִ� ���� ���̴� 3���� �ϴ� ����
            rotInterval = TOTAL_ROTATION / (cards.Count - 1);
            rotInterval = rotInterval > ROT_INTERVAL_MAX ? ROT_INTERVAL_MAX : rotInterval;
            // �ִ밢 (���� ���� ī�� ����)
            rotMax = rotInterval * (cards.Count - 1) / 2;
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
        if (_IndexCardLookingAt >= 0)
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

    public void DrawCard(CardData _data)
    {
        ItemCard newCard = Instantiate(prefabItemCard, transform);
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

                // hand ���� �ȿ��� ī�� ��Ŭ�� �� Deselect
                if (Utils.IsMouseOverRecttTransform(rectHand))
                {
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

                //BattleManager.Instance.SelectCard(newCard);
            });
        cards.Add(newCard);
    }

    public void StartTargeting(ItemCard _itemCard)
    {
        if (_itemCard.GetUsable() == false)
        {
            LogManager.Log("Ÿ���� ����  =>  ������ ����");
            DeselectCard(_itemCard);
            return;
        }

        _itemCard.SetTargeting(true);
        imgTargeting.gameObject.SetActive(true);
        UpdateTargetingObjectPosition();
    }

    public void StopTargeting(ItemCard _itemCard)
    {
        _itemCard.SetTargeting(false);
        imgTargeting.gameObject.SetActive(false);
    }

    private void UpdateTargetingObjectPosition()
    {
        imgTargeting.transform.position = Input.mousePosition;
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
}
