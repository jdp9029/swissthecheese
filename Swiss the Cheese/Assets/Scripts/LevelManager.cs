using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public bool ChosenCondition;

    [HideInInspector] private int chosenConditionNumber;

    //FOR INTS, MIN INCLUSIVE AND MAX EXCLUSIVE

    // Start is called before the first frame update
    void Start()
    {
        ChosenCondition = false;
        SetCondition();
    }

    // Update is called once per frame
    void Update()
    {
        //occurs if we are choosing the "target score" condition
        if(chosenConditionNumber == 1)
        {
            ChosenCondition = int.Parse(FindObjectOfType<HoleManager>().scoreCounter.text) % FindObjectOfType<ZoomManager>().targetScore == 0;
        }
    }

    public void SetCondition()
    {
        chosenConditionNumber = 1;
        //chosenConditionNumber = Random.Range(1, 4);
    }
}
