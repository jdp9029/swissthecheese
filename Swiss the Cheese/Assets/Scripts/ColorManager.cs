using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ColorManager : MonoBehaviour
{
    public UnityEngine.UI.Image Background;
    public UnityEngine.UI.Image Circle;
    public UnityEngine.UI.Image CircleCenter;
    private int colorIndex;
    private Color32[] colors;

    // Start is called before the first frame update
    void Start()
    {
        colors = new Color32[]
        {
            new Color32(167, 165, 19, 255),
            new Color32(198, 197, 84, 255),
            new Color32(204, 158, 29, 255),
            new Color32(216, 160, 3, 255), 
            new Color32(221, 175, 82, 255),
            new Color32(168, 113, 2, 255),
            new Color32(218, 244, 22, 255)
        };

        CycleColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CycleColor()
    {
        Circle.color = colors[colorIndex % colors.Length];
        Background.color = colors[(colorIndex + 1) % colors.Length];
        CircleCenter.color = colors[(colorIndex + 1) % colors.Length];
        colorIndex++;
    }

}
