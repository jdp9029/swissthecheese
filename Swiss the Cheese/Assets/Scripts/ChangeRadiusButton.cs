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
        float multiplier = Increase ? 250 * Time.deltaTime: -250 * Time.deltaTime;
        
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
        return centerCircle.GetComponent<CircleCollider2D>().bounds.Intersects(holeManager.mouseInstance.GetComponent<CircleCollider2D>().bounds);
    }

    //determine if the radius of the rotating circle is too large
    private bool RadiusTooLarge()
    {
        //the point on the circle of this angle
        Vector2 pointOnCircle = 2 * bigCircle.GetComponent<CircleCollider2D>().radius * new Vector2(Mathf.Cos(holeManager.angle), Mathf.Sin(holeManager.angle));

        //the point on the rotating circle at this angle
        Vector2 pointOnRotator = holeManager.mouseInstance.GetComponent<CircleCollider2D>().bounds.center - bigCircle.GetComponent<CircleCollider2D>().bounds.center;

        return Distance(pointOnRotator, Vector2.zero) + holeManager.mouseInstance.GetComponent<CircleCollider2D>().radius > Distance(pointOnCircle, Vector2.zero);
    }


    //return the distance between two points
    private float Distance(Vector2 point1, Vector2 point2)
    {
        return Mathf.Sqrt(Mathf.Pow(point1.x - point2.x, 2) + Mathf.Pow(point1.y - point2.y, 2));
    }
}
