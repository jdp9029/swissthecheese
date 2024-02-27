using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoleManager : MonoBehaviour
{
    //circle swinging
    [SerializeField] GameObject circularPrefab;

    //the instance of the swinging circle
    [SerializeField] public GameObject cutterInstance;

    //center of the big circle
    [SerializeField] GameObject centerOfCircle;

    //big circle
    [SerializeField] GameObject biggerCircle;

    //color manager
    [SerializeField] ColorManager colorManager;

    //radius of the swinging circle
    [HideInInspector] public float radius = 1.0f;

    //angle of the swinging circle from the center
    [HideInInspector] public float angle = 0;

    //speed of the rotating circle
    private float rotationSpeed = 4f;

    //List of holes punched through the big circle
    [HideInInspector] public List<GameObject> holesCut = new List<GameObject>();

    //Counter for how many holes have been cut
    [SerializeField] public TextMeshProUGUI scoreCounter;


    // Start is called before the first frame update
    void Start()
    {
        //find the center of the canvas
        Rect centerOfBackground = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>().rect;

        //set up the circling circle
        cutterInstance = Instantiate(circularPrefab, Vector3.zero, Quaternion.identity, biggerCircle.transform);

        //make the color white, for now
        cutterInstance.GetComponent<Image>().color = Color.white;

        //set up the radius
        radius = biggerCircle.GetComponent<RectTransform>().rect.height - (3 * centerOfCircle.GetComponent<RectTransform>().rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindObjectOfType<ZoomManager>().IsZooming)
        {
            return;
        }

        //iterate the angle
        angle += Time.deltaTime * rotationSpeed;
        ReturnAngleToZero();

        //move the circle around accordingly
        cutterInstance.transform.position = new Vector3(centerOfCircle.transform.position.x + (radius * Mathf.Cos(angle)), centerOfCircle.transform.position.y + (radius * Mathf.Sin(angle)), 0.0f);
        Physics.SyncTransforms();
    }

    //Loops the angle back to zero
    private void ReturnAngleToZero()
    {
        if(angle >= 2 * Mathf.PI)
        {
            angle -= 2 * Mathf.PI;
        }
    }

    //Ensures a newly cut hole doesn't intersect against previously cut holes
    public void CheckIntersections(GameObject newHole)
    {
        //Whether or not it intersects
        bool foundIntersection = false;

        //For loop that checks the new hole against all old holes
        for (int i = 0; i < holesCut.Count; i++)
        {
            GameObject hole = holesCut[i];
            if (hole.GetComponent<CircleCollider2D>().bounds.Intersects(newHole.GetComponent<CircleCollider2D>().bounds))
            {
                foundIntersection = true;
                break;
            }
        }

        //If it intersects
        if (foundIntersection)
        {
            //Destroy all the objects
            foreach(GameObject obj in holesCut)
            {
                Destroy(obj);
            }
            Destroy(newHole);
            
            //Clear holes cut
            holesCut.Clear();
        }

        //If it doesn't intersect, simply add it to the holes cut list
        else
        {
            holesCut.Add(newHole);
        }

        //set up the counter
        scoreCounter.text = holesCut.Count.ToString();
    }
}
