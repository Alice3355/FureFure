using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static string PlayerName;
    public static string OpponentName;
    // 0:自分　1:相手
    public static int[] SelectSkinNum = new int [2];

    // 自分で生成したアバター
    public static GameObject CreateAvatar;

    // 3つ目はゲームシーンでのスタート確認
    public static bool[] ISGameStart = { false, false, false};

    // 0:未設定、1以降はそこの場所
    // TODO:人数が変わると変える
    public static int[] GameMainPosSet = { 0, 0 };

    public static int[] Score = { 0, 0 };

    public static bool[] IsRetry = {false, false};

    public static bool IsUseRainItem = false;
    public static Vector3 RainPos;

    /// <summary>
    /// 初期値に戻す
    /// </summary>
    public static void Reset()
    {
        PlayerName = null;
        OpponentName = null;
        SelectSkinNum = new int[2]{0, 0};
        CreateAvatar = null;
        ISGameStart = new bool[3]{ false, false, false};
        GameMainPosSet = new int[2] { 0, 0 };
        Score = new int[2] { 0, 0 };
        IsRetry = new bool[2] { false, false };
    }

    public static void Retry()
    {
        Score = new int[2] { 0, 0 };
        IsRetry = new bool[2] { false, false };
        ISGameStart = new bool[3] { true, true, false };
    }
}
