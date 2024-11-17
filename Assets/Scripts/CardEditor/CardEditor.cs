using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardEditor : MonoBehaviour
{
    private void Awake()
    {
        PopupCardEditorMenu popupCardEditor = UIManager.Instance.MakePopup<PopupCardEditorMenu>();
        KeyboardManager.Instance.AddActionEscape(LoadMenu);
    }

    private void OnDestroy()
    {
        KeyboardManager.Instance.RemoveActionEscape(LoadMenu);
    }

    private void LoadMenu()
    {
        UIManager.Instance.DeleteAllPopup();
        SceneManager.LoadScene("MenuScene");
    }
}
