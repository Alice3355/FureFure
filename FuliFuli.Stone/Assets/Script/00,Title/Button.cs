using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstantsManager;
namespace Title
{
    /// <summary>
    /// Buttonが押された処理
    /// </summary>
    public class Button : MonoBehaviour
    {
        [SerializeField, Tooltip("設定用画面")]
        private GameObject[] _settingPrefab; 
            
        private GameObject _createPrefab = null;

        private SESet _getSESet = null;
        [SerializeField]
        private bool _isSetting;

        private void Start()
        {
            if (GameObject.Find("SE"))
                _getSESet = GameObject.Find("SE").GetComponent<SESet>();

            if(_isSetting)
                OnSoundButton();
        }

        /// <summary>
        /// 始めるボタン
        /// </summary>
        public void OnPlayStartButton()
        {
            SceneChange(4);
        }
        /// <summary>
        /// 設定ボタン
        /// </summary>
        public void OnSettingButton()
        {
            InstantiateAtCenter(_settingPrefab[0]);
        }
        /// <summary>
        /// 終了ボタン
        /// </summary>
        public void OnGameEndButton()
        {
            Application.Quit();
        }

        /// <summary>
        /// 音量調整ボタン
        /// </summary>
        public void OnSoundButton()
        {
            if (_getSESet != null)
                _getSESet.SetClip(0);

            if (_createPrefab != null)
                Destroy(_createPrefab.gameObject);

            InstantiateAtCenter(_settingPrefab[0]);
        }
        /// <summary>
        /// 操作ボタン
        /// </summary>
        public void OnOperationButton()
        {
            if (_getSESet != null)
                _getSESet.SetClip(0);

            if (_createPrefab != null)
                Destroy(_createPrefab.gameObject);

            InstantiateAtCenter(_settingPrefab[1]);
        }
        /// <summary>
        /// 設定終了ボタン
        /// </summary>
        public void OnSettingEndButton()
        {
            if (_getSESet != null)
                _getSESet.SetClip(0);

            Destroy(transform.parent.gameObject);
        }

        /// <summary>
        /// 中心に生成
        /// </summary>
        /// <param name="gameObject"></param>
        private void InstantiateAtCenter(GameObject gameObject)
        {
            _createPrefab = Instantiate(gameObject, transform.parent);
            RectTransform rt = _createPrefab.GetComponent<RectTransform>();
            if (rt == null)
            {
                Debug.LogError("RectTransformが見つかりません。対象のオブジェクトはUIオブジェクトですか？");
                return;
            }
            // 親オブジェクトの中央に配置
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero; // 親の中央に配置
        }

        public void OnLookButton(bool flg)
        {
            if(flg)
            {
                _settingPrefab[0].SetActive(true);
                _settingPrefab[1].SetActive(false);
            }
            else
            {
                _settingPrefab[0].SetActive(false);
                _settingPrefab[1].SetActive(true);
            }
        }
    }
}