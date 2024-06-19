using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager s_Instance;
    public static CardManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<CardManager>();
                if (s_Instance == null)
                {
                    GameObject obj = (GameObject)Instantiate(Resources.Load("Prefabs/Manager/CardManager"));
                    DontDestroyOnLoad(obj);
                }
            }

            return s_Instance;
        }
    }

    [SerializeField]
    private List<CardData> allCardList = new List<CardData>();                      // ��� ī�� ����

    [SerializeField]
    private List<CardData> deckList = new List<CardData>();                         // �÷��̾� ���� �� ����Ʈ

    private void Awake()
    {
        s_Instance = this;

        LoadAllCards();
    }

    private void OnDestroy()
    {
        s_Instance = null;
    }

    private void LoadAllCards()
    {
        allCardList = Resources.LoadAll<CardData>("Scriptables/CardData").ToList();
    }

    public void SetDeck(List<int> _listId)
    {
        deckList.Clear();

        for (int i = 0; i < _listId.Count; i++)
        {
            CardData card = allCardList.Find((card) => card.id == _listId[i]);
            if (card == null)
            {
                LogManager.Log("ī�� ���� ����  -  id = " + _listId[i]);
                continue;
            }
            deckList.Add(card);
        }
    }

    public List<CardData> GetDeckList()
    {
        return deckList;
    }
    
    public void AcquireCard(CardData _card)
    {
        deckList.Add(_card);
        PlayerDataManager.Instance.SetDeckIds(deckList.Select((card) => card.id).ToList());
    }

    public void RemoveCard(int _index)
    {
        deckList.RemoveAt(_index);
        PlayerDataManager.Instance.SetDeckIds(deckList.Select((card) => card.id).ToList());
    }

    public List<CardData> GetAllCardList() => allCardList;

    public CardData GetCardData(int _id)
    {
        return allCardList.Find((card) => card.id == _id);
    }
}
