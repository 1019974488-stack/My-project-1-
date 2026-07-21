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

    // 防止老虎机重复进入
    private bool hasEnteredSlotMachine = false;

    private float startMouseX;

    void Update()
    {
        // -----------------------------
        // 1. 鼠标拖动控制目标位置
        // -----------------------------
        if (isDragging)
        {
            float delta = Input.mousePosition.x - startMouseX;

            targetProgress += delta * 0.0015f;
            targetProgress = Mathf.Clamp01(targetProgress);

            startMouseX = Input.mousePosition.x;
        }

        // -----------------------------
        // 2. 当前进度缓慢追赶目标进度
        // -----------------------------
        currentProgress = Mathf.MoveTowards(
            currentProgress,
            targetProgress,
            2f * Time.deltaTime
        );

        // -----------------------------
        // 3. 按当前进度移动按钮
        // -----------------------------
        transform.position = BezierUtility.GetPoint(
            p0.position,
            p1.position,
            p2.position,
            p3.position,
            currentProgress
        );

        // -----------------------------
        // 4. 拉满后进入老虎机
        // 注意：这里判断 targetProgress，不判断 currentProgress
        // -----------------------------
        if (targetProgress >= 1f && !hasEnteredSlotMachine)
        {
            hasEnteredSlotMachine = true;

            Debug.Log("进入老虎机");

            if (gameManager != null)
            {
                gameManager.EnterSlotMachine();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        startMouseX = Input.mousePosition.x;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            StartCoroutine(HoverAnimation());
        }
    }

    IEnumerator HoverAnimation()
    {
        isAnimating = true;

        Vector3 origin = Vector3.one;
        Vector3 target = Vector3.one * 1.1f;

        float t = 0;

        while (t < 0.12f)
        {
            t += Time.deltaTime;

            transform.localScale = Vector3.Lerp(
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

            transform.localScale = Vector3.Lerp(
                target,
                origin,
                t / 0.12f
            );

            yield return null;
        }

        transform.localScale = origin;

        isAnimating = false;
    }

    // 第二轮开始时调用
    public void ResetButton()
    {
        targetProgress = 0;
        currentProgress = 0;
        hasEnteredSlotMachine = false;

        transform.position = p0.position;
    }
}