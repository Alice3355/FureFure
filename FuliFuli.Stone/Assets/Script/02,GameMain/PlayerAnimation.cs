using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// プレイヤーのアニメーションの管理
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    private bool[] _isFlgList = new bool[3];
    private string[] _isName = { "IsWalk", "IsJump", "IsLift", "Jump" };
    [SerializeField, Tooltip("アニメーター")]
    private Animator[] _animator;
    [SerializeField]
    private int _num;

    public void SendIs(bool flg, int num)
    {
        if (_isFlgList[num] == flg)
            return;

        int skinNum = PlayerData.SelectSkinNum[_num];
        _isFlgList[num] = flg;

        if (_animator[skinNum] == null)
        {
            Debug.Log("アニメーションがありません");
            return;
        }

        _animator[skinNum].SetBool(_isName[num], _isFlgList[num]);
    }

    public void SnedTrigger(int num)
    {
        int skinNum = PlayerData.SelectSkinNum[_num];
        _animator[skinNum].SetTrigger(_isName[num]);
    }
}
