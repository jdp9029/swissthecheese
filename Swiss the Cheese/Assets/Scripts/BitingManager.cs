using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BitingManager : MonoBehaviour
{
    [HideInInspector] public bool IsBiting = false;
    [HideInInspector] private float timer;
    [HideInInspector] const int fps = 50;
    [HideInInspector] private int mouseNumber = 0;
    [SerializeField] private Sprite normalMouseSprite;
    [SerializeField] private List<Sprite> mouseSprites;
    [HideInInspector] private GameObject holeBeingEaten;

    // Update is called once per frame
    void Update()
    {
        if(IsBiting)
        {
            //increment timer
            timer += Time.deltaTime;

            //get the frame that we are supposed to be at
            mouseNumber = FrameNumber(timer, (1 / (float)fps));

            //set the appropriate fill amount
            holeBeingEaten.GetComponent<Image>().fillAmount = ((float)mouseNumber / (float)mouseSprites.Count);

            //end bite when we're ready
            if(mouseNumber >= mouseSprites.Count)
            {
                EndBite();
                return;
            }
            
            //otherwise, update the frame accordingly
            GameObject.FindGameObjectWithTag("Mouse").GetComponent<Image>().sprite = mouseSprites[mouseNumber];
        }
    }

    public void StartBite(GameObject hole)
    {
        //set isbiting
        IsBiting = true;

        //set up the fill amount and angle
        hole.GetComponent<Image>().fillAmount = 0;
        SetFillAngle(hole);

        //apply variables
        holeBeingEaten = hole;
        timer = 0;
        GameObject.FindGameObjectWithTag("Mouse").transform.localScale *= .75f;
    }

    public void EndBite()
    {
        //reset variables
        IsBiting = false;
        timer = 0;
        GameObject.FindGameObjectWithTag("Mouse").transform.localScale /= .75f;
        GameObject.FindGameObjectWithTag("Mouse").GetComponent<Image>().sprite = normalMouseSprite;

        //check it against the other intersections in hole manager
        GameObject.FindObjectOfType<HoleManager>().CheckIntersections(holeBeingEaten);
    }

    //gets the correct frame the mouse should be at
    private int FrameNumber(float currentTime, float timePerFrame, int iterationsThrough = 0)
    {
        //loop throught recursive function until we're at the right frame
        if (currentTime > timePerFrame)
        {
            return FrameNumber(currentTime - timePerFrame, timePerFrame, iterationsThrough + 1);
        }
        else
        {
            return iterationsThrough;
        }

        //this should never hit
        throw new System.Exception("Couldn't calculate Frame Number");
    }

    //sets the fill angle start, depending on where the mouse is
    private void SetFillAngle(GameObject hole)
    {
        float angle = GameObject.FindObjectOfType<HoleManager>().angle;

        if(angle >= .25 * Mathf.PI && angle < .75 * Mathf.PI)
        {
            hole.GetComponent<Image>().fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else if(angle >= .75 * Mathf.PI && angle < 1.25 * Mathf.PI)
        {
            hole.GetComponent<Image>().fillOrigin = (int)Image.OriginVertical.Bottom;
        }
        else if(angle >= 1.25 * Mathf.PI && angle < 1.75 * Mathf.PI)
        {
            hole.GetComponent<Image>().fillOrigin = (int)Image.OriginHorizontal.Right;
        }
        else
        {
            hole.GetComponent<Image>().fillOrigin = (int)Image.OriginVertical.Top;
        }
    }
}
