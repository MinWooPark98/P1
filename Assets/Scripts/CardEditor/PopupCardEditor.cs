using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PopupCardEditor : Popup
{
    [SerializeField]
    private ItemCard itemCard = null;

    [SerializeField]
    private ScrollRect scroll = null;
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
    [SerializeField]
    private Transform parentFeature = null;
    [SerializeField]
    private ComponentEditor prefabComponentFeature = null;

    private List<ComponentEditor> listComponentFeature = new List<ComponentEditor>();
    private ComponentEditor componentSelected = null;

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

    protected override void OnDestroy()
    {
        KeyboardManager.Instance.RemoveObjInputNeed(this);
        base.OnDestroy();
    }

    protected override void Update()
    {
        if (componentSelected != null)
        {
            if (KeyboardManager.Instance.GetKeyDown(this, KeyCode.Delete))
            {
                KeyboardManager.Instance.RemoveObjInputNeed(this);
                listComponentFeature.Remove(componentSelected);
                Destroy(componentSelected.gameObject);
                SelectFeature(null);
            }
        }
        base.Update();
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
        SelectFeature(null);
        while (listComponentFeature.Count > 0)
        {
            Destroy(listComponentFeature[0].gameObject);
            listComponentFeature.RemoveAt(0);
        }
        if (cardData.featureList != null)
        {
            for (int i = 0; i < cardData.featureList.Count; i++)
            {
                ComponentEditor component = Instantiate(prefabComponentFeature, parentFeature);
                component.Set(cardData.featureList[i]);
                component.SetActionClicked(
                    () =>
                    {
                        SelectFeature(component);
                    });
                listComponentFeature.Add(component);
            }
        }
    }

    public void ButtonActions()
    {
        PopupCardEditorActions popupCardEditorActions = UIManager.Instance.MakePopup<PopupCardEditorActions>();
        popupCardEditorActions.Set(cardData);
    }

    public void SelectFeature(ComponentEditor _component)
    {
        if (componentSelected != null)
        {
            KeyboardManager.Instance.RemoveObjInputNeed(this);
            componentSelected.Deselect();
        }

        componentSelected = componentSelected == _component ? null : _component;

        if (componentSelected != null)
        {
            KeyboardManager.Instance.AddObjInputNeed(this);
            componentSelected.Select();
        }
    }

    public void ButtonAddFeature()
    {
        ComponentEditor component = Instantiate(prefabComponentFeature, parentFeature);
        component.Set(new CardData.CardFeature());
        component.SetActionClicked(
            () =>
            {
                SelectFeature(component);
            });
        listComponentFeature.Add(component);
        StartCoroutine(routine_Reposition());
    }

    private IEnumerator routine_Reposition()
    {
        yield return null;
        scroll.normalizedPosition = new Vector2(scroll.normalizedPosition.x, 0f);
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
        cardData.featureList.Clear();
        for (int i = 0; i < listComponentFeature.Count; i++)
        {
            cardData.featureList.Add((CardData.CardFeature)listComponentFeature[i].GetData());
        }

        CardManager.Instance.SaveBook();
    }
}
