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
        player.SetMaxHp(30);
        player.Recovery(30);
        Enemy.SetMaxHp(26);
        Enemy.Recovery(26);

        PopupBattle.Instance.gameObject.SetActive(false);
    }

    private void UPDATE_INIT()
    {
        initTimer -= Time.deltaTime;
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
        drawCount = initDrawCount;
        player.ClearEnergy();
        player.AddEnergy(3);
    }

    private void UPDATE_PLAYERTURN()
    {
        isDrawing = drawCount > 0;
        if (isDrawing)
        {
            drawTimer += Time.deltaTime;
            if (drawTimer >= drawDuration)
            {
                drawTimer = 0f;
                DrawCard();
            }
        }
    }

    private void EXIT_PLAYERTURN()
    {

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
