using UnityEngine;


public class CakeData : MonoBehaviour
{

    public static CakeData Instance;


    // 保存三轮蛋糕结果
    // 1 = result1
    // 2 = result2
    // 3 = result3

    public int[] cakeResults = new int[3];



    void Awake()
    {

        if (Instance == null)
        {

            Instance = this;

            DontDestroyOnLoad(gameObject);

        }
        else
        {

            Destroy(gameObject);

        }

    }





    // 保存结果

    public void SaveCake(
        int round,
        int result
    )
    {

        cakeResults[round - 1] = result;


        Debug.Log(
            "保存第 "
            + round
            + " 轮蛋糕结果:"
            + result
        );

    }





    // 读取结果

    public int GetCakeResult(
        int index
    )
    {

        return cakeResults[index];

    }


}