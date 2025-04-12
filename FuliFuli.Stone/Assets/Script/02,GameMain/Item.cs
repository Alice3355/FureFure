using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static GameConstantsManager;
/// <summary>
/// アイテムの処理
/// </summary>
public class Item : MonoBehaviourPunCallbacks, IPunObservable
{
    private GameObject _player;
    public bool _isItemHavint = false;
    private bool _isFirstRun = false;

    private Rigidbody _rb;
    private Collider _collider;

    private float[] _destroyTimer = { 0.0f, FoodItemTime[0] };

    private float _bombRaduis = FoodItemTime[1];

    [SerializeField, Tooltip("エフェクト")]
    private GameObject _effect;

    private SESet _getSESet = null;

    [SerializeField]
    private Renderer _renderer;
    private float _cycle = 0.5f;
    private double _time;

    private void Start()
    {
        _rb = this.transform.GetComponent<Rigidbody>();
        _collider = gameObject.GetComponent<Collider>();
        _destroyTimer[0] = _destroyTimer[1];

        if (GameObject.Find("SE"))
            _getSESet = GameObject.Find("SE").GetComponent<SESet>();
    }

    private void Update()
    {
        // ずっと
        if (_isItemHavint)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _destroyTimer[0] = _destroyTimer[1];
            if(!_renderer.enabled)
                _renderer.enabled = true;
        }
        else
        {
            _destroyTimer[0] -= Time.deltaTime;
            if(_destroyTimer[0] < 3)
            {
                Blinking();
            }
        }

        // マスター側でオフラインオブジェクトのとき
        if(PhotonNetwork.IsMasterClient || photonView.IsMine)
        {
            
            if (_destroyTimer[0] < 0)
            {
                if (transform.CompareTag("Bomb"))
                {
                    BombExplosion();
                    BombDestroy();
                    // 全クライアントにBombDestroyを通知
                    photonView.RPC("BombDestroy", RpcTarget.All);
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }

        // アイテムを持っていない
        if (_player != null && !_player.GetComponent<MyNamespace.PlayerMove>().IsHaveItem)
        {
            SetItemReleased();
        }

        // １回だけ
        if (_isItemHavint && !_isFirstRun)
        {
            SetItemHaving();
            _isFirstRun = true;
        }
        if(PhotonNetwork.IsMasterClient || photonView.IsMine)
        {
            if (!IsSceneCheck(2))
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 点滅
    /// </summary>
    private void Blinking()
    {
        if(_isItemHavint)
        {
            _renderer.enabled = true;
            return;
        }

        _time += Time.deltaTime;

        var repeatValue = Mathf.Repeat((float)_time, _cycle);
        _renderer.enabled = repeatValue >= _cycle * 0.2f;
    }

    /// <summary>
    /// 爆弾が爆発したときの処理
    /// </summary>
    private void BombExplosion()
    {
        Collider[] hitPlayer = Physics.OverlapSphere(transform.position, _bombRaduis);
        foreach(var hitPlayerSet in hitPlayer)
        {
            if(hitPlayerSet.CompareTag("Player"))
            {
                if(PlayerData.CreateAvatar != hitPlayerSet)
                {
                    PlayerData.CreateAvatar.GetComponent<DataSyncManager>()
                        .SendData(new object[] { true }, 9);
                }
                hitPlayerSet.GetComponent<MyNamespace.PlayerMove>().BombExplosion();
            }
        }
    }

    /// <summary>
    /// 爆弾の削除
    /// </summary>
    [PunRPC]
    public void BombDestroy()
    {
        if (!GameObject.Find("SE"))
            return;

        if (!transform.CompareTag("Bomb") || !IsSceneCheck(2))
            return;

        if (_getSESet == null && !GameObject.Find("SE"))
        {
            _getSESet = GameObject.Find("SE").GetComponent<SESet>();
        }
        if (_getSESet == null)
            return;

        Vector3 pos = transform.position;
        pos.y += 1.5f;
        if(IsSceneCheck(2))
            Instantiate(_effect, pos, Quaternion.identity);

        _getSESet.SetClip(4);
    }
    /// <summary>
    /// 当たり判定入り
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // クライアント側ははじく
        if (!PhotonNetwork.IsMasterClient)
            return;

        if(collision.gameObject.CompareTag("Box"))
        {
            // 全クライアントにBombDestroyを通知
            photonView.RPC("BombDestroy", RpcTarget.All);
        }

        // ステージ外に出たら
        if(collision.gameObject.CompareTag("Plane"))
        {
            PhotonNetwork.Destroy(gameObject);
        }

        // プレイヤーじゃないならはじく
        if (!collision.gameObject.CompareTag("Player"))
            return;

        // 代入と親子関係と位置セット
        _player = collision.gameObject;

        if (!_isItemHavint && !_player.GetComponent<MyNamespace.PlayerMove>().IsHaveItem)
        {
            transform.SetParent(collision.transform);
            GameObject haveItem = gameObject.transform.parent.Find("HaveItem").gameObject;
            transform.position = haveItem.transform.position;
            _isItemHavint = true;
            _player.GetComponent<MyNamespace.PlayerMove>().IsHaveItem = true;
            int playerID = _player.GetComponent<PhotonView>().ViewID;
            photonView.RPC("SetParent", RpcTarget.Others, playerID, _isItemHavint);
        }
    }

    /// <summary>
    /// 当たり判定出る
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isItemHavint = false;
            _isFirstRun = false;
        }
    }

    /// <summary>
    /// 親にしたいプレイヤーのIDと持ってるかについての受け渡し
    /// </summary>
    [PunRPC]
    private void SetParent(int playerID, bool isItem)
    {
        if(!_isItemHavint)
        {
            _player = PhotonView.Find(playerID).gameObject;
            _isItemHavint = isItem;
            _player.GetComponent<MyNamespace.PlayerMove>().IsHaveItem = true;
        }
    }

    /// <summary>
    /// 持ったときの処理
    /// </summary>
    private void SetItemHaving()
    {
        if (_player != null)
        {
            transform.SetParent(_player.transform);
            GameObject haveItem = gameObject.transform.parent.Find("HaveItem").gameObject;
            transform.position = haveItem.transform.position;
            // 位置移動や回転をさせない
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            _rb.constraints = RigidbodyConstraints.FreezePosition;
            _collider.enabled = false;
            _rb.useGravity = false; // 重力を無効化
            _rb.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    /// <summary>
    /// アイテムを離した処理
    /// </summary>
    private void SetItemReleased()
    {
        this.transform.parent = null;
        _rb.constraints = RigidbodyConstraints.None;
        _collider.enabled = true;
        _rb.useGravity = true; // 重力を有効効化
    }


    /// <summary>
    /// IPunObservableの実装
    /// </summary>
    /// <param name = "stream" ></ param >
    /// < param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }
}
