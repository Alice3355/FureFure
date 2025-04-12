using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameConstantsManager : MonoBehaviour
{
    public static float PlayTime = 120.0f;
    public static float CombTime = 10.0f;
    public static float[] PlayerSpeed = {6.0f, 20.0f, 20.0f };
    public static float RushTime = 0.5f;
    public static float JumpPower = 5.0f;
    public static float StanTime = 5.0f;
    public static float[] FoodItemTime = { 10.0f, 5.0f};

    public static string[] SceneName = 
        { "00,Title", "01,OnLine", "02,GameMain",
        "03,Result", "04,StageSelect", "05,OffLineGameMain",
        "06,Result", "test"};

    /// <summary>
    /// シーン移行を管理
    /// </summary>
    /// <param name="num"></param>
    public static void SceneChange(int num)
    {
        SceneManager.LoadScene(SceneName[num]);
    }

    /// <summary>
    /// 今のシーンがどれかを調べる
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static bool IsSceneCheck(int num)
    {
        if (SceneManager.GetActiveScene().name == SceneName[num])
            return true;
        else
            return false;
    }
}
