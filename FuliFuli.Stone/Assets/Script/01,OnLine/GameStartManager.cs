using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstantsManager;
/// <summary>
/// ゲームの始まりを管理する
/// </summary>
public class GameStartManager : MonoBehaviour
{
    public bool[] _isGameStart = { false, false };

    private void Update()
    {
        _isGameStart = PlayerData.ISGameStart;
        if(_isGameStart[0] && _isGameStart[1])
        {
            SceneChange(2);
        }
    }
}
