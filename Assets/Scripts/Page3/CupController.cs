using UnityEngine;

public class CupController : MonoBehaviour
{
    public Animator animator;

    public LeftButtonController leftButton;


    [Header("Cup Movement")]
    public float cupSpeed = 4f;


    private float cupProgress = 0;



    void Update()
    {

        if (animator == null || leftButton == null)
            return;



        cupProgress = Mathf.MoveTowards(
            cupProgress,
            leftButton.currentProgress,
            cupSpeed * Time.deltaTime
        );



        animator.Play(
            "Cup_Lift",
            0,
            cupProgress
        );



        // 手动控制动画进度
        animator.speed = 0;

    }





    //-----------------------------------------
    // 初始化杯子状态
    //-----------------------------------------

    public void ResetCup()
    {

        // 重置杯子进度
        cupProgress = 0;



        // 重置左按钮进度
        if (leftButton != null)
        {
            leftButton.currentProgress = 0;
        }



        // Animator回到最开始
        if (animator != null)
        {

            animator.Play(
                "Cup_Lift",
                0,
                0
            );


            animator.speed = 0;

        }



        Debug.Log(
            "杯子状态初始化完成"
        );

    }

}