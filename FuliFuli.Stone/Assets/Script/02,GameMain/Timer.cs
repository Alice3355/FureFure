using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// タイマー機能全般
/// </summary>
public class Timer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    private int _timer = -10;

    /// <summary>
    /// 始まる前のカウントダウン
    /// </summary>
    public void StartTimer(int time)
    {
        if (_timer == -10)
            _timer = time;

        _timer -= (int)Time.deltaTime;
        _text.text = _timer.ToString();
        if (_timer < 0)
        {
            PlayerData.ISGameStart[2] = true;
            Destroy(gameObject);
        }
        else if (_timer < 1)
            _text.text = "はじめ！";
    }
}
