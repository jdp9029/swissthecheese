using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreKeeper : MonoBehaviour
{
    string playerPref;

    private void Start()
    {
        switch (HardModeManager.Mode)
        {
            case HardModeManager.Modes.NORMAL:
                playerPref = "highscore";
                break;
            case HardModeManager.Modes.HARD:
                playerPref = "hard_highscore";
                break;
            case HardModeManager.Modes.TWICEMICE:
                playerPref = "twicemice_highscore";
                break;
            default:
                throw new NotImplementedException();
        }

        bool highscoreExists = PlayerPrefs.HasKey(playerPref);

        if(highscoreExists)
        {
            LoadHighScore();
        }
        else
        {
            SetHighScore(0);
        }
    }

    public void SetHighScore(int highscore)
    {
        PlayerPrefs.SetInt(playerPref, highscore);
        LoadHighScore();
    }

    void LoadHighScore()
    {
        int highscore = PlayerPrefs.GetInt(playerPref);

        GetComponent<TextMeshProUGUI>().text = highscore.ToString();
    }
}
