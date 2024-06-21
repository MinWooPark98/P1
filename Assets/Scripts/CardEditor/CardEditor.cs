using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEditor : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(UIManager.Instance.routineLoadPopup());
        DataTableManager.LoadAll();

        PopupCardEditorMenu popupCardEditor = UIManager.Instance.MakePopup<PopupCardEditorMenu>();
    }
}
