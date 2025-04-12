using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static GameConstantsManager;
namespace OffLine
{
    /// <summary>
    /// AIの動き
    /// </summary>
    public class AIMove : MonoBehaviour
    {
        public float[] SetSpeed { get; private set; } = { PlayerSpeed[0], PlayerSpeed[0] };
        private float _moveSpeed = PlayerSpeed[0];

        [SerializeField, Tooltip("スキン")]
        private GameObject[] _playerModel;

        private Rigidbody _rb;

        public bool IsHaveItem = false;
        public int PlayerPosID { get; private set; } = 0;

        public bool IsRushing { get; private set; } = false;
        private float _rushSpeed = PlayerSpeed[1];
        private float _rushTime = RushTime;

        public bool _isStan = false;

        private Vector3 _setPos;

        private Transform test;

        [SerializeField, Tooltip("アニメーション管理")]
        private PlayerAnimation GetPlayerAnimation;

        [System.NonSerialized]
        public int RequestedItem = 0;
        public GameObject[] FoodObjects { get; private set; } = new GameObject[10];

        private float[] _distance;
        [SerializeField]
        private GameObject _deliveryBox;

        [SerializeField, Tooltip("マネージャー")]
        private GameManager _getGameManager;

        string _wantFoodName = null;

        private float _randmTimer = 0.0f;

        [SerializeField]
        private NavMeshAgent _agent;

        private GameObject[] _groundObj;
        private int _groundNum = 0;
        private Vector3 _lastPos;

        private void Start()
        {
            PlayerData.CreateAvatar = this.gameObject;
            foreach (GameObject player in _playerModel)
            {
                player.SetActive(false);
            }
            _rb = GetComponent<Rigidbody>();
            test = transform.parent;
            _setPos = this.transform.position;
            PlayerData.SelectSkinNum[1] = Random.Range(0, 3);

            _groundObj = GameObject.FindGameObjectsWithTag("Ground");
            _groundNum = Random.Range(0, _groundObj.Length);
            _agent.acceleration = 10f; // デフォルト値を超えた値で調整
        }

        private void Update()
        {
            Stan();

            // シーンが切り替わったらオブジェクトを破壊
            if (!IsSceneCheck(5))
            {
                Destroy(this.gameObject);
            }

            GetPlayerAnimation.SendIs(IsHaveItem, 2);
            MoveSet();

            Vector3 angle = this.transform.localEulerAngles;
            angle.x = 0.0f;
            angle.z = 0.0f;
            this.transform.localEulerAngles = angle;

            if (!_getGameManager.IsStart)
                return;

            DisplayPlayer();
            _moveSpeed = SetSpeed[1];

            // スタン中でない場合のみ操作
            if (!_isStan)
            {
                Operate();
            }
            else
            {
                _agent.isStopped = true;
                Debug.Log("スタン中、操作停止");
            }

            AnotherItem();
        }

        /// <summary>
        /// 表示
        /// </summary>
        public void DisplayPlayer()
        {
            int num = PlayerData.SelectSkinNum[1];
            _playerModel[num].SetActive(true);
        }

        /// <summary>
        /// 動いている間のアニメーション切り替え
        /// </summary>
        private void MoveSet()
        {
            float speed = _agent.velocity.magnitude;
            if(speed > 0.1f)
            {
                GetPlayerAnimation.SendIs(true, 0);
            }
            else
            {
                GetPlayerAnimation.SendIs(false, 0);
            }
        }

        /// <summary>
        /// 目的のアイテムではないとき
        /// </summary>
        /// <param name="target"></param>
        private void AnotherItem()
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("FoodItem") && child.gameObject.name != _wantFoodName)
                {
                    DropItem();
                }
                if(child.CompareTag("Bomb"))
                    DropItem();

            }
        }

        /// <summary>
        /// 位置関係
        /// </summary>
        private void Operate()
        {
            _wantFoodName = $"{FoodObjects[RequestedItem].name}(Clone)";

            if (IsHaveItem)
            {
                _randmTimer = 20.0f;
                AimAtTheTarget(_deliveryBox);
                return;
            }

            if (GameObject.Find(_wantFoodName))
            {
                GameObject[] foodObjects = GameObject.FindGameObjectsWithTag("FoodItem");
                GameObject foodObject = null;
                float[] distance = { 0.0f, 0.0f };
                foreach (GameObject targetObj in foodObjects)
                {
                    if (targetObj.name == _wantFoodName)
                    {
                        distance[0] = Vector3.Distance(transform.position, targetObj.transform.position);
                        if (distance[1] == 0.0f || distance[0] < distance[1])
                        {
                            distance[1] = distance[0];
                            foodObject = targetObj;
                        }
                    }
                    foodObject = GameObject.Find(_wantFoodName);
                }

                if (!foodObject.GetComponent<Item>().IsItemHavint &&
                    foodObject.GetComponent<Item>().ISHaveCan)
                {
                    AimAtTheTarget(foodObject);
                    return;
                }              
            }

            // ランダムウォーク
            _randmTimer += Time.deltaTime;
            if (_randmTimer > 5.0f)
            {
                _groundNum = Random.Range(0, _groundObj.Length);
                _randmTimer = 0.0f;
            }
            else
            {
                AimAtTheTarget(_groundObj[_groundNum]);
                if (this.transform.position == _groundObj[_groundNum].transform.position)
                    _randmTimer = 20.0f;
            }
        }

        /// <summary>
        /// 対象を目指す
        /// </summary>
        /// <param name="taget"></param>
        private void AimAtTheTarget(GameObject taget)
        {

            if (taget == null)
                return;

            Vector3 targetPos = taget.transform.position;
            targetPos.y = this.transform.position.y;


            // 目的地が移動可能かどうか
            NavMeshHit navHit;
            if(NavMesh.SamplePosition(targetPos, out navHit, 2.0f, NavMesh.AllAreas))
            {
                this.transform.LookAt(targetPos);
                _agent.SetDestination(targetPos);
            }
            else
            {
                _randmTimer = 20.0f;
            }
        }

        /// <summary>
        /// アイテムを離す
        /// </summary>
        private void DropItem()
        {
            IsHaveItem = false;
        }

        /// <summary>
        /// スタンするかを受け取る
        /// </summary>
        public void Stan()
        {
            if (_isStan)
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
            _agent.isStopped = false;

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
                }
            }

            Vector3 pos = transform.position;
            pos = _setPos;
            transform.position = pos;
        }

        /// <summary>
        /// 当たり判定のトリガー入る
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            // 地面
            if (other.gameObject.CompareTag("Ground"))
            {
                if (transform.position.y < 1.5f)
                    transform.position = new Vector3(transform.position.x, 5.0f, transform.position.z);

            }

            if (other.gameObject.CompareTag("Player"))
            {
                GameObject player = other.gameObject;
                int playerId = player.GetComponent<PlayerMove>().PlayerPosID;
                if (player.GetComponent<PlayerMove>().IsRushing)
                {
                    _isStan = true;
                }
            }

            if (other.gameObject.CompareTag("Plane"))
            {
                Vector3 pos = transform.position;
                pos = _setPos;
                transform.position = pos;
            }
            if (IsHaveItem)
            {
                if (other.gameObject.CompareTag("FoodItem"))
                {
                    string foodName = other.gameObject.name;
                    if (_wantFoodName != foodName)
                    {
                        Destroy(other.gameObject);
                        DropItem();
                    }
                }
                if (other.gameObject == _deliveryBox)
                {
                    DropItem();
                }
            }
        }
    }
}
