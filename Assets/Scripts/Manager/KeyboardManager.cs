using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardManager : MonoBehaviour
{
    private static KeyboardManager s_Instance = null;
    public static KeyboardManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<KeyboardManager>();
                if (s_Instance == null)
                {
                    GameObject obj = (GameObject)Instantiate(Resources.Load("Prefabs/Manager/KeyboardManager"));
                    DontDestroyOnLoad(obj);
                }
            }

            return s_Instance;
        }
    }

    private bool ignore = false;
    public bool IsIgnore
    {
        get
        {
            return ignore;
        }
    }

    // ��ǲ�� �ʿ��� ��ũ��Ʈ ���������� ����Ʈ�� ��Ƽ� ���� �ڿ� �ִ� ��ũ��Ʈ������ �Է� �޵���
    private List<System.Object> listObjInputValid = new List<System.Object>();
    private List<System.Action> listActionEscape = new List<System.Action>();


    private void Awake()
    {
        s_Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        s_Instance = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscapeEvent();
        }
    }

    public void AddObjInputNeed(System.Object _obj)
    {
        // �̹� �����ϸ� ����� �� �ڿ� ���� �߰�
        RemoveObjInputNeed(_obj);
        listObjInputValid.Add(_obj);
    }

    public void RemoveObjInputNeed(System.Object _obj)
    {
        for (int i = 0; i < listObjInputValid.Count; i++)
        {
            if (listObjInputValid[i] == _obj)
            {
                listObjInputValid.Remove(i);
                i--;
            }
        }
    }

    private bool IsInputValidObject(System.Object _obj)
    {
        return listObjInputValid.Count > 0 && listObjInputValid[listObjInputValid.Count - 1] == _obj;
    }

    public bool GetKey(System.Object _obj, KeyCode _keyCode)
    {
        if (IsInputValidObject(_obj) == false)
        {
            return false;
        }

        return Input.GetKey(_keyCode);
    }

    public bool GetKeyDown(System.Object _obj, KeyCode _keyCode)
    {
        if (IsInputValidObject(_obj) == false)
        {
            return false;
        }

        return Input.GetKeyDown(_keyCode);
    }
    public bool GetKeyUp(System.Object _obj, KeyCode _keyCode)
    {
        if (IsInputValidObject(_obj) == false)
        {
            return false;
        }

        return Input.GetKeyUp(_keyCode);
    }

    public void AddActionEscape(System.Action _action)
    {
        listActionEscape.Add(_action);
    }

    public void RemoveActionEscape(System.Action _action)
    {
        if (listActionEscape.Contains(_action))
        {
            listActionEscape.Remove(_action);
        }
    }

    private void EscapeEvent()
    {
        if (listActionEscape.Count > 0)
        {
            System.Action action = listActionEscape[listActionEscape.Count - 1];
            if (action != null)
            {
                action.Invoke();
                RemoveActionEscape(action);
            }
        }
    }
}