using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubItem_ScorePanel : UI_Base
{
    enum Texts
    {
        ScoreText,
        ComboText
    }

    public int score = 0;
    public int combo = 0;
    TMP_Text scoreText;
    TMP_Text comboText;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        BindText(typeof(Texts));
        scoreText = GetText((int)Texts.ScoreText);
        comboText = GetText((int)Texts.ComboText);
        comboText.gameObject.SetActive(false);
    }

    public void UpdateScore(bool isCombo = false)
    {
        if (isCombo)
        {
            if (combo == 0)
            {
                comboText.gameObject.SetActive(true);
            }
            combo++;
            comboText.text = $"COMBO {combo}";
        }
        else
        {
            if (combo != 0)
            {
                comboText.gameObject.SetActive(false);
                combo = 0;
            }
        }
        score++;
        scoreText.text = score.ToString();
    }
}
