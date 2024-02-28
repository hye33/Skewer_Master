using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubItem_ToastMsg : UI_Base
{
    enum Texts
    {
        ToastText
    }

    public override void Init()
    {
        BindText(typeof(Texts));
    }

    // 0 : LackCoin
    // 1 : AlreadyBuy

    public void Show(int idx)
    {
        Init();
        if (idx == 0)
        {
            GetText((int)Texts.ToastText).text = "Lack Coins!";
        }
        else if (idx == 1)
        {
            GetText((int)Texts.ToastText).text = "Already Buy All Themes!";
        }
    }
}
