using UnityEngine;

public class CakeData : MonoBehaviour
{
    public static CakeData Instance;

    // 保存三轮蛋糕结果
    // 数组位置：
    // cakeResults[0] = 第1轮
    // cakeResults[1] = 第2轮
    // cakeResults[2] = 第3轮
    //
    // 蛋糕结果：
    // 0 = result1
    // 1 = result2
    // 2 = result3

    public int[] cakeResults = new int[3];

    // 用来判断每一轮是否已经保存
    private bool[] resultSaved = new bool[3];


    void Awake()
    {
        // 第一次创建 CakeData
        if (Instance == null)
        {
            Instance = this;

            // 切换到下一场景时保留该对象
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果场景中出现第二个 CakeData，则删除重复对象
            Destroy(gameObject);
        }
    }


    // 保存一轮游戏结果
    // round 使用 1、2、3
    // result 使用 0、1、2
    public void SaveCake(int round, int result)
    {
        int index = round - 1;

        // 检查轮次是否正确
        if (index < 0 || index >= cakeResults.Length)
        {
            Debug.LogError(
                "保存蛋糕失败：round 必须是 1、2 或 3。当前值："
                + round
            );

            return;
        }

        // 检查蛋糕结果是否正确
        if (result < 0 || result > 2)
        {
            Debug.LogError(
                "保存蛋糕失败：result 必须是 0、1 或 2。当前值："
                + result
            );

            return;
        }

        cakeResults[index] = result;

        resultSaved[index] = true;

        Debug.Log(
            "保存第 "
            + round
            + " 轮蛋糕结果："
            + result
        );
    }


    // 读取蛋糕结果
    // index 使用 0、1、2
    public int GetCakeResult(int index)
    {
        // 检查数组位置是否正确
        if (index < 0 || index >= cakeResults.Length)
        {
            Debug.LogError(
                "读取蛋糕失败：index 必须是 0、1 或 2。当前值："
                + index
            );

            return -1;
        }

        // 如果这一轮还没有保存结果
        if (resultSaved[index] == false)
        {
            Debug.LogWarning(
                "第 "
                + (index + 1)
                + " 轮还没有保存蛋糕结果"
            );

            return -1;
        }

        return cakeResults[index];
    }


    // 检查三轮游戏是否全部完成
    public bool HasAllCakeResults()
    {
        for (int i = 0; i < resultSaved.Length; i++)
        {
            if (resultSaved[i] == false)
            {
                return false;
            }
        }

        return true;
    }
}