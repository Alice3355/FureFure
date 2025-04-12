using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SESet : MonoBehaviour
{
    [SerializeField, Tooltip("SE")]
    private AudioClip[] _seClips;

    [System.NonSerialized]
    public AudioClip GicClip = null;

    private void Update()
    {
        // 確認用
        if(Input.GetKeyDown(KeyCode.V))
        {
            SetClip(0);
        }
    }

    public void SetClip(int num)
    {
        GicClip = _seClips[num];
    }
}
