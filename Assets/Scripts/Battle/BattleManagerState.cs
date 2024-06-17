using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{
    // INIT
    private void ENTER_INIT()
    {
        StartCoroutine(UIManager.Instance.routineLoadPopup());
        DataTableManager.LoadAll();

        popupBattle = UIManager.Instance.MakePopup<PopupBattle>();

        PlayerDataManager.Instance.SetCardIds(StaticData.startDeckIronclad.ToList());
        CardManager.Instance.SetDeck(PlayerDataManager.Instance.GetCardIds());
        drawPile = Utils.ShuffleList(CardManager.Instance.GetDeckList().ToList());
        player = new Character();
        player.SetCharacterInfo(popupBattle.GetPlayerInfo());
        player.Init(30, 30, 6, null);
        player.SetActionDie(
            () =>
            {
                ChangeState(BATTLE_STATE.GAMEOVER);
            });

        LogManager.Log("플레이어 생성 완료");
        enemyList = new List<Enemy>();
        enemyList.Add(new Enemy());
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].SetInfo("Enemy1");
            enemyList[i].SetCharacterInfo(popupBattle.GetEnemyInfos()[i]);
            enemyList[i].Init(26, 26, 4, null);
            enemyList[i].SetActionDie(
                () =>
                {
                    bool allEnemyDead = true;
                    for (int i = 0; i < enemyList.Count; i++)
                    {
                        if (enemyList[i].IsDead() == false)
                        {
                            allEnemyDead = false;
                            break;
                        }
                    }

                    if (allEnemyDead)
                    {
                        ChangeState(BATTLE_STATE.WIN);
                    }
                });
        }
        LogManager.Log("적 생성 완료");

        popupBattle.gameObject.SetActive(false);
    }

    private void UPDATE_INIT()
    {
        initTimer -= Time.deltaTime;
        //LogManager.Log("InitTimer = " + initTimer);
        if (initTimer <= 0)
        {
            ChangeState(BATTLE_STATE.START);
        }
    }

    private void EXIT_INIT()
    {
        
    }


    // START
    private void ENTER_START()
    {
        popupBattle.gameObject.SetActive(true);
        LogManager.Log("팝업 배틀 활성화");
    }

    private void UPDATE_START()
    {
        startTimer -= Time.deltaTime;
        if (startTimer <= 0)
        {
            ChangeState(BATTLE_STATE.PLAYERTURN);
        }
    }

    private void EXIT_START()
    {

    }


    // PLAYERTURN
    private void ENTER_PLAYERTURN()
    {
        turnCount++;
        drawCount = initDrawCount;
        AddEnergy(refillEnergy);

        popupBattle.ActivateBtnTurnEnd(false);
    }

    private void UPDATE_PLAYERTURN()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeState(BATTLE_STATE.WIN);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            drawCount++;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            AddEnergy(2);
        }

        // 드로우 중에는 다른 행동 불가
        isDrawing = drawCount > 0;
        if (isDrawing)
        {
            popupBattle.ActivateBtnTurnEnd(false);

            drawTimer += Time.deltaTime;
            if (drawTimer >= drawDuration)
            {
                drawTimer = 0f;
                DrawCard();
            }

            return;
        }
        popupBattle.ActivateBtnTurnEnd(true);
    }

    private void EXIT_PLAYERTURN()
    {
        popupBattle.DiscardAll();
        LoseEnergy(energy);
    }


    // ENEMYTURN
    private void ENTER_ENEMYTURN()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].ActivePattern();
        }
    }

    private void UPDATE_ENEMYTURN()
    {
        if (player.IsDead() == false)
        {
            ChangeState(BATTLE_STATE.PLAYERTURN);
        }
    }

    private void EXIT_ENEMYTURN()
    {

    }


    // WIN
    private void ENTER_WIN()
    {
        // 임시 테스트
        PlayerDataManager.Instance.SetCardRewards(TableCardRewardRarity.GetFrom.NormalCombats);
        PopupBattleReward popupBattleReward = UIManager.Instance.MakePopup<PopupBattleReward>();
    }

    private void UPDATE_WIN()
    {
        //Utils.AppQuit();
    }

    private void EXIT_WIN()
    {

    }


    // GAMEOVER
    private void ENTER_GAMEOVER()
    {

    }

    private void UPDATE_GAMEOVER()
    {
        Utils.AppQuit();
    }

    private void EXIT_GAMEOVER()
    {

    }
}
