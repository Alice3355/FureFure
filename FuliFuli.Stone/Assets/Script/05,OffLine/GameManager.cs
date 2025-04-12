using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace OffLine
{
    /// <summary>
    /// プレイヤーの動き
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private float _timer = 3.0f;
        [SerializeField, Tooltip("アイテム納品ボックス")]
        public HandleDelivery HandleDelivery;

        [SerializeField, Tooltip("スタート画像")]
        private GameObject _startImage;
        [SerializeField, Tooltip("開始タイマーテキスト")]
        private TextMeshProUGUI[] _startTimerText;
        private PlayerMove _getPlayerMove;

        [SerializeField, Tooltip("ゲーム中タイマー")]
        private TextMeshProUGUI[] _gameTimer;

        [SerializeField, Tooltip("スコア")]
        private TextMeshProUGUI[] _score;
        [SerializeField, Tooltip("スコア")]
        private TextMeshProUGUI[] _scoreOutLine;

        public bool IsStart = false;

        private void Start()
        {
            if(_getPlayerMove == null)
            {
                _getPlayerMove = PlayerData.CreateAvatar.GetComponent<PlayerMove>();
                _getPlayerMove.DisplayPlayer();
            }
        }

        private void Update()
        {
            if(!IsStart)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                foreach (TextMeshProUGUI text in _startTimerText)
                {
                    text.text = _timer.ToString("f0");
                }
                _timer -= Time.deltaTime;
                if (_timer < 1)
                {
                    foreach (TextMeshProUGUI text in _startTimerText)
                    {
                        text.text = "";
                    }
                    _startImage.SetActive(true);
                }
                if (_timer < 0)
                {
                    _startImage.SetActive(false);
                    HandleDelivery.BoxPosID = 0;
                    IsStart = true;
                    _timer = GameConstantsManager.PlayTime;
                    _gameTimer[0].gameObject.SetActive(true);
                }
            }
            else
            {
                foreach (TextMeshProUGUI text in _gameTimer)
                {
                    text.text = _timer.ToString("f1");
                }
                _timer -= Time.deltaTime;
                if(_timer <= 0)
                {
                    Debug.Log("時間切れ");
                    PlayerData.IsRetry[1] = true;
                    GameConstantsManager.SceneChange(6);
                }
                for (int i = 0; i < _score.Length; i++)
                {
                    _score[i].text = PlayerData.Score[i].ToString();
                    _scoreOutLine[i].text = PlayerData.Score[i].ToString();
                }
            }
        }
    }
}