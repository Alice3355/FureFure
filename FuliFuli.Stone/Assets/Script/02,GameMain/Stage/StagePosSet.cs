using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ゲームが始まったときの定位置
/// </summary>
public class StagePosSet : MonoBehaviour
{
    [SerializeField, Tooltip("箱の定位置")]
    public Vector3[] BoxPos;

    [SerializeField, Tooltip("プレイヤーの定位置")]
    public Vector3[] PlayerPos;

    [SerializeField, Tooltip("アイテムが落ちる範囲")]
    public Vector2[] ItemRainPos;
}
