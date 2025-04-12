using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using static GameConstantsManager;
/// <summary>
/// ゲームメインの全体管理
/// </summary>
public class GameMainManager : MonoBehaviourPunCallbacks
{
    private int _objectCnt = 0;
    [SerializeField]
    private GameObject _startUISet;
    [SerializeField]
    private TextMeshProUGUI[] _timerText;
    [SerializeField]
    private TextMeshProUGUI[] _timerTextOutLine;

    private float[] _timer = {5.0f, PlayTime };

    [SerializeField, Tooltip("ゲームに必要なもの")]
    private GameObject _rainBoxSet;
    private int _startCnt = 0;

    [SerializeField, Tooltip("納品箱")]
    private GameObject _deliveryBox;

    [SerializeField, Tooltip("補助アイテム関連")]
    private RandomBuffDistributor _buffDistributor;

    public HandleDelivery HandleDelivery { get; private set; }

    [System.NonSerialized]
    public GameObject RainBox;

    [SerializeField, Tooltip("ステージ")]
    private GameObject[] _stageObj;
    private Vector3[] _setPos = new Vector3[10];
    private int _stageSelectNum = 1;

    [SerializeField, Tooltip("スタート画像")]
    private GameObject _startImage;

    private StagePosSet _posSet;
    private bool _isNowGame = false;

    private void Start()
    {
        _startImage.SetActive(false);
        GameObject player = PlayerData.CreateAvatar;
        GameObject stage = Instantiate(_stageObj[_stageSelectNum], new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        _posSet = stage.GetComponent<StagePosSet>();
        for (int i = 0; i < _posSet.BoxPos.Length; i++)
        {
            _setPos[i] = _posSet.BoxPos[i];
        }
        player.GetComponent<MyNamespace.PlayerMove>().GetStagePosSet = _posSet;
    }

    private void Update()
    {
        // TODO:指定人数以下
        if(_objectCnt < 2)
        {
            Check();
            if (_isNowGame)
                SceneChange(0);
            return;
        }
        // 始まる前
        if(_startUISet != null)
        {

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _timer[0] -= Time.deltaTime;
            // _timerTextOutLine
            _timerText[0].text = _timer[0].ToString("f0");
            if (_timer[0] < 1)
            {
                _startImage.SetActive(true);
                _timerText[0].text = "";
            }
            if (_timer[0] < 0)
            {
                PlayerData.ISGameStart[2] = true;
                Destroy(_startUISet);
            }
            _timerTextOutLine[0].text = _timerText[0].text;
            return;
        }

        // 始まり
        if (_timer[1] > 0)
        {
            _isNowGame = true;
            _timer[1] -= Time.deltaTime;
            string text = _timer[1].ToString("f1");
            text = text.Replace(".", ":");
            _timerText[1].text = text;
            _timerTextOutLine[1].text = _timerText[1].text;
        }
        else
        {
            Camera.main.gameObject.GetComponent<CameraMove>().ResetCameraPos();
            SceneChange(3);
        }

        if (_startCnt < 1)
        {
            RainBox = Instantiate(_rainBoxSet, transform.position, Quaternion.identity);
            RainBox.GetComponent<ItemRainSpawner>().GetStagePosSet = _posSet;
            int num = PlayerData.GameMainPosSet[0];

            Vector3 pos = _setPos[num - 1];
            GameObject obj = PhotonNetwork.Instantiate(_deliveryBox.name, pos, Quaternion.identity);
            HandleDelivery = obj.GetComponent<HandleDelivery>();
            HandleDelivery.BoxPosID = num;
            HandleDelivery.BuffDistributor = _buffDistributor;

            _startCnt++;
        }
    }

    private void LateUpdate()
    {
        if (!_isNowGame)
            return;
        // 何人いるか確認
        Check();
    }

    /// <summary>
    /// オブジェクトの数を確認
    /// </summary>
    private void Check()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");
        _objectCnt = objects.Length;
    }

}
