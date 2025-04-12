using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static GameConstantsManager;
namespace OffLine
{
    /// <summary>
    /// Button関係の処理
    /// </summary>
    public class Button : MonoBehaviourPun
    {
        /// <summary>
        /// リトライ
        /// </summary>
        public void OnRetryButton()
        {
            PlayerData.Retry();
            SceneChange(5);
        }
        /// <summary>
        /// タイトル
        /// </summary>
        public void OnTitleButton()
        {
            PlayerData.Reset();
            SceneChange(0);
        }
    }
}
