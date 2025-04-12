using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Title
{
    /// <summary>
    /// Titleの文字を動かす
    /// </summary>
    public class ItemUIMove : MonoBehaviour
    {
        private Quaternion _rota;
        private float _rotaSpeed = 100.0f;
        private int _RorL = 0;

        private void Start()
        {
            _rota = this.transform.rotation;
            _RorL = Random.Range(0, 2);
            if (_RorL == 0)
                _rotaSpeed = -_rotaSpeed;
        }

        private void Update()
        {
            transform.Rotate(new Vector3(0.0f, 0.0f, _rotaSpeed) * Time.deltaTime);
            if (transform.position.y < -400.0f)
                Destroy(gameObject);
        }
    }
}
