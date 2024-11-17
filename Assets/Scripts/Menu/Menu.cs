using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(UIManager.Instance.routineLoadPopup());
        DataTableManager.LoadAll();
    }

    public void ButtonGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ButtonEditor()
    {
        SceneManager.LoadScene("CardEditorScene");
    }
}
