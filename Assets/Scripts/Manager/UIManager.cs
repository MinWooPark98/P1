using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UIManager : MonoBehaviour
{
    // --------------------------------------------------------------------
    // Static Instance
    // --------------------------------------------------------------------
    static UIManager s_Instance = null;

    public static UIManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<UIManager>();
                if (s_Instance == null)
                {
                    GameObject obj = (GameObject)Instantiate(Resources.Load("Prefabs/Manager/UIManager"));
                    DontDestroyOnLoad(obj);
                }
            }

            return s_Instance;
        }
    }

    public static bool IsInstance()
    {
        return (s_Instance == null) ? false : true;
    }

    private void Awake()
    {
        //Debug.Log("<color=yellow> Awake - " + Instance + "</color> ");
        s_Instance = this;
        canvasUI = this.GetComponent<Canvas>();

        parentUI = transform.Find("-Popup");


        DontDestroyOnLoad(this.gameObject);
    }


    private Dictionary<string, GameObject> dictPopup = new Dictionary<string, GameObject>();
    private Canvas canvasUI = null;
    private Transform parentUI = null;

    private void Update()
    {
        if (canvasUI.worldCamera == null)
        {
            canvasUI.worldCamera = Camera.main;
        }
    }


    public IEnumerator routineLoadPopup()
    {
        if (dictPopup.Count > 0)
        {
            dictPopup.Clear();
        }

        UIManager.Instance.AddPopup("PopupBattle", "Prefabs/UI/Popups/PopupBattle");

        // 어드레서블 사용
        //AsyncOperationHandle<IList<GameObject>> objectHandle = Addressables.LoadAssetsAsync("PreLoadPopup",
        //    (GameObject obj) =>
        //    {
        //        //Debug.Log(obj.name);
        //        if (dictPopup.ContainsKey(obj.name) == false)
        //        {
        //            AddPopup(obj.name, obj);
        //        }
        //    });

        //while (objectHandle.IsDone == false)
        //{
        //    //Debug.Log(objectHandle.PercentComplete);
        //    yield return null;
        //}
        ////Debug.Log(objectHandle.PercentComplete);

        yield break;
    }

    public GameObject GetPopup(string _szName)
    {
        return dictPopup[_szName];
    }

    public bool IsPopup(string _szName)
    {
        if (dictPopup.ContainsKey(_szName))
        {
            return true;
        }
        return false;
    }

    public void AddPopup(string _szName, GameObject _obj)
    {
        if (dictPopup.ContainsKey(_szName) == false)
        {
            dictPopup.Add(_szName, _obj);
        }
    }

    public void AddPopup(string _szName, string _szPath)
    {
        if (dictPopup.ContainsKey(_szName) == false)
        {
            dictPopup.Add(_szName, Resources.Load(_szPath) as GameObject);
        }
    }

    public T MakePopup<T>()
    {
        System.Type type = typeof(T);

        T popup = Instantiate(GetPopup(type.Name), parentUI).GetComponent<T>();

        return popup;
    }

    public void CloseAllPopup()
    {
        for (int i = parentUI.childCount - 1; i >= 0; i--)
        {
            Popup popup = parentUI.GetChild(i).GetComponent<Popup>();
            if (popup != null)
            {
                popup.ButtonClose();
            }
        }
    }

    public void DeleteAllPopup()
    {
        Utils.DeleteChild(parentUI.gameObject);
    }
}
