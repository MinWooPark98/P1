using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{
    // INIT
    private void ENTER_INIT()
    {
        popupBattle = UIManager.Instance.MakePopup<PopupBattle>();
        var playerInfo = popupBattle.GetPlayerInfo();

        drawPile = Utils.ShuffleList(CardManager.Instance.GetDeckList().ToList());
        handCards.Clear();
        discardPile.Clear();
        exhaustedCards.Clear();

        player = new Character();
        player.AddHpListner(playerInfo);
        player.AddBuffListner(playerInfo);
        player.AddDefenseListner(playerInfo);
        player.Init(30, 30, 6, null);                       // 테스트용
        playerInfo.Init(player.CurrHp, player.MaxHp, player.BuffList);
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
            var info = popupBattle.GetEnemyInfos()[i];
            enemyList[i].AddHpListner(info);
            enemyList[i].AddBuffListner(info);
            enemyList[i].AddDefenseListner(info);
            enemyList[i].Init(26, 26, 4, null);
            info.Init(enemyList[i].CurrHp, enemyList[i].MaxHp, enemyList[i].BuffList);
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
    }

    private void UPDATE_INIT()
    {
        ChangeState(BATTLE_STATE.START);
    }

    private void EXIT_INIT()
    {
        
    }


    // START
    private void ENTER_START()
    {
        popupBattleStart = UIManager.Instance.MakePopup<PopupBattleStart>();
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
        popupBattleStart.ButtonClose();
        popupBattleStart = null;
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
        popupBattleReward.SetAction(
            () =>
            {
                popupBattle.ButtonClose();
            });
    }

    private void UPDATE_WIN()
    {

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

    }

    private void EXIT_GAMEOVER()
    {

    }
}
