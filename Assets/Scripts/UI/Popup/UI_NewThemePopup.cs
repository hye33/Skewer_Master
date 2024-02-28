using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;
using static Define;

public class UI_NewThemePopup : UI_Popup
{
    Image themeImage;
    TMP_Text themeTitle;

    private void Bind()
    {
        Util.FindChild(gameObject, "CloseButton", true).BindEvent(ClosePopup);
        themeImage = Util.FindChild(gameObject, "ThemeImage", true).GetOrAddComponent<Image>();
        themeTitle = Util.FindChild(gameObject, "ThemeTitle", true).GetOrAddComponent<TMP_Text>();
    }

    public void showPopup(int idx)
    {
        Bind();
        string name = Enum.GetName(typeof(ThemeName), idx);
        themeImage.sprite = Managers.Resource.Load<Sprite>($"Images/Themes/{name}");
        themeTitle.text = name.ToUpper();
    }

    private void ClosePopup(PointerEventData data)
    {
        ClosePopupUI();
    }
}


