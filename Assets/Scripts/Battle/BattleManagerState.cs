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
        drawPile = Utils.ShuffleList(CardManager.instance.GetCardList().ToList());
        player.Init(30, 30, 6, null);
        for (int i = 0; i < enemyList.Length; i++)
        {
            enemyList[i].Init(26, 26, 4, null);
        }

        PopupBattle.Instance.gameObject.SetActive(false);
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
        PopupBattle.Instance.gameObject.SetActive(true);

        player.gameObject.SetActive(true);
        for (int i = 0; i < enemyList.Length; i++)
        {
            enemyList[i].gameObject.SetActive(true);
        }
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
        // 선택된 카드가 있을 때
        if (selectedCard != null)
        {
            // esc키 입력 혹은 마우스 우클릭 시, 선택된 카드 선택 취소
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                DeselectCard();
            }
            else if (Input.GetMouseButtonDown(0))                 // 마우스 좌클릭 시
            {
                // 타겟팅 중일 때 (주로 공격카드 발동 시)
                if (isTargeting)
                {
                    bool isTargetExist = false;
                    for (int i = 0; i < enemyList.Length; i++)
                    {
                        if (enemyList[i].GetOnPointer())
                        {
                            isTargetExist = true;
                            selectedCard.Use(enemyList[i]);
                            break;
                        }
                    }
                    if (!isTargetExist)
                    {
                        DeselectCard();
                    }
                }
                else
                {
                    selectedCard.Use();
                }
            }
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

    }


    // ENEMYTURN
    private void ENTER_ENEMYTURN()
    {
        PopupBattle.Instance.gameObject.SetActive(false);
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
