using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{
    public enum BATTLE_STATE
    {
        NONE = -1,
        INIT,                   // 초기화
        START,                  // 시작 연출
        PLAYERTURN,             // 플레이어의 턴
        ENEMYTURN,              // 적의 턴
        WIN,                    // 승리 연출
        GAMEOVER,               // 패배 연출
    }

    public static BattleManager s_Instance;
    public static BattleManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<BattleManager>();
                if (s_Instance == null)
                {
                    GameObject obj = (GameObject)Instantiate(Resources.Load("Prefabs/Manager/BattleManager"));
                    DontDestroyOnLoad(obj);
                }
            }

            return s_Instance;
        }
    }

    private BATTLE_STATE lastState = BATTLE_STATE.NONE;
    private BATTLE_STATE currState = BATTLE_STATE.NONE;
    private BATTLE_STATE nextState = BATTLE_STATE.INIT;

    private List<CardData> drawPile = new List<CardData>();                                 // 뽑을 카드 더미
    private List<CardData> handCards = new List<CardData>();                                // 현재 손패
    private List<CardData> discardPile = new List<CardData>();                              // 버린 카드 더미 
    private List<CardData> exhaustedCards = new List<CardData>();                           // 소멸 더미
    
    // 테스트용 임시
    private int turnCount = 0;
    private float initTimer = 1.0f;
    private float startTimer = 1.0f;

    private int initDrawCount = 5;
    private int drawCount = 0;
    private bool isDrawing = false;
    private float drawDuration = 0.25f;
    private float drawTimer = 0f;

    private int refillEnergy = 3;
    private int energy = 0;

    private Character player;
    private List<Enemy> enemyList;

    //private ItemCard selectedCard = null;
    //private bool isTargeting = false;

    private PopupBattle popupBattle = null;
    private PopupBattleStart popupBattleStart = null;
    // 테스트용 임시 끝


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
            LogManager.Log("<color=yellow> BattleManager State Change : </color> " + currState + " -> " + nextState);

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
                EXIT_INIT();
                break;
            case BATTLE_STATE.START:
                EXIT_START();
                break;
            case BATTLE_STATE.PLAYERTURN:
                EXIT_PLAYERTURN();
                break;
            case BATTLE_STATE.ENEMYTURN:
                EXIT_ENEMYTURN();
                break;
            case BATTLE_STATE.WIN:
                EXIT_WIN();
                break;
            case BATTLE_STATE.GAMEOVER:
                EXIT_GAMEOVER();
                break;
            default:
                break;
        }
    }
}
