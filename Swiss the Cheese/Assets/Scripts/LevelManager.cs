using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public bool ChosenCondition;

    [HideInInspector] private int chosenConditionNumber;
    [SerializeField] GameObject bigCircle;
    [SerializeField] GameObject voronoiPrefab;

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
        if (chosenConditionNumber == 1)
        {
            ChosenCondition = FindObjectOfType<HoleManager>().holesCut.Count % FindObjectOfType<ZoomManager>().targetScore == 0;
        }
        //fake condition for testing, occurs if we are choosing target score and subtracting 3
        else if (chosenConditionNumber == 2)
        {
            ChosenCondition = FindObjectOfType<HoleManager>().holesCut.Count % (FindObjectOfType<ZoomManager>().targetScore - 3) == 0;
        }
    }

    public void SetCondition()
    {
        int totalConditions = 2;

        chosenConditionNumber = Random.Range(1, totalConditions + 1);
    }

    private bool Voronoi()
    {
        //does there exist a point
        //that is > two radius's length away from every other circle
        //and > one radius length away from the nearest edge of the big circle and the center
        


        return false;
    }
}
