using UnityEngine;

public class PicnicManager : MonoBehaviour
{
    [Header("Page 3 已生成蛋糕的三个容器")]
    public Transform[] resultContainers = new Transform[3];

    [Header("野餐布有效区域")]
    public Collider2D picnicDropArea;

    [Header("三个吸附位置")]
    public Transform[] snapPoints = new Transform[3];

    [Header("音效，可以暂时留空")]
    public AudioSource audioSource;
    public AudioClip placeSound;
    public AudioClip returnSound;

    public int TotalCakeCount { get; private set; }
    public int PlacedCakeCount { get; private set; }

    private bool[] snapPointUsed;


    void Start()
    {
        PrepareExistingCakes();
    }


    public void PrepareExistingCakes()
    {
        TotalCakeCount = 0;
        PlacedCakeCount = 0;

        snapPointUsed =
            new bool[snapPoints.Length];


        for (int i = 0;
             i < resultContainers.Length;
             i++)
        {
            if (resultContainers[i] == null)
            {
                Debug.LogError(
                    "Result Containers 的 Element "
                    + i
                    + " 没有连接"
                );

                continue;
            }


            CakeDragController cake =
                resultContainers[i]
                .GetComponentInChildren
                <CakeDragController>(true);


            if (cake == null)
            {
                Debug.LogError(
                    "第 "
                    + (i + 1)
                    + " 个容器中没有找到蛋糕拖拽脚本"
                );

                continue;
            }


            // 先初始化，再开启拖拽组件
            cake.Initialize(this);

            cake.enabled = true;

            TotalCakeCount++;
        }


        Debug.Log(
            "已准备拖拽蛋糕数量："
            + TotalCakeCount
        );
    }


    public bool TryReserveDropPoint(
        Vector2 cakePosition,
        out Transform selectedPoint
    )
    {
        selectedPoint = null;


        // 没有进入野餐布区域
        if (picnicDropArea == null ||
            !picnicDropArea.OverlapPoint(
                cakePosition
            ))
        {
            return false;
        }


        int nearestIndex = -1;

        float nearestDistance =
            Mathf.Infinity;


        for (int i = 0;
             i < snapPoints.Length;
             i++)
        {
            if (snapPoints[i] == null)
                continue;

            if (snapPointUsed[i])
                continue;


            float distance =
                Vector2.SqrMagnitude(
                    (Vector2)snapPoints[i].position
                    - cakePosition
                );


            if (distance < nearestDistance)
            {
                nearestDistance = distance;

                nearestIndex = i;
            }
        }


        // 放置区已经没有空位
        if (nearestIndex == -1)
        {
            return false;
        }


        snapPointUsed[nearestIndex] =
            true;

        selectedPoint =
            snapPoints[nearestIndex];

        return true;
    }


    public void NotifyCakePlaced()
    {
        PlacedCakeCount++;

        Debug.Log(
            "蛋糕放置进度："
            + PlacedCakeCount
            + " / "
            + TotalCakeCount
        );


        if (TotalCakeCount == 3 &&
            PlacedCakeCount >= TotalCakeCount)
        {
            Debug.Log(
                "三个蛋糕全部放置完成，等待接入下一页"
            );

            // 后期在这里调用统一翻页系统
        }
    }


    public void PlayPlaceSound()
    {
        if (audioSource != null &&
            placeSound != null)
        {
            audioSource.PlayOneShot(
                placeSound
            );
        }
    }


    public void PlayReturnSound()
    {
        if (audioSource != null &&
            returnSound != null)
        {
            audioSource.PlayOneShot(
                returnSound
            );
        }
    }
}