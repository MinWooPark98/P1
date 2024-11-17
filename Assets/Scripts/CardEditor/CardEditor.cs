using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEditor : MonoBehaviour
{
    private void Awake()
    {
        PopupCardEditorMenu popupCardEditor = UIManager.Instance.MakePopup<PopupCardEditorMenu>();
    }
}
