using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreKeeper : MonoBehaviour
{
    private void Start()
    {
        bool highscoreExists = !HardModeManager.HardMode ? PlayerPrefs.HasKey("highscore") : PlayerPrefs.HasKey("hard_highscore");

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
        if (HardModeManager.HardMode)
        {
            PlayerPrefs.SetInt("hard_highscore", highscore);
        }
        else
        {
            PlayerPrefs.SetInt("highscore", highscore);
        }

        PlayerPrefs.Save();
        LoadHighScore();
    }

    void LoadHighScore()
    {
        int highscore = !HardModeManager.HardMode ? PlayerPrefs.GetInt("highscore") : PlayerPrefs.GetInt("hard_highscore");

        GetComponent<TextMeshProUGUI>().text = highscore.ToString();
    }
}
