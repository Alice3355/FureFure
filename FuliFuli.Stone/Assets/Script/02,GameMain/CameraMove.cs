using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// カメラをプレイヤーに追従させる
/// </summary>
public class CameraMove : MonoBehaviour
{
    private GameObject _player;
    [SerializeField, Tooltip("高さ")]
    private float _height = 5.0f;
    [SerializeField, Tooltip("離れ具合")]
    private float _distance = 8.0f;
    [SerializeField, Tooltip("動く速度")]
    private float _moveSpeed = 3.0f;

    [SerializeField, Tooltip("衝突判定のレイヤー")]
    private LayerMask _collisionLayer;

    private Vector3 _nowPos;
    private Vector3 _pastPos;

    private Vector3 _diff; // 移動距離

    private bool _followEnabled = true;

    private void Start()
    {
        _player = PlayerData.CreateAvatar;
        _pastPos = _player.transform.position;
        SetCameraPos();
    }

    private void Update()
    {
        // プレイヤーが破棄されたかチェック
        if (_player == null || _player.Equals(null))
        {
            // プレイヤーを再取得
            _player = PlayerData.CreateAvatar;

            if (_player != null)
            {
                SetCameraPos(); // カメラ位置をリセット
                _pastPos = _player.transform.position; // プレイヤー位置を更新
            }
        }

        if (_player != null)
        {
            Operate();
            Rotate();
        }
    }

    private void LateUpdate()
    {
        transform.LookAt(_player.transform); // 常にプレイヤーを向く
        if (_followEnabled)
        {
            // 通常のカメラ追従ロジック
            HandleCollision();
        }
    }

    /// <summary>
    /// カメラの衝突判定を行い、位置を調整する
    /// </summary>
    private void HandleCollision()
    {
        if (_player.GetComponent<MyNamespace.PlayerMove>().IsRushing || _player.GetComponent<MyNamespace.PlayerMove>().SetSpeed[1]> 6.0f)
            return;

        RaycastHit hit;

        //　キャラクターとカメラの間に障害物があったら障害物の位置にカメラを移動させる
        if (Physics.Linecast(_player.transform.position, transform.position, out hit, _collisionLayer))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                transform.position = hit.point;
                return;
            }
        }
        else
        {
            // 壁がない場合、カメラを元の距離に戻す
            Vector3 desiredPos = _player.transform.position - transform.forward * _distance;
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * _moveSpeed);
        }

        //　レイを視覚的に確認
        Debug.DrawLine(_player.transform.position, transform.position, Color.red, 0f, false);
    }

    public void SetFollowEnabled(bool enabled)
    {
        _followEnabled = enabled;
    }

    /// <summary>
    /// カメラの初期位置
    /// </summary>
    private void SetCameraPos()
    {
        Vector3 pos = _player.transform.position - _player.transform.forward * _distance;
        pos.y += _height;
        transform.position = pos;
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Operate()
    {
        // プレイヤーの現在地点
        _nowPos = _player.transform.position;
        _diff = _nowPos - _pastPos;

        transform.position = Vector3.Lerp(transform.position, transform.position + _diff, _moveSpeed);
        _pastPos = _nowPos;
    }

    /// <summary>
    /// 回転
    /// </summary>
    private void Rotate()
    {
        // マウスの移動量取得
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        Vector3 pos = _player.transform.position;

        // x方向に一定量移動していれば横回転
        if (Mathf.Abs(mx) > 0.01f)
        {
            // 回転軸はワールド座標のY軸
            transform.RotateAround(pos, Vector3.up, mx);
        }
        // Y方向に一定量移動していれば縦軸回転
        if (Mathf.Abs(my) > 0.01f)
        {
            // 回転軸はカメラ自身のX軸
            transform.RotateAround(pos, transform.right, -my);
        }
    }

    public void ResetCameraPos()
    {
        if (_player == null)
            _player = PlayerData.CreateAvatar;

        // カメラの初期位置にリセット
        Vector3 pos = _player.transform.position - _player.transform.forward * _distance;
        pos.y += _height;
        transform.position = pos;

        // カメラの回転もリセット（必要に応じて）
        transform.LookAt(_player.transform);
    }
}
