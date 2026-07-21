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


        animator.speed = 0;
    }
}