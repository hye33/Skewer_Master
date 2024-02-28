using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_GamePopup : UI_Popup
{
    enum GameObjects
    {
        CoinPanel,
        TouchPanel,
        ScorePanel
    }

    enum Buttons
    {
        PauseButton
    }

    public int currentScore;
    private int coinStack = 0;

    public int adsCount = 0;

    private BlockController blockRoot;
    private static SubItem_CoinPanel coinPanel;
    private static SubItem_ScorePanel scorePanel;
    public Action TouchPanel;

    void Start()
    {
        Bind();
        Init();
        Setting();
    }

    private void Bind()
    {
        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetObject((int)GameObjects.TouchPanel).BindEvent(DropBlock);
        GetButton((int)Buttons.PauseButton).gameObject.BindEvent(ShowPausePopup);
        coinPanel = GetObject((int)GameObjects.CoinPanel).GetOrAddComponent<SubItem_CoinPanel>();
        scorePanel = GetObject((int)GameObjects.ScorePanel).GetOrAddComponent<SubItem_ScorePanel>();
    }

    public override void Init()
    {
        base.Init();

        Camera uiCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
        gameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        gameObject.GetComponent<Canvas>().worldCamera = uiCamera;
    }

    private void Setting()
    {
        blockRoot = Managers.Resource.Instantiate("Items/Block_Root").GetComponent<BlockController>();
    }

    private void ShowPausePopup(PointerEventData data)
    {
        UI_PausePopup ui = Managers.UI.ShowPopupUI<UI_PausePopup>();
        ui.ActDestoryBlocks -= EndGame;
        ui.ActDestoryBlocks += EndGame;
    }

    private void DropBlock(PointerEventData data)
    {
        TouchPanel.Invoke();
    }

    private void AddCoinStack(int combo = 0)
    {
        combo++;
        coinStack += combo;
        if (coinStack >= 20)
        {
            coinStack -= 20;
            Managers.Game.AddCoin(1);
        }
    }

    public void SetGoodBound()
    {
        scorePanel.UpdateScore(isCombo: false);
        AddCoinStack();
    }

    public void SetPerfectBound()
    {
        scorePanel.UpdateScore(isCombo: true);
        AddCoinStack(scorePanel.combo);
    }

    public void SetFail()
    {
        currentScore = scorePanel.score;
        scorePanel.gameObject.SetActive(false);

        UI_GameOverPopup ui = Managers.UI.ShowPopupUI<UI_GameOverPopup>();
        ui.ContinueAction -= ContinueGame;
        ui.ContinueAction += ContinueGame;
        ui.EndGameAction -= EndGame;
        ui.EndGameAction += EndGame;
    }

    private void ContinueGame()
    {
        scorePanel.gameObject.SetActive(true);
        blockRoot.MadeNextBlock();
    }

    private void EndGame()
    {
        Destroy(blockRoot.gameObject);
    }
}