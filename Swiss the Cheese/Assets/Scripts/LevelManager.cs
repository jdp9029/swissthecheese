using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public bool ChosenCondition;

    [HideInInspector] private int chosenConditionNumber;
    [SerializeField] GameObject centerCircle;
    [SerializeField] GameObject closestPathPoint;
    [HideInInspector] private List<Vector2> testPoints;

    //FOR INTS, MIN INCLUSIVE AND MAX EXCLUSIVE

    // Start is called before the first frame update
    void Start()
    {
        ChosenCondition = false;
        SetCondition();

        SetTestPoints();
    }

    // Update is called once per frame
    void Update()
    {
        //occurs if we are choosing the "target score" condition
        if (chosenConditionNumber == 1)
        {
            ChosenCondition = FindObjectOfType<HoleManager>().holesCut.Count % FindObjectOfType<ZoomManager>().targetScore == 0;
        }
        //occurs if we are choosing the "full slots" condition
        else if (chosenConditionNumber == 2)
        {
            ChosenCondition = NoAvailableSlots(testPoints);
            SetTestPoints();
        }
        else if (chosenConditionNumber == 3)
        {
            ChosenCondition = FindObjectOfType<HoleManager>().holesCut.Count % 6 == 0;
        }
    }

    public void SetCondition()
    {
        switch (HardModeManager.Mode)
        {
            case HardModeManager.Modes.HARD:
                chosenConditionNumber = 2;
                break;
            case HardModeManager.Modes.NORMAL:
                chosenConditionNumber = 1;
                break;
            case HardModeManager.Modes.TWICEMICE:
                chosenConditionNumber = 3;
                break;
        }
    }

    private void SetTestPoints()
    {
        RectTransform rectTransform = centerCircle.GetComponent<RectTransform>();

        testPoints = new List<Vector2>()
        {
            (Vector2)rectTransform.TransformPoint(rectTransform.rect.center) + (2 * GetRadius(centerCircle) * new Vector2(1, 0).normalized),
            (Vector2)rectTransform.TransformPoint(rectTransform.rect.center) + (2 * GetRadius(centerCircle) * new Vector2(1, 1).normalized),
            (Vector2)rectTransform.TransformPoint(rectTransform.rect.center) + (2 * GetRadius(centerCircle) * new Vector2(0, 1).normalized),
            (Vector2)rectTransform.TransformPoint(rectTransform.rect.center) + (2 * GetRadius(centerCircle) * new Vector2(-1, 1).normalized),
            (Vector2)rectTransform.TransformPoint(rectTransform.rect.center) + (2 * GetRadius(centerCircle) * new Vector2(-1, 0).normalized),
            (Vector2)rectTransform.TransformPoint(rectTransform.rect.center) + (2 * GetRadius(centerCircle) * new Vector2(-1, -1).normalized),
            (Vector2)rectTransform.TransformPoint(rectTransform.rect.center) + (2 * GetRadius(centerCircle) * new Vector2(0, -1).normalized),
            (Vector2)rectTransform.TransformPoint(rectTransform.rect.center) + (2 * GetRadius(centerCircle) * new Vector2(1, -1).normalized)
        };
    }

    public float GetRadius(GameObject circle)
    {
        Vector2 center = circle.GetComponent<RectTransform>().TransformPoint(circle.GetComponent<RectTransform>().rect.center);
        Vector2 edgePoint = circle.GetComponent<RectTransform>().TransformPoint(circle.GetComponent<RectTransform>().rect.center + 
            new Vector2(circle.GetComponent<CircleCollider2D>().radius, 0f));

        return Vector2.Distance(center, edgePoint);
    }

    private bool NoAvailableSlots(List<Vector2> testPoints, int numIterations = 0)
    {
        //if there are less than three current cuts, don't even bother
        if(GameObject.FindObjectOfType<HoleManager>().holesCut.Count < 3) { return false; }

        //if we've looped through eight times, there are no available slots
        if(numIterations >= 8) { return true; }

        //set up the radius converted from ugui to gameobject space
        float radius = GetRadius(GameObject.FindObjectOfType<HoleManager>().holesCut[0]);
        float minDistanceBetweenCircles = 2.25f * radius;

        //for each of the existing test points
        for (int i = 0; i < testPoints.Count; i++)
        {
            //current test point
            Vector2 testPoint = testPoints[i];

            //number of circles within radius
            int nearbyCircles = 0;

            Vector2 distanceTraveled = Vector2.zero;

            //loop through all of the circles to see if they are too close
            foreach(GameObject circle in GameObject.FindObjectOfType<HoleManager>().holesCut)
            {
                Vector2 circlePos = circle.GetComponent<RectTransform>().TransformPoint(circle.GetComponent<RectTransform>().rect.center);

                //distance between the circle and this point
                float distance = Vector2.Distance(testPoint, circlePos);

                //if we are neaby another circle
                if (distance <= minDistanceBetweenCircles)
                {
                    //change the point accordingly and increment nearby circles
                    Vector2 pointChange = 1.5f * (minDistanceBetweenCircles - distance) * (testPoints[i] - circlePos).normalized;
                    distanceTraveled += pointChange;
                    testPoints[i] += pointChange;
                    nearbyCircles++;
                }
            }

            //if there are no nearby circles, return false
            if(nearbyCircles == 0) { return false; }

            //take care of edge case when distance traveled equals zero
            else if(distanceTraveled == Vector2.zero)
            {
                testPoints[i] += new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized * minDistanceBetweenCircles;
            }


            //get the position and the radius of the big circle
            Vector2 bigCirclePos = centerCircle.GetComponent<RectTransform>().TransformPoint(centerCircle.GetComponent<RectTransform>().rect.center);
            //float bigCircleRadius = GetRadius(bigCircle);
            float bigCircleRadius = Vector2.Distance(closestPathPoint.GetComponent<RectTransform>().position, bigCirclePos);

            //make the object move away from the center
            /*if (Vector2.Distance(testPoints[i], bigCirclePos) <= GetRadius(centerCircle) * 1.5f)
            {
                //adjust position accordingly
                testPoints[i] = bigCirclePos + GetRadius(centerCircle) * 1.5f * (testPoints[i] - bigCirclePos).normalized;
            }*/

            //make the object move away from the perimeter
            if (Vector2.Distance(testPoints[i], bigCirclePos) >= bigCircleRadius - (minDistanceBetweenCircles / 2))
            {
                testPoints[i] = bigCirclePos + bigCircleRadius * 0.75f * (testPoints[i] - bigCirclePos).normalized;
            }
        }

        //if we've looped through all tested points and they've all been close, keep looking
        return NoAvailableSlots(testPoints, numIterations + 1);
    }
}
