using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Linq;
using System;

public class HoleManager : MonoBehaviour
{
    //holes being cut
    [SerializeField] GameObject circularPrefab;

    //the instance of the swinging circle
    [SerializeField] public GameObject mouseInstance;

    [SerializeField] public GameObject mouseInstance2;

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
    [SerializeField] public float angle = 0;
    [SerializeField] public float angle2 = 0;

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

    [SerializeField]
    TextMeshProUGUI CoinsText;

    // Start is called before the first frame update
    void Start()
    {
        //find the size of the canvas
        centerCircleSize = centerOfCircle.GetComponent<RectTransform>().rect;

        //set up the circling circle
        mouseInstance = Instantiate(mousePrefab, Vector3.zero, Quaternion.identity, biggerCircle.transform);

        var msl = FindObjectOfType<MouseSkinLoader>();

        //make the color white, for now
        mouseInstance.GetComponent<Image>().color = Color.white;
        mouseInstance.GetComponent<Image>().sprite = msl.EquippedSkin.Sprite;
        mouseInstance.transform.GetChild(0).GetComponent<Image>().sprite = msl.EquippedTopAccessory.Sprite;
        mouseInstance.transform.GetChild(1).GetComponent<Image>().sprite = msl.EquippedBottomAccessory.Sprite;
        if (msl.EquippedTopAccessory == msl.Accessories[0])
        {
            mouseInstance.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        }
        else
        {
            mouseInstance.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        if (msl.EquippedBottomAccessory == msl.Accessories[1])
        {
            mouseInstance.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        }
        else
        {
            mouseInstance.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        if (HardModeManager.Mode == HardModeManager.Modes.TWICEMICE)
        {
            mouseInstance2 = Instantiate(mousePrefab, Vector3.zero, Quaternion.identity, biggerCircle.transform);
            //make the color white, for now
            mouseInstance2.GetComponent<Image>().color = Color.white;
            mouseInstance2.GetComponent<Image>().sprite = msl.EquippedSkin.Sprite;
            mouseInstance2.transform.GetChild(0).GetComponent<Image>().sprite = msl.EquippedTopAccessory.Sprite;
            mouseInstance2.transform.GetChild(1).GetComponent<Image>().sprite = msl.EquippedBottomAccessory.Sprite;
            if (msl.EquippedTopAccessory == msl.Accessories[0])
            {
                mouseInstance2.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
            }
            else
            {
                mouseInstance2.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }

            if (msl.EquippedBottomAccessory == msl.Accessories[1])
            {
                mouseInstance2.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
            }
            else
            {
                mouseInstance2.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }



        radius = 50;
    }

    // Update is called once per frame
    void Update()
    {
        CoinsText.text = GameObject.FindObjectOfType<MouseSkinLoader>().Coins.ToString();

        if(GameObject.FindObjectOfType<ZoomManager>().IsZooming || GameObject.FindObjectOfType<BitingManager>().IsBiting)
        {
            return;
        }

        //adjust the local position of the holes cut
        /*for (int i = 0; i < holesCut.Count; i++)
        {
            RectTransform holeRect = holesCut[i].GetComponent<RectTransform>();
            RectTransform circleRect = biggerCircle.GetComponent<RectTransform>();
            AdjustHoleToAnchor(holeRect, circleRect);
        }*/

        //set the rotation speed of the mouse
        if (HardModeManager.Mode == HardModeManager.Modes.HARD)
        {
            rotationSpeed = 4f  + (.4f * int.Parse(scoreCounter.text));

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
            //radius = 3 * GetRadius(centerOfCircle);

            //update the size of the circle
            centerCircleSize = centerOfCircle.GetComponent<RectTransform>().rect;
        }

        //iterate the angle
        angle += Time.deltaTime * rotationSpeed;
        angle2 += Time.deltaTime * (rotationSpeed / 2.5f);
        ReturnAngleToZero();

        //move the circle around accordingly
        Vector3 pos = centerOfCircle.GetComponent<RectTransform>().localPosition
            + new Vector3(3 * GetRadius(centerOfCircle) * Mathf.Cos(angle), 3 * GetRadius(centerOfCircle) * Mathf.Sin(angle), 0.0f);

        closestPointObject.GetComponent<RectTransform>().localPosition = ClosestPathPoint(pathObject.GetComponent<PolygonCollider2D>().points, pos);

        mouseInstance.GetComponent<RectTransform>().position = (Vector2)centerOfCircle.transform.position +
            ((radius / 100f) * Vector2.Distance(closestPointObject.transform.position, centerOfCircle.transform.position) * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));

        mouseInstance.GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, (angle * 180 / Mathf.PI) - 180);

        if (HardModeManager.Mode == HardModeManager.Modes.TWICEMICE)
        {
            //move the circle around accordingly
            pos = centerOfCircle.GetComponent<RectTransform>().localPosition
                + new Vector3(3 * GetRadius(centerOfCircle) * Mathf.Cos(angle2), 3 * GetRadius(centerOfCircle) * Mathf.Sin(angle2), 0.0f);

            closestPointObject.GetComponent<RectTransform>().localPosition = ClosestPathPoint(pathObject.GetComponent<PolygonCollider2D>().points, pos);

            mouseInstance2.GetComponent<RectTransform>().position = (Vector2)centerOfCircle.transform.position +
                ((radius / 100f) * Vector2.Distance(closestPointObject.transform.position, centerOfCircle.transform.position) * new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle2)));

            mouseInstance2.GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, (angle2 * 180 / Mathf.PI) - 180);
        }

        Physics.SyncTransforms();
    }

    //Loops the angle back to zero
    private void ReturnAngleToZero()
    {
        if(angle >= 2 * Mathf.PI)
        {
            angle -= 2 * Mathf.PI;
        }
        if (angle2 >= 2 * Mathf.PI)
        {
            angle2 -= 2 * Mathf.PI;
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
            if(Vector2.Distance(hole.transform.position, newHole.transform.position) <= GameObject.FindObjectOfType<LevelManager>().GetRadius(hole) * 2.1f)
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
            clipIndex = UnityEngine.Random.Range(0, 10) > 6 ? 0 : 1;

            AudioClip failClip = failClips[clipIndex];
            failSource = GameObject.FindObjectOfType<SoundManager>().PlaySoundFXClip(failClip, transform, .8f);
            StartCoroutine(DestroyFailClip());
        }

        //If it doesn't intersect, simply add it to the holes cut list
        else
        {
            //initialHolePositions.Add(newHole.transform.position - centerOfCircle.transform.position);

            /*RectTransform rectTransform = newHole.GetComponent<RectTransform>();
            RectTransform parentRect = biggerCircle.GetComponent<RectTransform>();
            AdjustAnchorToHole(rectTransform, parentRect);*/

            newHole.GetComponent<AspectRatioFitter>().enabled = true;

            holesCut.Add(newHole);

            //set up the counter
            scoreCounter.text = (int.Parse(scoreCounter.text) + 1).ToString();

            //set up just zoomed
            GameObject.FindObjectOfType<ZoomManager>().justZoomed = false;

            //set up high score
            string highScore;

            switch (HardModeManager.Mode)
            {
                case HardModeManager.Modes.NORMAL:
                    highScore = "highscore";
                    break;
                case HardModeManager.Modes.HARD:
                    highScore = "hard_highscore";
                    break;
                case HardModeManager.Modes.TWICEMICE:
                    highScore = "twicemice_highscore";
                    break;
                default:
                    throw new NotImplementedException();
            }

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
