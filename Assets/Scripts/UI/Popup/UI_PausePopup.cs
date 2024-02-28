using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_PausePopup : UI_Popup
{
    enum Buttons
    {
        ResumeButton,
        RestartButton,
        HomeButton
    }

    public Action ActDestoryBlocks;

    void Start()
    {
        Bind();
    }

    void Bind()
    {
        BindButton(typeof(Buttons));
        GetButton((int)Buttons.ResumeButton).gameObject.BindEvent(Resume);
        GetButton((int)Buttons.RestartButton).gameObject.BindEvent(Restart);
        GetButton((int)Buttons.HomeButton).gameObject.BindEvent(LoadHome);
    }

    private void Resume(PointerEventData data)
    {
        Managers.UI.ClosePopupUI();
    }

    private void Restart(PointerEventData data)
    {
        ActDestoryBlocks?.Invoke();
        Managers.UI.CloseAllPopupUI();
        Managers.UI.ShowPopupUI<UI_GamePopup>();
    }

    private void LoadHome(PointerEventData data)
    {
        ActDestoryBlocks?.Invoke();
        Managers.UI.CloseAllPopupUI();
        Managers.UI.ShowPopupUI<UI_MainPopup>();
    }
}
