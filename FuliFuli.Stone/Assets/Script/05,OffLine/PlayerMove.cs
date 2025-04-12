using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstantsManager;
namespace OffLine
{
    /// <summary>
    /// プレイヤーの動き
    /// </summary>
    public class PlayerMove : MonoBehaviour
    {
        private Collider _collider;

        public float[] SetSpeed { get; private set; } = { PlayerSpeed[0], PlayerSpeed[0] };
        private float _moveSpeed = PlayerSpeed[0];

        [SerializeField, Tooltip("スキン")]
        private GameObject[] _playerModel;
        private float _jumpPower = JumpPower;

        private Rigidbody _rb;
        private bool _isJump = false;

        public bool IsHaveItem = false;
        public int PlayerPosID { get; private set; } = 0;

        public bool IsRushing { get; private set; } = false;
        private float _rushSpeed = PlayerSpeed[1];
        private float _rushTime = RushTime;

        private bool _isStan = false;

        private Vector3 _setPos;

        private SESet _getSESet = null;
        private bool _isRingSE = false;

        [SerializeField, Tooltip("アニメーション管理")]
        private PlayerAnimation GetPlayerAnimation;

        [SerializeField, Tooltip("マネージャー")]
        private GameManager _getGameManager;

        public bool IsBuffSpeed { get; private set; } = false;

        private void Start()
        {
            PlayerData.CreateAvatar = this.gameObject;
            foreach (GameObject player in _playerModel)
            {
                player.SetActive(false);
            }
            _rb = GetComponent<Rigidbody>();
            _setPos = this.transform.position;
            _collider = GetComponent<Collider>();
        }

        private void Update()
        {
            if (!IsSceneCheck(5))
            {
                Destroy(this.gameObject);
            }


            if (!_getGameManager.IsStart)
                return;
            if (_isStan)
                return;

            _moveSpeed = SetSpeed[1];
            Operate();
            Jump();
            DropItem();
            Rush();
            SESet();
        }

        public void DisplayPlayer()
        {
            int num = PlayerData.SelectSkinNum[0];
            _playerModel[num].SetActive(true);
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
                _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
                //_rb.velocity = Vector3.up * _jumpPower;

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
            // 0.2秒後に再度有効化
            yield return new WaitForSeconds(0.2f); 
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

            transform.position = _setPos;
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
                if (!plaeyr.GetComponent<MyNamespace.PlayerMove>())
                    return;

                int playerId = plaeyr.GetComponent<MyNamespace.PlayerMove>().PlayerPosID;
                if (IsRushing)
                    GetComponent<DataSyncManager>()
                        .SendData(new object[] { true, playerId }, 7);
            }

            if (collision.gameObject.CompareTag("Plane"))
            {
                transform.position = _setPos;
            }
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
    }
}
