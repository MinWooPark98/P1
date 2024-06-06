using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Utils
{
    public static bool IsNull(this UnityEngine.Object _obj)
    {
        return (object)_obj == null;
    }

    public static bool IsNotNull(this UnityEngine.Object _obj)
    {
        return (object)_obj != null;
    }

    public static bool IsComponentNotNull(this UnityEngine.Object _obj)
    {
        return _obj;
    }

    public static bool IsComponentNull(this UnityEngine.Object _obj)
    {
        return _obj == false;
    }

    public static float Vector2ToDegree(Vector2 _vec)
    {
        return Mathf.Atan2(_vec.y, _vec.x) * 180 / Mathf.PI;
    }

    public static Vector2 DegreeToVector2(float _degree)
    {
        return RadianToVector2(_degree * Mathf.Deg2Rad);
    }

    public static Vector2 RadianToVector2(float _radian)
    {
        return new Vector2(Mathf.Cos(_radian), Mathf.Sin(_radian));
    }

    public static float AngleDirVector2(Vector2 _A, Vector2 _B)
    {
        return -_A.x * _B.y + _A.y * _B.x;
    }

    public static void DeleteChild(GameObject _objParent)
    {
        DeleteChild(_objParent.transform);
    }

    public static void DeleteChild(Transform _objParent)
    {
        if (_objParent.childCount > 0)
        {
            while (_objParent.childCount > 0)
            {
                Transform objChild_ = _objParent.GetChild(0);
                objChild_.SetParent(null);
                GameObject.Destroy(objChild_.gameObject);
            }
        }
    }

    public static T ResourceLoad<T>(string _szPath) where T : Object
    {
        T temp = Resources.Load<T>(_szPath);
        if (IsNull(temp))
        {
            Debug.LogError("Load Failed // Path : " + _szPath);
        }
        return temp;
    }

    public static List<T> ShuffleList<T>(List<T> _list)
    {
        for (int i = _list.Count - 1; i > 0; i--)
        {
            int n = UnityEngine.Random.Range(0, i);

            T temp = _list[i];
            _list[i] = _list[n];
            _list[n] = temp;
        }

        return _list;
    }

    /// <summary>
    /// 실수 베지어
    /// </summary>
    /// <param name="_f1"></param>
    /// <param name="_f2"></param>
    /// <param name="_f3"></param>
    /// <param name="_fDuration"></param>
    /// <returns></returns>
    public static float BezierCurvesFloat(float _f1, float _f2, float _f3, float _fDuration)
    {
        if (_fDuration < 0.0f)
        {
            _fDuration = 0.0f;
        }
        else if (_fDuration > 1.0f)
        {
            _fDuration = 1.0f;
        }

        float f4 = Mathf.Lerp(_f1, _f2, _fDuration);
        float f5 = Mathf.Lerp(_f2, _f3, _fDuration);

        return Mathf.Lerp(f4, f5, _fDuration);
    }

    /// <summary>
    /// 스크롤을 컨텐츠 안에 있는 자식오브젝트를 센터로 잡아버림 (세로)
    /// </summary>
    /// <param name="_scroll"></param>
    /// <param name="_nIdx"></param>
    /// <returns></returns>
    public static Vector3 ScrollVerticalTargetCenter(ScrollRect _scroll, int _nIdx)
    {
        if (_nIdx < 0 || _nIdx >= _scroll.content.childCount)
        {
            return _scroll.content.localPosition;
        }

        // 스크롤의 위치를 가져옴
        Vector3 vector = _scroll.content.localPosition;

        // 자식오브젝트의 좌표와 스크롤 절반크기를 합함
        vector.y = (_scroll.content.GetChild(_nIdx).localPosition.y * (-1.0f)) - (_scroll.viewport.rect.height * 0.5f);

        _scroll.content.localPosition = vector;

        //Canvas.ForceUpdateCanvases();

        // 너무 작거나 너무 크면 처리함
        if (vector.y > _scroll.content.sizeDelta.y - _scroll.viewport.rect.height)
        {
            vector.y = _scroll.content.sizeDelta.y - _scroll.viewport.rect.height;
        }
        else if (vector.y < 0.0f)
        {
            vector.y = 0.0f;
        }

        return vector;
    }

    /// <summary>
    /// 스크롤을 컨텐츠 안에 있는 자식오브젝트를 센터로 잡아버림 (가로)
    /// </summary>
    /// <param name="_scroll"></param>
    /// <param name="_nIdx"></param>
    /// <returns></returns>
    public static Vector3 ScrollHorizonTargetCenter(ScrollRect _scroll, int _nIdx)
    {
        if (_nIdx < 0 || _nIdx >= _scroll.content.childCount)
        {
            return _scroll.content.localPosition;
        }

        // 스크롤의 위치를 가져옴
        Vector3 vector = _scroll.content.localPosition;

        // 자식오브젝트의 좌표와 스크롤 절반크기를 합함
        vector.x = (_scroll.content.GetChild(_nIdx).localPosition.x * (-1.0f)) - (_scroll.viewport.rect.width * 0.5f);

        _scroll.content.localPosition = vector;

        //Canvas.ForceUpdateCanvases();

        // 너무 작거나 너무 크면 처리함
        if (vector.x > _scroll.content.sizeDelta.x - _scroll.viewport.rect.width)
        {
            vector.x = _scroll.content.sizeDelta.x - _scroll.viewport.rect.width;
        }
        else if (vector.x < 0.0f)
        {
            vector.x = 0.0f;
        }

        return vector;
    }

    public static string RandomString(int _nCount)
    {
        string sz = "";
        for (int i = 0; i < _nCount; i++)
        {
            int rndVal = Random.Range(0, 62);
            if (rndVal < 10)
            {
                sz += rndVal;
            }
            else if (rndVal > 35)
            {
                sz += (char)(rndVal + 61);
            }
            else
            {
                sz += (char)(rndVal + 55);
            }
        }
        return sz;
    }

    public static int IntParseFast(string _szValue)
    {
        int nResult = 0;
        for (int i = 0; i < _szValue.Length; i++)
        {
            char cLetter = _szValue[i];
            nResult = 10 * nResult + (cLetter - 48);
        }
        return nResult;
    }

    public static int IntParseFast(char _cValue)
    {
        return _cValue - 48;
    }

    public static void RefreshGUI(RectTransform _rectTransform)
    {
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
    }

    // 마우스가 다른 그래픽과 관계없이 해당 그래픽(이미지, TMP 등) 위에 있는지 확인
    public static bool IsMouseOverGraphic(Canvas _canvas, Graphic _target)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        GraphicRaycaster raycaster = _canvas.GetComponent<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == _target.gameObject)
            {
                return true;
            }
        }
        return false;
    }

    // 마우스가 rectTransform 위에 있는지 확인
    public static bool IsMouseOverRecttTransform(RectTransform _rectTransform)
    {
        Vector2 localMousePosition;
        bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Input.mousePosition, null, out localMousePosition);
        if (isInside == false)
        {
            return false;
        }
        return _rectTransform.rect.Contains(localMousePosition);
    }

    public static void AppQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void OpenURL(string _url)
    {
        Application.OpenURL(_url);
    }

    public static class EnumUtil<T>
    {
        public static T Parse(string s)
        {
            return (T)System.Enum.Parse(typeof(T), s);
        }
    }
}
