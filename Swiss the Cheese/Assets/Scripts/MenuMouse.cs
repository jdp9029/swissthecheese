using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuMouse : MonoBehaviour
{
    private float timePerAngle = 0;
    public float angle;
    private float radius;
    private float speed = 50f;
    private bool beenInViewRecently = false;
    [SerializeField] Button playButton;
    [SerializeField] GameObject holePrefab;
    [SerializeField] BitingManager bitingManager;
    private List<GameObject> bitesMade;

    // Start is called before the first frame update
    void Start()
    {
        angle = 0f;
        radius = speed * Time.deltaTime;
        bitesMade = new List<GameObject>();
        playButton.onClick.AddListener(delegate
        {
            SceneManager.LoadScene("UpdatedGameplay");
        });
    }

    // Update is called once per frame
    void Update()
    {
        //if we are making a bite, don't move the mouse
        if (bitingManager.IsBiting) { return; }

        //set the new position based on the current angle
        transform.position = new Vector3(transform.position.x + (radius * Mathf.Cos(angle - (Mathf.PI / 2))), transform.position.y + (radius * Mathf.Sin(angle - (Mathf.PI / 2))), 0.0f);

        //increment time per angle
        timePerAngle += Time.deltaTime;

        //reset time per angle every half second
        if(timePerAngle >= .5f)
        {
            //change the angle if we're in view
            if(beenInViewRecently)
            {
                angle = Random.Range(angle - (Mathf.PI / 2), angle + (Mathf.PI / 2));
            }
            timePerAngle = 0;
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
            angle -= Mathf.PI;
        }
        else
        {
            beenInViewRecently = true;
        }

        //randomly make bites
        if(Random.Range(0,800) == 1 && !OverlapsCurrentBites())
        {
            GameObject hole = Instantiate(holePrefab, transform.position, Quaternion.identity, transform.parent);
            hole.GetComponent<Image>().color = new Color32(198, 197, 84, 255);
            hole.transform.SetAsFirstSibling();
            bitesMade.Add(hole);
            bitingManager.StartBite(hole);
        }
    }

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

    private float GetRadius(GameObject circle)
    {
        Vector2 center = circle.GetComponent<RectTransform>().TransformPoint(circle.GetComponent<RectTransform>().rect.center);
        Vector2 edgePoint = circle.GetComponent<RectTransform>().TransformPoint(circle.GetComponent<RectTransform>().rect.center +
            new Vector2(circle.GetComponent<CircleCollider2D>().radius, 0f));
        return Vector2.Distance(center, edgePoint);
    }
}
