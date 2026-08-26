using System.Collections;
using UnityEngine;

public class CakeDragController : MonoBehaviour
{
    [Header("拖拽显示")]
    public int normalOrder = 20;
    public int draggingOrder = 50;
    public float draggingScale = 1.08f;

    [Header("动画速度")]
    public float snapDuration = 0.25f;
    public float returnDuration = 0.3f;

    private PicnicManager picnicManager;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private Collider2D cakeCollider;

    private Vector3 startPosition;
    private Vector3 originalScale;
    private Vector3 mouseOffset;

    private bool isDragging;
    private bool isAnimating;
    private bool isLocked;
    private bool isInitialized;


    // 进入 Page 4 后由 PicnicManager 调用
    public void Initialize(PicnicManager manager)
    {
        picnicManager = manager;

        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        cakeCollider = GetComponent<Collider2D>();

        startPosition = transform.position;
        originalScale = transform.localScale;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = normalOrder;
        }

        if (cakeCollider == null)
        {
            Debug.LogError(
                gameObject.name
                + " 没有 Collider2D，无法拖拽"
            );

            return;
        }

        isInitialized = true;
    }


    void OnMouseDown()
    {
        if (!isInitialized)
            return;

        if (isLocked || isAnimating)
            return;

        if (mainCamera == null)
            return;

        isDragging = true;

        mouseOffset =
            transform.position
            - GetMouseWorldPosition();

        transform.localScale =
            originalScale * draggingScale;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder =
                draggingOrder;
        }
    }


    void OnMouseDrag()
    {
        if (!isDragging)
            return;

        Vector3 targetPosition =
            GetMouseWorldPosition()
            + mouseOffset;

        targetPosition.z =
            transform.position.z;

        transform.position =
            targetPosition;
    }


    void OnMouseUp()
    {
        if (!isDragging)
            return;

        isDragging = false;

        Transform snapPoint;

        bool canPlace =
            picnicManager.TryReserveDropPoint(
                transform.position,
                out snapPoint
            );

        if (canPlace)
        {
            isLocked = true;

            StartCoroutine(
                SnapToPicnic(snapPoint)
            );
        }
        else
        {
            StartCoroutine(
                ReturnToStart()
            );
        }
    }


    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition =
            Input.mousePosition;

        mousePosition.z =
            Mathf.Abs(
                mainCamera.transform.position.z
                - transform.position.z
            );

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(
                mousePosition
            );

        worldPosition.z =
            transform.position.z;

        return worldPosition;
    }


    IEnumerator SnapToPicnic(
        Transform snapPoint
    )
    {
        isAnimating = true;

        Vector3 fromPosition =
            transform.position;

        Vector3 fromScale =
            transform.localScale;

        float timer = 0f;

        picnicManager.PlayPlaceSound();

        while (timer < snapDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / snapDuration
                );

            float smoothT =
                1f - Mathf.Pow(1f - t, 3f);

            float bounce =
                Mathf.Sin(t * Mathf.PI) * 0.1f;

            transform.position =
                Vector3.Lerp(
                    fromPosition,
                    snapPoint.position,
                    smoothT
                );

            transform.localScale =
                Vector3.Lerp(
                    fromScale,
                    originalScale,
                    smoothT
                )
                * (1f + bounce);

            yield return null;
        }

        transform.position =
            snapPoint.position;

        transform.localScale =
            originalScale;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder =
                normalOrder;
        }

        // 成功放置后不能再次点击
        if (cakeCollider != null)
        {
            cakeCollider.enabled = false;
        }

        isAnimating = false;

        picnicManager.NotifyCakePlaced();
    }


    IEnumerator ReturnToStart()
    {
        isAnimating = true;

        Vector3 fromPosition =
            transform.position;

        float timer = 0f;

        picnicManager.PlayReturnSound();

        while (timer < returnDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / returnDuration
                );

            float smoothT =
                1f - Mathf.Pow(1f - t, 3f);

            transform.position =
                Vector3.Lerp(
                    fromPosition,
                    startPosition,
                    smoothT
                );

            transform.localScale =
                Vector3.Lerp(
                    transform.localScale,
                    originalScale,
                    smoothT
                );

            yield return null;
        }

        transform.position =
            startPosition;

        transform.localScale =
            originalScale;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder =
                normalOrder;
        }

        isAnimating = false;
    }
}