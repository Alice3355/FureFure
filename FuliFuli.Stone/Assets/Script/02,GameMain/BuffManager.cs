using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using static GameConstantsManager;
/// <summary>
/// 所持している補助アイテムの使用や効果の管理等
/// </summary>
public class BuffManager : MonoBehaviour
{
    private int[] _buffItemCnt = new int[10];
    private MyNamespace.PlayerMove _getPlayerMove = null;
    private OffLine.PlayerMove _getOffLinePlayerMove = null;
    private GameMainManager _mainManager;
    private OffLine.HandleDelivery _getOffLineHandleDelivery;
    private HandleDelivery _getHandleDelivery;

    private Stopwatch _stopwatch;
    // 移動速度
    private float[] _useCanTime = {5.0f, 5.0f, 0.0f};
    private int _selectNum = 100;

    // 補助アイテムが使われている状態
    private bool _isBuffUseing = false;
    public List<int> HaveItemList { get; private set; }
    private ChooseBuffItem _getChooseBuffItem;

    [System.NonSerialized]
    public bool IsRainItem = false;

    private void Start()
    {
        if (IsSceneCheck(2))
        {
            _getPlayerMove = PlayerData.CreateAvatar.GetComponent<MyNamespace.PlayerMove>();
            _getPlayerMove.GetBuffManager = this;
        }
        if (IsSceneCheck(5))
        {
            _getOffLinePlayerMove = this.GetComponent<OffLine.PlayerMove>();
        }
        _stopwatch = new Stopwatch();
        HaveItemList = new List<int>();
    }

    private void Update()
    {
        // 強制終了
        if (Input.GetKey(KeyCode.P))
            SceneChange(0);

        if(IsSceneCheck(2) && _getPlayerMove == null)
            _getPlayerMove = PlayerData.CreateAvatar.GetComponent<MyNamespace.PlayerMove>();

        if (!IsSceneCheck(2) && !IsSceneCheck(5))
        {
            Reset();
            return;
        }
        if (_isBuffUseing)
            UseItemEnd();


    }

    /// <summary>
    /// アイテムの数を増やす
    /// </summary>
    /// <param name="selectNum"></param>
    public void GivSelectBuffItem(int selectNum)
    {
        _buffItemCnt[selectNum]++;
        // 持っていないアイテムなら
        if (!HaveItemList.Contains(selectNum))
        {
            HaveItemList.Add(selectNum);
        }
    }

    /// <summary>
    /// アイテムの使用する
    /// </summary>
    /// <param name="useNum">使うもの</param>
    /// <param name="choose"></param>
    public void UseItem(int useNum, ChooseBuffItem choose)
    {
        // 所持していれば
        if (_buffItemCnt[useNum] < 1 || _isBuffUseing)
            return;

        _getChooseBuffItem = choose;

        switch (useNum)
        {
            case 0: // スピードアップ
                if(_getPlayerMove != null)
                {
                    _getPlayerMove.SetSpeed[1] = PlayerSpeed[2];
                    _getPlayerMove.IsBuffSpeed = true;
                }
                if(_getOffLinePlayerMove != null)
                {
                    _getOffLinePlayerMove.SetSpeed[1] = 20.0f;
                }
                break;
            case 1: // 次の要求品が分かる
                if (IsSceneCheck(2))
                {
                    _mainManager = GameObject.Find("GameMainManager").GetComponent<GameMainManager>();
                    _getHandleDelivery = _mainManager.HandleDelivery;
                    if (_getHandleDelivery != null)
                    {
                        int num = _getHandleDelivery.NextItemDecision();
                        _getChooseBuffItem.NextItemSetImageData(num);
                    }
                }
                if (IsSceneCheck(5))
                {
                    _getOffLineHandleDelivery = GameObject.Find("DeliveryBox").GetComponent<OffLine.HandleDelivery>();
                    if (_getOffLineHandleDelivery != null)
                    {
                        int num = _getOffLineHandleDelivery.NextItemDecision();
                        _getChooseBuffItem.NextItemSetImageData(num);
                    }
                }
                break;
            case 2: // 目の前に降らす
                GameObject obj = GameObject.Find("ItemRain(Clone)");
                if (!obj)
                    return;
                if (IsSceneCheck(2))
                {
                    IsRainItem = true;
                }
                if (IsSceneCheck(5))
                {
                    OffLine.ItemRainSpawner itemRain = obj.GetComponent<OffLine.ItemRainSpawner>();
                    int rand = Random.Range(3, 9);
                    Vector3 pos = PlayerData.CreateAvatar.transform.position;
                    itemRain.BuffSpawnerItem(pos, rand);
                }
                break;
        }

        _buffItemCnt[useNum]--;
        _selectNum = useNum;
        _isBuffUseing = true;
        if (!_stopwatch.IsRunning)
            _stopwatch.Start();
    }

    /// <summary>
    /// アイテムの時間切れがきたとき
    /// </summary>
    private void UseItemEnd()
    {
        if (_stopwatch.IsRunning && _stopwatch.Elapsed.TotalSeconds > _useCanTime[_selectNum])
        {
            switch (_selectNum)
            {
                case 0:// スピードアップ
                    if(_getPlayerMove != null)
                    {
                        _getPlayerMove.SetSpeed[1] = _getPlayerMove.SetSpeed[0];
                        _getPlayerMove.IsBuffSpeed = false;
                    }
                    else
                        _getOffLinePlayerMove.SetSpeed[1] = _getOffLinePlayerMove.SetSpeed[0];
                    break;
                case 1:// 次の要求品が分かる
                    if (_getHandleDelivery != null)
                    {
                        _getChooseBuffItem.NextItemSetImageData(8);
                        _getChooseBuffItem.NextItemSetImageDataEnd();
                    }
                    break;
            }
            if (_buffItemCnt[_selectNum] < 1)
            {
                HaveItemList.Remove(_selectNum);
                _getChooseBuffItem.NextItemSetImageData(8);
            }

            _stopwatch.Reset();
            _selectNum = 100;
            _isBuffUseing = false;
        }
    }

    private void Reset()
    {
        for(int i = 0; i < _buffItemCnt.Length; i++)
        {
            _buffItemCnt[i] = 0;
        }
        _mainManager = null;
        _getHandleDelivery = null;
        // 移動速度
        _useCanTime = new float [3]{ 5.0f, 5.0f, 0.0f };
        _selectNum = 100;

        // 補助アイテムが使われている状態
        _isBuffUseing = false;
        HaveItemList = new List<int>();
        _getChooseBuffItem = null;

        if (IsSceneCheck(2))
            _getPlayerMove = PlayerData.CreateAvatar.GetComponent<MyNamespace.PlayerMove>();
        if (IsSceneCheck(5))
            _getOffLinePlayerMove = this.GetComponent<OffLine.PlayerMove>();

        _stopwatch = new Stopwatch();
    }
}
