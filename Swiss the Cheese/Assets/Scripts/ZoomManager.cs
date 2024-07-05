using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ZoomManager : MonoBehaviour
{
    [HideInInspector] public bool IsZooming = false;
    [SerializeField] private GameObject OuterCircle;
    [SerializeField] private GameObject Background;
    [SerializeField] private GameObject circlePrefab;
    [HideInInspector] private GameObject circleInstance;
    [HideInInspector] private Vector2 targetCircleInstanceSize = Vector2.one;
    [HideInInspector] private CheeseImageManager ColorManager;
    [HideInInspector] public int targetScore;
    [HideInInspector] public bool justZoomed;
    [HideInInspector] private float OuterCircleSpeed;
    [HideInInspector] private float InnerCircleSpeed;

    [SerializeField] AudioClip zoomSound;
    [HideInInspector] AudioSource zoomInstance;


    // Start is called before the first frame update
    void Start()
    {
        ColorManager = GameObject.FindObjectOfType<CheeseImageManager>();
        justZoomed = true;
        targetScore = 14;
    }

    // Update is called once per frame
    void Update()
    {
        //if we are zooming out
        if(IsZooming)
        {
            //Decrease the scale of the outer circle and the inner circle
            OuterCircle.transform.localScale = DecreaseScale(InnerCircleSpeed, OuterCircle);
            circleInstance.transform.localScale = DecreaseScale(OuterCircleSpeed, circleInstance);

            //Don't let the outer circle get smaller than zero
            if (OuterCircle.transform.localScale.x <=0)
            {
                OuterCircle.transform.localScale = Vector3.zero;
            }

            //if we've zoomed far enough, end the zoom
            if(circleInstance.transform.localScale.x * circleInstance.GetComponent<RectTransform>().rect.width <= targetCircleInstanceSize.x)
            {
                EndZoom();
            }
        }

        //if we are not zooming out
        else
        {
            //if we need to zoom out, do so once we finish biting
            if(GameObject.FindObjectOfType<LevelManager>().ChosenCondition && !justZoomed && !GameObject.FindObjectOfType<BitingManager>().IsBiting)
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

        //set up the expected final size
        targetCircleInstanceSize = OuterCircle.transform.localScale * OuterCircle.GetComponent<RectTransform>().rect.width;

        //create and populate the larger circle object
        circleInstance = Instantiate(circlePrefab, Background.transform);
        circleInstance.transform.SetAsFirstSibling();
        circleInstance.transform.localScale = OuterCircle.transform.localScale * OuterCircle.GetComponent<RectTransform>().rect.width * 5 /
            circleInstance.GetComponent<RectTransform>().rect.width;
        circleInstance.transform.position = OuterCircle.transform.position;
        circleInstance.GetComponent<Image>().sprite = ColorManager.NextCheese();

        //make the spinning object deactivated
        OuterCircle.transform.GetChild(OuterCircle.transform.childCount - 1).gameObject.SetActive(false);

        //make the center transparent
        ColorManager.CircleCenter.color = new Color32((byte)ColorManager.CircleCenter.color.r, (byte)ColorManager.CircleCenter.color.g, (byte)ColorManager.CircleCenter.color.b, 0);

        //set the speed at which the two circles have to decrease so that they reach their endpoints at the same time
        InnerCircleSpeed = Speed(targetCircleInstanceSize.x, ColorManager.CircleCenter.GetComponent<RectTransform>().rect.width, TotalFrames(.3f,Time.deltaTime));
        OuterCircleSpeed = Speed(circleInstance.GetComponent<RectTransform>().rect.width * circleInstance.transform.localScale.x, targetCircleInstanceSize.x, TotalFrames(.3f,Time.deltaTime));

        //disable responsive box
        OuterCircle.GetComponent<ResponsiveBox2>().enabled = false;

        //play our sound effect
        zoomInstance = GameObject.FindObjectOfType<SoundManager>().PlaySoundFXClip(zoomSound, transform, 1, 0.5f);
    }

    //To end the zoom
    public void EndZoom()
    {
        //set zooming to be false
        IsZooming = false;

        //reset the big circle size
        OuterCircle.transform.localScale = Vector3.one;

        //disable responsive box
        OuterCircle.GetComponent<ResponsiveBox2>().enabled = true;

        //cycle the colors
        GameObject.FindObjectOfType<CheeseImageManager>().CycleCheese();

        //destroy big circle
        GameObject.Destroy(circleInstance);

        //make the spinning object active
        OuterCircle.transform.GetChild(OuterCircle.transform.childCount - 1).gameObject.SetActive(true);
        GameObject.FindObjectOfType<HoleManager>().mouseInstance = OuterCircle.transform.GetChild(OuterCircle.transform.childCount - 1).gameObject;
        GameObject.FindObjectOfType<HoleManager>().mouseInstance.GetComponent<Image>().color = Color.white;
        GameObject.FindObjectOfType<HoleManager>().holesCut.Clear();

        //destroy any former holes that were cut
        for (int i = 2; i < OuterCircle.transform.childCount - 1; i++)
        {
            GameObject.Destroy(OuterCircle.transform.GetChild(i).gameObject);
        }

        //find the new level type
        GameObject.FindObjectOfType<LevelManager>().SetCondition();

        //delete our sound effect
        Destroy(zoomInstance.gameObject);
    }

    //return the total number of frames that will occur over the duration of the program
    private float TotalFrames(float totalTime, float timePerFrame)
    {
        return totalTime / timePerFrame;
    }

    //return the size that is needed to decrease per frame
    private float Speed(float startSize, float targetSize, float numFrames)
    {
        return (startSize - targetSize) / numFrames;
    }

    //actually decrease the scale
    private Vector3 DecreaseScale(float value, GameObject scale)
    {
        float initialWidth = scale.GetComponent<RectTransform>().rect.width * scale.transform.localScale.x;
        float newWidth = initialWidth - value;
        return new Vector3(scale.transform.localScale.x * (newWidth/initialWidth), scale.transform.localScale.y * (newWidth/initialWidth),1);
    }
}
