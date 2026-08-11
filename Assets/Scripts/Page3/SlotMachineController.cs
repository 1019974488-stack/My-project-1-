using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


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




    [Header("Result Objects")]
    public GameObject result1;
    public GameObject result2;
    public GameObject result3;

    public GameObject resultContainer1;
    public GameObject resultContainer2;
    public GameObject resultContainer3;

    [Header("Restart / Next Button")]
    public GameObject reStartButton;

    public TMP_Text buttonText;

    public Button button;

    private bool isSpinning = false;

    private int currentIndex = 0;

    private int direction = 1;



    public int CurrentIndex => currentIndex;


    void Start()
    {
        if (reStartButton != null)
        {
            reStartButton.SetActive(false);
        }
    }







    //------------------------------------------------
    // 开始扫描
    //------------------------------------------------

    public void StartSpin()
    {

        if (isSpinning)
            return;



        // 注意：
        // 不再HideResults
        // 保留之前轮次结果


        isSpinning = true;



        StopAllCoroutines();



        StartCoroutine(
            SpinRoutine()
        );


        Debug.Log(
            "老虎机开始"
        );

    }

    //------------------------------------------------
    // 停止
    //------------------------------------------------

    public void StopSpin()
    {

        if (!isSpinning)
            return;

        isSpinning = false;

        StopAllCoroutines();

        RefreshSlots();

        StartCoroutine(
            ResultRoutine()
        );

    }

    IEnumerator SpinRoutine()
    {


        while (isSpinning)
        {


            RefreshSlots();



            yield return new WaitForSeconds(
                selectorSpeed
            );



            currentIndex += direction;



            if (currentIndex >= cupSlots.Length - 1)
            {

                currentIndex =
                    cupSlots.Length - 1;


                direction = -1;

            }


            else if (currentIndex <= 0)
            {

                currentIndex = 0;


                direction = 1;

            }


        }

    }









    void RefreshSlots()
    {

        for (int i = 0; i < cupSlots.Length; i++)
        {

            cupSlots[i].color =
            (i == currentIndex)
            ? selectedColor
            : normalColor;

        }

    }









    IEnumerator ResultRoutine()
    {


        yield return new WaitForSeconds(0.15f);



        SpriteRenderer slot =
            cupSlots[currentIndex];



        for (int i = 0; i < flashCount; i++)
        {

            slot.color = normalColor;


            yield return new WaitForSeconds(
                flashSpeed
            );


            slot.color = selectedColor;


            yield return new WaitForSeconds(
                flashSpeed
            );

        }



        slot.color = selectedColor;



        Debug.Log(
           "当前是第：" + gameManager.currentRound +"轮。"+ "最终结果：" + currentIndex
        );



        if (gameManager != null)
        {

            gameManager.FinishRound(
                currentIndex
            );

        }
        // show result

        yield return StartCoroutine(
            ShowResultOutCome(gameManager.currentRound, currentIndex)
        );


    }









    //------------------------------------------------
    // 显示结果
    //------------------------------------------------

    public IEnumerator ShowResultOutCome(int round ,int index)
    {
        yield return new WaitForSeconds(0.3f);

        GameObject result = result1;

        switch (index)
        {

            case 0:
                result = result1;
                //if (result1 != null)
                //    result1.SetActive(true);
                break;
            case 1:
                result = result2;
                //if (result2 != null)
                //    result2.SetActive(true);
                break;
            case 2:
                result = result3;
                //if (result3 != null)
                //    result3.SetActive(true);
                break;
        }


        GameObject temp = Instantiate(result);

        if (round == 1)
        {
            temp.transform.parent = resultContainer1.transform;
            temp.transform.localPosition = Vector3.zero;
        }
        if (round == 2)
        {
            temp.transform.parent = resultContainer2.transform;
            temp.transform.localPosition = Vector3.zero;
        }
        if (round == 3)
        {
            temp.transform.parent = resultContainer3.transform;
            temp.transform.localPosition = Vector3.zero;
        }
        yield return new WaitForSeconds(0.8f);

        ShowUpRestartButton();
    }

    void HideResults()
    {

        if (result1 != null)
            result1.SetActive(false);


        if (result2 != null)
            result2.SetActive(false);


        if (result3 != null)
            result3.SetActive(false);

    }
    //------------------------------------------------
    // Restart / Next按钮
    //------------------------------------------------

    public void ShowUpRestartButton()
    {

        if (reStartButton == null)
            return;

        reStartButton.SetActive(true);

        // 第三轮
        if (gameManager.currentRound >= gameManager.maxRound)
        {
            buttonText.text = "Next Page";

            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(
                gameManager.GoNextPage
            );

            Debug.Log(
                "第三轮完成，显示Next"
            );

            // 自动初始化
            StartCoroutine(
                AutoResetFinalRound()
            );
        }
        else
        {


            buttonText.text = "Restart";

            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(
                gameManager.StartNextRound
            );


            Debug.Log(
                "显示Restart"
            );

        }


    }









    //------------------------------------------------
    // 第三轮自动初始化
    //------------------------------------------------

    IEnumerator AutoResetFinalRound()
    {


        yield return new WaitForSeconds(1f);



        // 重置老虎机
        FinalResetSlotMachine();



        // 重置杯子
        if (gameManager.cupController != null)
        {

            gameManager.cupController.ResetCup();

        }



        // 重置左按钮
        if (gameManager.leftButton != null)
        {

            gameManager.leftButton.ResetButton();

        }



        Debug.Log(
            "第三轮结束，操作区域初始化完成"
        );


    }









    //------------------------------------------------
    // 普通Restart
    //------------------------------------------------

    public void ResetSlotMachine()
    {

        StopAllCoroutines();


        isSpinning = false;


        currentIndex = 0;


        direction = 1;



        if (reStartButton != null)
        {
            reStartButton.SetActive(false);
        }



        RefreshSlots();


    }









    //------------------------------------------------
    // 最终初始化
    // 保留结果
    //------------------------------------------------

    public void FinalResetSlotMachine()
    {

        StopAllCoroutines();


        isSpinning = false;


        currentIndex = 0;


        direction = 1;



        RefreshSlots();



        Debug.Log(
            "老虎机初始化，结果保留"
        );

    }



}