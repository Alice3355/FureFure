using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameConstantsManager;
/// <summary>
/// サーバーに名前をつけて作成
/// </summary>
public class CreateServerName : MonoBehaviour
{
    [SerializeField, Tooltip("ルーム名")]
    private TMP_InputField _getRoomName;
    [SerializeField, Tooltip("プレイヤー名")]
    private TMP_InputField _getPlayerName;
    [SerializeField, Tooltip("生成プレハブ")]
    private GameObject _networkPrefab;

    private Vector3 _startPos = new Vector3(0.0f, 0.0f, 0.0f);

    public void Start()
    {
        _getRoomName.text = null;
        _getPlayerName.text = null;
    }

    /// <summary>
    /// 参加ボタンを押されたときの処理
    /// </summary>
    public void PushParticipateButton()
    {
        if (!string.IsNullOrEmpty(_getRoomName.text) && !string.IsNullOrEmpty(_getPlayerName.text))
        {
            GameObject gameObject = Instantiate(_networkPrefab, _startPos, Quaternion.identity);
            PunConnectionManager manager = gameObject.GetComponent<PunConnectionManager>();
            manager.RoomName = _getRoomName.text;
            manager.PlayerName = _getPlayerName.text;
            manager.GetCreateServerName = GetComponent<CreateServerName>();
            this.gameObject.SetActive(false);
        }
        else
            Debug.Log("名前の入力無し");
    }

    /// <summary>
    /// 戻るボタンを押されたときの処理
    /// </summary>
    public void PushReturnButton()
    {
        SceneChange(4);
    }

    /// <summary>
    /// 部屋に入れなかったときの処理
    /// </summary>
    public void OnRoomJoinFailed()
    {
        _getPlayerName.text = "";
        _getRoomName.text = "";
    }
}
