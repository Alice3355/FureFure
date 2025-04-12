using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OffLine
{
    /// <summary>
    /// アイテム処理
    /// </summary>
    public class Item : MonoBehaviour
    {
        public bool ISHaveCan { get; private set; } = false;

        private GameObject _player;
        public bool IsItemHavint = false;
        private bool _isFirstRun = false;

        private Rigidbody _rb;
        private Collider _collider;

        private float[] _destroyTimer = { 0.0f, 10.0f };

        private float _bombRaduis = 8.0f;

        [SerializeField, Tooltip("エフェクト")]
        private GameObject _effect;

        private SESet _getSESet = null;
        private AIMove _getAIMove;

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
            if (IsItemHavint)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                _destroyTimer[0] = _destroyTimer[1];
            }

            _destroyTimer[0] -= Time.deltaTime;
            if (_destroyTimer[0] < 3)
            {
                Blinking();
            }
            if (_destroyTimer[0] < 0)
            {
                if (transform.CompareTag("Bomb"))
                {
                    BombExplosion();
                    BombDestroy();
                }
                Destroy(gameObject);
            }


            // アイテムを持っていない
            if (_player != null)
            {
                if (_player.GetComponent<PlayerMove>() && !_player.GetComponent<PlayerMove>().IsHaveItem)
                {
                    SetItemReleased();
                }
                else if (!_player.GetComponent<PlayerMove>() && !_player.GetComponent<AIMove>().IsHaveItem)
                {
                    SetItemReleased();
                }
            }

            // １回だけ
            if (IsItemHavint && !_isFirstRun)
            {
                SetItemHaving();
                _isFirstRun = true;
            }
        }

        /// <summary>
        /// 点滅
        /// </summary>
        private void Blinking()
        {
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
            foreach (var hitPlayerSet in hitPlayer)
            {
                if (hitPlayerSet.CompareTag("Player"))
                {
                    if(hitPlayerSet.GetComponent<PlayerMove>())
                        hitPlayerSet.GetComponent<PlayerMove>().BombExplosion();
                    else
                        hitPlayerSet.GetComponent<AIMove>().BombExplosion();
                }
            }
        }

        public void BombDestroy()
        {
            if (!transform.CompareTag("Bomb"))
                return;

            if (_getSESet == null && !GameObject.Find("SE"))
            {
                _getSESet = GameObject.Find("SE").GetComponent<SESet>();
            }
            if (_getSESet == null)
                return;

            Vector3 pos = transform.position;
            pos.y += 1.5f;
            Instantiate(_effect, pos, Quaternion.identity);

            _getSESet.SetClip(4);
        }
        /// <summary>
        /// 当たり判定入り
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                ISHaveCan = true;
            }

            // ステージ外に出たら
            if (collision.gameObject.CompareTag("Plane"))
            {
                Destroy(gameObject);
            }

            if(collision.gameObject.CompareTag("Tree") && !IsItemHavint)
            {
                Destroy(gameObject);
            }

            // プレイヤーじゃないならはじく
            if (!collision.gameObject.CompareTag("Player"))
                return;

            // 代入と親子関係と位置セット
            _player = collision.gameObject;

            if(_player.GetComponent<PlayerMove>())
            {
                if (!IsItemHavint && !_player.GetComponent<PlayerMove>().IsHaveItem)
                {
                    transform.SetParent(collision.transform);
                    GameObject haveItem = gameObject.transform.parent.Find("HaveItem").gameObject;
                    transform.position = haveItem.transform.position;
                    IsItemHavint = true;
                    _player.GetComponent<PlayerMove>().IsHaveItem = true;
                }
            }
            else
            {
                if (!IsItemHavint && !_player.GetComponent<AIMove>().IsHaveItem)
                {
                    transform.SetParent(collision.transform);
                    GameObject haveItem = gameObject.transform.parent.Find("HaveItem").gameObject;
                    transform.position = haveItem.transform.position;
                    IsItemHavint = true;
                    _player.GetComponent<AIMove>().IsHaveItem = true;
                }
            }
        }

        /// <summary>
        /// 当たり判定出る
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionExit(Collision collision)
        {
            if(collision.gameObject.CompareTag("NPC"))
            {
                IsItemHavint = false;
                _isFirstRun = false;
            }
            if (collision.gameObject.CompareTag("Player"))
            {
                IsItemHavint = false;
                _isFirstRun = false;
            }

            if(collision.gameObject.CompareTag("Ground"))
            {
                ISHaveCan = false;
            }
        }

        /// <summary>
        /// 当たり判定のトリガー入ったとき
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            // プレイヤーじゃないならはじく
            if (!other.gameObject.CompareTag("Player"))
                return;

            // 代入と親子関係と位置セット
            _player = other.gameObject;

            if (_player.GetComponent<PlayerMove>())
            {
                if (!IsItemHavint && !_player.GetComponent<PlayerMove>().IsHaveItem)
                {
                    transform.SetParent(other.transform);
                    GameObject haveItem = gameObject.transform.parent.Find("HaveItem").gameObject;
                    transform.position = haveItem.transform.position;
                    IsItemHavint = true;
                    _player.GetComponent<PlayerMove>().IsHaveItem = true;
                }
            }
            else
            {
                if (!IsItemHavint && !_player.GetComponent<AIMove>().IsHaveItem)
                {
                    transform.SetParent(other.transform);
                    GameObject haveItem = gameObject.transform.parent.Find("HaveItem").gameObject;
                    transform.position = haveItem.transform.position;
                    IsItemHavint = true;
                    _player.GetComponent<AIMove>().IsHaveItem = true;
                }
            }
        }

        /// <summary>
        /// 当たり判定のトリガー出たとき
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                IsItemHavint = false;
                _isFirstRun = false;
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
    }
}
