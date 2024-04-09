using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResponsiveBox : MonoBehaviour
{
    Rect canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.FindObjectOfType<Canvas>().GetComponent<RectTransform>().rect;
        if(GetComponent<AspectRatioFitter>() == null )
        {
            transform.AddComponent<AspectRatioFitter>();
            GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.None;
        }
    }

    // Update is called once per frame
    void Update()
    {
        canvas = GameObject.FindObjectOfType<Canvas>().GetComponent<RectTransform>().rect;

        if (canvas.width > canvas.height)
        {
            GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            GetComponent<AspectRatioFitter>().aspectRatio = 3f;
        }
        else if(canvas.height / 2 > canvas.width)
        {
            GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
            GetComponent<AspectRatioFitter>().aspectRatio = 2.0f;
        }
        else
        {
            GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.None;
        }

        ZeroOffset();

        if(stationaryonX)
        {

        }
    }

    void ZeroOffset()
    {
        GetComponent<RectTransform>().offsetMin = Vector2.zero;
        GetComponent<RectTransform>().offsetMax = Vector2.zero;
    }
}
