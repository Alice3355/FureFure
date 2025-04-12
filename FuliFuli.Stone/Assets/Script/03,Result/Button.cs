using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static GameConstantsManager;
namespace Result
{
    /// <summary>
    /// Button関係の処理
    /// </summary>
    public class Button : MonoBehaviourPun
    {
        private DataSyncManager _data;

        private void Start()
        {
            if(PlayerData.CreateAvatar.GetComponent<DataSyncManager>())
            {
                _data = PlayerData.CreateAvatar.GetComponent<DataSyncManager>();
            }
            else
            {
                Debug.Log("アバターまたはDataSyncManagerが見つかりません。タイトル画面に戻ります。");
                PhotonNetwork.Disconnect();
                PlayerData.Reset();
                PunConnectionManager manager = gameObject.GetComponent<PunConnectionManager>();
                manager.RoomName = null;
                manager.PlayerName = null;
                manager.GetCreateServerName = null;
                SceneChange(0);
            }
        }

        public void CheckRetryCondition()
        {
            if(PlayerData.IsRetry[0] && PlayerData.IsRetry[1])
            {
                PlayerData.Retry();
                SceneChange(1);
            }
        }

        /// <summary>
        /// リトライ
        /// </summary>
        public void OnRetryButton()
        {
            _data.SendData(new object[] { true }, 8);

            PlayerData.IsRetry[0] = true;

            CheckRetryCondition();
        }

        /// <summary>
        /// タイトル
        /// </summary>
        public void OnTitleButton()
        {
            PlayerData.Reset();
            StartCoroutine(DisconnectAndLoadScene(0));
        }

        private IEnumerator DisconnectAndLoadScene(int scoreName)
        {
            PhotonNetwork.Disconnect();
            while(PhotonNetwork.IsConnected)
            {
                yield return null; // 切断が完了するまで待つ
            }
            PlayerData.Reset();
            SceneChange(scoreName);
        }
    }
}
