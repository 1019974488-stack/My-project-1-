using UnityEngine;


public class PicnicManager : MonoBehaviour
{


    void Start()
    {

        LoadCakeData();

    }




    void LoadCakeData()
    {


        for (int i = 0; i < 3; i++)
        {


            int result =
                CakeData.Instance.GetCakeResult(i);



            Debug.Log(
                "第 "
                + (i + 1)
                + " 个蛋糕结果:"
                + result
            );


        }


    }


}