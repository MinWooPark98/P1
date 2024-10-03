using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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

    private CardData cardData = null;

    protected override void Awake()
    {
        dropDownRarity.options.Clear();

        string[] listRarity = System.Enum.GetNames(typeof(CARD_RARITY));
        for (int i = 0; i < listRarity.Length; i++)
        {
            CARD_RARITY rarity = CARD_RARITY.NONE;
            if (System.Enum.TryParse(listRarity[i], out rarity))
            {
                if (rarity != CARD_RARITY.NONE)
                {
                    dropDownRarity.options.Add(new TMP_Dropdown.OptionData(listRarity[i]));
                }
            }
        }

        textCardId.onValueChanged.AddListener((text) => { textCardId.text = Utils.GetIntFromText(text); });
        textEnhance.onValueChanged.AddListener((text) => { textEnhance.text = Utils.GetIntFromText(text); });
        textEnergy.onValueChanged.AddListener((text) => { textEnergy.text = Utils.GetIntFromText(text); });

        base.Awake();
    }

    public void Set(CardData _cardData)
    {
        cardData = _cardData;
        
        itemCard.Set(cardData);

        textCardId.text = _cardData.id.ToString();
        textName.text = _cardData.idName.ToString();
        dropDownRarity.value = (int)_cardData.rarity;
        textEnhance.text = _cardData.enhanced.ToString();
        textEnergy.text = _cardData.energy.ToString();
    }

    public void ButtonActions()
    {
        PopupCardEditorActions popupCardEditorActions = UIManager.Instance.MakePopup<PopupCardEditorActions>();
        popupCardEditorActions.Set(cardData);
        popupCardEditorActions.SetAction(
            () =>
            {
                Set(cardData);
            });
    }

    public void ButtonFeatures()
    {

    }

    public void ButtonRemove()
    {
        CardManager.Instance.RemoveCard(cardData);
        CardManager.Instance.SaveBook();
        ButtonClose();
    }

    public void ButtonSave()
    {
        cardData.id = System.Int32.Parse(textCardId.text);
        cardData.idName = textName.text;
        cardData.rarity = (CARD_RARITY)dropDownRarity.value;
        cardData.enhanced = System.Int32.Parse(textEnhance.text);
        cardData.energy = System.Int32.Parse(textEnergy.text);

        CardManager.Instance.SaveBook();
    }
}
