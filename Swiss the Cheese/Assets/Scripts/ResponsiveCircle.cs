using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResponsiveCircle : MonoBehaviour
{
    //the rect of the canvas
    RectTransform canvasRect;

    //some bool's determining what we want to do to this object
    [SerializeField] bool stationaryOnX;
    [SerializeField] bool updateAspectRatio;
    [SerializeField] bool updateCollider;
    [SerializeField] bool leadByHeight;
    [SerializeField] bool colliderHasShader;

    //the height and width of the canvas
    private float canvasHeight;
    private float canvasWidth;

    //the initial position of this object
    private Vector2 initialPosition;

    //the minimum height we'd like or the minimum width we'd like, depending on leadByHeight
    [SerializeField] private float minimumSizeOfChoice;

    private void Start()
    {
        canvasRect = FindObjectOfType<RectTransform>();
        initialPosition = GetComponent<RectTransform>().localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //update the canvas attributes
        canvasHeight = canvasRect.rect.height * canvasRect.localScale.y;
        canvasWidth = canvasRect.rect.width * canvasRect.localScale.x;

        //update the aspect ratio
        if(updateAspectRatio)
        {
            UpdateCircleAspect(GetComponent<RectTransform>());
        }

        //keep the x the same
        if(stationaryOnX)
        {
            GetComponent<RectTransform>().localPosition = new Vector2(initialPosition.x, GetComponent<RectTransform>().localPosition.y);
        }

        //update collider
        if(updateCollider)
        {
            GetComponent<CircleCollider2D>().radius = GetComponent<RectTransform>().rect.width / 2.005f;

            if(colliderHasShader)
            {
                GetComponent<CircleCollider2D>().radius = GetComponent<RectTransform>().rect.width / 2.75f;
            }
        }
    }

    void UpdateCircleAspect(RectTransform rectTransform)
    {
        //get the recommended width and height that we want
        float wantedWidth = canvasWidth * (rectTransform.anchorMax.x - rectTransform.anchorMin.x);
        float wantedHeight = canvasHeight * (rectTransform.anchorMax.y - rectTransform.anchorMin.y);

        //decide orientation based on whether height or width gets priority
        if(leadByHeight)
        {
            if (wantedWidth < rectTransform.rect.width && wantedHeight >= minimumSizeOfChoice)
            {
                rectTransform.gameObject.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
            }
            if (wantedHeight < rectTransform.rect.height)
            {
                rectTransform.gameObject.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            }
        }
        else
        {
            if (wantedHeight < rectTransform.rect.height && wantedWidth >= minimumSizeOfChoice)
            {
                rectTransform.gameObject.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            }
            if (wantedWidth < rectTransform.rect.width)
            {
                rectTransform.gameObject.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
            }
        }

        //set the aspect ratio to be one and zero the offset
        rectTransform.gameObject.GetComponent<AspectRatioFitter>().aspectRatio = 1;
        ZeroOffset(rectTransform);
    }    

    void ZeroOffset(RectTransform rect)
    {
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    } 
}
