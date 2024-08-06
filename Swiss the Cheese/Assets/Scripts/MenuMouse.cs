using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuMouse : MonoBehaviour
{
    private float timeSinceLastAngleChange = 0;
    public float angle;
    private float speed = 50f;
    private bool beenInViewRecently = false;
    [SerializeField] Button playButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button instructionsButton;
    [SerializeField] Button shopButton;
    [SerializeField] GameObject holePrefab;
    [SerializeField] GameObject soundManager;
    [SerializeField] BitingManager bitingManager;
    [SerializeField] HardModeManager hardModeManager;
    private List<GameObject> bitesMade;

    private Rect canvasRect;

    // Start is called before the first frame update
    void Start()
    {
        //set up the necessary variables
        angle = 0f;
        speed = 1f;
        timeSinceLastAngleChange = 0;
        beenInViewRecently = false;
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

        //set up the play button and options button
        playButton.onClick.AddListener(delegate
        {
            SceneManager.LoadScene("PlayScreen");
        });
        optionsButton.onClick.AddListener(delegate
        {
            SceneManager.LoadScene("Options");
        });
        instructionsButton.onClick.AddListener(delegate
        {
            SceneManager.LoadScene("Instructions");
        });
        shopButton.onClick.AddListener(delegate
        {
            SceneManager.LoadScene("Shop");
        });
    }

    // Update is called once per frame
    void Update()
    {
        //if we are making a bite, don't move the mouse
        if (bitingManager.IsBiting) { return; }

        //set the new position based on the current angle
        transform.position = new Vector3(transform.position.x + (speed * Mathf.Cos(angle - (Mathf.PI / 2))), transform.position.y + (speed * Mathf.Sin(angle - (Mathf.PI / 2))), 0.0f);

        speed = 300 * Time.deltaTime;

        //increment time per angle
        timeSinceLastAngleChange += Time.deltaTime;

        //reset time per angle every half second
        if(timeSinceLastAngleChange >= .5f)
        {
            //change the angle if we're in view
            if(beenInViewRecently)
            {
                angle = Random.Range(angle - (Mathf.PI / 2), angle + (Mathf.PI / 2));
            }
            timeSinceLastAngleChange = 0;
        }

        //update rotation accordingly
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle * 180 / Mathf.PI);
        
        //handle leaving the view
        RectTransform canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        
        if (transform.position.x < canvas.TransformPoint(canvas.rect.center).x * .05f ||
            transform.position.x > canvas.TransformPoint(canvas.rect.center).x * 2.05f ||
            transform.position.y < canvas.TransformPoint(canvas.rect.center).y * .05f ||
            transform.position.y > canvas.TransformPoint(canvas.rect.center).y * 2.05f)
        {
            beenInViewRecently = false;
            angle = SendAngleBackToCenter(canvas);
            timeSinceLastAngleChange = 0;
        }
        else
        {
            beenInViewRecently = true;
        }

        //randomly make bites
        if(Random.Range(0,50) == 1 && !OverlapsCurrentBites())
        {
            GameObject hole = Instantiate(holePrefab, transform.position, Quaternion.identity, transform.parent);
            hole.GetComponent<Image>().color = new Color32(198, 197, 84, 255);
            hole.transform.SetAsFirstSibling();
            bitesMade.Add(hole);
            bitingManager.StartBite(hole);
        }

        //reset mouse position if canvas size changes
        if(canvas.rect.width != canvasRect.width || canvas.rect.height != canvasRect.height)
        {
            GetComponent<RectTransform>().localPosition = Vector2.zero;
            canvasRect = canvas.rect;
        }
    }

    private float SendAngleBackToCenter(RectTransform canvas)
    {
        /*if (transform.position.x < canvas.TransformPoint(canvas.rect.center).x * .05f ||
            transform.position.x > canvas.TransformPoint(canvas.rect.center).x * 2.05f ||
            transform.position.y < canvas.TransformPoint(canvas.rect.center).y * .05f ||
            transform.position.y > canvas.TransformPoint(canvas.rect.center).y * 2.05f)*/

        //if we are on the left
        if (transform.position.x < canvas.TransformPoint(canvas.rect.center).x * .05f) { return Mathf.PI / 2; }

        //if we are on the right
        if(transform.position.x > canvas.TransformPoint(canvas.rect.center).x * 2.05f) { return 3 * Mathf.PI / 2; }

        //if we are on the bottom
        if(transform.position.y < canvas.TransformPoint(canvas.rect.center).y * .05f) { return Mathf.PI; }

        //if we are on the right
        return 0;
    }

    //checks if a new bite that we are making overlaps a current bite
    private bool OverlapsCurrentBites()
    {
        foreach(GameObject bite in bitesMade)
        {
            if(Vector2.Distance(transform.position, bite.transform.position) <= GetRadius(bite) * 2)
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
