using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstantsManager;
namespace StageSelect
{
    /// <summary>
    /// ボタンが押されたときの処理
    /// </summary>
    public class Button : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _gameObject;
        [SerializeField]
        private GameObject[] _camera;

        public void OnTutorial()
        {
            _gameObject[0].SetActive(false);
            _gameObject[1].SetActive(true);
            _camera[0].SetActive(false);
            _camera[1].SetActive(true);
        }
        public void OnOnLine()
        {
            SceneChange(1);
        }
        public void OnOffLine()
        {
            SceneChange(5);
        }
        public void OnTitle()
        {
            SceneChange(0);
        }
        
    }
}
