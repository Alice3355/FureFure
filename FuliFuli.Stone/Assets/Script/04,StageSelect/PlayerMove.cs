using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StageSelect
{
    /// <summary>
    /// プレイヤーの動き
    /// </summary>
    public class PlayerMove : MonoBehaviour
    {
        private float _moveSpeed = 6.0f;
        private bool _isJump = false;
        private float _jumpPower = 5.0f;
        private Rigidbody _rb;
        private float _timer = 0.0f;

        [System.NonSerialized]
        public bool IsHaveItem = false;

        [SerializeField, Tooltip("アニメーション管理")]
        private PlayerAnimation GetPlayerAnimation;
        [SerializeField, Tooltip("補助アイテム")]
        private GameObject _itemUI;

        //[System.NonSerialized]
        public bool IsCanMove = true;
        public bool IsTrigger /*{ get; private set; }*/ = false;
        public bool IsItemUse { get; private set; } = false;
        public bool IsRushing { get; private set; } = false;
        public bool IsSpeedUp { get; private set; } = false;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (!IsCanMove)
                return;
            if(_itemUI.activeSelf && Input.GetKey(KeyCode.E))
            {
                IsItemUse = true;
                _moveSpeed = 20.0f;
                IsSpeedUp = true;
            }
            if(_moveSpeed == 20.0f)
            {
                _timer += Time.deltaTime;
                if (_timer > 5.0f)
                {
                    IsItemUse = false;
                    _moveSpeed = 6.0f;
                    _timer = 0.0f;
                    IsSpeedUp = false;
                    _itemUI.SetActive(false);
                }
            }
            Operate();
            WalkAnimation();
            Jump();
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
                GetPlayerAnimation.SendIs(true, 1);
                _rb.velocity = Vector3.up * _jumpPower;
                _isJump = true;
            }
        }

        /// <summary>
        /// 当たり判定
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            // 地面
            if (collision.gameObject.CompareTag("Ground"))
            {
                if (transform.position.y < 1.5f)
                    transform.position = new Vector3(transform.position.x, 5.0f, transform.position.z);

                _isJump = false;
                GetPlayerAnimation.SendIs(false, 1);
            }
            if(collision.gameObject.CompareTag("Plane"))
            {
                transform.position = new Vector3(3.0f, 3.0f, -5.0f);
            }
        }

        /// <summary>
        /// Trigger判定
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.name == "Trigger" && !IsTrigger)
            {
                IsTrigger = true;
                IsCanMove = false;
            }
        }
    }
}
