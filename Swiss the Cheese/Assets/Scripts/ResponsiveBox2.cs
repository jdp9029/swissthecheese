using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResponsiveBox2 : MonoBehaviour
{
    [SerializeField] float rawWidth;
    [SerializeField] float rawHeight;
    [SerializeField] Vector2 expectedMin;
    [SerializeField] Vector2 expectedMax;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<AspectRatioFitter>() == null)
        {
            transform.AddComponent<AspectRatioFitter>();
        }
        GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        float maxWidth = (GetComponent<RectTransform>().anchorMax.x - GetComponent<RectTransform>().anchorMin.x) * transform.parent.GetComponent<RectTransform>().rect.width;
        float maxHeight = (GetComponent<RectTransform>().anchorMax.y - GetComponent<RectTransform>().anchorMin.y) * transform.parent.GetComponent<RectTransform>().rect.height;

        float percentWidthFilledOut = rawWidth / maxWidth;
        float percentHeightFilledOut = rawHeight / maxHeight;

        if(percentWidthFilledOut > 1 && percentHeightFilledOut > 1)
        {
            float biggerDimension = percentWidthFilledOut > percentHeightFilledOut ? percentWidthFilledOut : percentHeightFilledOut;
            transform.localScale = new Vector2(1 / biggerDimension, 1 / biggerDimension);
        }
        else if(percentWidthFilledOut > 1 && percentHeightFilledOut <= 1)
        {
            transform.localScale = new Vector2(1 / percentWidthFilledOut, 1 / percentWidthFilledOut);
        }
        else if(percentWidthFilledOut <=1 && percentHeightFilledOut > 1)
        {
            transform.localScale = new Vector2(1 / percentHeightFilledOut, 1 / percentHeightFilledOut);
        }   
        
        //if they are both under 1
        else
        {
            float biggerDimension = percentWidthFilledOut > percentHeightFilledOut ? percentWidthFilledOut : percentHeightFilledOut;
            transform.localScale = new Vector2(1 / biggerDimension, 1 / biggerDimension);
        }

        GetComponent<RectTransform>().localPosition = Vector2.zero;



        /*if (maxWidth / maxWidthRatio > maxHeight)
        {
            GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            GetComponent<AspectRatioFitter>().aspectRatio = maxWidthRatio;
        }
        else if (maxHeight / maxHeightRatio > maxWidth)
        {
            GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
            GetComponent<AspectRatioFitter>().aspectRatio = maxHeightRatio;
        }
        else
        {
            GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.None;
        }

        ZeroOffset();

        float posX = transform.parent.GetComponent<RectTransform>().rect.width * .5f *
            (GetComponent<RectTransform>().anchorMin.x + GetComponent<RectTransform>().anchorMax.x - (2 * transform.parent.GetComponent<RectTransform>().pivot.x));
        float posY = transform.parent.GetComponent<RectTransform>().rect.height * .5f *
            (GetComponent<RectTransform>().anchorMin.y + GetComponent<RectTransform>().anchorMax.y - (2 * transform.parent.GetComponent<RectTransform>().pivot.y));
        GetComponent<RectTransform>().localPosition = new Vector3(posX, posY);*/
    }

    void ZeroOffset()
    {
        GetComponent<RectTransform>().offsetMin = Vector2.zero;
        GetComponent<RectTransform>().offsetMax = Vector2.zero;
    }
}
