using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEditor;

public class ChangeRadiusButton : MonoBehaviour
{
    [SerializeField] bool Increase;
    [SerializeField] HoleManager holeManager;
    [SerializeField] RectTransform bigCircle;
    [SerializeField] GameObject centerCircle;
    [SerializeField] GameObject pathObject;
    [SerializeField] GameObject closestPointObject;

    private bool mouseBeingHeld;

    // Update is called once per frame
    void Update()
    {
        //a multiplier to change the radius by
        /* float multiplier = Increase ? bigCircle.GetComponent<RectTransform>(). * Time.deltaTime / 1.25f:
             bigCircle.GetComponent<CircleCollider2D>().radius * Time.deltaTime / -1.25f;*/

        float multiplier = Increase ? 40 * Time.deltaTime : -40 * Time.deltaTime;

        //if the mouse is being held
        if (mouseBeingHeld )
        {
            //change the value of the radius by the multiplier
            holeManager.radius += multiplier;

            if (holeManager.radius > 65)
            {
                holeManager.radius = 65;
            }
            else if (holeManager.radius < 15)
            {
                holeManager.radius = 15;
            }

            //update the position of the rotating circle
            Physics.SyncTransforms();
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

        return Vector2.Distance(centerCirclePoint, mousePoint) <= 1.25 * GameObject.FindObjectOfType<LevelManager>().GetRadius(centerCircle);
    }

    //determine if the radius of the rotating circle is too large
    private bool RadiusTooLarge()
    {
        return Vector2.Distance(holeManager.mouseInstance.GetComponent<RectTransform>().localPosition, Vector2.zero) > 
            .7f * Vector2.Distance(closestPointObject.GetComponent<RectTransform>().localPosition, Vector2.zero);
    }
}
