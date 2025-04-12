using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// プレイヤーに補助アイテムを受け渡す
/// </summary>
public class RandomBuffDistributor : MonoBehaviour
{
    [SerializeField, Tooltip("補助アイテム")]
    private Image[] _itemImages;

    public void GivBuffItem(int comb)
    {
        if(comb > 0)
        {
            int rand = Random.Range(0, _itemImages.Length);

            if(PlayerData.CreateAvatar != null)
                PlayerData.CreateAvatar.GetComponent<BuffManager>().GivSelectBuffItem(rand);
        }
    }
}
