using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StageSelect
{
    /// <summary>
    /// テキスト関係
    /// </summary>
    public class Text : MonoBehaviour
    {
        [SerializeField, Tooltip("表示させたい文"), Multiline(3)]
        public string Sentence;
    }
}
