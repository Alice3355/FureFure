using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
/// <summary>
/// アイテムを空から降らせる
/// </summary>
public class ItemRainSpawner : MonoBehaviourPunCallbacks
{
    [System.NonSerialized]
    private Vector2[] _rainPos = new Vector2[2];

    [SerializeField, Tooltip("生成する高さ")]
    private float _rainPosY;

    [SerializeField, Tooltip("生成するアイテム")]
    private GameObject[] _itemePrefab;

    private Vector3 _spawnerPos;
    private DataSyncManager _data;
    [System.NonSerialized]
    public StagePosSet GetStagePosSet;

    private float[] _timer = {0.0f, 0.3f };

    private void Start()
    {
        _spawnerPos.y = _rainPosY;
        _data = PlayerData.CreateAvatar.GetComponent<DataSyncManager>();
        _data.GetRainSpawner = this;
    }

    private void Update()
    {
        if(GetStagePosSet != null)
        {
            for (int i = 0; i < GetStagePosSet.ItemRainPos.Length; i++)
            {
                _rainPos[i] = GetStagePosSet.ItemRainPos[i];
            }
            // ルームマスターのみがアイテムを生成できる
            if (!PhotonNetwork.IsMasterClient)
                return;

            if(PlayerData.IsUseRainItem)
            {
                Vector3 pos = PlayerData.RainPos;
                int rand = Random.Range(3, 9);
                BuffSpawnerItem(pos, rand);
                PlayerData.IsUseRainItem = false;
                PlayerData.CreateAvatar.GetComponent<BuffManager>().IsRainItem = false;
                //Debug.Log("降らす");
            }

            _timer[0] += Time.deltaTime;
            if(_timer[0] > _timer[1])
            {
                _timer[0] = 0.0f;
                SpawnerItem();
            }
        }
    }

    /// <summary>
    /// 生成処理
    /// </summary>
    private void SpawnerItem()
    {
        _spawnerPos.x = Random.Range(_rainPos[0].x, _rainPos[1].x);
        _spawnerPos.z = Random.Range(_rainPos[0].y, _rainPos[1].y);

        int rand = Random.Range(0, _itemePrefab.Length);
        Instantiate(_spawnerPos, rand);
    }

    /// <summary>
    /// アイテムの生成
    /// </summary>
    public void Instantiate(Vector3 pos, int num)
    {
        PhotonNetwork.Instantiate(_itemePrefab[num].name, pos, Quaternion.identity);
    }

    /// <summary>
    /// 補助アイテムでアイテムを降らすとき
    /// </summary>
    public void BuffSpawnerItem(Vector3 pos, int num)
    {
        for(int i = 0; i < num; i++)
        {
            pos.y = _rainPosY;
            int rand = Random.Range(0, _itemePrefab.Length);
            Instantiate(pos, rand);
        }
    }
}
