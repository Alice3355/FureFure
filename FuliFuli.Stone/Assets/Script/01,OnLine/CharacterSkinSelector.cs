using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static GameConstantsManager;
/// <summary>
/// キャラクターの見た目選択
/// </summary>
public class CharacterSkinSelector : MonoBehaviour
{
    [SerializeField, Tooltip("見た目画像")]
    private GameObject[] _skinList;
    [SerializeField, Tooltip("自分のならtrue")]
    private bool _isMySkin;
    [SerializeField, Tooltip("左右のボタン")]
    private GameObject[] _buttonObject;

    private int _opponentPrefabCnt = 0;

    private int _selectNum = 0;

    private GameObject _prefab = null;
    private bool _isSkinDecision = false;

    private SESet _getSESet = null;

    private void Start()
    {
        _prefab = Instantiate(_skinList[_selectNum], transform.position, Quaternion.identity, transform);
        if(GameObject.Find("SE"))
        {
            _getSESet = GameObject.Find("SE").GetComponent<SESet>();
        }

        if (PlayerData.CreateAvatar == null)
            return;
        GameObject player = PlayerData.CreateAvatar;
        // 選んだスキンの数を受け渡し
        PlayerData.SelectSkinNum[0] = _selectNum;
        // 選んだスキンデータの受け渡し
        player.GetComponent<DataSyncManager>().
                    SendData(new object[] { PlayerData.SelectSkinNum[0], PlayerData.ISGameStart[0] }, 2);

    }

    private void Update()
    {
        // 相手のスキンを表示させる処理のとき
        if (!_isMySkin)
            DisplayOpponentSkin();

        if (!PlayerData.ISGameStart[0])
            return;

        if(_buttonObject != null && _buttonObject.Length > 0)
        {
            foreach(GameObject _object in _buttonObject)
            {
                if (_object != null)
                    _object.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 相手のスキンを表示させる
    /// </summary>
    private void DisplayOpponentSkin()
    {
        // 同じものは複数生成させない
        if (_opponentPrefabCnt == 0)
        {
            _selectNum = PlayerData.SelectSkinNum[1];
            if (_prefab != null)
                Destroy(_prefab);
            _prefab = Instantiate(_skinList[_selectNum], transform.position, Quaternion.identity, transform);
            _opponentPrefabCnt++;
        }
        // 選ばれたものと表示されているものが違うとき
        if (_selectNum != PlayerData.SelectSkinNum[1])
        {
            _opponentPrefabCnt = 0;
        }
    }


    public void OnRightButton()
    {
        // スキンが決まってないとき
        if (!_isSkinDecision)
        {
            _selectNum++;
            if (_selectNum > _skinList.Length - 1)
            {
                _selectNum = 0;
            }
            if (_prefab != null)
                Destroy(_prefab);
            _prefab = Instantiate(_skinList[_selectNum], transform.position, Quaternion.identity, transform);

            if (PlayerData.CreateAvatar == null)
                return;
            GameObject player = PlayerData.CreateAvatar;
            // 選んだスキンの数を受け渡し
            PlayerData.SelectSkinNum[0] = _selectNum;
            // 選んだスキンデータの受け渡し
            player.GetComponent<DataSyncManager>().
                SendData(new object[] { PlayerData.SelectSkinNum[0], PlayerData.ISGameStart[0] }, 2);
        }

        if (_getSESet != null)
            _getSESet.SetClip(0);
    }

    public void OnLeftButton()
    {
        // スキンが決まってないとき
        if(!_isSkinDecision)
        {
            _selectNum--;
            if (_selectNum < 0)
            {
                _selectNum = _skinList.Length - 1;
            }
            if (_prefab != null)
                Destroy(_prefab);
            _prefab = Instantiate(_skinList[_selectNum], transform.position, Quaternion.identity, transform);

            if (PlayerData.CreateAvatar == null)
                return;
            GameObject player = PlayerData.CreateAvatar;
            // 選んだスキンの数を受け渡し
            PlayerData.SelectSkinNum[0] = _selectNum;
            // 選んだスキンデータの受け渡し
            player.GetComponent<DataSyncManager>().
                SendData(new object[] { PlayerData.SelectSkinNum[0], PlayerData.ISGameStart[0] }, 2);
        }

        if (_getSESet != null)
            _getSESet.SetClip(0);
    }

    /// <summary>
    /// 決定ボタン
    /// </summary>
    public void OnDecisionButton()
    {
        if (_getSESet != null)
            _getSESet.SetClip(0);

        if (string.IsNullOrWhiteSpace(PlayerData.OpponentName) ||
            string.IsNullOrWhiteSpace(PlayerData.PlayerName))
            return;

        if (PlayerData.CreateAvatar == null)
            return;
        GameObject player = PlayerData.CreateAvatar;

        // 選んだスキンの数を受け渡し
        PlayerData.SelectSkinNum[0] = _selectNum;
        _isSkinDecision = true;
        PlayerData.ISGameStart[0] = true;
        // 選んだスキンデータの受け渡し
        player.GetComponent<DataSyncManager>().
            SendData(new object[] { PlayerData.SelectSkinNum[0], PlayerData.ISGameStart[0] }, 2);
    }

    /// <summary>
    /// サーバーから抜ける処理
    /// </summary>
    public void OnLogoutButton()
    {
        if (_getSESet != null)
            _getSESet.SetClip(0);

        _isMySkin = false;
        PlayerData.ISGameStart[0] = false;
        // 選んだスキンデータの受け渡し
        GameObject player = PlayerData.CreateAvatar;
        player.GetComponent<DataSyncManager>().
            SendData(new object[] { PlayerData.SelectSkinNum[0], PlayerData.ISGameStart[0] }, 2);
        PhotonNetwork.Disconnect();
        SceneChange(4);
    }
}
