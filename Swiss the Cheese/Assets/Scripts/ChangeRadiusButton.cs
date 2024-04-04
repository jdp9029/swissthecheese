using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ChangeRadiusButton : MonoBehaviour
{
    [SerializeField] bool Increase;
    [SerializeField] HoleManager holeManager;
    [SerializeField] GameObject bigCircle;
    [SerializeField] GameObject centerCircle;

    private bool mouseBeingHeld;

    // Update is called once per frame
    void Update()
    {
        //a multiplier to change the radius by
        float multiplier = Increase ? bigCircle.GetComponent<CircleCollider2D>().radius * Time.deltaTime / 2: bigCircle.GetComponent<CircleCollider2D>().radius * Time.deltaTime / -2;
        
        //if the mouse is being held
        if (mouseBeingHeld)
        {
            //change the value of the radius by the multiplier
            holeManager.radius += multiplier;

            //update the position of the rotating circle
            Physics.SyncTransforms();

            //make the circle does not go outside the bounds of the outer circle
            if ((RadiusTooSmall() && !Increase) || (RadiusTooLarge() && Increase))
            {
                holeManager.radius -= multiplier;
            }
        }
    }

    //gets called when the button is pressed
    public void OnPress()
    {
        if(!GameObject.FindObjectOfType<ZoomManager>().IsZooming && !GameObject.FindObjectOfType<BitingManager>().IsBiting)
        {
            mouseBeingHeld = true;
        }
    }

    //gets called when the button is released
    public void OnRelease()
    {
        if (!GameObject.FindObjectOfType<ZoomManager>().IsZooming && !GameObject.FindObjectOfType<BitingManager>().IsBiting)
        {
            mouseBeingHeld = false;
        }
    }
    
    //determine if the radius of the rotating circle is too large
    private bool RadiusTooSmall()
    {
        Vector2 centerCirclePoint = centerCircle.GetComponent<RectTransform>().TransformPoint(centerCircle.GetComponent<RectTransform>().rect.center);
        Vector2 mousePoint = holeManager.mouseInstance.GetComponent<RectTransform>().TransformPoint(holeManager.mouseInstance.GetComponent<RectTransform>().rect.center);

        bool tooSmall = Vector2.Distance(centerCirclePoint, mousePoint) <= 2 * GameObject.FindObjectOfType<LevelManager>().GetRadius(centerCircle);
        if(tooSmall) { Debug.Log("too small"); }
        return tooSmall;
    }

    //determine if the radius of the rotating circle is too large
    private bool RadiusTooLarge()
    {
        Vector2 edgePoint = bigCircle.GetComponent<RectTransform>().TransformPoint(bigCircle.GetComponent<RectTransform>().rect.center +
            (bigCircle.GetComponent<CircleCollider2D>().radius * new Vector2(Mathf.Cos(holeManager.angle), Mathf.Sin(holeManager.angle))));
        Vector2 mousePoint = holeManager.mouseInstance.GetComponent<RectTransform>().TransformPoint(holeManager.mouseInstance.GetComponent<RectTransform>().rect.center);

        bool tooLarge = Vector2.Distance(edgePoint, mousePoint) <= 1.25f * GameObject.FindObjectOfType<LevelManager>().GetRadius(centerCircle);
        if (tooLarge) { Debug.Log("too large"); }
        return tooLarge;
    }
}
