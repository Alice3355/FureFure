using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
/// <summary>
/// BGM、SEの音量を変える
/// </summary>
public class VolumeSliderController : MonoBehaviour
{
    [SerializeField, Tooltip("オーディオミキサー")]
    private AudioMixer _audioMixer;

    [SerializeField, Tooltip("スライダー")]
    private Slider[] _sliders;

    private void Start()
    {
        if (_audioMixer == null)
            return;

        AudioMixerGetFloat("BGM", 1);
        AudioMixerGetFloat("SE", 2);
        AudioMixerGetFloat("Master", 0);
    }

    public void SetBGM(float volume)
    {
        _audioMixer.SetFloat("BGM", volume);
    }

    public void SetSE(float volume)
    {
        _audioMixer.SetFloat("SE", volume);
        GameObject.Find("SE").GetComponent<SESet>().SetClip(0);
    }

    public void SetMaster(float volume)
    {
        _audioMixer.SetFloat("Master", volume);
    }

    private void AudioMixerGetFloat(string name, int num)
    {
        _audioMixer.GetFloat(name, out float volume);
        _sliders[num].value = volume;
    }
}
// https://note.com/calm_otter351/n/need65fbb2810