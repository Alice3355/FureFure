using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstantsManager;
namespace OffLine
{
    /// <summary>
    /// キャラクターの見た目選択
    /// </summary>
    public class CharacterSkinSelector : MonoBehaviour
    {
        [SerializeField, Tooltip("見た目の画像")]
        private GameObject[] _skinList;

        private int _selectNum = 0;
        private GameObject _prefabSkin = null;

        [SerializeField, Tooltip("消す")]
        private GameObject[] _offObject;
        [SerializeField, Tooltip("表示する")]
        private GameObject[] _onObject;

        private void Start()
        {
            _prefabSkin = Instantiate(_skinList[_selectNum], transform.position, Quaternion.identity, transform);
        }

        /// <summary>
        /// 右ボタン
        /// </summary>
        public void OnRightButton()
        {
            _selectNum++;
            if(_selectNum > _skinList.Length - 1)
            {
                _selectNum = 0;
            }
            if (_prefabSkin != null)
                Destroy(_prefabSkin);

            _prefabSkin = Instantiate(_skinList[_selectNum], transform.position, Quaternion.identity, transform);
        }

        /// <summary>
        /// 左ボタン
        /// </summary>
        public void OnLeftButton()
        {
            _selectNum--;
            if(_selectNum < 0)
            {
                _selectNum = _skinList.Length - 1;
            }
            if (_prefabSkin != null)
                Destroy(_prefabSkin);

            _prefabSkin = Instantiate(_skinList[_selectNum], transform.position, Quaternion.identity, transform);
        }

        /// <summary>
        /// 決定ボタン
        /// </summary>
        public void OnDecisionButton()
        {
            Debug.Log("ゲーム開始");
            foreach(GameObject gameObject in _offObject)
            {
                gameObject.SetActive(false);
            }
            foreach(GameObject gameObject in _onObject)
            {
                gameObject.SetActive(true);
            }
            PlayerData.SelectSkinNum[0] = _selectNum;
        }
        
        /// <summary>
        /// 戻る
        /// </summary>
        public void OnBack()
        {
            SceneChange(4);
        }
    }
}
