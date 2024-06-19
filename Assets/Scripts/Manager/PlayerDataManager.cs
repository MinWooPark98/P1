using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    static PlayerDataManager s_Instance = null;

    public static PlayerDataManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<PlayerDataManager>();
                if (s_Instance == null)
                {
                    GameObject obj = (GameObject)Instantiate(Resources.Load("Prefabs/Manager/PlayerDataManager"));
                    DontDestroyOnLoad(obj);
                }
            }

            return s_Instance;
        }
    }

    [System.Serializable]
    public class Data
    {
        public int currHp;
        public int maxHp;
        public List<int> deckIds = new List<int>();

        // ī�� ���� ����
        // cardRewardOffset : �⺻���� Ȯ���� �߰� ����Ǵ� Ȯ�� (-5% �� default, Commonī�尡 ���� ������ 1%�� �����ؼ� 40%�� �ִ�)
        // offset > 0�̸�, �׸�ŭ RareȮ���� ���̰�, UncommonȮ���� ���ҽ�Ŵ
        // offset < 0�̸�, �׸�ŭ RareȮ���� ���߰�, CommonȮ���� ������Ŵ
        public float cardRewardOffset = -0.05f;
        public List<int> cardRewards = new List<int>();

        // �� ����
    }

    [SerializeField]
    private Data data = new Data();


    private void Awake()
    {
        s_Instance = this;
    }

    private void OnDestroy()
    {
        s_Instance = null;
    }

    public void SetCurrHp(int _currHp)
    {
        data.currHp = _currHp;
    }

    public void SetMaxHp(int _maxHp)
    {
        data.maxHp = _maxHp;
    }

    public int GetCurrHp() => data.currHp;

    public int GetMaxHp() => data.maxHp;

    public void SetDeckIds(List<int> _cardIds)
    {
        data.deckIds = _cardIds;
    }

    public List<int> GetCardIds() => data.deckIds;

    public void SetCardRewards(TableCardRewardRarity.GetFrom _getFrom)
    {
        data.cardRewards.Clear();

        TableCardRewardRarity table = DataTableManager.GetTable<TableCardRewardRarity>();
        var book = table.GetBook(_getFrom);
        float[] rate = new float[3] { book.COMMON, book.UNCOMMON, book.RARE };
        rate[2] += data.cardRewardOffset;
        if (data.cardRewardOffset > 0f)
        {
            rate[1] -= data.cardRewardOffset;
        }
        else
        {
            rate[0] += data.cardRewardOffset;
        }

        for (int i = 0; i < 3; i++)
        {
            float randomRate = Random.Range(0f, 1f);
            CARD_RARITY rarity = CARD_RARITY.NONE;
            for (int j = rate.Length - 1; j >= 0; j--)
            {
                randomRate -= rate[j];
                if (randomRate < 0f)
                {
                    switch (j)
                    {
                        case 0:
                            rarity = CARD_RARITY.COMMON;
                            break;
                        case 1:
                            rarity = CARD_RARITY.UNCOMMON;
                            break;
                        case 2:
                            rarity = CARD_RARITY.RARE;
                            break;
                        default:
                            break;
                    }
                }
            }

            // �� ��������� common����
            if (rarity == CARD_RARITY.NONE)
            {
                rarity = CARD_RARITY.COMMON;
            }

            var allCardList = CardManager.Instance.GetAllCardList();
            var cardOptionList = allCardList.FindAll((card) => card.rarity == rarity).ToList();
            int index;
            while (true)
            {
                index = Random.Range(0, cardOptionList.Count);
                if (data.cardRewards.Contains(cardOptionList[index].id) == false)
                {
                    break;
                }
            }
            data.cardRewards.Add(cardOptionList[index].id);
        }
    }

    public List<int> GetCardRewardList() => data.cardRewards;

    public void ClearCardRewardList()
    {
        data.cardRewards.Clear();
    }

    public void ResetCardRewardOffset()
    {
        data.cardRewardOffset = -0.05f;
    }

    public void IncreaseCardRewardOffset()
    {
        data.cardRewardOffset += 0.01f;
        if (data.cardRewardOffset >= 0.4f)
        {
            data.cardRewardOffset = 0.4f;
        }
    }


    public void Save()
    {
        string jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString("UserData", jsonData);
    }

    public void Load()
    {
        string jsonData = PlayerPrefs.GetString("UserData");
        if (string.IsNullOrEmpty(jsonData) == false)
        {
            data = JsonUtility.FromJson<Data>(jsonData);
        }
    }
}
