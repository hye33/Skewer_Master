using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_ShopPopup : UI_Popup
{
    enum GameObjects
    {
        Content, // 스크롤 뷰 내 content
        CoinPanel,
        SubRoot
    }

    enum Buttons
    {
        HomeButton,
        BuyButton
    }

    enum Texts
    {
        CoinText
    }

    List<GameObject> btnList = new List<GameObject>();
    SubItem_CoinPanel coinPanel;
    Transform subRoot;

    void Start()
    {
        Bind();
        AddButtons();
        CheckAllBtnState();
    }

    private void Bind()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));
        GetButton((int)Buttons.HomeButton).gameObject.BindEvent(LoadHome);
        GetButton((int)Buttons.BuyButton).gameObject.BindEvent(ClickBuyBtn);

        subRoot = GetObject((int)GameObjects.SubRoot).transform;
        coinPanel = GetObject((int)GameObjects.CoinPanel).GetOrAddComponent<SubItem_CoinPanel>();
    }


    private void AddButtons()
    {
        Transform btnRoot = GetObject((int)GameObjects.Content).transform;
        foreach (ThemeName theme in Enum.GetValues(typeof(ThemeName)))
        {
            if ((int)theme == (int)ThemeName.MaxCount) // MaxCount는 버튼 생성 X
                return;

            GameObject themeBtn = Managers.Resource.Instantiate(
                "UI/SubItem/ThemeButton", btnRoot);
            themeBtn.name = theme.ToString();
            themeBtn.GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>($"Images/Themes/{theme}");
            themeBtn.GetComponentInChildren<TMP_Text>().text = theme.ToString().ToUpper();
            themeBtn.BindEvent(ClickTheme);
            btnList.Add(themeBtn);
        }
    }

    void LoadHome(PointerEventData data)
    {
        Managers.UI.ClosePopupUI();
        Managers.UI.ShowPopupUI<UI_MainPopup>();
    }

    private void ClickBuyBtn(PointerEventData data)
    {
        int coin = Managers.Game.Coin;
        if (coin < BUY_THEME_COST)
        {
            StartCoroutine(ShowToast(0));
            return;
        }
        Managers.Game.SubtractCoin(50);
        GetRandomTheme();
    }

    private void GetRandomTheme()
    {
        int themeIdx = Util.GetRandomThemeIdx();

        // themeIdx == 0 : get all theme
        if (themeIdx == 0)
        {
            Managers.Game.AddCoin(50);
            StartCoroutine(ShowToast(1));
            return;
        }

        UI_NewThemePopup ui = Managers.UI.ShowPopupUI<UI_NewThemePopup>();
        ui.showPopup(themeIdx);

        UnlockTheme(themeIdx);
        SelectTheme(themeIdx);
    }


    #region ThemeButton
    private void CheckAllBtnState()
    {
        List<bool> hasTheme = Managers.Game.HasTheme;
        int selectTheme = Managers.Game.SelectTheme;
             
        for (int i = 0; i < hasTheme.Count; i++)
        {
            if (hasTheme[i])
                Util.FindChild(btnList[i], "Locked").SetActive(false);                

        }

        Util.FindChild(btnList[selectTheme], "Selected").SetActive(true);
    }

    private void UnlockTheme(int idx)
    {
        Managers.Game.HasTheme[idx] = true;
        Util.FindChild(btnList[idx], "Locked").SetActive(false);
    }

    private void SelectTheme(int idx)
    {
        UnselectTheme(Managers.Game.SelectTheme);
        Managers.Game.SelectTheme = idx;
        Util.FindChild(btnList[idx], "Selected").SetActive(true); 
    }

    private void UnselectTheme(int idx)
    {
        Util.FindChild(btnList[idx], "Selected").SetActive(false);
    }

    private void ClickTheme(PointerEventData data)
    {
        string btnName = data.pointerClick.name;
        int idx = (int)Enum.Parse(typeof(ThemeName), btnName);

        if (Managers.Game.HasTheme[idx] == false)
            return;
        
        SelectTheme(idx);
    }
    #endregion

    IEnumerator ShowToast(int func)
    {
        GameObject toast = Managers.Resource.Instantiate("UI/SubItem/ToastMsg", subRoot);
        if (func == 0)
            toast.GetComponent<SubItem_ToastMsg>().Show(0);
        else if (func == 1)
            toast.GetComponent<SubItem_ToastMsg>().Show(1);
        yield return new WaitForSeconds(1.0f);
        Managers.Resource.Destory(toast);
    }

}
