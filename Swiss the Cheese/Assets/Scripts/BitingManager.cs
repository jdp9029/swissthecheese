using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BitingManager : MonoBehaviour
{
    [HideInInspector] public bool IsBiting = false;
    [HideInInspector] private float timer;
    [HideInInspector] const int fps = 60;
    [HideInInspector] private int mouseNumber = 0;

    [SerializeField] private List<Sprite> NibblesSpriteSheet;
    [SerializeField] private List<Sprite> AlbinoSpriteSheet;
    [SerializeField] private List<Sprite> BlackSpriteSheet;
    [SerializeField] private List<Sprite> OrangeSpriteSheet;
    [SerializeField] private List<Sprite> StrawberrySpriteSheet;
    [SerializeField] private List<Sprite> CheeseSpriteSheet;

    [HideInInspector] private MouseSkinLoader MouseSkinLoader => FindObjectOfType<MouseSkinLoader>();
    
    
    [HideInInspector] private GameObject holeBeingEaten;
    [HideInInspector] private GameObject holeBeingEaten2;
    [SerializeField] private AudioClip[] bitingSoundClips;
    [HideInInspector] private AudioSource bitingSoundPlaying;

    [HideInInspector] private GameObject mouseObject;
    [HideInInspector] private GameObject mouseObject2;

    // Update is called once per frame
    void Update()
    {
        if (mouseObject == null)
        {
            var mice = GameObject.FindGameObjectsWithTag("Mouse");

            mouseObject = mice[0];

            if (HardModeManager.Mode == HardModeManager.Modes.TWICEMICE && SceneManager.GetActiveScene().name == "UpdatedGameplay")
            {
                mouseObject2 = mice[1];
            }
        }

        if (IsBiting)
        {
            List<Sprite> mouseSprites = GetSpriteSheet();

            //increment timer
            timer += Time.deltaTime;

            var lastmousenumber = mouseNumber;

            //get the frame that we are supposed to be at
            mouseNumber = FrameNumber(timer, (1 / (float)fps));

            //set the appropriate fill amount
            holeBeingEaten.GetComponent<Image>().fillAmount = ((float)mouseNumber / (float)mouseSprites.Count);

            if (holeBeingEaten2 != null)
            {
                holeBeingEaten2.GetComponent<Image>().fillAmount = ((float)mouseNumber / (float)mouseSprites.Count);
            }

            //Rotate child objects
            float accessoryAngle;

            switch (mouseNumber)
            {
                case 1:
                    accessoryAngle = 1;
                    break;
                case 2:
                    accessoryAngle = 10;
                    break;
                case 3:
                    accessoryAngle = 45;
                    break;
                case 4:
                    accessoryAngle = 90;
                    break;
                case 5:
                    accessoryAngle = 120;
                    break;
                case 6:
                    accessoryAngle = 150;
                    break;
                case 7:
                    accessoryAngle = 180;
                    break;
                case 8:
                    accessoryAngle = 210;
                    break;
                case 9:
                    accessoryAngle = 240;
                    break;
                case 10:
                    accessoryAngle = 270;
                    break;
                case 11:
                    accessoryAngle = 300;
                    break;
                case 12:
                    accessoryAngle = 330;
                    break;
                default:
                    accessoryAngle = 0;
                    break;
            }

            accessoryAngle += mouseObject.transform.GetComponent<RectTransform>().rotation.eulerAngles.z;
            mouseObject.transform.GetChild(0).GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, accessoryAngle);
            mouseObject.transform.GetChild(1).GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, accessoryAngle);

            if (mouseObject2 != null)
            {
                mouseObject2.transform.GetChild(0).GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, accessoryAngle);
                mouseObject2.transform.GetChild(1).GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, accessoryAngle);
            }

            //end bite when we're ready
            if (mouseNumber >= mouseSprites.Count)
            {
                EndBite();

                if (mouseObject2 != null)
                {
                    EndBite(false);
                }

                return;
            }

            //otherwise, update the frame accordingly
            mouseObject.GetComponent<Image>().sprite = mouseSprites[mouseNumber];

            if (mouseObject2 != null)
            {
                mouseObject2.GetComponent<Image>().sprite = mouseSprites[mouseNumber];
            }
        }
    }

    private List<Sprite> GetSpriteSheet()
    {
        switch(MouseSkinLoader.EquippedSkin.Name)
        {
            case "Nibbles":
                return NibblesSpriteSheet;
            case "Jasper":
                return AlbinoSpriteSheet;
            case "Gir":
                return BlackSpriteSheet;
            case "Peanut":
                return OrangeSpriteSheet;
            case "Jelly":
                return StrawberrySpriteSheet;
            case "Swisstopher":
                return CheeseSpriteSheet;
            default:
                throw new NotImplementedException();
        }
    }

    public void StartBite(GameObject hole, bool playSound = true)
    {
        //set isbiting
        IsBiting = true;

        //set up the fill amount and angle
        hole.GetComponent<Image>().fillAmount = 0;
        SetFillAngle(hole);
        timer = 0;

        if (playSound)
        {
            mouseObject.transform.localScale *= .75f;
            bitingSoundPlaying = GameObject.FindObjectOfType<SoundManager>().PlayRandomSoundFX(bitingSoundClips, transform, 1, .5f);
            holeBeingEaten = hole;
            var x = mouseObject.transform.GetChild(0).GetComponent<RectTransform>().localScale.x;
            var y = mouseObject.transform.GetChild(0).GetComponent<RectTransform>().localScale.y;
            var z = mouseObject.transform.GetChild(0).GetComponent<RectTransform>().localScale.z;
            mouseObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(x * 2, y, z);
            mouseObject.transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(x * 2, y, z);
        }
        else
        {
            mouseObject2.transform.localScale *= .75f;
            holeBeingEaten2 = hole;
            var x2 = mouseObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale.x;
            var y2 = mouseObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale.y;
            var z2 = mouseObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale.z;
            mouseObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(x2 * 2, y2, z2);
            mouseObject2.transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(x2 * 2, y2, z2);
        }
    }

    public void EndBite(bool playSound = true)
    {
        if (playSound)
        {
            var x = mouseObject.transform.GetChild(0).GetComponent<RectTransform>().localScale.x;
            var y = mouseObject.transform.GetChild(0).GetComponent<RectTransform>().localScale.y;
            var z = mouseObject.transform.GetChild(0).GetComponent<RectTransform>().localScale.z;
            mouseObject.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(x * .5f, y, z);
            mouseObject.transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(x * .5f, y, z);

            //reset variables
            IsBiting = false;
            timer = 0;
            mouseObject.transform.localScale /= .75f;
            mouseObject.GetComponent<Image>().sprite = FindObjectOfType<MouseSkinLoader>().EquippedSkin.Sprite;

            //destroy the audio source
            Destroy(bitingSoundPlaying.gameObject);

            //if this bite is in the menu, we don't need to check it against other holes
            if (SceneManager.GetActiveScene().name == "Menu" || SceneManager.GetActiveScene().name == "PlayScreen") { return; }

            //check it against the other intersections in hole manager
            GameObject.FindObjectOfType<HoleManager>().CheckIntersections(holeBeingEaten);

            mouseObject.transform.GetChild(0).GetComponent<RectTransform>().rotation = mouseObject.transform.rotation;
            mouseObject.transform.GetChild(1).GetComponent<RectTransform>().rotation = mouseObject.transform.rotation;
        }
        else
        {
            var x = mouseObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale.x;
            var y = mouseObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale.y;
            var z = mouseObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale.z;
            mouseObject2.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(x * .5f, y, z);
            mouseObject2.transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(x * .5f, y, z);

            mouseObject2.transform.localScale /= .75f;
            mouseObject2.GetComponent<Image>().sprite = FindObjectOfType<MouseSkinLoader>().EquippedSkin.Sprite;

            //check it against the other intersections in hole manager
            if (GameObject.FindObjectOfType<HoleManager>().holesCut.Any())
            {
                GameObject.FindObjectOfType<HoleManager>().CheckIntersections(holeBeingEaten2);
            }
            else
            {
                Destroy(holeBeingEaten2.gameObject);
            }

            mouseObject2.transform.GetChild(0).GetComponent<RectTransform>().rotation = mouseObject2.transform.rotation;
            mouseObject2.transform.GetChild(1).GetComponent<RectTransform>().rotation = mouseObject2.transform.rotation;
        }
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
        else if(SceneManager.GetActiveScene().name == "PlayScreen")
        {
            angle = FindObjectOfType<PlayScreenMouse>().angle;
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
