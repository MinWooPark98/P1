using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class BattleManager : MonoBehaviour
{
    public enum BATTLE_STATE
    {
        NONE = -1,
        INIT,                   // �ʱ�ȭ
        START,                  // ���� ����
        PLAYERTURN,             // �÷��̾��� ��
        ENEMYTURN,              // ���� ��
        WIN,                    // �¸� ����
        GAMEOVER,               // �й� ����
    }

    public static BattleManager s_Instance;
    public static BattleManager Instance
    {
        get
        {
            return s_Instance;
        }
    }

    private BATTLE_STATE lastState = BATTLE_STATE.NONE;
    private BATTLE_STATE currState = BATTLE_STATE.NONE;
    private BATTLE_STATE nextState = BATTLE_STATE.INIT;

    private List<CardData> drawPile = new List<CardData>();                                 // ���� ī�� ����
    private List<CardData> handCards = new List<CardData>();                                // ���� ����
    private List<CardData> discardPile = new List<CardData>();                              // ���� ī�� ���� 
    private List<CardData> exhaustedCards = new List<CardData>();                           // �Ҹ� ����

    // �׽�Ʈ�� �ӽ�
    private float initTimer = 1.0f;
    private float startTimer = 1.0f;

    private int initDrawCount = 5;
    private int drawCount = 0;
    private bool isDrawing = false;
    private float drawDuration = 0.5f;
    private float drawTimer = 0f;

    private int refillEnergy = 3;
    private int energy = 0;

    [SerializeField]
    private Character player;
    [SerializeField]
    private Character[] enemyList;

    private ItemCard selectedCard = null;
    private bool isTargeting = false;
    // �׽�Ʈ�� �ӽ� ��


    private void Awake()
    {
        s_Instance = this;
    }

    private void OnDestroy()
    {
        s_Instance = null;
    }

    public void ChangeState(BATTLE_STATE _state)
    {
        nextState = _state;
    }

    public BATTLE_STATE GetState()
    {
        return currState;
    }

    private void Update()
    {
        if (currState != nextState)
        {
            LogManager.Log("<color=yellow> InGameManager State Change : </color> " + currState + " -> " + nextState);

            lastState = currState;
            Exit(currState);
            currState = nextState;
            Enter(currState);
        }

        switch (currState)
        {
            case BATTLE_STATE.INIT:
                UPDATE_INIT();
                break;
            case BATTLE_STATE.START:
                UPDATE_START();
                break;
            case BATTLE_STATE.PLAYERTURN:
                UPDATE_PLAYERTURN();
                break;
            case BATTLE_STATE.ENEMYTURN:
                UPDATE_ENEMYTURN();
                break;
            case BATTLE_STATE.WIN:
                UPDATE_WIN();
                break;
            case BATTLE_STATE.GAMEOVER:
                UPDATE_GAMEOVER();
                break;
            default:
                break;
        }
    }

    private void Enter(BATTLE_STATE _state)
    {
        switch (_state)
        {
            case BATTLE_STATE.INIT:
                ENTER_INIT();
                break;
            case BATTLE_STATE.START:
                ENTER_START();
                break;
            case BATTLE_STATE.PLAYERTURN:
                ENTER_PLAYERTURN();
                break;
            case BATTLE_STATE.ENEMYTURN:
                ENTER_ENEMYTURN();
                break;
            case BATTLE_STATE.WIN:
                ENTER_WIN();
                break;
            case BATTLE_STATE.GAMEOVER:
                ENTER_GAMEOVER();
                break;
            default:
                break;
        }
    }

    private void Exit(BATTLE_STATE _state)
    {
        switch (_state)
        {
            case BATTLE_STATE.INIT:
                ENTER_INIT();
                break;
            case BATTLE_STATE.START:
                ENTER_START();
                break;
            case BATTLE_STATE.PLAYERTURN:
                ENTER_PLAYERTURN();
                break;
            case BATTLE_STATE.ENEMYTURN:
                ENTER_ENEMYTURN();
                break;
            case BATTLE_STATE.WIN:
                ENTER_WIN();
                break;
            case BATTLE_STATE.GAMEOVER:
                ENTER_GAMEOVER();
                break;
            default:
                break;
        }
    }
}
