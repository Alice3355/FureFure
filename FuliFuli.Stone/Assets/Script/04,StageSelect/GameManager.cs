using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static GameConstantsManager;
namespace StageSelect
{
    /// <summary>
    /// 管理をメインに行う
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField, Tooltip("表示テキスト")]
        private TextMeshProUGUI[] _text;
        [SerializeField, Tooltip("スコア用のテキスト")]
        private TextMeshProUGUI[] _scoreText;
        [SerializeField, Tooltip("入れる文字列")]
        private Text[] _getTextData;
        [SerializeField, Tooltip("解説のひよこ")]
        private GameObject _guideCanvas;
        [SerializeField, Tooltip("ボタンの進行度")]
        private GameObject[] _circleUI;
        [SerializeField, Tooltip("アイテムを落とすスクリプト")]
        private RainItem _getRainItem;
        [SerializeField, Tooltip("補助アイテムのUI")]
        private GameObject _itemUI;

        [SerializeField, Tooltip("プレイヤー")]
        private GameObject _player;
        private PlayerMove _getPlayerMove;
        [SerializeField, Tooltip("納品箱")]
        private HandleDelivery _getHandleDelivery;
        [SerializeField]
        private GameObject _box;

        private float _timer = 0;
        private bool[] _isButton = {false, false, false, false, false};

        // 進行度を数値化したもの
        public int TutorialStep { get; private set; } = 0;

        private int _score = 0;

        private Action[] _tutorialSteps;
        private void Start()
        {
            RecordText(0, 0);
            _getPlayerMove = _player.GetComponent<PlayerMove>();
            // メソッド配列の初期化
            _tutorialSteps = new Action[]
            {
                ExplainControls, PromptPickup, CarryToTarget,
                ItemDropGuide, ExplainBomb, ExplainBombEffect,
                GuideBombToTarget, BombPenalty, PromptDelivery,
                Combo, ExplainComboReward, PromptItemUsage,
                ExplainItemEffect, End
            };

        }

        private void Update()
        {
            _scoreText[0].text = ($"Score:{_getHandleDelivery.Score}");
            _scoreText[1].text = _scoreText[0].text;
            // チュートリアルステップに対応するメソッドを実行
            if(TutorialStep >= 0 && TutorialStep < _tutorialSteps.Length)
            {
                // _tutorialStepsの_tutorialStepを実行
                // ？はnullでない場合
                _tutorialSteps[TutorialStep]?.Invoke();
            }
        }

        /// <summary>
        /// テキストに文字列を入れる
        /// </summary>
        private void RecordText(int num, int selectNum)
        {
            _text[selectNum].text = _getTextData[num].Sentence;
            _text[selectNum + 1].text = _text[selectNum].text;
        }

        /// <summary>
        /// タイマー機能
        /// </summary>
        /// <param name="time">制限時間</param>
        private bool Timer(float time)
        {
            _timer += Time.deltaTime;
            if(_timer > time)
            {
                // タイマーがおわったよ
                _timer = 0.0f;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移動に関数ボタンが押されたか
        /// </summary>
        /// <returns></returns>
        private bool IsInput()
        {
            KeyCode[] keys = 
                { KeyCode.A, KeyCode.D, KeyCode.W,
                  KeyCode.S, KeyCode.Space };

            if(Input.GetKeyDown(keys[0]) || Input.GetKeyDown(keys[1]) ||
                Input.GetKeyDown(keys[2]) || Input.GetKeyDown(keys[3]))
            {
                _isButton[0] = true;
                _circleUI[0].SetActive(true);
            }

            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                _isButton[1] = true;
                _circleUI[1].SetActive(true);
            }

            if (Input.GetKeyDown(keys[4]))
            {
                _isButton[2] = true;
                _circleUI[2].SetActive(true);
            }

            if (_isButton[0] && _isButton[1] && _isButton[2])
                return true;
            else
                return false;
        }


        /// <summary>
        /// 移動のためのコルーチン
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        private IEnumerator MoveOverTime(Vector3 pos, float duration, int num)
        {
            _text[2].text = "";
            _text[3].text = "";

            // はじめの位置
            Vector3 startPos = _guideCanvas.transform.position;
            float time = 0.0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);

                // 線形補間で位置を計算
                _guideCanvas.transform.position = Vector3.Lerp(startPos, pos, t);
                yield return null;// 次のフレームまで待機
            }
            // 最後に目標を正確に設定
            _guideCanvas.transform.position = pos;
            RecordText(num, 2);

        }

        private void StartMove(Vector3 pos, float duration, int num, bool flg)
        {
            _getPlayerMove.IsCanMove = false;
            StartCoroutine(MoveAndEnable(pos, duration, num, flg));
        }

        private IEnumerator MoveAndEnable(Vector3 pos, float duration, int num, bool flg)
        {
            if(flg)
                _getPlayerMove.IsCanMove = true;
            yield return MoveOverTime(pos, duration, num);
        }


        /// <summary>
        /// 00,操作方法関係
        /// </summary>
        private void ExplainControls()
        {
            // 操作説明
            if (IsInput())
            {
                _text[0].gameObject.SetActive(false);
                _getPlayerMove.IsCanMove = false;
                _getRainItem.SetRainItem(0);
                _guideCanvas.SetActive(true);
                Destroy(_circleUI[0].gameObject);
                Destroy(_circleUI[1].gameObject);
                Destroy(_circleUI[2].gameObject);
                RecordText(1, 2);
                TutorialStep++;
            }
        }

        /// <summary>
        /// 01,アイテム拾いを促す
        /// </summary>
        private void PromptPickup()
        {
            if(Timer(3.0f))
            {
                _getPlayerMove.IsCanMove = true;
            }
            if(_getPlayerMove.IsHaveItem)
            {
                _timer = 0.0f;
                TutorialStep++;
                _box.SetActive(true);
            }
        }

        /// <summary>
        /// 02,アイテムを運ぶ
        /// </summary>
        private void CarryToTarget()
        {
            Vector3 pos = new Vector3(-8.0f, 4.0f, 2.5f);
            if (_guideCanvas.transform.position != pos)
                StartMove(pos, 1.0f, 2, true);

            if (_getPlayerMove.IsTrigger)
            {
                RecordText(3, 2);
                TutorialStep++;
            }
            if (_getHandleDelivery.Score > 0)
            {
                TutorialStep = 3;
            }
        }

        /// <summary>
        /// 03,アイテムを落とす
        /// </summary>
        private void ItemDropGuide()
        {
            
            if (_getHandleDelivery.Score > 0)
            {
                _getPlayerMove.IsCanMove = true;
                RecordText(4, 2);

                if(Timer(2.0f))
                    TutorialStep++;

            }
            else if(Input.GetKey(KeyCode.Q))
                _getPlayerMove.IsCanMove = true;
        }

        /// <summary>
        /// 04,爆弾解説
        /// </summary>
        private void ExplainBomb()
        {
            Vector3 pos = new Vector3(0.0f, 4.0f, 0.0f);
            // 移動する
            StartMove(pos, 1.0f, 5, false);
            _getRainItem.SetRainItem(8);
            TutorialStep++;
        }


        /// <summary>
        /// 05,爆弾の効果を説明
        /// </summary>
        private void ExplainBombEffect()
        {
            if (Timer(6.0f))
            {
                RecordText(6, 2);
                TutorialStep++;
            }
        }

        /// <summary>
        /// 06,爆弾を運ぶ
        /// </summary>
        private void GuideBombToTarget()
        {
            if (Timer(5.0f))
            {
                RecordText(7, 2);
                _getRainItem.SetRainItem(9);
                _getPlayerMove.IsCanMove = true;
                TutorialStep++;
            }
        }

        /// <summary>
        /// 07,爆弾のペナルティー
        /// </summary>
        private void BombPenalty()
        {
            if (_getHandleDelivery.Score < 1)
            {
                _guideCanvas.transform.position = new Vector3(-8.0f, 4.0f, 2.5f);
                RecordText(8, 2);
                TutorialStep++;
            }
                
        }

        /// <summary>
        /// 08,コンボの実演
        /// </summary>
        private void PromptDelivery()
        {
            if(Timer(3.0f))
            {
                _getRainItem.SetRainItem(1);
                _getRainItem.SetRainItem(2);
                RecordText(9, 2);
                TutorialStep++;
                _score = _getHandleDelivery.Score;
            }
        }

        /// <summary>
        /// 09,コンボ解説
        /// </summary>
        private void Combo()
        {
            // スコアが２つ分上がっている状態なら
            if(_score + 2 == _getHandleDelivery.Score)
            {
                RecordText(10, 2);
                TutorialStep++;
            }
        }

        /// <summary>
        /// 10,コンボ報酬解説
        /// </summary>
        private void ExplainComboReward()
        {
            if(Timer(4.0f))
            {
                RecordText(11, 2);
                _itemUI.SetActive(true);
                TutorialStep++;
            }
        }

        /// <summary>
        /// 11,補助アイテムを使わせる
        /// </summary>
        private void PromptItemUsage()
        {
            if (Timer(4.0f))
            {
                RecordText(12, 2);
            }
            if(Input.GetKey(KeyCode.E))
                TutorialStep++;

        }

        /// <summary>
        /// 12,補助アイテムの解説
        /// </summary>
        private void ExplainItemEffect()
        {
            _guideCanvas.SetActive(false);
            _text[0].gameObject.SetActive(true);
            RecordText(13, 0);
            if (Timer(5.0f))
                TutorialStep++;
        }

        /// <summary>
        /// 13,最後に
        /// </summary>
        private void End()
        {
            RecordText(14, 0);
            if (Timer(5.0f))
                SceneChange(0);
        }
    }
}
