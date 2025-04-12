using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
/// <summary>
/// オンライン接続待機
/// </summary>
public class ConnectionWaitHandler : MonoBehaviour
{
    private GameObject _player;
    private PhotonView _photonView;

    private void Start()
    {
        if (PlayerData.CreateAvatar != null)
        {
            _player = PlayerData.CreateAvatar;
            _photonView = _player.transform.GetComponent<PhotonView>();
            Debug.Log("Playerあり");
        }
        else
            Debug.Log("Playerなし");
    }

    private void Update()
    {
        if(_photonView == null)
        {
            _player = PlayerData.CreateAvatar;
            _photonView = _player.transform.GetComponent<PhotonView>();
            return;
        }

        if (_photonView.IsMine)
            Destroy(gameObject);
        else
        {
            if (PlayerData.OpponentName != null)
                Destroy(gameObject);
        }
    }
}
