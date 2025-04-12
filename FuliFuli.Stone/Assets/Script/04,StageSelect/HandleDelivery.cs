using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StageSelect
{
    /// <summary>
    /// アイテムが納品されたときの処理チュートリアル
    /// </summary>
    public class HandleDelivery : MonoBehaviour
    {
        [SerializeField, Tooltip("アイテムの種類")]
        private GameObject[] _foodObject;
        [SerializeField, Tooltip("マテリアル")]
        private Material[] _materials;
        [SerializeField, Tooltip("自分のマテリアル")]
        private MeshRenderer _meshRenderer;

        private int _selectItemNum = 0;
        public int Score { get; private set; } = 0;
        public bool IsBomb { get; private set; } = false;

        /// <summary>
        /// はじめに行う
        /// </summary>
        private void Start()
        {
            _meshRenderer.material = _materials[_selectItemNum];
        }
        
        /// <summary>
        /// 当たり判定入る
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.CompareTag("FoodItem"))
            {
                string itemName = $"{_foodObject[_selectItemNum].name}(Clone)";
                
                if (collision.gameObject.name == itemName)
                {
                    Destroy(collision.gameObject);
                    _selectItemNum++;
                    Score++;
                    _meshRenderer.material = _materials[_selectItemNum];
                }
            }
            if (collision.gameObject.CompareTag("Bomb"))
            {
                collision.gameObject.GetComponent<Item>().Explosion();
                int rand = Random.Range(1, 8);
                Score -= rand;
                IsBomb = true;
            }
        }
    }
}
