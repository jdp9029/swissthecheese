using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResponsiveCircle : MonoBehaviour
{
    RectTransform canvasRect;
    [SerializeField] bool stationaryOnX;
    [SerializeField] bool updateAspectRatio;
    [SerializeField] bool updateCollider;

    private float canvasHeight;
    private float canvasWidth;

    private Vector2 initialPosition;

    private float optimalWidth = 450;

    private void Start()
    {
        canvasRect = FindObjectOfType<RectTransform>();
        initialPosition = GetComponent<RectTransform>().localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        canvasHeight = canvasRect.rect.height * canvasRect.localScale.y;
        canvasWidth = canvasRect.rect.width * canvasRect.localScale.x;

        if(updateAspectRatio)
        {
            UpdateCircleAspect(GetComponent<RectTransform>());
        }

        if(stationaryOnX)
        {
            GetComponent<RectTransform>().localPosition = new Vector2(initialPosition.x, GetComponent<RectTransform>().localPosition.y);
        }

        if(updateCollider)
        {
            GetComponent<CircleCollider2D>().radius = GetComponent<RectTransform>().rect.width / 2.005f;
        }
    }

    void UpdateCircleAspect(RectTransform rectTransform)
    {
        //float wantedWidth = canvasWidth * (rectTransform.anchorMax.x - rectTransform.anchorMin.x);
        //float wantedHeight = canvasHeight * (rectTransform.anchorMax.y - rectTransform.anchorMin.y);

        float wantedWidth = canvasWidth * (rectTransform.anchorMax.x - rectTransform.anchorMin.x);
        float wantedHeight = canvasHeight * (rectTransform.anchorMax.y - rectTransform.anchorMin.y);
        
        if (wantedHeight < rectTransform.rect.height && wantedWidth >= optimalWidth)
        {
            rectTransform.gameObject.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
        }
        if(wantedWidth < rectTransform.rect.width)
        {
            rectTransform.gameObject.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
        }

        rectTransform.gameObject.GetComponent<AspectRatioFitter>().aspectRatio = 1;
        ZeroOffset(rectTransform);
    }    

    void ZeroOffset(RectTransform rect)
    {
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    } 
}
