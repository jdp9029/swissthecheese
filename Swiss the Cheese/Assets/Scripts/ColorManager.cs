using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    public Image Background;
    public Image Circle;
    public Image CircleCenter;
    private int colorIndex;


    [SerializeField]
    private Sprite[] backgroundImages;

    private float timePassed;

    // Start is called before the first frame update
    void Start()
    {
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
        Circle.sprite = backgroundImages[colorIndex % backgroundImages.Length];
        //CircleCenter.color = backgroundImages[(colorIndex + 1) % backgroundImages.Length];
        colorIndex++;
    }

    //Change the color of background components
    public void CycleBackground()
    {
        //Background.color = backgroundImages[(colorIndex + 1) % backgroundImages.Length];
    }

    //Slowly shift the color of the regular circle midzoom
    public void SlowBurn(Color32 baseColor, Color32 targetColor, GameObject targetObject)
    {
        //Only do it every .01 seconds
        if (timePassed < 0.01f) { return; }
        else { timePassed = 0; }

        //math
        /*int r = baseColor.r;
        int g = baseColor.g;
        int b = baseColor.b;
        int a = baseColor.a;

        if (targetColor.r > baseColor.r) { r = baseColor.r + 1; }
        else if (targetColor.r != baseColor.r) { r = baseColor.r - 1; }
        if (targetColor.g > baseColor.g) { g = baseColor.g + 1; }
        else if (targetColor.g != baseColor.g) { g = baseColor.g - 1; }
        if (targetColor.b > baseColor.b) { b = baseColor.b + 1; }
        else if (targetColor.b != baseColor.b) { b = baseColor.b - 1; }
        if (targetColor.a > baseColor.a) { a = baseColor.a + 1; }
        else if (targetColor.a != baseColor.a) { a = baseColor.a - 1; }

        targetObject.GetComponent<Image>().color = new Color32((byte)r, (byte)g, (byte)b, (byte)a);*/
    }

    public void ResetColors()
    {
        colorIndex = 0;
        CycleBackground();
        CycleForeground();
    }

}
