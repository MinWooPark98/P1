using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{
    // INIT
    private void ENTER_INIT()
    {
        StartCoroutine(UIManager.Instance.routineLoadPopup());
        popupBattle = UIManager.Instance.MakePopup<PopupBattle>();

        drawPile = Utils.ShuffleList(CardManager.instance.GetCardList().ToList());
        player = new Character();
        player.SetCharacterInfo(popupBattle.GetPlayerInfo());
        player.Init(30, 30, 6, null);

        enemyList = new List<Character>();
        enemyList.Add(new Character());
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].SetCharacterInfo(popupBattle.GetEnemyInfos()[i]);
            enemyList[i].Init(26, 26, 4, null);
        }

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
    }

    private void UPDATE_START()
    {
        startTimer -= Time.deltaTime;
        //LogManager.Log("StartTimer = " + startTimer);
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
        drawCount = initDrawCount;
        AddEnergy(refillEnergy);
    }

    private void UPDATE_PLAYERTURN()
    {
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
            drawTimer += Time.deltaTime;
            if (drawTimer >= drawDuration)
            {
                drawTimer = 0f;
                DrawCard();
            }

            return;
        }
    }

    private void EXIT_PLAYERTURN()
    {
        popupBattle.DiscardAll();
    }


    // ENEMYTURN
    private void ENTER_ENEMYTURN()
    {

    }

    private void UPDATE_ENEMYTURN()
    {

    }

    private void EXIT_ENEMYTURN()
    {

    }


    // WIN
    private void ENTER_WIN()
    {

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
