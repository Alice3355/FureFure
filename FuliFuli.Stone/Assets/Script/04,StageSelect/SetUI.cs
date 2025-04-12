using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StageSelect
{
    /// <summary>
    /// UI関係
    /// </summary>
    public class SetUI : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;

        private void LateUpdate()
        {
            _canvas.transform.rotation = Camera.main.transform.rotation;
        }
    }
}