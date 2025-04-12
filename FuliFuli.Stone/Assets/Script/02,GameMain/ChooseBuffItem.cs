using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 補助アイテムを選択する処理
/// </summary>
public class ChooseBuffItem : MonoBehaviour
{
    private BuffManager _manager;
    [SerializeField, Tooltip("アイテムUI")]
    private Image[] _setImage = new Image[3];
    [SerializeField, Tooltip("アイテムの画像")]
    private Sprite[] _sprites;
    [SerializeField, Tooltip("次の要求品表示")]
    private Sprite[] _itemSprites;
    private int[] _selectItemNum = new int[3];

    private int _selectCnt = 0;

    private int _haveItemCnt = 0;

    private void Start()
    {
        _manager = PlayerData.CreateAvatar.GetComponent<BuffManager>();
    }

    private void Update()
    {
        HaveItemCnt();
        UseItemselect();
        SetImageData();
        // アイテムを使う
        if (_haveItemCnt < 1)
            return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("アイテム使う");
            _manager.UseItem(_selectItemNum[0], this);
            _selectCnt = (_selectCnt - 1 + _haveItemCnt) % _haveItemCnt;
        }
    }

    /// <summary>
    /// 使うアイテムを選ぶ
    /// </summary>
    private void UseItemselect()
    {
        if (_haveItemCnt < 2)
            return;
        _haveItemCnt = _manager.HaveItemList.Count;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectCnt = (_selectCnt - 1 + _haveItemCnt) % _haveItemCnt;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectCnt = (_selectCnt + 1) % _haveItemCnt;
        }
    }

    /// <summary>
    /// 持っている補助が少ないとき(2個未満)
    /// </summary>
    private void HaveItemCnt()
    {

        if (_manager.HaveItemList == null || _manager.HaveItemList.Count < 1)
        {
            for (int i = 0; i < _setImage.Length; i++)
            {
                if (_setImage[i] != null)
                {
                    _setImage[i].enabled = false;
                }
            }
        }
        else if (_manager.HaveItemList.Count < 2)
        {
            if (_setImage[0] != null)
            {
                _setImage[0].enabled = true;
            }

            for (int i = 1; i < _setImage.Length; i++)
            {
                if (_setImage[i] != null)
                {
                    _setImage[i].enabled = false;
                }
            }
        }
        else if(_manager.HaveItemList.Count < 3)
        {
            if (_setImage[2] != null)
            {
                _setImage[0].enabled = false;
            }

            for (int i = 0; i < _setImage.Length - 1; i++)
            {
                if (_setImage[i] != null)
                {
                    _setImage[i].enabled = transform;
                }
            }
        }
        else
        {
            for (int i = 0; i < _setImage.Length; i++)
            {
                if (_setImage[i] != null)
                {
                    _setImage[i].enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Imageの画像を変える
    /// </summary>
    private void SetImageData()
    {
        _haveItemCnt = _manager.HaveItemList.Count;
        if (_haveItemCnt < 1)
            return;
        int[] cnt = { 0, 0, 0 };
        cnt[0] = _selectCnt;
        cnt[1] = (_selectCnt + 1) % _haveItemCnt;
        cnt[2] = (_selectCnt - 1 + _haveItemCnt) % _haveItemCnt;

        if (_haveItemCnt < 2)
        {
            cnt[0] = 0;
            _selectItemNum[0] = _manager.HaveItemList[cnt[0]];
        }
        else if(_haveItemCnt < 3)
        {
            _selectItemNum[0] = _manager.HaveItemList[cnt[0]];
            _selectItemNum[1] = _manager.HaveItemList[cnt[1]];
        }
        else
        {
            _selectItemNum[0] = _manager.HaveItemList[cnt[0]];
            _selectItemNum[1] = _manager.HaveItemList[cnt[1]];
            _selectItemNum[2] = _manager.HaveItemList[cnt[2]];
        }

        for (int i = 0; i < _setImage.Length; i++)
        {
            _setImage[i].sprite = _sprites[_selectItemNum[i]];
        }
    }

    /// <summary>
    /// スプライト変更
    /// </summary>
    /// <param name="num"></param>
    public void NextItemSetImageData(int num)
    {
        //Debug.Log($"{num}:スプライトを変える");
        _sprites[1] = _itemSprites[num];
    }

    /// <summary>
    /// 表示終わり
    /// </summary>
    public void NextItemSetImageDataEnd()
    {
        _sprites[1] = _itemSprites[_itemSprites.Length - 1];
    }
}
