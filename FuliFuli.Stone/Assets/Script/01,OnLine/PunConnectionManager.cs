using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using static GameConstantsManager;
/// <summary>
/// MonoBehaviourPunCallbacksを継承して、
/// PUNのコールバックを受け取れるようにする
/// </summary>
public class PunConnectionManager : MonoBehaviourPunCallbacks
{
    [System.NonSerialized]
    public string RoomName = null;
    [System.NonSerialized]
    public string PlayerName = null;

    [System.NonSerialized]
    public CreateServerName GetCreateServerName;

    [SerializeField]
    private TextMeshProUGUI[] _text;
    [SerializeField]
    private TextMeshProUGUI[] _textOutLine;
    [SerializeField]
    private TextMeshProUGUI[] _warningText;

    private string _opponentName = null;

    private void Start()
    {
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
        _text[0].text = PlayerName;
        _text[1].text = RoomName;

        _textOutLine[0].text = PlayerName;
        _textOutLine[1].text = RoomName;
        PhotonNetwork.NickName = PlayerName;
        PlayerData.PlayerName = PlayerName;
    }

    private void Update()
    {
        if (_opponentName != PlayerData.OpponentName)
            DisplayOpponentName(PlayerData.OpponentName);
    }

    /// <summary>
    /// マスターサーバへの接続に成功したときに呼ばれるコールバック
    /// </summary>
    public override void OnConnectedToMaster()
    {
        // ルームオプションを作成して、最大人数までに制限
        RoomOptions roomOptions = new RoomOptions();
        // TODO:後々作成者が決定できるようにしたい
        roomOptions.MaxPlayers = 2; // 最大数を入れる

        // 参加する部屋(ルームが無ければ作成する)
        PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);
    }

    /// <summary>
    /// ゲームサーバーへの接続が成功したときに呼ばれるコールバック
    /// </summary>
    public override void OnJoinedRoom()
    {
        // ルーム内のプレイヤー数を確認
        // 人数を設定させる
        if (PhotonNetwork.CurrentRoom.PlayerCount < 3)
        {
            // アバター生成
            var pos = new Vector3(0.0f, 0.0f);
            GameObject prefab = PhotonNetwork.Instantiate("TestAvatar", pos, Quaternion.identity);
            PlayerData.CreateAvatar = prefab;
        }
        else
        {
            foreach(TextMeshProUGUI text in _warningText)
            {
                text.text = "満員です！参加できません！";
            }
        }
    }

    /// <summary>
    /// ルームに入れなかったときの処理
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //_warningText.text = "ルームに参加できませんでした: " + message;
        GetCreateServerName.OnRoomJoinFailed();
        GetCreateServerName = null;
        RoomName = "";
        PlayerName = "";
        if(IsSceneCheck(1))
            Destroy(gameObject);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount > 2)
        {
            // ルームに2人以上入った場合は他のプレイヤーに通知
            foreach (TextMeshProUGUI text in _warningText)
            {
                text.text = "満員です！プレイヤーが退出したか、定員オーバーになりました";
            }
        }
    }

    /// <summary>
    /// 敵の名前の表示
    /// </summary>
    private void DisplayOpponentName(string name)
    {
        _opponentName = name;
        _textOutLine[2].text = _opponentName;
        _text[2].text = _opponentName;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        GetCreateServerName.OnRoomJoinFailed();
        if(IsSceneCheck(1))
            Destroy(gameObject);
    }
}
