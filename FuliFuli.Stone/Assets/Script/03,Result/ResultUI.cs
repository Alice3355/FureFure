using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Result
{
    /// <summary>
    /// リサルト画面のUI処理
    /// </summary>
    public class ResultUI : MonoBehaviour
    {
        [SerializeField, Tooltip("勝ちか負けか")]
        private TextMeshProUGUI _resultText;
        [SerializeField, Tooltip("勝ちか負けか")]
        private TextMeshProUGUI _resultTextOutLine;

        [SerializeField, Tooltip("スコアとハイスコア")]
        private TextMeshProUGUI[] _scoreText = new TextMeshProUGUI[2];
        [SerializeField, Tooltip("スコアとハイスコア")]
        private TextMeshProUGUI[] _scoreTextOutLine = new TextMeshProUGUI[2];

        private int _highScore;
        private string _key = "HIGH SCORE";

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // 呼び出し(値がないときは0)
            _highScore = PlayerPrefs.GetInt(_key, 0);

            if (PlayerData.Score[0] > PlayerData.Score[1])
            {
                _resultText.text = "勝ち！";
            }
            else if (PlayerData.Score[1] > PlayerData.Score[0])
            {
                _resultText.text = "負け";
            }
            else
            {
                _resultText.text = "引き分け";
            }

            _resultTextOutLine.text = _resultText.text;

            if (_highScore < PlayerData.Score[0])
            {
                _highScore = PlayerData.Score[0];
                // ハイスコア保存
                PlayerPrefs.SetInt(_key, _highScore);
                // デスクへの書き込み
                PlayerPrefs.Save();
            }
            _scoreText[0].text = ($"Score:{PlayerData.Score[0].ToString()}");
            _scoreText[1].text = ($"HighScore:{_highScore.ToString()}");
            _scoreTextOutLine[0].text = _scoreText[0].text;
            _scoreTextOutLine[1].text = _scoreText[1].text;
        }
    }
}
