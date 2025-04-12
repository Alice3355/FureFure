using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 回転ハンマー
/// </summary>
public class RotatingHammer : MonoBehaviour
{
    private float _rotaSpeed = 100.0f;

    private void Update()
    {
        transform.Rotate(new Vector3(0.0f, _rotaSpeed, 0.0f) * Time.deltaTime);
    }
}
