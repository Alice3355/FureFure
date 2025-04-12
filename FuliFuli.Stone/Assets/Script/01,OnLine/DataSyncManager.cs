using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;  // イベント送受信に必要
using static GameConstantsManager;
/// <summary>
/// 相手にデータ送信、受け取り
/// </summary>
public class DataSyncManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private string _playerName;
    private enum EventCode
    {
        Name = 1,
        Num, Pos, Item, SetMaterial,
        Score, Rush, Retry, Bomb, BuffItem
    }

    public PunConnectionManager GetPunConnectionManager { get; private set; }
    public ItemRainSpawner GetRainSpawner;
    public MyNamespace.PlayerMove GetPlayerMove { get; private set; }
    public HandleDelivery GetHandheld;

    private void Start()
    {
        _playerName = PhotonNetwork.NickName;
        // イベントを受け取るためにOnEventコールバックを登録
        PhotonNetwork.AddCallbackTarget(this);
        DontDestroyOnLoad(gameObject);

        // 人数変更ここで
        if (PlayerData.GameMainPosSet[1] != 0)
        {
            // 相手が選んだ数字を選択
            int exclusionNum = PlayerData.GameMainPosSet[1];
            int rand = Random.Range(1, 3 - 1);

            if (rand >= exclusionNum)
                rand++;

            PlayerData.GameMainPosSet[0] = rand;
            SendData(new object[] { rand }, (byte)EventCode.Pos);
        }
        else
        {
            int rand = Random.Range(1, 3);
            PlayerData.GameMainPosSet[0] = rand;
            SendData(new object[] { rand }, (byte)EventCode.Pos);
        }
    }

    /// <summary>
    /// 他のプレイヤーが入室したときに呼ばれるコールバック
    /// </summary>
    /// <param name="newPlayer">入って来たプレイヤー</param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("新しいプレイヤーが入室しました:" + newPlayer.NickName);
        PlayerData.OpponentName = newPlayer.NickName;
        // 新しいプレイヤーに今までの変更を送信
        SendData(new object[] { _playerName }, (byte)EventCode.Name);
        Debug.Log("自分の名前 " + name + " をプレイヤー " + newPlayer.NickName + " に送信しました。");

        //SendNumber(PlayerData.SelectSkinNum[0], PlayerData.ISGameStart[0]);
        // イベントのデータとして数字を送信
        // 配列でデータを送る(単一データも配列で送る必要がある)
        SendData(new object[] { PlayerData.SelectSkinNum[0], PlayerData.ISGameStart[0] }, (byte)EventCode.Num);
        SendData(new object[] { PlayerData.GameMainPosSet[0] }, (byte)EventCode.Pos);
    }

    /// <summary>
    /// 他のプレイヤーが部屋を退室したときに呼ばれるコールバック
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerData.OpponentName = null;
        if (IsSceneCheck(1))
        {
            SceneChange(0);
            PhotonNetwork.Disconnect();
        }
        Debug.Log("プレイヤーが退室しました:" + otherPlayer.NickName);
    }

    /// <summary>
    /// イベントを受信するコールバック
    /// </summary>
    /// <param name="photonEvent"></param>
    public void OnEvent(EventData photonEvent)
    {
        // イベントコードが自分に設定したもの(1)であることを確認
        if (photonEvent.Code == (byte)EventCode.Num)
        {
            object[] data = (object[])photonEvent.CustomData;
            int receivedNumber = (int)data[0];
            PlayerData.ISGameStart[1] = (bool)data[1];

            // 受け取ったスキンのデータを保存する
            PlayerData.SelectSkinNum[1] = receivedNumber;
        }
        // 名前の受け渡し
        if (photonEvent.Code == (byte)EventCode.Name)
        {
            object[] data = (object[])photonEvent.CustomData;
            string receivedName = (string)data[0];

            // 受け取った名前を保存する
            PlayerData.OpponentName = receivedName;
        }
        // 選んだポジションの受け渡し
        if (photonEvent.Code == (byte)EventCode.Pos)
        {
            object[] data = (object[])photonEvent.CustomData;
            int receivedPos = (int)data[0];

            PlayerData.GameMainPosSet[1] = receivedPos;
        }
        // アイテムの状態受け渡し
        if (photonEvent.Code == (byte)EventCode.Item)
        {
            object[] data = (object[])photonEvent.CustomData;
            bool isHaveItem = (bool)data[0];
            int playerId = (int)data[1];
            GetPlayerMove = transform.GetComponent<MyNamespace.PlayerMove>();

            GetPlayerMove.OpponentDropItem(isHaveItem, playerId);
        }
        // マテリアルの状態受け渡し
        if (photonEvent.Code == (byte)EventCode.SetMaterial)
        {
            object[] data = (object[])photonEvent.CustomData;
            int selectNum = (int)data[0];
            if (GetHandheld != null)
            {
                GetHandheld.GetMaterial(selectNum);
            }
        }
        // スコアの受け渡し
        if (photonEvent.Code == (byte)EventCode.Score)
        {
            object[] data = (object[])photonEvent.CustomData;
            int score = (int)data[0];
            bool isMy = (bool)data[1];

            if (GetHandheld != null)
            {
                GetHandheld.GetScore(score, isMy);
            }
        }
        // 突進中か受け渡し
        if (photonEvent.Code == (byte)EventCode.Rush)
        {
            object[] data = (object[])photonEvent.CustomData;
            bool isRush = (bool)data[0];
            int playerId = (int)data[1];
            GetPlayerMove = this.GetComponent<MyNamespace.PlayerMove>();
            GetPlayerMove.Stan(isRush, playerId);
        }
        // リトライするか受け渡し
        if (photonEvent.Code == (byte)EventCode.Retry)
        {
            object[] data = (object[])photonEvent.CustomData;
            bool isRetry = (bool)data[0];
            PlayerData.IsRetry[1] = isRetry;
        }

        // 爆発したとき
        if (photonEvent.Code == (byte)EventCode.Bomb)
        {
            object[] data = (object[])photonEvent.CustomData;
            bool isFlg = (bool)data[0];

            if (!isFlg)
                return;

            GetPlayerMove = this.GetComponent<MyNamespace.PlayerMove>();
            GetPlayerMove.BombExplosion();
        }
        // アイテムを降らす
        if (photonEvent.Code == (byte)EventCode.BuffItem)
        {
            object[] data = (object[])photonEvent.CustomData;
            bool isFlg = (bool)data[0];
            Vector3 pos = (Vector3)data[1];

            if (!isFlg)
                return;

            GetPlayerMove = this.GetComponent<MyNamespace.PlayerMove>();
            PlayerData.IsUseRainItem = isFlg;
            PlayerData.RainPos = pos;
        }
    }

    /// <summary>
    /// イベントの登録
    /// </summary>
    public override void OnEnable()
    {
        // イベントのリスナーとして自分を登録
        PhotonNetwork.AddCallbackTarget(this);
    }

    /// <summary>
    /// イベントリスナーの解除(自分が消されたときに)
    /// </summary>
    private void OnDestroy()
    {
        // イベントのリスナーを解除
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SendData(object[] content, byte num)
    {
        // 他の全プレイヤー
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        // に送信
        PhotonNetwork.RaiseEvent(num, content, raiseEventOptions, SendOptions.SendReliable);
    }
}