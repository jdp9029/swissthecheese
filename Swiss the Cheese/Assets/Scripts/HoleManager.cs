using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Linq;

public class HoleManager : MonoBehaviour
{
    //holes being cut
    [SerializeField] GameObject circularPrefab;

    //the instance of the swinging circle
    [SerializeField] public GameObject mouseInstance;

    //center of the big circle
    [SerializeField] public GameObject centerOfCircle;

    //path object
    [SerializeField] GameObject pathObject;

    //closest point object
    [SerializeField] GameObject closestPointObject;

    //big circle
    [SerializeField] GameObject biggerCircle;

    //color manager
    [SerializeField] CheeseImageManager colorManager;

    //radius of the swinging circle
    [SerializeField] public float radius = 1.0f;

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
    [SerializeField] List<AudioClip> failClips;
    [HideInInspector] AudioSource failSource;
    [HideInInspector] int clipIndex;

    //keeps the high score
    [SerializeField] HighScoreKeeper highScoreKeeper;

    //the size of the center circle
    [HideInInspector] private Rect centerCircleSize;

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

        //adjust the local position of the holes cut
        for (int i = 0; i < holesCut.Count; i++)
        {
            RectTransform holeRect = holesCut[i].GetComponent<RectTransform>();
            RectTransform circleRect = biggerCircle.GetComponent<RectTransform>();
            AdjustHoleToAnchor(holeRect, circleRect);
        }

        //set the rotation speed of the mouse
        if (HardModeManager.HardMode)
        {
            rotationSpeed = 7f  + (.4f * int.Parse(scoreCounter.text));

            //set 18 as the max speed
            if(rotationSpeed >= 18f) { rotationSpeed = 18f; }
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
        }

        //iterate the angle
        angle += Time.deltaTime * rotationSpeed;
        ReturnAngleToZero();

        //move the circle around accordingly
        mouseInstance.GetComponent<RectTransform>().position = new Vector3(centerOfCircle.transform.position.x + (radius * Mathf.Cos(angle)), centerOfCircle.transform.position.y + (radius * Mathf.Sin(angle)), 0.0f);

        mouseInstance.GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, (angle * 180 / Mathf.PI) - 180);

        Vector2 pos = (Vector2)mouseInstance.GetComponent<RectTransform>().localPosition;

        closestPointObject.GetComponent<RectTransform>().localPosition = ClosestPathPoint(pathObject.GetComponent<PolygonCollider2D>().points, pos);

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

            //set up the counter
            scoreCounter.text = "0";

            //set up just zoomed
            GameObject.FindObjectOfType<ZoomManager>().justZoomed = true;

            //70% chance of the quiet fail clip, 30% chance of the loud
            clipIndex = Random.Range(0, 10) > 6 ? 0 : 1;

            AudioClip failClip = failClips[clipIndex];
            failSource = GameObject.FindObjectOfType<SoundManager>().PlaySoundFXClip(failClip, transform, .8f);
            StartCoroutine(DestroyFailClip());
        }

        //If it doesn't intersect, simply add it to the holes cut list
        else
        {
            //initialHolePositions.Add(newHole.transform.position - centerOfCircle.transform.position);

            RectTransform rectTransform = newHole.GetComponent<RectTransform>();
            RectTransform parentRect = biggerCircle.GetComponent<RectTransform>();
            AdjustAnchorToHole(rectTransform, parentRect);

            newHole.GetComponent<AspectRatioFitter>().enabled = true;

            holesCut.Add(newHole);

            //set up the counter
            scoreCounter.text = (int.Parse(scoreCounter.text) + 1).ToString();

            //set up just zoomed
            GameObject.FindObjectOfType<ZoomManager>().justZoomed = false;

            //set up high score
            string highScore = !HardModeManager.HardMode ? "highscore" : "hard_highscore";

            if (PlayerPrefs.GetInt(highScore) < int.Parse(scoreCounter.text))
            {
                highScoreKeeper.SetHighScore(int.Parse(scoreCounter.text));
            }
        }

        //move the mouse to the back
        mouseInstance.transform.SetAsLastSibling();
    }

    private static void AdjustAnchorToHole(RectTransform rectTransform, RectTransform parentRect)
    {
        Vector2 minCorner = rectTransform.anchorMin + (rectTransform.offsetMin / parentRect.rect.width);
        Vector2 maxCorner = rectTransform.anchorMax + (rectTransform.offsetMax / parentRect.rect.width);

        rectTransform.anchorMin = minCorner;
        rectTransform.anchorMax = maxCorner;

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private static void AdjustHoleToAnchor(RectTransform rectTransform, RectTransform parentRect)
    {
        Vector2 minCorner = parentRect.rect.height * (rectTransform.anchorMin - parentRect.pivot);
        Vector2 maxCorner = parentRect.rect.height * (rectTransform.anchorMax - parentRect.pivot);
        Vector2 position = (minCorner + maxCorner) / 2;

        rectTransform.localPosition = position;
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
        float length = clipIndex == 0 ? failSource.clip.length / 2 : failSource.clip.length;
        yield return new WaitForSeconds(length);

        if(failSource != null)
        {
            Destroy(failSource.gameObject);
        }
    }

    private Vector2 ClosestPathPoint(Vector2[] points, Vector2 mousePos)
    {
        float shortestDistance = float.MaxValue;
        Vector2 closestPoint = points.First();

        foreach (Vector2 point in points)
        {
            if (Vector2.Distance(point, mousePos) < shortestDistance)
            {
                shortestDistance = Vector2.Distance(point, mousePos);
                closestPoint = point;
            }
        }
        return closestPoint;
    }
}
