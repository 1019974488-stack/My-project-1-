using System.Collections;
using UnityEngine;


public class GameManager : MonoBehaviour
{


    public enum GameState
    {
        Dragging,
        SlotMachine,
        Result,
        Finished
    }



    public GameState currentState =
        GameState.Dragging;



    [Header("References")]

    public SlotMachineController slotMachine;

    public CupController cupController;

    public LeftButtonController leftButton;



    [Header("Round")]

    public int currentRound = 1;

    public int maxRound = 3;



    public float[] roundSpeeds =
    {
        0.18f,
        0.12f,
        0.08f
    };







    //------------------------------------------------
    // 进入老虎机
    //------------------------------------------------

    public void EnterSlotMachine()
    {

        if (currentRound > maxRound)
        {
            Debug.Log("已经完成全部轮次");
            return;
        }



        currentState =
            GameState.SlotMachine;



        slotMachine.selectorSpeed =
            roundSpeeds[currentRound - 1];



        slotMachine.StartSpin();



        Debug.Log(
            "开始第 " + currentRound + " 轮"
        );

    }









    //------------------------------------------------
    // 一轮结束
    //------------------------------------------------

    public void FinishRound(int result)
    {


        currentState =
            GameState.Result;

        CakeData.Instance.SaveCake(
    currentRound,
    result
);


        Debug.Log(
            "第 " + currentRound +
            " 轮完成，结果：" + result
        );





        // 第三轮结束

        if (currentRound >= maxRound)
        {

            currentState =
                GameState.Finished;


            StartCoroutine(
                FinalReset()
            );


            return;

        }


    }









    //------------------------------------------------
    // Restart进入下一轮
    //------------------------------------------------

    public void StartNextRound()
    {


        if (currentRound >= maxRound)
        {

            Debug.Log(
                "已经是最后一轮"
            );


            return;

        }





        currentRound++;



        currentState =
            GameState.Dragging;



        slotMachine.ResetSlotMachine();



        Debug.Log(
            "进入第 " + currentRound + " 轮"
        );


    }












    //------------------------------------------------
    // 第三轮结束初始化
    //------------------------------------------------

    IEnumerator FinalReset()
    {


        yield return new WaitForSeconds(1f);





        // 重置老虎机

        if (slotMachine != null)
        {

            slotMachine.FinalResetSlotMachine();

        }







        // 重置杯子

        if (cupController != null)
        {

            cupController.ResetCup();

        }








        // 重置左按钮

        if (leftButton != null)
        {

            leftButton.ResetButton();

        }








        // 第三轮结束隐藏Restart按钮

        if (leftButton != null)
        {

            leftButton.gameObject.SetActive(false);

        }






        Debug.Log(
            "三轮结束，等待进入下一页"
        );


    }












    //------------------------------------------------
    // Next按钮（以后制作后再使用）
    //------------------------------------------------

    public void GoNextPage()
    {

        Debug.Log(
            "进入下一页"
        );


    }



}