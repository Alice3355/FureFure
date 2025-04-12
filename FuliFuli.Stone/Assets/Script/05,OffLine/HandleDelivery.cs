using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using TMPro;
namespace OffLine
{
    /// <summary>
    /// アイテムが納品されたときの処理
    /// </summary>
    public class HandleDelivery : MonoBehaviour
    {
        [SerializeField, Tooltip("アイテムの種類")]
        private GameObject[] _foodObject;
        [SerializeField, Tooltip("マテリアル")]
        private Material[] _materials;
        [SerializeField]
        private TextMeshProUGUI _npcName;
        [SerializeField, Tooltip("AIかどうか")]
        private bool _isNPC;

        [System.NonSerialized]
        public int BoxPosID = 0;
        [SerializeField, Tooltip("UI")]
        private Canvas _canvas;

        //private Item _getItem;
        private int _score = 0;

        private float[] _combTimer = { 10.0f, 0.0f };
        private Stopwatch _stopwatch;
        // コンボ数Count
        private int _combCnt = 0;
        // 選ばれたアイテムの番号
        private int _selectItemNum = 0;
        // 対象のプレイヤーの名前

        public TextMeshProUGUI text;

        // 補助アイテム関連
        [SerializeField, Tooltip("補助アイテム")]
        private RandomBuffDistributor _getBuffDistributor;

        private bool _isUseBuffItemDecision = false;
        private int _nextItemNum = 0;

        [SerializeField, Tooltip("AIのときのスクリプト")]
        private AIMove _getAIMove;

        public string ScoreUpName { get; private set; } = null;

        private void Start()
        {
            if(_isNPC)
            {
                _npcName.text = "NPCの名前";
            }

            // コンボ用ストップウィッチ
            _stopwatch = new Stopwatch();
            // マテリアル設定
            ChangeMaterial();

            if(_getAIMove != null)
            {
                for (int i = 0; i < _foodObject.Length; i++)
                {
                    _getAIMove.FoodObjects[i] = _foodObject[i];
                }
            }
        }

        private void LateUpdate()
        {
            if(ScoreUpName != null)
            {
                string itemName = $"{_foodObject[_selectItemNum].name}(Clone)";

                if (ScoreUpName == itemName)
                {
                    if (_stopwatch != null)
                        CheckCombo();

                    _score++;

                    if (_getAIMove)
                        SetScore(1);
                    else
                        SetScore(0);

                    ChangeMaterial();
                }
            }

            // コンボタイムが過ぎたらコンボをリセット
            if (_stopwatch.IsRunning && _stopwatch.Elapsed.TotalSeconds > _combTimer[0])
            {
                if (_getBuffDistributor != null && !_getAIMove)
                    _getBuffDistributor.GivBuffItem(_combCnt);

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
                //UnityEngine.Debug.Log("爆発！");
                int rand = Random.Range(1, 8);

                _score -= rand;
                if(_getAIMove)
                    SetScore(1);
                else
                    SetScore(0);


                collision.gameObject.GetComponent<Item>().BombDestroy();
                Destroy(collision.gameObject);
            }

            // 接触したのが食べ物なら
            if (collision.gameObject.CompareTag("FoodItem"))
            {
                string itemName = $"{_foodObject[_selectItemNum].name}(Clone)";

                if (collision.gameObject.name == itemName)
                {
                    if (_stopwatch != null)
                        CheckCombo();

                    _score++;

                    if (_getAIMove)
                        SetScore(1);
                    else
                        SetScore(0);

                    Destroy(collision.gameObject);

                    ChangeMaterial();
                }
            }
        }

        /// <summary>
        /// 一端の受け渡し
        /// TODO:networkでの受け渡しの変更
        /// </summary>
        private void SetScore(int num)
        {
            PlayerData.Score[num] = _score;
        }

        /// <summary>
        /// コンボ判定
        /// </summary>
        private void CheckCombo()
        {
            if (!_stopwatch.IsRunning)
            {
                _stopwatch.Start();
            }
            else
            {
                // 経過時間
                _combTimer[1] = (float)_stopwatch.Elapsed.TotalSeconds;
                if (_combTimer[1] < _combTimer[0])
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
            // ランダムで選ぶ
            if (!_isUseBuffItemDecision)
                _selectItemNum = Random.Range(0, _materials.Length);
            else
                _selectItemNum = _nextItemNum;

            // 変更
            GetComponent<MeshRenderer>().material = _materials[_selectItemNum];
            // 相手に自分のマテリアル情報を送信
            _isUseBuffItemDecision = false;

            if(_getAIMove != null)
            {
                _getAIMove.RequestedItem = _selectItemNum;
            }
        }

        public int NextItemDecision()
        {
            _nextItemNum = Random.Range(0, _materials.Length);
            _isUseBuffItemDecision = true;
            return _nextItemNum;
        }
    }

}