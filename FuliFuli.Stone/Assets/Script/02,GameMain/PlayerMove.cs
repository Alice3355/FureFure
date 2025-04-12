using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static GameConstantsManager;
namespace MyNamespace
{
    /// <summary>
    /// プレイヤーの動き
    /// </summary>
    public class PlayerMove : MonoBehaviourPun
    {
        public float[] SetSpeed { get; private set; } = { PlayerSpeed[0], PlayerSpeed[0] };
        private float _moveSpeed = PlayerSpeed[0];
        private int _startSetCnt = 0;
        [SerializeField]
        private GameObject[] _playerModel;
        [SerializeField, Tooltip("ジャンプの高さ")]
        private float _jumpPower = JumpPower;

        private Rigidbody _rb;
        private bool _isJump = false;

        [System.NonSerialized]
        public bool IsHaveItem = false;
        public int PlayerPosID { get; private set; } = 0;

        public bool IsRushing { get; private set; } = false;
        private float _rushSpeed = PlayerSpeed[1];
        private float _rushTime = RushTime;

        private bool _isStan = false;

        [System.NonSerialized]
        public StagePosSet GetStagePosSet;
        private Vector3[] _setPos = new Vector3[10];

        private SESet _getSESet = null;
        private bool _isRingSE = false;

        [SerializeField, Tooltip("アニメーション管理")]
        private PlayerAnimation GetPlayerAnimation;

        private Collider _collider;

        [System.NonSerialized]
        public bool IsBuffSpeed = false;

        [System.NonSerialized]
        public BuffManager GetBuffManager;

        private void Start()
        {
            foreach (GameObject player in _playerModel)
            {
                player.SetActive(false);
            }
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        private void Update()
        {
            if (_isStan)
                return;

            if (transform.position.y < 0)
            {
                transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
            }

            if (GetStagePosSet != null)
            {
                for (int i = 0; i < GetStagePosSet.PlayerPos.Length; i++)
                {
                    _setPos[i] = GetStagePosSet.PlayerPos[i];
                }
            }

            if (IsSceneCheck(7))
            {
                _playerModel[PlayerData.SelectSkinNum[1]].SetActive(true);

                Operate();
                Jump();
                DropItem();
                Rush();
                return;
            }
            if (IsSceneCheck(0))
            {
                PhotonNetwork.Destroy(gameObject);
            }
            if (IsSceneCheck(3))
            {
                foreach (GameObject player in _playerModel)
                {
                    player.SetActive(false);
                }
                _startSetCnt = 0;
                IsHaveItem = false;
                if (photonView.IsMine)
                    PlayerData.CreateAvatar = this.gameObject;

            }

            _moveSpeed = SetSpeed[1];

            // ローカルプレイヤーでない場合、処理をスキップ
            if (!photonView.IsMine)
            {
                PlayerPosID = PlayerData.GameMainPosSet[1];
                if (IsSceneCheck(2))
                {
                    if(GetBuffManager != null && GetBuffManager.IsRainItem)
                        GetComponent<DataSyncManager>()
                            .SendData(new object[] { GetBuffManager, transform.position }, 10);


                    _playerModel[PlayerData.SelectSkinNum[1]].SetActive(true);
                    return;
                }
                else
                {
                    return;
                }
            }

            PlayerPosID = PlayerData.GameMainPosSet[0];
            // ゲーム開始していないなら動かない
            if (!IsSceneCheck(2))
                return;

            if (GetBuffManager != null && GetBuffManager.IsRainItem)
            {
                PlayerData.IsUseRainItem = true;
                PlayerData.RainPos = this.transform.position;
            }

            if (GetStagePosSet != null && _startSetCnt < 1)
            {
                _playerModel[PlayerData.SelectSkinNum[0]].SetActive(true);
                _startSetCnt++;
                Vector3 pos = transform.position;
                switch (PlayerData.GameMainPosSet[0])
                {
                    case 1:
                        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                        break;
                    case 2:
                        transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                        break;
                }
                pos = _setPos[PlayerData.GameMainPosSet[0] - 1];
                transform.position = pos;
            }

            // 自分と相手の準備ができたか
            if (!PlayerData.ISGameStart[2])
                return;

            Operate();
            Jump();
            DropItem();
            Rush();
            SESet();

            // 他のプレイヤーに移動情報を送信
            photonView.RPC("SyncPosition", RpcTarget.Others, transform.position);
        }


        [PunRPC]
        private void SyncPosition(Vector3 pos)
        {
            // 他のプレイヤーに移動情報を同期
            transform.position = pos;
        }

        /// <summary>
        /// SEのセットをする
        /// </summary>
        private void SESet()
        {
            if (_getSESet == null)
                _getSESet = GameObject.Find("SE").GetComponent<SESet>();

            if (!_isRingSE && IsHaveItem)
            {
                _getSESet.SetClip(1);
                _isRingSE = true;
            }
            else if (_isRingSE && !IsHaveItem)
            {
                _getSESet.SetClip(2);
                _isRingSE = false;
            }
        }

        /// <summary>
        /// 位置関係
        /// </summary>
        private void Operate()
        {
            // カメラの全方向と左右を取得
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            // カメラの上下を無視する
            cameraForward.y = 0;
            cameraRight.y = 0;

            // 移動準備
            cameraForward = cameraForward.normalized;
            cameraRight = cameraRight.normalized;

            // 簡単な移動処理
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = (cameraForward * moveVertical + cameraRight * moveHorizontal).normalized;
            transform.position += movement * _moveSpeed * Time.deltaTime;

            // プレイヤーが移動していく方向を向く
            if (movement != Vector3.zero)
                transform.forward = Vector3.Slerp(transform.forward, movement, Time.deltaTime * 10.0f);

            WalkAnimation();
        }

        /// <summary>
        /// 歩くアニメーション
        /// </summary>
        private void WalkAnimation()
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
                Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
                GetPlayerAnimation.SendIs(true, 0);

            else
                GetPlayerAnimation.SendIs(false, 0);

            GetPlayerAnimation.SendIs(IsHaveItem, 2);
        }

        /// <summary>
        /// ジャンプ処理
        /// </summary>
        private void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !_isJump)
            {
                if (_getSESet == null)
                    _getSESet = GameObject.Find("SE").GetComponent<SESet>();

                _getSESet.SetClip(3);

                // Rigidbodyに上方向の力を加える
                //_rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
                _rb.velocity = Vector3.up * _jumpPower;

                // 一時的にColliderを無効化
                StartCoroutine(DisableColliderTemporarily());

                _isJump = true;
                GetPlayerAnimation.SendIs(true, 1);
                GetPlayerAnimation.SnedTrigger(3);
            }
            if (Ground())
            {
                _isJump = false;
                GetPlayerAnimation.SendIs(false, 1);
            }
        }

        /// <summary>
        /// 一時的にコライダーを消す
        /// </summary>
        /// <returns></returns>
        private IEnumerator DisableColliderTemporarily()
        {
            _collider.enabled = false;
            yield return new WaitForSeconds(0.2f); // 0.2秒後に再度有効化
            _collider.enabled = true;
        }

        /// <summary>
        /// アイテムを離す
        /// </summary>
        private void DropItem()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                IsHaveItem = false;
                GetComponent<DataSyncManager>()
                    .SendData(new object[] { false, PlayerPosID }, 4);
            }
        }

        /// <summary>
        /// 突進
        /// </summary>
        private void Rush()
        {
            if (Input.GetKeyDown(KeyCode.R) && !IsRushing)
            {
                StartCoroutine(RushForward());
            }
        }
        /// <summary>
        /// ラッシュのコルーチン
        /// </summary>
        private IEnumerator RushForward()
        {
            IsRushing = true;
            // 正面
            Vector3 direction = transform.forward;
            float rushTimer = 0.0f;

            // 突進中は通常操作無効
            while (rushTimer < _rushTime)
            {
                // 前に突進
                transform.position += direction * _rushSpeed * Time.deltaTime;
                rushTimer += Time.deltaTime;

                // 一定時間が終了するまで続ける
                yield return null;
            }

            IsRushing = false;
        }

        /// <summary>
        /// 通信相手がアイテムを落とす
        /// </summary>
        public void OpponentDropItem(bool flg, int playerId)
        {
            if (PlayerPosID == playerId)
                IsHaveItem = flg;
        }

        private bool Ground()
        {
            bool flg = false;

            // プレイヤーの足元から下に向かってRayを飛ばして、地面に接しているか調べる
            // OnCollisionEnterより正確に判定が可能
            flg = Physics.Raycast(transform.position, Vector3.down, 1.1f);
            return flg;
        }

        /// <summary>
        /// 当たり判定
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GameObject plaeyr = collision.gameObject;
                int playerId = plaeyr.GetComponent<MyNamespace.PlayerMove>().PlayerPosID;
                if (IsRushing)
                    GetComponent<DataSyncManager>()
                        .SendData(new object[] { true, playerId }, 7);

            }

            if (collision.gameObject.CompareTag("Plane"))
            {
                transform.position = _setPos[PlayerData.GameMainPosSet[0] - 1];
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Limit"))
            {
                Vector3 pos = transform.position;
                pos.y += 3.0f;
                transform.position = pos;
            }
        }

        /// <summary>
        /// スタンするかを受け取る
        /// </summary>
        public void Stan(bool isStan, int playerId)
        {
            if (isStan && PlayerPosID == playerId)
                StartCoroutine(NowStan());
        }
        /// <summary>
        /// スタン中のコルーチン
        /// </summary>
        private IEnumerator NowStan()
        {
            float stanTime = 0f;
            _isStan = true;
            while (stanTime < StanTime)
            {
                stanTime += Time.deltaTime;
                yield return null;
            }
            _isStan = false;
        }

        /// <summary>
        /// 爆発された
        /// </summary>
        public void BombExplosion()
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("FoodItem"))
                {
                    Destroy(child.gameObject);
                    IsHaveItem = false;
                    GetPlayerAnimation.SendIs(IsHaveItem, 2);
                }
            }

            transform.position = _setPos[PlayerData.GameMainPosSet[0] - 1];
        }
    }
}



