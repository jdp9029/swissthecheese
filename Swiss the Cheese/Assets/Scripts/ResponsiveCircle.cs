using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResponsiveCircle : MonoBehaviour
{
    RectTransform canvasRect;
    [SerializeField] bool isBigCircle;

    private float canvasHeight;
    private float canvasWidth;

    private void Start()
    {
        canvasRect = FindObjectOfType<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        canvasHeight = canvasRect.rect.height * canvasRect.localScale.y;
        canvasWidth = canvasRect.rect.width * canvasRect.localScale.x;

        UpdateCircleAspect(GetComponent<RectTransform>());

        if(isBigCircle)
        {
            //GetComponent<RectTransform>().position = new Vector2(canvasWidth / 2, GetComponent<RectTransform>().position.y);
        }
    }

    void UpdateCircleAspect(RectTransform rectTransform)
    {
        float wantedWidth = canvasWidth * (rectTransform.anchorMax.x - rectTransform.anchorMin.x);
        float wantedHeight = canvasHeight * (rectTransform.anchorMax.y - rectTransform.anchorMin.y);

        if (wantedWidth > wantedHeight)
        {
            rectTransform.gameObject.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
        }
        else
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
