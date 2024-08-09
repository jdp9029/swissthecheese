using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrePlayInstructions : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    // Start is called before the first frame update
    void Start()
    {
        //set up the play button and options button
        playButton.onClick.AddListener(delegate
        {
            SceneManager.LoadScene("UpdatedGameplay");
        });

        switch (HardModeManager.Mode)
        {
            case HardModeManager.Modes.HARD:
                textMeshProUGUI.text = "Fill up the cheese with bites until no more bites can be made to advance to the next cheese.";
                break;
            case HardModeManager.Modes.NORMAL:
                textMeshProUGUI.text = "Fill up the cheese with 12 bites to advance to the next cheese.";
                break;
            case HardModeManager.Modes.TWICEMICE:
                textMeshProUGUI.text = "Two mice will be going at different speeds but bite at the same time. Fill up the cheese with 8 bites to advance to the next cheese.";
                break;
        }
    }
}
