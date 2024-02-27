using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomManager : MonoBehaviour
{
    [HideInInspector] public bool IsZooming = false;
    [SerializeField] private GameObject OuterCircle;
    [SerializeField] private GameObject Background;
    [SerializeField] private GameObject circlePrefab;
    [HideInInspector] private GameObject circleInstance;
    [HideInInspector] private float ZoomSpeed = 1;
    [HideInInspector] private Vector2 OriginalCircleScale = Vector2.one;


    // Start is called before the first frame update
    void Start()
    {
        OriginalCircleScale = OuterCircle.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //set zoom speed to be dependent on time.deltatime
        float speed = Time.deltaTime * ZoomSpeed;

        //if we are zooming out
        if(IsZooming)
        {
            //shrink the sizes of the circles
            OuterCircle.transform.localScale = new Vector3(OuterCircle.transform.localScale.x - speed, OuterCircle.transform.localScale.y - speed, OuterCircle.transform.localScale.z);
            circleInstance.transform.localScale = new Vector3(circleInstance.transform.localScale.x - speed, circleInstance.transform.localScale.y - speed, circleInstance.transform.localScale.z);
            
            if(OuterCircle.transform.localScale.x <=0)
            {
                OuterCircle.transform.localScale = Vector3.zero;
            }


            //if we've zoomed far enough, end the zoom
            if(circleInstance.transform.localScale.x <= OriginalCircleScale.x)
            {
                EndZoom();
            }
        }

        //if we are not zooming out
        else
        {
            if(int.Parse(FindObjectOfType<HoleManager>().scoreCounter.text) >= 10)
            {
                StartZoom();
            }
        }
    }

    //To start a zoom
    public void StartZoom()
    {
        //set isZooming to be true
        IsZooming = true;

        //create and populate the larger circle object
        circleInstance = Instantiate(circlePrefab, Background.transform);
        circleInstance.transform.SetAsFirstSibling();
        circleInstance.transform.localScale = 2 * OuterCircle.transform.localScale;
        circleInstance.transform.position = OuterCircle.transform.position;
        circleInstance.GetComponent<Image>().color = Background.GetComponent<Image>().color;

        //cycle the background
        GameObject.FindObjectOfType<ColorManager>().CycleBackground();

        //destroy any holes that were cut
        for (int i = 2; i < OuterCircle.transform.childCount; i++)
        {
            GameObject.Destroy(OuterCircle.transform.GetChild(i).gameObject);
        }

        //set the counter accordingly
        FindObjectOfType<HoleManager>().scoreCounter.text = "0";

        //make the spinning object deactivated
        OuterCircle.transform.GetChild(1).gameObject.SetActive(false);
    }

    //To end the zoom
    public void EndZoom()
    {
        //set zooming to be false
        IsZooming = false;

        //reset the big circle size
        OuterCircle.transform.localScale = OriginalCircleScale;

        //cycle the colors
        GameObject.FindObjectOfType<ColorManager>().CycleForeground();

        //destroy big circle
        GameObject.Destroy(circleInstance);

        //make the spinning object active
        OuterCircle.transform.GetChild(1).gameObject.SetActive(true);
        GameObject.FindObjectOfType<HoleManager>().cutterInstance = OuterCircle.transform.GetChild(1).gameObject;
        GameObject.FindObjectOfType<HoleManager>().cutterInstance.GetComponent<Image>().color = Color.white;
    }
}
