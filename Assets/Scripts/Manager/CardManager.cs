using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;

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

    [System.Serializable]
    public class CardBook
    {
        [JsonProperty]
        private List<CardData> allCardList = new List<CardData>();                      // 모든 카드 정보

        public List<CardData> GetAllCards()
        {
            SortById();
            return allCardList;
        }
        public void SetAllCards(List<CardData> _book) => allCardList = _book;
        public void AddCard(CardData _newData) => allCardList.Add(_newData);
        public void RemoveCard(CardData _data) => allCardList.Remove(_data);
        public void SortById()
        {
            allCardList = allCardList.OrderBy((card) => card.id).ToList();
        }
    }

    [SerializeField]
    private CardBook cardBook = null;

    [SerializeField]
    private List<CardData> deckList = new List<CardData>();                         // 플레이어 현재 덱 리스트

    private void Awake()
    {
        s_Instance = this;

        LoadBook();
    }

    private void OnDestroy()
    {
        s_Instance = null;
    }

    public void SetDeck(List<int> _listId)
    {
        deckList.Clear();

        for (int i = 0; i < _listId.Count; i++)
        {
            CardData card = cardBook.GetAllCards().Find((card) => card.id == _listId[i]);
            if (card == null)
            {
                LogManager.Log("카드 정보 없음  -  id = " + _listId[i]);
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

    public List<CardData> GetAllCardList() => cardBook.GetAllCards();

    public CardData GetCardData(int _id)
    {
        return cardBook.GetAllCards().Find((card) => card.id == _id);
    }
    
    public void AddNewCard(CardData _card)
    {
        cardBook.AddCard(_card);
    }

    public void RemoveCard(CardData _card)
    {
        cardBook.RemoveCard(_card);
    }

    public void SaveBook()
    {
        cardBook.SortById();
        string jsonData = JsonConvert.SerializeObject(cardBook, 
            new JsonSerializerSettings 
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            });

        // 디버그용
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "CardBook.json"), jsonData);
    }

    public void LoadBook()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "CardBook.json");
        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            if (string.IsNullOrEmpty(jsonData) == false)
            {
                cardBook = JsonConvert.DeserializeObject<CardBook>(jsonData, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            }
        }
    }
}
