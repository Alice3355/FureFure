using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// スコアのディスプレイ
/// </summary>
public class ScoreDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] _text;
    [SerializeField]
    private TextMeshProUGUI[] _textOutLine;

    private void Update()
    {
        for(int i = 0; i < _text.Length; i++)
        {
            _text[i].text = ($"Score:{ PlayerData.Score[i]}");
            _textOutLine[i].text = ($"Score:{ PlayerData.Score[i]}");
        }
    }
}
