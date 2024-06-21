using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class PopupCardEditor : Popup
{
    [SerializeField]
    private ItemCard itemCard = null;

    [SerializeField]
    private TMP_InputField textCardId = null;
    [SerializeField]
    private TMP_InputField textName = null;
    [SerializeField]
    private TMP_Dropdown dropDownRarity = null;
    [SerializeField]
    private TMP_InputField textEnhance = null;
    [SerializeField]
    private TMP_InputField textEnergy = null;

    private CardData cardDara = null;


    protected override void Awake()
    {
        base.Awake();
    }

    public void Set(CardData _cardData)
    {
        cardDara = _cardData;
        
        itemCard.Set(cardDara);

        textCardId.text = _cardData.id.ToString();
        textName.text = _cardData.name.ToString();
        textEnhance.text = _cardData.enhanced.ToString();
        textEnergy.text = _cardData.energy.ToString();
    }

    private void OnValidate()
    {
        textCardId.text = GetIntFromText(textCardId.text);
        textEnhance.text = GetIntFromText(textEnhance.text);
        textEnergy.text = GetIntFromText(textEnergy.text);
    }

    private string GetIntFromText(string _str)
    {
        return Regex.Replace(_str, @"[^0-9]", "");
    }

    public void Save()
    {

    }
}
