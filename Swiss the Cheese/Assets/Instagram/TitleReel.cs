using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleReel : MonoBehaviour
{
    private float timePerAngle = 0;
    public float angle;
    private float speed = 20f;
    private bool beenInViewRecently = false;
    [SerializeField] GameObject holePrefab;
    [SerializeField] GameObject soundManager;
    [SerializeField] BitingManager bitingManager;
    [SerializeField] HardModeManager hardModeManager;
    [SerializeField] RectTransform title;
    private List<GameObject> bitesMade;

    bool beenOnScreen;

    // Start is called before the first frame update
    void Start()
    {
        //set up the necessary variables
        angle = 0f;
        speed = 1f;
        timePerAngle = 0;
        beenInViewRecently = false;
        beenOnScreen = false;
        bitesMade = new List<GameObject>();

        //set up the sound and hard mode managers
        if (GameObject.FindObjectsOfType<SoundManager>().Length == 0)
        {
            Instantiate(soundManager);
        }
        if (GameObject.FindObjectsOfType<HardModeManager>().Length == 0)
        {
            Instantiate(hardModeManager);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if we are making a bite, don't move the mouse
        if (bitingManager.IsBiting) { return; }

        //set the new position based on the current angle
        transform.position = new Vector3(transform.position.x + (speed * Mathf.Cos(angle - (Mathf.PI / 2))), transform.position.y + (speed * Mathf.Sin(angle - (Mathf.PI / 2))), 0.0f);

        speed = 300 * Time.deltaTime;

        if(bitesMade.Count == 0)
        {
            if(OverlapsTitle())
            {
                beenOnScreen = true;
            }

            if(!beenOnScreen)
            {
                angle = 0;

                //update rotation accordingly
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle * 180 / Mathf.PI);

                return;
            }
        }
        if(bitesMade.Count >= 15)
        {
            angle = 2 * Mathf.PI;

            //update rotation accordingly
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle * 180 / Mathf.PI);
            return;
        }

        //increment time per angle
        timePerAngle += Time.deltaTime;

        //reset time per angle every half second
        if (timePerAngle >= .5f)
        {
            //change the angle if we're in view
            if (beenInViewRecently)
            {
                angle = Random.Range(angle - (Mathf.PI / 4), angle + (Mathf.PI / 4));
            }
            timePerAngle = 0;
        }

        //update rotation accordingly
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle * 180 / Mathf.PI);

        //handle leaving the view
        RectTransform canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();

        if (transform.position.x < canvas.TransformPoint(canvas.rect.center).x * .05f ||
            transform.position.x > canvas.TransformPoint(canvas.rect.center).x * 1.95f ||
            transform.position.y < canvas.TransformPoint(canvas.rect.center).y * .05f ||
            transform.position.y > canvas.TransformPoint(canvas.rect.center).y * 1.95f)
        {
            beenInViewRecently = false;
            angle -= Mathf.PI;
        }
        else
        {
            beenInViewRecently = true;
        }

        //randomly make bites
        if (Random.Range(0, 350) == 1 && !OverlapsCurrentBites() && !OverlapsTitle())
        {
            GameObject hole = Instantiate(holePrefab, transform.position, Quaternion.identity, transform.parent);
            hole.GetComponent<Image>().color = new Color32(198, 197, 84, 255);
            hole.transform.SetAsFirstSibling();
            bitesMade.Add(hole);
            bitingManager.StartBite(hole);

            hole.transform.parent = hole.transform.parent.parent;
            hole.transform.SetAsFirstSibling();
        }
    }

    private bool OverlapsTitle()
    {
        bool insideLeft = title.rect.width * title.localScale.x * -.5f < GetComponent<RectTransform>().localPosition.x + 10;// + (.5f * title.rect.width);
        bool insideRight = title.rect.width * title.localScale.x / 2 > GetComponent<RectTransform>().localPosition.x - 10;// + (.5f * title.rect.width);
        bool insideBottom = title.rect.height * title.localScale.y * -.5f < GetComponent<RectTransform>().localPosition.y + 10;// + (.5f * title.rect.height);
        bool insideTop = title.rect.height * title.localScale.y / 2 > GetComponent<RectTransform>().localPosition.y - 10;// + (.5f * title.rect.height);

        Debug.Log(insideLeft + "," + insideRight + "," + insideBottom + "," + insideTop);

        return insideLeft && insideRight && insideBottom && insideTop;
    }

    //checks if a new bite that we are making overlaps a current bite
    private bool OverlapsCurrentBites()
    {
        foreach (GameObject bite in bitesMade)
        {
            if (Vector2.Distance(transform.position, bite.transform.position) <= GetRadius(bite) * 2.5f)
            {
                return true;
            }
        }
        return false;
    }

    //Get the radius of an object in transform.position form
    private float GetRadius(GameObject circle)
    {
        Vector2 center = circle.GetComponent<RectTransform>().TransformPoint(circle.GetComponent<RectTransform>().rect.center);
        Vector2 edgePoint = circle.GetComponent<RectTransform>().TransformPoint(circle.GetComponent<RectTransform>().rect.center +
            new Vector2(circle.GetComponent<CircleCollider2D>().radius, 0f));
        return Vector2.Distance(center, edgePoint);
    }
}
