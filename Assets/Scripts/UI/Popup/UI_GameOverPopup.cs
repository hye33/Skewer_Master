using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GameOverPopup : UI_Popup
{
    enum Buttons
    {
        AdsButton,
        NoThanksButton,
        HomeButton
    }

    enum Texts
    {
        ContinueText,
        ScoreText,
        BestScoreText
    }

    public Action ContinueAction;
    public Action EndGameAction;
    private UI_GamePopup gameUI;

    private TMP_Text continueText;
    private TMP_Text scoreText;
    private TMP_Text bestScoreText;

    private void Start()
    {
        Bind();

        if (gameUI.currentScore > Managers.Game.BestScore)
        {
            Managers.Game.BestScore = gameUI.currentScore;
        }

        scoreText.text = gameUI.currentScore.ToString();
        bestScoreText.text = "Best: " + Managers.Game.BestScore.ToString();

        // 광고 찬스 소진
        if (gameUI.adsCount > 0)
        {
            GetButton((int)Buttons.HomeButton).gameObject.SetActive(true);
            GetButton((int)Buttons.AdsButton).gameObject.SetActive(false);
            GetButton((int)Buttons.NoThanksButton).gameObject.SetActive(false);
            continueText.gameObject.SetActive(false);
        }
        // 광고 찬스 가능
        else
        {
            GetButton((int)Buttons.HomeButton).gameObject.SetActive(false);
            GetButton((int)Buttons.AdsButton).gameObject.SetActive(true);
            GetButton((int)Buttons.NoThanksButton).gameObject.SetActive(true);
            continueText.gameObject.SetActive(true);
        }

        gameUI.adsCount++;
    }

    private void Bind()
    {
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        GetButton((int)Buttons.AdsButton).gameObject.BindEvent(ShowAds);
        GetButton((int)Buttons.NoThanksButton).gameObject.BindEvent(EndGame);
        GetButton((int)Buttons.HomeButton).gameObject.BindEvent(EndGame);

        continueText = GetText((int)Texts.ContinueText);
        scoreText = GetText((int)Texts.ScoreText);
        bestScoreText = GetText((int)Texts.BestScoreText);

        gameUI = transform.parent.GetComponentInChildren<UI_GamePopup>();
    }

    private void EndGame(PointerEventData data)
    {
        EndGameAction?.Invoke();
        Managers.UI.CloseAllPopupUI();
        Managers.UI.ShowPopupUI<UI_MainPopup>();
    }

    private void ShowAds(PointerEventData data)
    {
        // TODO: 광고 송출
        Debug.Log("Show Ads");
        ContinueAction?.Invoke();
        ClosePopupUI();
    }

}
