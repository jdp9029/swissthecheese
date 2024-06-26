using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheeseImageManager : MonoBehaviour
{
    public Image Circle;
    public Image CircleCenter;
    private int imageIndex;


    [SerializeField]
    private Sprite[] backgroundImages;

    // Start is called before the first frame update
    void Start()
    {
        imageIndex = 0;
        CycleCheese();
    }

    //Change the color of foreground components
    public void CycleCheese()
    {
        Circle.sprite = backgroundImages[imageIndex % backgroundImages.Length];
        imageIndex++;
    }

    public Sprite NextCheese()
    {
        return backgroundImages[imageIndex % backgroundImages.Length];
    }

}
