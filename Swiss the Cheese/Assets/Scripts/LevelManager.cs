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

        Debug.Log(centerCircle.GetComponent<CircleCollider2D>().radius);
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
        //if there are less than eight current cuts, don't even bother
        if(GameObject.FindObjectOfType<HoleManager>().holesCut.Count < 8) { return false; }

        //if we've looped through five times, there are no available slots
        if(numIterations >= 5) { return true; }

        //for each of the existing test points
        for(int i = 0; i < testPoints.Count; i++)
        {
            //current test point
            Vector2 testPoint = testPoints[i];

            //number of circles within radius
            int nearbyCircles = 0;

            Vector2 distanceTraveled = Vector2.zero;

            //loop through all of the circles to see if they are too close
            foreach(GameObject circle in GameObject.FindObjectOfType<HoleManager>().holesCut)
            {
                //distance between the circle and this point
                float distance = Vector2.Distance(testPoint, circle.transform.position);
                

                //if we are neaby another circle
                if (distance <= circle.GetComponent<CircleCollider2D>().radius * 2)
                {
                    //change the point accordingly and increment nearby circles
                    Vector2 pointChange = (((2 * circle.GetComponent<CircleCollider2D>().radius) - distance) / circle.GetComponent<CircleCollider2D>().radius) *
                        (testPoints[i] - (Vector2)circle.transform.position).normalized;
                    distanceTraveled += pointChange;
                    testPoints[i] += pointChange;
                    nearbyCircles++;
                }
            }

            //if there are no nearby circles, return false
            if(nearbyCircles == 0)
            {
                foreach(GameObject circle in GameObject.FindObjectOfType<HoleManager>().holesCut)
                {
                    Debug.Log(Vector2.Distance(testPoints[i], circle.transform.position));
                }

                if(testPointsPrefabList.Count == 0)
                {
                    testPointsPrefabList.Add(Instantiate(testPointPrefab, testPoints[i], Quaternion.identity, bigCircle.transform));
                }

                return false;
            }

            //take care of edge case when distance traveled equals zero
            if(distanceTraveled == Vector2.zero)
            {
                Debug.Log("rare call that distance traveled is zero");
                testPoints[i] += new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
            }

            //make the object move away from the center
            if (Vector2.Distance(testPoints[i],bigCircle.transform.position) <= centerCircle.GetComponent<CircleCollider2D>().radius * 2)
            {
                //adjust position accordingly
                testPoints[i] = (Vector2)bigCircle.transform.position + centerCircle.GetComponent<CircleCollider2D>().radius * 2 * (testPoints[i] - (Vector2)bigCircle.transform.position).normalized;
            }

            //make the object move away from the perimeter
            if (Vector2.Distance(testPoints[i], bigCircle.transform.position) >= centerCircle.GetComponent<CircleCollider2D>().radius +
                bigCircle.GetComponent<CircleCollider2D>().radius)
            {
                testPoints[i] = (Vector2)bigCircle.transform.position + bigCircle.GetComponent<CircleCollider2D>().radius * 0.75f * (testPoints[i] - (Vector2)bigCircle.transform.position).normalized;
            }
        }

        //if we've looped through all tested points and they've all been close, keep looking
        return NoAvailableSlots(testPoints, numIterations + 1);
    }
}
