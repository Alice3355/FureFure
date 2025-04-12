using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StageSelect
{
    /// <summary>
    /// アイテムの処理
    /// </summary>
    public class Item : MonoBehaviour
    {
        private GameObject _player;
        public bool _isItemHavint = false;

        private Rigidbody _rb;
        private Collider _collider;

        [SerializeField, Tooltip("エフェクト")]
        private GameObject _effect;
        private SESet _getSESet = null;
        private float _timer = 0.0f;

        [SerializeField]
        private bool _isBomb = false;
        private void Start()
        {
            _rb = this.transform.GetComponent<Rigidbody>();
            _collider = gameObject.GetComponent<Collider>();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Q) && _isItemHavint)
            {
                _player.GetComponent<PlayerMove>().IsHaveItem = false;
                SetItemReleased();
                _timer = 0;
            }

            if (_effect == null)
                return;

            if (_isBomb)
            {
                _timer += Time.deltaTime;
                if(_timer > 5.0f)
                {
                    Explosion();
                    _timer = 0;
                }
            }
        }

        /// <summary>
        /// 当たり判定入り
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player") && !_isItemHavint)
            {
                _player = collision.gameObject;
                if (_player.GetComponent<PlayerMove>().IsHaveItem)
                    return;
                transform.SetParent(collision.transform);
                GameObject haveItem = gameObject.transform.parent.Find("HaveItem").gameObject;
                transform.position = haveItem.transform.position;
                _isItemHavint = true;
                _player.GetComponent<PlayerMove>().IsHaveItem = true;
                SetItemHaving();
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

        public void Explosion()
        {
            if (GameObject.Find("SE"))
                _getSESet = GameObject.Find("SE").GetComponent<SESet>();

            Vector3 pos = transform.position;
            pos.y += 1.5f;
            Instantiate(_effect, pos, Quaternion.identity);
            Destroy(gameObject);
            _getSESet.SetClip(4);
        }
    }
}
