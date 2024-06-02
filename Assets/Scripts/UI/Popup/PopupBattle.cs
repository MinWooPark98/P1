using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupBattle : MonoBehaviour
{
    public static PopupBattle s_Instance;
    public static PopupBattle Instance
    {
        get
        {
            return s_Instance;
        }
    }

    [SerializeField]
    private GameObject objHand = null;

    [SerializeField]
    private ItemCard prefabItemCard = null;


    private void Awake()
    {
        s_Instance = this;
    }

    private void OnDestroy()
    {
        s_Instance = null;
    }

    public void DrawCard(CardData _data)
    {
        ItemCard newCard = Instantiate(prefabItemCard, objHand.transform);
        newCard.Set(_data);
    }
}
