using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Dragging,
        SlotMachine,
        Result
    }

    [Header("Current State")]
    public GameState currentState = GameState.Dragging;

    [Header("References")]
    public SlotMachineController slotMachine;

    [Header("Round Settings")]
    public int currentRound = 1;
    public int maxRound = 2;

    // 第一轮、第二轮扫描速度
    public float[] roundSpeeds =
    {
        0.18f,
        0.12f
    };

    //------------------------------------------------
    // 左按钮拖满后调用
    //------------------------------------------------
    public void EnterSlotMachine()
    {
        if (currentState == GameState.SlotMachine)
            return;

        currentState = GameState.SlotMachine;

        slotMachine.selectorSpeed = roundSpeeds[currentRound - 1];

        slotMachine.StartSpin();

        Debug.Log("开始第 " + currentRound + " 轮");
    }

    //------------------------------------------------
    // 老虎机结束
    //------------------------------------------------
    public void FinishRound(int result)
    {
        currentState = GameState.Result;

        Debug.Log("第 " + currentRound + " 轮结果：" + result);

        // 下一步这里生成蛋糕
        // CakeManager.ShowCake(result);

        if (currentRound >= maxRound)
        {
            Debug.Log("小游戏结束！");
            return;
        }

        currentRound++;

        Debug.Log("准备第二轮...");
    }

    //------------------------------------------------
    // 开始下一轮
    //------------------------------------------------
    public void StartNextRound()
    {
        currentState = GameState.Dragging;

        slotMachine.ResetSlotMachine();

        Debug.Log("开始第 " + currentRound + " 轮");
    }
}