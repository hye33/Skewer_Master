using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MainPopup : UI_Popup
{
    enum Buttons
    {
        StartButton,
        ShopButton
    }

    void Start()
    {
        BindButton(typeof(Buttons));
        BindEvent(GetButton((int)Buttons.StartButton).gameObject, StartGame);
        BindEvent(GetButton((int)Buttons.ShopButton).gameObject, LoadShop);
    }

    private void StartGame(PointerEventData data)
    {
        // 게임 시작
        Managers.UI.ClosePopupUI();
        Managers.UI.ShowPopupUI<UI_GamePopup>();
    }

    private void LoadShop(PointerEventData data)
    {
        // shop Scene으로 전환
        Managers.UI.ClosePopupUI();
        Managers.UI.ShowPopupUI<UI_ShopPopup>();
    }
}
