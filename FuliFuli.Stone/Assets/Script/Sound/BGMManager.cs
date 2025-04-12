using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameConstantsManager;
/// <summary>
/// BGMの管理を行う
/// </summary>
public class BGMManager : MonoBehaviour
{
    private static BGMManager _instance;
    [SerializeField]
    private AudioSource _audioSource;
    public BGMSet _getBGMSet;
    public string _sceneName = null;

    private void Awake()
    {
        DontDestroyOnLoad(transform);

        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// オブジェクトが有効になったら
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// 設定されているゲームオブジェクトがアクティブになったら
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _sceneName)
            return;

        if(_sceneName == SceneName[2])
            _audioSource.volume += 0.5f;

        if (!GameObject.Find("BGM"))
            return;
        _getBGMSet = GameObject.Find("BGM").GetComponent<BGMSet>();
        if (_getBGMSet == null)
            return;

        _audioSource.clip = _getBGMSet.BGMClips;
        _audioSource.Play();

        if (_sceneName == SceneName[2])
            _audioSource.volume -= 0.5f;

        _sceneName = scene.name;
    }
}
