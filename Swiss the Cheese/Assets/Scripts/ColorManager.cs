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
    private float timePassed;

    // Start is called before the first frame update
    void Start()
    {
        colors = new Color32[]
        {
            new Color32(167, 165, 19, 255),
            new Color32(198, 197, 84, 255),
            new Color32(204, 158, 29, 255),
            new Color32(216, 200, 3, 255), 
            new Color32(221, 175, 82, 255),
            new Color32(168, 113, 2, 255),
            new Color32(218, 244, 22, 255)
        };
        
        timePassed = 0;
        colorIndex = 0;

        CycleBackground();
        CycleForeground();
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
    }

    //Change the color of foreground components
    public void CycleForeground()
    {
        Circle.color = colors[colorIndex % colors.Length];
        CircleCenter.color = colors[(colorIndex + 1) % colors.Length];
        colorIndex++;
    }

    //Change the color of background components
    public void CycleBackground()
    {
        Background.color = colors[(colorIndex + 1) % colors.Length];
    }

    //Slowly shift the color of the regular circle midzoom
    public void SlowBurn(Color32 baseColor, Color32 targetColor, GameObject targetObject)
    {
        //Only do it every .01 seconds
        if(timePassed < 0.01f) { return; }
        else { timePassed = 0; }

        //math
        int r = baseColor.r;
        int g = baseColor.g;
        int b = baseColor.b;
        int a = baseColor.a;

        if(targetColor.r > baseColor.r) { r = baseColor.r + 1; }
        else if(targetColor.r != baseColor.r) { r = baseColor.r - 1; }
        if (targetColor.g > baseColor.g) { g = baseColor.g + 1; }
        else if (targetColor.g != baseColor.g) { g = baseColor.g - 1; }
        if (targetColor.b > baseColor.b) { b = baseColor.b + 1; }
        else if (targetColor.b != baseColor.b) { b = baseColor.b - 1; }
        if (targetColor.a > baseColor.a) { a = baseColor.a + 1; }
        else if (targetColor.a != baseColor.a) { a = baseColor.a - 1; }

        targetObject.GetComponent<UnityEngine.UI.Image>().color = new Color32((byte)r, (byte)g, (byte)b, (byte)a);
    }

    public void ResetColors()
    {
        colorIndex = 0;
        CycleBackground();
        CycleForeground();
    }

}
