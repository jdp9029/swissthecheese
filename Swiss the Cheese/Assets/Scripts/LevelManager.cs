using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public bool ChosenCondition;

    [HideInInspector] private int chosenConditionNumber;
    [SerializeField] GameObject bigCircle;
    [SerializeField] GameObject centerCircle;
    [HideInInspector] private List<Vector2> testPoints;
    [SerializeField] TextMeshProUGUI chosenNumberText;
    [SerializeField] GameObject testPointPrefab;
    [HideInInspector] public List<GameObject> testPointsPrefabList;

    //FOR INTS, MIN INCLUSIVE AND MAX EXCLUSIVE

    // Start is called before the first frame update
    void Start()
    {
        ChosenCondition = false;
        SetCondition();

        testPoints = new List<Vector2>()
        {
            (Vector2)bigCircle.transform.position + (centerCircle.GetComponent<CircleCollider2D>().radius * 8f * new Vector2(1, 0).normalized),
            (Vector2)bigCircle.transform.position + (centerCircle.GetComponent<CircleCollider2D>().radius * 8f * new Vector2(1, 1).normalized),
            (Vector2)bigCircle.transform.position + (centerCircle.GetComponent<CircleCollider2D>().radius * 8f * new Vector2(0, 1).normalized),
            (Vector2)bigCircle.transform.position + (centerCircle.GetComponent<CircleCollider2D>().radius * 8f * new Vector2(-1, 1).normalized),
            (Vector2)bigCircle.transform.position + (centerCircle.GetComponent<CircleCollider2D>().radius * 8f * new Vector2(-1, 0).normalized),
            (Vector2)bigCircle.transform.position + (centerCircle.GetComponent<CircleCollider2D>().radius * 8f * new Vector2(-1, -1).normalized),
            (Vector2)bigCircle.transform.position + (centerCircle.GetComponent<CircleCollider2D>().radius * 8f * new Vector2(0, -1).normalized),
            (Vector2)bigCircle.transform.position + (centerCircle.GetComponent<CircleCollider2D>().radius * 8f * new Vector2(1, -1).normalized)
        };

        /*foreach(Vector2 point in testPoints)
        {
            GameObject obj = Instantiate(testPointPrefab, point, Quaternion.identity, bigCircle.transform);
            obj.GetComponent<Image>().color = Color.blue;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        //occurs if we are choosing the "target score" condition
        if (chosenConditionNumber == 1)
        {
            ChosenCondition = FindObjectOfType<HoleManager>().holesCut.Count % FindObjectOfType<ZoomManager>().targetScore == 0;
            chosenNumberText.text = "Target Score";
        }
        //fake condition for testing, occurs if we are choosing target score and subtracting 3
        else if (chosenConditionNumber == 2)
        {
            ChosenCondition = NoAvailableSlots(testPoints);
            chosenNumberText.text = "Full Slots";
        }
    }

    public void SetCondition()
    {
        int totalConditions = 2;

        chosenConditionNumber = Random.Range(1, totalConditions + 1);
    }

    private bool NoAvailableSlots(List<Vector2> testPoints, int numIterations = 0)
    {
        //Copy list so we don't accidentally edit testPoints
        List<Vector2> updatedPoints = new List<Vector2>();

        foreach (Vector2 point in testPoints)
        {
            updatedPoints.Add(point);
        }

        //TEMP BOOL
        bool returnFalse = false;

        //if there are less than eight current cuts, don't even bother
        if(GameObject.FindObjectOfType<HoleManager>().holesCut.Count < 3) { return false; }

        //if we've looped through five times, there are no available slots
        if(numIterations >= 8) { return true; }

        //set up the radius converted from ugui to gameobject space
        GameObject hole = GameObject.FindObjectOfType<HoleManager>().holesCut[0];
        RectTransform rect = hole.GetComponent<RectTransform>();
        CircleCollider2D collider = hole.GetComponent<CircleCollider2D>();
        Vector2 point1 = rect.TransformPoint(rect.rect.center + new Vector2(collider.radius, 0f));
        Vector2 point2 = rect.TransformPoint(rect.rect.center);


        float radius = 2 * Vector2.Distance(point1, point2);

        //for each of the existing test points
        for (int i = 0; i < updatedPoints.Count; i++)
        {
            //current test point
            Vector2 testPoint = updatedPoints[i];

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
                if (distance <= radius)
                {
                    //change the point accordingly and increment nearby circles
                    Vector2 pointChange = (((2 * circle.GetComponent<CircleCollider2D>().radius) - distance) / circle.GetComponent<CircleCollider2D>().radius) *
                        (updatedPoints[i] - (Vector2)circle.transform.position).normalized;
                    distanceTraveled += pointChange;
                    updatedPoints[i] += pointChange;
                    nearbyCircles++;
                }
            }

            //if there are no nearby circles, return false
            if(nearbyCircles == 0)
            {
                if(testPointsPrefabList.Count < updatedPoints.Count)
                {
                    testPointsPrefabList.Add(Instantiate(testPointPrefab, updatedPoints[i], Quaternion.identity, bigCircle.transform));
                    GameObject.FindGameObjectWithTag("Mouse").transform.SetAsLastSibling();
                }

                returnFalse = true;
                //return false;
            }

            //take care of edge case when distance traveled equals zero
            if(distanceTraveled == Vector2.zero)
            {
                Debug.Log("rare call that distance traveled is zero");
                updatedPoints[i] += new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
            }

            //make the object move away from the center
            if (Vector2.Distance(updatedPoints[i],bigCircle.transform.position) <= centerCircle.GetComponent<CircleCollider2D>().radius * 2)
            {
                //adjust position accordingly
                updatedPoints[i] = (Vector2)bigCircle.transform.position + centerCircle.GetComponent<CircleCollider2D>().radius * 2 * (updatedPoints[i] - (Vector2)bigCircle.transform.position).normalized;
            }

            //make the object move away from the perimeter
            if (Vector2.Distance(updatedPoints[i], bigCircle.transform.position) >= centerCircle.GetComponent<CircleCollider2D>().radius +
                bigCircle.GetComponent<CircleCollider2D>().radius)
            {
                updatedPoints[i] = (Vector2)bigCircle.transform.position + bigCircle.GetComponent<CircleCollider2D>().radius * 0.75f * (updatedPoints[i] - (Vector2)bigCircle.transform.position).normalized;
            }
        }

        if(returnFalse) { return false; }

        //if we've looped through all tested points and they've all been close, keep looking
        return NoAvailableSlots(updatedPoints, numIterations + 1);
    }
}
