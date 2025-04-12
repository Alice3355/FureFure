using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// SEの管理を行う
/// </summary>
public class SEManager : MonoBehaviour
{
    private static SEManager _instance;
    [SerializeField]
    private AudioSource[] _audioSource;
    private SESet _getSESet;
    private string _sceneName = null;

    private void Awake()
    {
        DontDestroyOnLoad(transform);

        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name != _sceneName && GameObject.Find("SE"))
        {
            _sceneName = SceneManager.GetActiveScene().name;
            _getSESet = GameObject.Find("SE").GetComponent<SESet>();
        }
        else if(SceneManager.GetActiveScene().name != _sceneName)
        {
            _sceneName = SceneManager.GetActiveScene().name;
            return;
        }

        if (_getSESet == null)
            return;

        if(_getSESet.GicClip != null)
        {
            // メイン画面で多少音がかぶるようにするため
            if(_audioSource[0].isPlaying)
            {
                _audioSource[1].Stop();
                _audioSource[1].PlayOneShot(_getSESet.GicClip);
                _getSESet.GicClip = null;
                return;
            }
            _audioSource[0].Stop();
            _audioSource[0].PlayOneShot(_getSESet.GicClip);
            _getSESet.GicClip = null;
        }
    }
}
