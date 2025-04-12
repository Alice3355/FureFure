using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Title
{
    /// <summary>
    /// UIを生成する
    /// </summary>
    public class CreateUI : MonoBehaviour
    {
        [SerializeField, Tooltip("生成する場所")]
        private GameObject[] _createPos;
        [SerializeField, Tooltip("生成するUI")]
        private GameObject[] _itemUi;

        private float _timer = 0.0f;
        private float _timeSet = 2.0f;

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            for (int i = 0; i < _createPos.Length; i++)
            {
                int rand = Random.Range(0, _itemUi.Length);
                Instantiate(_itemUi[rand], _createPos[i].transform.position, Quaternion.identity, transform);
            }
            PlayerData.ISGameStart[0] = false;
            PlayerData.ISGameStart[1] = false;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if(_timer > _timeSet)
            {
                for (int i = 0; i < _createPos.Length; i++)
                {
                    int rand = Random.Range(0, _itemUi.Length);
                    Instantiate(_itemUi[rand], _createPos[i].transform.position, Quaternion.identity, transform);
                }
                _timer = 0.0f;
            }
        }
    }
}