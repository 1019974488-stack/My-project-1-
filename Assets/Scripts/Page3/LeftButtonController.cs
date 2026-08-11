using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;


public class LeftButtonController : MonoBehaviour,
    IPointerEnterHandler,
    IPointerDownHandler,
    IPointerUpHandler
{


    [Header("Bezier")]
    public Transform p0;
    public Transform p1;
    public Transform p2;
    public Transform p3;



    [Header("Progress")]
    [Range(0, 1)]
    public float targetProgress = 0;


    [HideInInspector]
    public float currentProgress = 0;



    [Header("Manager")]
    public GameManager gameManager;



    private bool isDragging = false;

    private bool isAnimating = false;


    private bool hasEnteredSlotMachine = false;


    private float startMouseX;






    void Update()
    {

        //----------------------------
        // 拖动
        //----------------------------

        if (isDragging)
        {

            float delta =
                Input.mousePosition.x - startMouseX;


            targetProgress +=
                delta * 0.0015f;


            targetProgress =
                Mathf.Clamp01(targetProgress);



            startMouseX =
                Input.mousePosition.x;

        }





        //----------------------------
        // 平滑移动
        //----------------------------

        currentProgress =
            Mathf.MoveTowards(
                currentProgress,
                targetProgress,
                2f * Time.deltaTime
            );





        //----------------------------
        // 贝塞尔移动
        //----------------------------

        transform.position =
            BezierUtility.GetPoint(
                p0.position,
                p1.position,
                p2.position,
                p3.position,
                currentProgress
            );






        //----------------------------
        // 进入老虎机
        //----------------------------

        if (targetProgress >= 1f &&
           !hasEnteredSlotMachine)
        {

            hasEnteredSlotMachine = true;



            Debug.Log(
                "进入老虎机"
            );



            if (gameManager != null)
            {
                gameManager.EnterSlotMachine();
            }

        }

    }







    public void OnPointerDown(
        PointerEventData eventData
    )
    {

        isDragging = true;

        startMouseX =
            Input.mousePosition.x;

    }






    public void OnPointerUp(
        PointerEventData eventData
    )
    {

        isDragging = false;

    }






    public void OnPointerEnter(
        PointerEventData eventData
    )
    {

        if (!isAnimating)
        {
            StartCoroutine(
                HoverAnimation()
            );
        }

    }








    IEnumerator HoverAnimation()
    {

        isAnimating = true;


        Vector3 origin =
            Vector3.one;


        Vector3 target =
            Vector3.one * 1.1f;



        float t = 0;



        while (t < 0.12f)
        {

            t += Time.deltaTime;


            transform.localScale =
                Vector3.Lerp(
                    origin,
                    target,
                    t / 0.12f
                );


            yield return null;

        }




        t = 0;



        while (t < 0.12f)
        {

            t += Time.deltaTime;


            transform.localScale =
                Vector3.Lerp(
                    target,
                    origin,
                    t / 0.12f
                );


            yield return null;

        }




        transform.localScale =
            origin;


        isAnimating = false;

    }








    //------------------------------------------------
    // 点击Restart时调用
    // 开始下一轮
    //------------------------------------------------

    public void StartNextRoundReset()
    {

        targetProgress = 0;

        currentProgress = 0;


        hasEnteredSlotMachine = false;



        transform.position =
            p0.position;



        if (gameManager != null)
        {
            gameManager.StartNextRound();
        }


    }








    //------------------------------------------------
    // 第三轮结束调用
    // 只初始化按钮
    //------------------------------------------------

    public void ResetButton()
    {

        targetProgress = 0;


        currentProgress = 0;


        hasEnteredSlotMachine = false;



        transform.position =
            p0.position;



        Debug.Log(
            "左按钮初始化完成"
        );

    }


}