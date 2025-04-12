using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OffLine
{
    /// <summary>
    /// アイテムを空から降らせる
    /// </summary>
    public class ItemRainSpawner : MonoBehaviour
    {
        [System.NonSerialized]
        private Vector2[] _rainPos = new Vector2[2];

        [SerializeField, Tooltip("生成する高さ")]
        private float _rainPosY;

        [SerializeField, Tooltip("生成するアイテム")]
        private GameObject[] _itemePrefab;

        private Vector3 _spawnerPos;
        private DataSyncManager _data;
        [SerializeField, Tooltip("ステージの親")]
        private StagePosSet GetStagePosSet;

        private float[] _timer = { 0.0f, 0.3f };

        private void Start()
        {
            _spawnerPos.y = _rainPosY;
        }

        private void Update()
        {
            if (GetStagePosSet != null)
            {
                for (int i = 0; i < GetStagePosSet.ItemRainPos.Length; i++)
                {
                    _rainPos[i] = GetStagePosSet.ItemRainPos[i];
                }

                _timer[0] += Time.deltaTime;
                if (_timer[0] > _timer[1])
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
            Instantiate(_itemePrefab[num], pos, Quaternion.identity);
        }

        public void BuffSpawnerItem(Vector3 pos, int num)
        {
            for (int i = 0; i < num; i++)
            {
                pos.y = _rainPosY;
                int rand = Random.Range(0, _itemePrefab.Length);
                Instantiate(pos, rand);
            }
        }
    }

}