using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager s_Instance;
    public static CardManager instance
    {
        get
        {
            return s_Instance;
        }
    }

    [SerializeField]
    private List<CardData> cardList = null;                     // È¹µæÇÑ ¼ø¼­·Î ³ª¿­µÇ¾î ÀÖ´Â µ¦ Á¤º¸


    private void Awake()
    {
        s_Instance= this;
    }

    private void OnDestroy()
    {
        s_Instance = null;
    }

    public List<CardData> GetCardList()
    {
        return cardList;
    }
    
    public void AcquireCard(CardData _card)
    {
        cardList.Add(_card);
    }

    public void RemoveCard(CardData _card)
    {
        cardList.Remove(_card);
    }
}
