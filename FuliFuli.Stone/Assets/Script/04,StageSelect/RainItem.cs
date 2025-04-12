using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StageSelect
{
    public class RainItem : MonoBehaviour
    {
        [SerializeField, Tooltip("アイテムの種類")]
        private GameObject[] _foodObject;

        public void SetRainItem(int num)
        {
            Instantiate(_foodObject[num], transform.position, Quaternion.identity);
        }
    }
}
