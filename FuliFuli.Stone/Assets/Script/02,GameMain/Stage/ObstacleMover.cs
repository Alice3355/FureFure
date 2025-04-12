using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstantsManager;
/// <summary>
/// ステージ内の動き
/// </summary>
public class ObstacleMover : MonoBehaviour
{
    [SerializeField, Tooltip("動く範囲")]
    private Vector3[] _movePos;

    private Vector3 _pos;
    private float _moveSpeed = 2.0f;

    // 動かす方向
    private bool _isMove = true;
    private int _num = 2;

    private GameObject _plaeyr = null;

    private void Start()
    {
        _pos = this.transform.position;
        transform.SetParent(null);
        DontDestroyOnLoad(transform);
    }

    private void Update()
    {
        _pos = this.transform.position;

        if(_pos == _movePos[_num])
        {
            if (_isMove && _num >= _movePos.Length - 1)
            {
                _isMove = false;
                _num--;
            }
            else if (!_isMove && _num <= 0)
            {
                _isMove = true;
                _num++;
            }
            else
                _num += _isMove ? 1 : -1;
        }

        _pos = Vector3.MoveTowards(_pos, _movePos[_num], _moveSpeed * Time.deltaTime);
        this.transform.position = _pos;

        if (!IsSceneCheck(2) && !IsSceneCheck(5))
        {
            if(_plaeyr != null)
                _plaeyr.transform.SetParent(null);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 当たり判定入り
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.CompareTag("Player"))
        {
            _plaeyr = collision.gameObject;
            collision.transform.SetParent(this.transform);
        }
    }
    /// <summary>
    /// 当たり判定出る
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            _plaeyr = null;
            collision.transform.SetParent(null);
        }
    }
}
