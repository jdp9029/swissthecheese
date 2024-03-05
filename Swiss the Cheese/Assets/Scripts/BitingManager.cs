using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BitingManager : MonoBehaviour
{
    [HideInInspector] public bool IsBiting;
    [HideInInspector] private float timer;
    [HideInInspector] const int spriteSheetFrames = 14;
    [HideInInspector] const int fps = 2;
    [HideInInspector] private int mouseNumber = 0;
    [SerializeField] private Sprite normalMouseSprite;
    [SerializeField] private List<Sprite> mouseSprites;
    [HideInInspector] private GameObject holeBeingEaten;

    // Start is called before the first frame update
    void Start()
    {
        IsBiting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsBiting)
        {
            timer += Time.deltaTime;

            mouseNumber = (int)((100 * timer) / (spriteSheetFrames / fps));
            holeBeingEaten.GetComponent<Image>().fillAmount = mouseNumber / spriteSheetFrames;

            if(mouseNumber >= mouseSprites.Count)
            {
                EndBite();
                return;
            }
            
            GameObject.FindGameObjectWithTag("Mouse").GetComponent<Image>().sprite = mouseSprites[mouseNumber];
        }
    }

    public void StartBite(GameObject hole)
    {
        IsBiting = true;
        holeBeingEaten = hole;
        GameObject.FindGameObjectWithTag("Mouse").transform.localScale *= .75f;
    }

    public void EndBite()
    {
        IsBiting = false;
        timer = 0;
        GameObject.FindGameObjectWithTag("Mouse").transform.localScale /= .75f;
        GameObject.FindGameObjectWithTag("Mouse").GetComponent<Image>().sprite = normalMouseSprite;
        GameObject.FindObjectOfType<HoleManager>().CheckIntersections(holeBeingEaten);
    }
}
