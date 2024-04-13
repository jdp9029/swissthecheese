using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BitingManager : MonoBehaviour
{
    [HideInInspector] public bool IsBiting = false;
    [HideInInspector] private float timer;
    [HideInInspector] const int fps = 60;
    [HideInInspector] private int mouseNumber = 0;
    [SerializeField] private Sprite normalMouseSprite;
    [SerializeField] private List<Sprite> mouseSprites;
    [HideInInspector] private GameObject holeBeingEaten;
    [SerializeField] private AudioClip[] bitingSoundClips;
    [HideInInspector] private AudioSource bitingSoundPlaying;

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

        bitingSoundPlaying = GameObject.FindObjectOfType<SoundManager>().PlayRandomSoundFX(bitingSoundClips, transform, 1, .5f);
    }

    public void EndBite()
    {
        //reset variables
        IsBiting = false;
        timer = 0;
        GameObject.FindGameObjectWithTag("Mouse").transform.localScale /= .75f;
        GameObject.FindGameObjectWithTag("Mouse").GetComponent<Image>().sprite = normalMouseSprite;

        //destroy the audio source
        Destroy(bitingSoundPlaying.gameObject);

        //if this bite is in the menu, we don't need to check it against other holes
        if(SceneManager.GetActiveScene().name == "Menu" || SceneManager.GetActiveScene().name == "TitleReel") { return; }

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
        //set up the angle we need to be filling
        float angle;

        //if we are in the menu, we use the menu mouse angle
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            angle = GameObject.FindObjectOfType<MenuMouse>().angle;
        }
        else if(SceneManager.GetActiveScene().name == "TitleReel")
        {
            angle = GameObject.FindObjectOfType<TitleReel>().angle;
        }

        //if we are in gameplay, we use the hole manager angle
        else { angle = GameObject.FindObjectOfType<HoleManager>().angle; }

        //determine direction of fill
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
