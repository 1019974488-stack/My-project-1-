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


    [Header("Current State")]

    public GameState currentState =
        GameState.Dragging;


    [Header("Page 3 References")]

    public SlotMachineController slotMachine;

    public CupController cupController;

    public LeftButtonController leftButton;


    [Header("Page Switch")]

    // Page 3 的根对象
    public GameObject page3;

    // Page 4 的根对象
    public GameObject page4;

    // Page 3 和 Page 4 共用的结果托盘
    public GameObject resultContainer;

    // Page 4 的拖拽管理器
    public PicnicManager picnicManager;


    [Header("Round")]

    public int currentRound = 1;

    public int maxRound = 3;


    // 三轮老虎机的扫描速度
    private float[] roundSpeeds =
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
            Debug.Log(
                "已经完成全部轮次"
            );

            return;
        }


        currentState =
            GameState.SlotMachine;


        if (slotMachine == null)
        {
            Debug.LogError(
                "GameManager 没有连接 SlotMachineController"
            );

            return;
        }


        slotMachine.selectorSpeed =
            roundSpeeds[currentRound - 1];


        slotMachine.StartSpin();


        Debug.Log(
            "开始第 "
            + currentRound
            + " 轮"
        );
    }


    //------------------------------------------------
    // 一轮结束
    //------------------------------------------------

    public void FinishRound(int result)
    {
        currentState =
            GameState.Result;


        if (CakeData.Instance != null)
        {
            CakeData.Instance.SaveCake(
                currentRound,
                result
            );
        }
        else
        {
            Debug.LogError(
                "没有找到 CakeData，无法保存蛋糕结果"
            );
        }


        Debug.Log(
            "第 "
            + currentRound
            + " 轮完成，结果："
            + result
        );
    }


    //------------------------------------------------
    // 进入下一轮
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


        if (slotMachine != null)
        {
            slotMachine.ResetSlotMachine();
        }


        Debug.Log(
            "进入第 "
            + currentRound
            + " 轮"
        );
    }


    //------------------------------------------------
    // 第三轮结束后的操作区初始化
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


        // 隐藏左按钮
        if (leftButton != null)
        {
            leftButton.gameObject.SetActive(false);
        }


        Debug.Log(
            "三轮结束，等待进入下一页"
        );
    }


    //------------------------------------------------
    // 从 Page 3 进入 Page 4
    //------------------------------------------------

    public void GoNextPage()
    {
        currentState =
            GameState.Finished;


        // 先显示 Page 4
        if (page4 != null)
        {
            page4.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "GameManager 没有连接 Page 4"
            );
        }


        // 保证共用托盘和三个结果蛋糕继续显示
        if (resultContainer != null)
        {
            resultContainer.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "GameManager 没有连接 ResultContainer"
            );
        }


        // 初始化三个实际结果蛋糕的拖拽功能
        if (picnicManager != null)
        {
            picnicManager.PrepareExistingCakes();
        }
        else
        {
            Debug.LogError(
                "GameManager 没有连接 PicnicManager"
            );
        }


        // 最后隐藏 Page 3
        if (page3 != null)
        {
            page3.SetActive(false);
        }
        else
        {
            Debug.LogError(
                "GameManager 没有连接 Page 3"
            );
        }


        Debug.Log(
            "进入 Page 4，蛋糕拖拽初始化完成"
        );
    }
}