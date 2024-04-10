using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResponsiveBox : MonoBehaviour
{
    [SerializeField] float maxWidthRatio;
    [SerializeField] float maxHeightRatio;

    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<AspectRatioFitter>() == null )
        {
            transform.AddComponent<AspectRatioFitter>();
            GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.None;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float width = (GetComponent<RectTransform>().anchorMax.x - GetComponent<RectTransform>().anchorMin.x) * transform.parent.GetComponent<RectTransform>().rect.width;
        float height = (GetComponent<RectTransform>().anchorMax.y - GetComponent<RectTransform>().anchorMin.y) * transform.parent.GetComponent<RectTransform>().rect.height;

        if (width / maxWidthRatio > height)
        {
            GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            GetComponent<AspectRatioFitter>().aspectRatio = maxWidthRatio;
        }
        else if(height / maxHeightRatio > width)
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
        GetComponent<RectTransform>().localPosition = new Vector3(posX, posY);
    }

    void ZeroOffset()
    {
        GetComponent<RectTransform>().offsetMin = Vector2.zero;
        GetComponent<RectTransform>().offsetMax = Vector2.zero;
    }
}
