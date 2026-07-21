using System.Collections;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    [Header("Manager")]
    public GameManager gameManager;

    [Header("Cup Slots")]
    public SpriteRenderer[] cupSlots;

    [Header("Scan Speed")]
    public float selectorSpeed = 0.18f;

    [Header("Flash")]
    public float flashSpeed = 0.12f;
    public int flashCount = 2;

    [Header("Color")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.black;

    private bool isSpinning = false;

    private int currentIndex = 0;
    private int direction = 1;

    public GameObject result1;
    public GameObject result2;
    public GameObject result3;

    public GameObject reStartButton;
    public int CurrentIndex => currentIndex;

    //------------------------------------------------
    // 开始扫描
    //------------------------------------------------
    public void StartSpin()
    {
        if (isSpinning)
            return;

        isSpinning = true;

        StopAllCoroutines();
        StartCoroutine(SpinRoutine());

        Debug.Log("老虎机开始");
    }

    //------------------------------------------------
    // 停止扫描（右按钮调用）
    //------------------------------------------------
    public void StopSpin()
    {
        if (!isSpinning)
            return;

        isSpinning = false;

        StopAllCoroutines();

        RefreshSlots();

        // 持续闪光闪烁

        StartCoroutine(ResultRoutine());
    }

    //------------------------------------------------
    // 扫描逻辑（钟摆）
    //------------------------------------------------
    IEnumerator SpinRoutine()
    {
        while (isSpinning)
        {
            RefreshSlots();

            yield return new WaitForSeconds(selectorSpeed);

            currentIndex += direction;

            if (currentIndex >= cupSlots.Length - 1)
            {
                currentIndex = cupSlots.Length - 1;
                direction = -1;
            }
            else if (currentIndex <= 0)
            {
                currentIndex = 0;
                direction = 1;
            }
        }
    }

    //------------------------------------------------
    // 更新三个杯子的颜色
    //------------------------------------------------
    void RefreshSlots()
    {
        for (int i = 0; i < cupSlots.Length; i++)
        {
            cupSlots[i].color = (i == currentIndex)
                ? selectedColor
                : normalColor;
        }
    }

    //------------------------------------------------
    // 结果反馈
    //------------------------------------------------
    IEnumerator ResultRoutine()
    {
        yield return new WaitForSeconds(0.15f);

        // currentIndex 随机结果

        SpriteRenderer slot = cupSlots[currentIndex];

        for (int i = 0; i < flashCount; i++)
        {
            slot.color = normalColor;
            yield return new WaitForSeconds(flashSpeed);

            slot.color = selectedColor;
            yield return new WaitForSeconds(flashSpeed);
        }

        slot.color = selectedColor;

        Debug.Log("最终结果：" + currentIndex);

        if (gameManager != null)
        {
            gameManager.FinishRound(currentIndex);
        }

        // qidong
        StartCoroutine(ShowResultOutCome(currentIndex));
    }

    //------------------------------------------------
    // 第二轮重新开始
    //------------------------------------------------
    public void ResetSlotMachine()
    {
        StopAllCoroutines();

        isSpinning = false;

        currentIndex = 0;
        direction = 1;

        RefreshSlots();
    }

    public void ShowUpRestartButton()
    {
        // canvas group shows up by alpha 0-1
    }

    public IEnumerator ShowResultOutCome(int index)
    {
        // result in which case to show up.?
        // 


        yield break;
        //ShowUpRestartButton();?

    }
}