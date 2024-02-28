using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubItem_CoinPanel : UI_Base
{
    TMP_Text coinText;
    bool isInit;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Managers.Game.CoinAction -= UpdateCoin;
        Managers.Game.CoinAction += UpdateCoin;
        coinText = Util.FindChild(gameObject, "CoinText").GetOrAddComponent<TMP_Text>();
        isInit = true;
        UpdateCoin();
    }

    public void UpdateCoin()
    {
        if (!isInit)
            Init();
        coinText.text = Managers.Game.Coin.ToString();
    }
}
