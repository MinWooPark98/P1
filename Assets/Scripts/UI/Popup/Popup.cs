using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    protected System.Action actionClose = null;
    protected static Dictionary<System.Type, Popup> dictPopup = new Dictionary<System.Type, Popup>();

    [SerializeField]
    private GameObject objPanel = null; // ���� ������ ȿ�� �ִϸ��̼� / null�̸� �۵�����
    private bool bPopupOpen = false;
    private bool bStartClose = false;

    private float fTimeForce1 = 10.0f;   // (0 -> fMaxScale �ӵ�) 2�� 2��ӵ� : 0.5�� ����
    private float fTimeForce2 = 30.0f;   // (fMaxScale -> 1.0 �ӵ�) 
    private float fMaxScale = 1.1f;

    protected virtual void Make()
    {

    }

    protected virtual void Awake()
    {
        this.name = this.GetType().Name;

        // �˾� �������� �ʿ��Ҷ� ũ�� 0���� ������
        if (objPanel != null)
        {
            //objPanel.transform.localScale = Vector3.zero;
            objPanel.gameObject.SetActive(false);
        }
        else
        {
            bPopupOpen = true;
        }
    }

    protected virtual void Start()
    {
        // �˾� �������� ������ ����
        if (objPanel != null)
        {
            StartCoroutine(routinePopupOpen());
        }
    }

    protected virtual void Update()
    {

    }

    protected virtual void OnDestroy()
    {

    }

    protected virtual void OnEnable()
    {
    }

    public virtual void ButtonClose()
    {
        //Debug.Log("Popup - ButtonClose");

        void ClosePopup()
        {
            if (objPanel != null)
            {
                if (bStartClose == false)
                {
                    bStartClose = true;
                    StartCoroutine(routinePopupClose());
                }
            }
            else
            {
                if (actionClose != null)
                {
                    actionClose();
                    actionClose = null;
                }

                if (gameObject != null)
                {
                    Destroy(gameObject);
                }
            }
        }

        ClosePopup();
    }

    public void SetAction(System.Action _actionClose)
    {
        if (actionClose != null)
        {
            actionClose = null;
        }
        actionClose = _actionClose;
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void BackgroundIgnore(bool _bTouch)
    {

    }

    public bool IsPopupOpen()
    {
        return bPopupOpen;
    }

    private IEnumerator routinePopupOpen()
    {
        objPanel.transform.localScale = Vector3.zero;
        objPanel.gameObject.SetActive(true);

        float fTime = 0.0f;
        while (fTime < 1.0f)
        {
            fTime += (Time.deltaTime * fTimeForce1);

            float fScale = Utils.BezierCurvesFloat(0.0f, fMaxScale, fMaxScale, fTime);
            objPanel.transform.localScale = new Vector3(fScale, fScale, 1.0f);
            yield return null;
        }

        objPanel.transform.localScale = new Vector3(fMaxScale, fMaxScale, 1.0f);
        yield return null;

        fTime = 0.0f;
        while (fTime < 1.0f)
        {
            fTime += (Time.deltaTime * fTimeForce2);
            float fScale = Mathf.Lerp(fMaxScale, 1.0f, fTime);
            objPanel.transform.localScale = new Vector3(fScale, fScale, 1.0f);
            yield return null;
        }

        objPanel.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        bPopupOpen = true;

        yield break;
    }

    private IEnumerator routinePopupClose()
    {
        float fTime = 0.0f;
        while (fTime < 1.0f)
        {
            fTime += (Time.deltaTime * fTimeForce2);
            float fScale = Mathf.Lerp(1.0f, fMaxScale, fTime);
            objPanel.transform.localScale = new Vector3(fScale, fScale, 1.0f);
            yield return null;
        }

        objPanel.transform.localScale = new Vector3(fMaxScale, fMaxScale, 1.0f);
        yield return null;

        fTime = 0.0f;
        while (fTime < 1.0f)
        {
            fTime += (Time.deltaTime * fTimeForce1);
            float fScale = Utils.BezierCurvesFloat(fMaxScale, 0.0f, 0.0f, fTime);
            objPanel.transform.localScale = new Vector3(fScale, fScale, 1.0f);
            yield return null;
        }

        objPanel.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);

        if (actionClose != null)
        {
            actionClose();
            actionClose = null;
        }

        if (gameObject != null)
        {
            Destroy(gameObject);
        }

        yield break;
    }
}
