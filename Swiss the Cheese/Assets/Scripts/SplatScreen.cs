using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplatScreen : MonoBehaviour
{
    [HideInInspector] private float timer = 0;
    [HideInInspector] private float finalWaitTimer = 0;
    [HideInInspector] const int fps = 18;
    [HideInInspector] private int frameNumber = 0;
    [SerializeField] List<Sprite> sprites = new List<Sprite>();

    // Start is called before the first frame update
    void Start()
    {
        RectTransform canvasTransform = FindObjectOfType<Canvas>().GetComponent<RectTransform>();

        if (canvasTransform.rect.width < 600 || canvasTransform.rect.height < 600)
        {
            float smallerSize = canvasTransform.rect.width < canvasTransform.rect.height ? canvasTransform.rect.width : canvasTransform.rect.height;

            transform.localScale = new Vector3(smallerSize / 600f, smallerSize / 600f, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //increment timer
        timer += Time.deltaTime;

        //get the frame that we are supposed to be at
        frameNumber = FrameNumber(timer, (1 / (float)fps));

        //end bite when we're ready
        if (frameNumber >= sprites.Count)
        {
            GetComponent<Image>().sprite = sprites[sprites.Count - 1];
            finalWaitTimer += Time.deltaTime;
        }
        else
        {
            GetComponent<Image>().sprite = sprites[frameNumber];
        }

        if(finalWaitTimer > 0)
        {
            finalWaitTimer += Time.deltaTime;
            if (finalWaitTimer > 1.5f)
            {
                SceneManager.LoadScene("Menu");
            }
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
}
