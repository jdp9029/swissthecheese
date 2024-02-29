using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [HideInInspector] private float OriginalCircleWidth;
    [HideInInspector] private float prefabCircleWidth;
    [HideInInspector] private ColorManager ColorManager;
    [HideInInspector] private int targetScore;
    [HideInInspector] public bool justZoomed;
    [HideInInspector] public int ZoomFrameLength;
    [HideInInspector] public int ZoomCircleSpeed;


    // Start is called before the first frame update
    void Start()
    {
        OriginalCircleScale = OuterCircle.transform.localScale;
        OriginalCircleWidth = OuterCircle.GetComponent<RectTransform>().rect.width;
        prefabCircleWidth = circlePrefab.GetComponent<RectTransform>().rect.width;
        ColorManager = GameObject.FindObjectOfType<ColorManager>();
        justZoomed = true;
        targetScore = 15;
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
            OuterCircle.transform.localScale = new Vector3(DecreaseSpeed(OuterCircle.transform.localScale.x, speed), DecreaseSpeed(OuterCircle.transform.localScale.y, speed), OuterCircle.transform.localScale.z);
            circleInstance.transform.localScale = new Vector3(DecreaseSpeed(circleInstance.transform.localScale.x, speed), DecreaseSpeed(circleInstance.transform.localScale.y, speed), circleInstance.transform.localScale.z);

            /*OuterCircle.transform.localScale = new Vector3(OuterCircle.transform.localScale.x - ZoomCircleSpeed, OuterCircle.transform.localScale.y - ZoomCircleSpeed, OuterCircle.transform.localScale.z);
            circleInstance.transform.localScale = new Vector3(circleInstance.transform.localScale.x - speed, circleInstance.transform.localScale.y - speed, circleInstance.transform.localScale.z);*/

            //Don't let the outer circle get smaller than zero
            if (OuterCircle.transform.localScale.x <=0)
            {
                OuterCircle.transform.localScale = Vector3.zero;
            }

            //slowly turn the outer color to the appropriate color
            ColorManager.SlowBurn(OuterCircle.GetComponent<Image>().color,Background.GetComponent<Image>().color, OuterCircle);

            //if we've zoomed far enough, end the zoom
            if(circleInstance.transform.localScale.x <= (OriginalCircleScale.x * OriginalCircleWidth / prefabCircleWidth))
            {
                EndZoom();
            }
        }

        //if we are not zooming out
        else
        {
            if(int.Parse(FindObjectOfType<HoleManager>().scoreCounter.text) % targetScore == 0 && !justZoomed)
            {
                justZoomed = true;
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
        circleInstance.transform.localScale = OuterCircle.transform.localScale * OuterCircle.GetComponent<RectTransform>().rect.width / 25;
        circleInstance.transform.position = OuterCircle.transform.position;
        circleInstance.GetComponent<Image>().color = Background.GetComponent<Image>().color;

        //cycle the background
        GameObject.FindObjectOfType<ColorManager>().CycleBackground();

        //make the spinning object deactivated
        OuterCircle.transform.GetChild(1).gameObject.SetActive(false);

        //make the center transparent
        ColorManager.CircleCenter.color = new Color32((byte)ColorManager.CircleCenter.color.r, (byte)ColorManager.CircleCenter.color.g, (byte)ColorManager.CircleCenter.color.b, 0);

        //set zoom frame length
        ZoomFrameLength = ZoomFrameCount(circleInstance.transform.localScale.x, OriginalCircleScale.x * OriginalCircleWidth / prefabCircleWidth, Time.deltaTime * ZoomSpeed);

        //set zoom circle speed
        ZoomCircleSpeed = ApplyZoomFrameCount((OriginalCircleScale.x * OriginalCircleWidth / prefabCircleWidth),
            ColorManager.CircleCenter.transform.localScale.x * ColorManager.CircleCenter.GetComponent<RectTransform>().rect.width / prefabCircleWidth,
            ZoomFrameLength);
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
        GameObject.FindObjectOfType<HoleManager>().holesCut.Clear();

        //destroy any former holes that were cut
        for (int i = 2; i < OuterCircle.transform.childCount; i++)
        {
            GameObject.Destroy(OuterCircle.transform.GetChild(i).gameObject);
        }
    }

    private float DecreaseSpeed(float value, float speed)
    {
        return value - (speed * value);
    }

    //Get the number of frames it will take to zoom
    private int ZoomFrameCount(float startScale, float targetScale, float time)
    {
        return (int)((startScale - time) / targetScale);
    }

    //Get the amount we should decrease per frame
    private int ApplyZoomFrameCount(float startScale, float targetScale, float timeAllotted)
    {
        return (int) ((startScale - targetScale) / timeAllotted);
    }
}
