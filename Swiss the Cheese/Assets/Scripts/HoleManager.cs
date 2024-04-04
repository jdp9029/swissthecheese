using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class HoleManager : MonoBehaviour
{
    //holes being cut
    [SerializeField] GameObject circularPrefab;

    //the instance of the swinging circle
    [SerializeField] public GameObject mouseInstance;

    //center of the big circle
    [SerializeField] public GameObject centerOfCircle;

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

    //the mouse prefab
    [SerializeField] public GameObject mousePrefab;

    //the clip and source for the audio fail sound effect
    [SerializeField] AudioClip failClip;
    [HideInInspector] AudioSource failSource;

    //keeps the high score
    [SerializeField] HighScoreKeeper highScoreKeeper;

    //the size of the center circle
    [HideInInspector] private Rect centerCircleSize;

    //the positions of each cut as you make them
    [HideInInspector] private List<(float radius, float angle)> initialHolePositions = new List<(float radius, float angle)>();


    // Start is called before the first frame update
    void Start()
    {
        //find the size of the canvas
        centerCircleSize = centerOfCircle.GetComponent<RectTransform>().rect;

        //set up the circling circle
        mouseInstance = Instantiate(mousePrefab, Vector3.zero, Quaternion.identity, biggerCircle.transform);

        //make the color white, for now
        mouseInstance.GetComponent<Image>().color = Color.white;

        //set up the radius
        radius = 3 * GetRadius(centerOfCircle);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindObjectOfType<ZoomManager>().IsZooming || GameObject.FindObjectOfType<BitingManager>().IsBiting)
        {
            return;
        }

        //set the rotation speed of the mouse
        if(HardModeManager.HardMode)
        {
            rotationSpeed = 4f  + (.4f * int.Parse(scoreCounter.text));

            //set 15 as the max speed
            if(rotationSpeed >= 15f) { rotationSpeed = 15f; }
        }
        else
        {
            rotationSpeed = 4f;
        }

        //if the center of the circle has changed sizes
        if(centerOfCircle.GetComponent<RectTransform>().rect.width != centerCircleSize.width || centerOfCircle.GetComponent<RectTransform>().rect.height != centerCircleSize.height)
        {
            //reset the radius
            radius = 3 * GetRadius(centerOfCircle);

            //update the size of the circle
            centerCircleSize = centerOfCircle.GetComponent<RectTransform>().rect;

            //if we are up to height controls width
            if(biggerCircle.GetComponent<AspectRatioFitter>().aspectMode == AspectRatioFitter.AspectMode.HeightControlsWidth)
            {
                //adjust the local position of the holes cut
                for(int i = 0; i < holesCut.Count; i++)
                {
                    holesCut[i].GetComponent<RectTransform>().localPosition = (radius / initialHolePositions[i].radius) * new Vector2(Mathf.Cos(initialHolePositions[i].angle), Mathf.Sin(initialHolePositions[i].angle));
                }
            }
        }

        //iterate the angle
        angle += Time.deltaTime * rotationSpeed;
        ReturnAngleToZero();

        //move the circle around accordingly
        mouseInstance.transform.position = new Vector3(centerOfCircle.transform.position.x + (radius * Mathf.Cos(angle)), centerOfCircle.transform.position.y + (radius * Mathf.Sin(angle)), 0.0f);

        mouseInstance.transform.rotation = Quaternion.Euler(0.0f, 0.0f, (angle * 180 / Mathf.PI) - 180);
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
            if(Vector2.Distance(hole.transform.position, newHole.transform.position) <= GameObject.FindObjectOfType<LevelManager>().GetRadius(hole) * 2)
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

            //Clear initial hole positions
            initialHolePositions.Clear();

            //set up the counter
            scoreCounter.text = "0";

            //set up just zoomed
            GameObject.FindObjectOfType<ZoomManager>().justZoomed = true;

            failSource = GameObject.FindObjectOfType<SoundManager>().PlaySoundFXClip(failClip, transform, .8f);
            StartCoroutine(DestroyFailClip());
        }

        //If it doesn't intersect, simply add it to the holes cut list
        else
        {
            holesCut.Add(newHole);
            initialHolePositions.Add((radius / GetRadius(biggerCircle), angle));
            
            //set up the counter
            scoreCounter.text = (int.Parse(scoreCounter.text) + 1).ToString();

            //set up just zoomed
            GameObject.FindObjectOfType<ZoomManager>().justZoomed = false;

            //set up high score
            string highScore = !HardModeManager.HardMode ? "highscore" : "hard_highscore";

            if(PlayerPrefs.GetInt(highScore) < int.Parse(scoreCounter.text))
            {
                highScoreKeeper.SetHighScore(int.Parse(scoreCounter.text));
            }
        }

        //move the mouse to the back
        mouseInstance.transform.SetAsLastSibling();
    }

    private float GetRadius(GameObject circle)
    {
        Vector2 center = circle.GetComponent<RectTransform>().TransformPoint(circle.GetComponent<RectTransform>().rect.center);
        Vector2 edgePoint = circle.GetComponent<RectTransform>().TransformPoint(circle.GetComponent<RectTransform>().rect.center +
            new Vector2(circle.GetComponent<CircleCollider2D>().radius, 0f));
        return Vector2.Distance(center, edgePoint);
    }

    private IEnumerator DestroyFailClip()
    {
        yield return new WaitForSeconds(failSource.clip.length);

        Destroy(failSource.gameObject);
    }
}
