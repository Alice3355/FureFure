using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using TMPro;
using Photon.Pun;
/// <summary>
/// アイテムが納品されたときの処理
/// </summary>
public class HandleDelivery : MonoBehaviourPun
{
    [SerializeField, Tooltip("アイテムの種類")]
    private GameObject[] _foodObject;
    [SerializeField, Tooltip("マテリアル")]
    private Material[] _materials;
    [SerializeField, Tooltip("名前の表示")]
    private TextMeshProUGUI _nameText;

    [System.NonSerialized]
    public int BoxPosID = 0;
    [SerializeField, Tooltip("UI")]
    private Canvas _canvas;

    //private Item _getItem;
    private int _score = 0;

    private float[] _combTimer = {10.0f, 0.0f };
    private Stopwatch _stopwatch;
    // コンボ数Count
    private int _combCnt = 0;
    // 選ばれたアイテムの番号
    private int _selectItemNum = 0;
    // 対象のプレイヤーの名前
    private string _playerName;
    private GameObject _playerObject;

    public TextMeshProUGUI text;

    // 補助アイテム関連
    [System.NonSerialized]
    public RandomBuffDistributor BuffDistributor;

    private bool _isUseBuffItemDecision = false;
    private int _nextItemNum = 0;

    private void Start()
    {
        // 相手の情報
        if (!photonView.IsMine)
        {
            PlayerData.CreateAvatar.GetComponent<DataSyncManager>().GetHandheld = this;
            _playerName = PlayerData.OpponentName;
            _nameText.text = _playerName;
            return;
        }

        // オフラインのプレイヤーを探す
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < objects.Length; i++)
        {
            if(objects[i].GetComponent<MyNamespace.PlayerMove>().PlayerPosID == BoxPosID)
            {
                _playerObject = objects[i];
                break;
            }
        }
        // 自分のプレイヤーの情報
        _playerName = PlayerData.PlayerName;
        _nameText.text = _playerName;

        // コンボ用ストップウィッチ
        _stopwatch = new Stopwatch();
        // マテリアル設定
        ChangeMaterial();
    }

    private void LateUpdate()
    {
        if(photonView.IsMine)
        {
            // コンボタイムが過ぎたらコンボをリセット
            if(_stopwatch.IsRunning && _stopwatch.Elapsed.TotalSeconds > _combTimer[0])
            {
                if (BuffDistributor != null)
                    BuffDistributor.GivBuffItem(_combCnt);

                _combCnt = 0;
                //UnityEngine.Debug.Log("コンボ失敗");

                // タイマーをリセット
                _stopwatch.Reset();
            }
            if (_combCnt > 0)
                text.text = ($"コンボ数：{_combCnt.ToString()}");
            else
                text.text = "";
            // マテリアルの同期処理
            PlayerData.CreateAvatar.gameObject.GetComponent<DataSyncManager>()
                .SendData(new object[] { _selectItemNum }, 5);
        }
        // プレイヤーの名前のUIが常にカメラの方を向く
        _canvas.transform.rotation = Camera.main.transform.rotation;        
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // 爆弾なら
        if (collision.gameObject.CompareTag("Bomb"))
        {
            int rand = Random.Range(1, 8);
            if(!photonView.IsMine)
                SetScore(rand);

            else
            {
                _score -= rand;
                SetScore(0);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(collision.gameObject);
                collision.gameObject.GetComponent<Item>().BombDestroy();
            }
            else
            {
                int itemId = collision.gameObject.GetComponent<PhotonView>().ViewID;
                photonView.RPC("RequestObjectDestroy", RpcTarget.MasterClient, itemId);
            }
        }

        if (!photonView.IsMine)
            return;

        // 接触したのが食べ物なら
        if (collision.gameObject.CompareTag("FoodItem"))
        {
            string itemName = $"{_foodObject[_selectItemNum].name}(Clone)";

            if (collision.gameObject.name == itemName)
            {
                if(_stopwatch != null)
                    CheckCombo();

                _score++;
                SetScore(0);

                if (PhotonNetwork.IsMasterClient)
                    PhotonNetwork.Destroy(collision.gameObject);
                else
                {
                    int itemId = collision.gameObject.GetComponent<PhotonView>().ViewID;
                    photonView.RPC("RequestObjectDestroy", RpcTarget.MasterClient, itemId);
                }

                ChangeMaterial();
            }
        }
    }

    /// <summary>
    /// デリートの依頼を受ける
    /// </summary>
    [PunRPC]
    private void RequestObjectDestroy(int itemId)
    {
        if (PhotonNetwork.IsMasterClient && PhotonView.Find(itemId))
        {
            GameObject item = PhotonView.Find(itemId).gameObject;
            item.gameObject.GetComponent<Item>().BombDestroy();
            PhotonNetwork.Destroy(item);
        }
    }

    /// <summary>
    /// 一端の受け渡し
    /// TODO:networkでの受け渡しの変更
    /// </summary>
    private void SetScore(int rand)
    {
        if(photonView.IsMine)
        {
            PlayerData.Score[0] = _score;
            PlayerData.CreateAvatar.GetComponent<DataSyncManager>()
                .SendData(new object[] { _score, false }, 6);
        }
        else
        {
            PlayerData.Score[1] -= rand;
            PlayerData.CreateAvatar.GetComponent<DataSyncManager>()
                .SendData(new object[] { PlayerData.Score[1], true }, 6);

        }
    }

    /// <summary>
    /// コンボ判定
    /// </summary>
    private void CheckCombo()
    {
        if(!_stopwatch.IsRunning)
        {
            _stopwatch.Start();
        }
        else
        {
            // 経過時間
            _combTimer[1] = (float)_stopwatch.Elapsed.TotalSeconds;
            if(_combTimer[1] < _combTimer[0])
            {
                _combCnt++;
                //UnityEngine.Debug.Log($"コンボ{_combCnt}成功");
            }
            _stopwatch.Reset();
            _stopwatch.Start();
        }
    }

    /// <summary>
    /// マテリアルを変える
    /// </summary>
    private void ChangeMaterial()
    {
        if (!photonView.IsMine)
            return;
        
        // ランダムで選ぶ
        if (!_isUseBuffItemDecision)
            _selectItemNum = Random.Range(0, _materials.Length);
        else
            _selectItemNum = _nextItemNum;

        // 変更
        GetComponent<MeshRenderer>().material = _materials[_selectItemNum];
        // 相手に自分のマテリアル情報を送信
        PlayerData.CreateAvatar.gameObject.GetComponent<DataSyncManager>()
            .SendData(new object[] { _selectItemNum }, 5);
        _isUseBuffItemDecision = false;
    }

    public int NextItemDecision()
    {
        _nextItemNum = Random.Range(0, _materials.Length);
        _isUseBuffItemDecision = true;
        return _nextItemNum;
    }

    /// <summary>
    /// 変更されたマテリアルを所得
    /// </summary>
    /// <param name="selectNum"></param>
    public void GetMaterial(int selectNum)
    {
        if (!photonView.IsMine)
        {
            _selectItemNum = selectNum;
            GetComponent<MeshRenderer>().material = _materials[selectNum];
        }
    }

    /// <summary>
    /// 増加されたスコアを取得
    /// </summary>
    /// <param name="score"></param>
    public void GetScore(int score, bool isMy)
    {
        if(isMy)
        {
            PlayerData.Score[0] = score;
            _score = score;
        }
        else
            PlayerData.Score[1] = score;
    }
}
