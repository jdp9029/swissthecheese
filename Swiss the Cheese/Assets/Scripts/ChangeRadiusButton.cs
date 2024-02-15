using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeRadiusButton : MonoBehaviour
{
    [SerializeField] bool Increase;
    [SerializeField] HoleManager holeManager;
    [SerializeField] GameObject bigCircle;
    [SerializeField] GameObject centerCircle;

    private bool mouseBeingHeld;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float multiplier = Increase ? 250 * Time.deltaTime: -250 * Time.deltaTime;
        
        if (mouseBeingHeld)
        {
            //moves the radius accordingly
            holeManager.radius += multiplier;

            //handles getting too close to the center
            if (RadiusTooSmall() && !Increase)
            {
                holeManager.radius -= multiplier;
            }
        }

        //handles getting too close to the edge
        if (RadiusTooLarge() && Increase)
        {
            Debug.Log("too big");
            holeManager.radius -= multiplier;
        }
    }

    public void OnPress()
    {
        mouseBeingHeld = true;
    }

    public void OnRelease()
    {
        mouseBeingHeld=false;
    }

    private bool RadiusTooSmall()
    {
        return centerCircle.GetComponent<CircleCollider2D>().bounds.Intersects(holeManager.cutterInstance.GetComponent<CircleCollider2D>().bounds);
    }

    private bool RadiusTooLarge()
    {
        /*return !bigCircle.GetComponent<CircleCollider2D>().bounds.Contains(holeManager.cutterInstance.GetComponent<CircleCollider2D>().bounds.min) ||
            !bigCircle.GetComponent<CircleCollider2D>().bounds.Contains(holeManager.cutterInstance.GetComponent<CircleCollider2D>().bounds.max);*/
        
        return Distance((Vector2)Camera.main.ViewportToWorldPoint(holeManager.cutterInstance.transform.position), (Vector2)Camera.main.ViewportToWorldPoint(bigCircle.transform.position))
            + holeManager.cutterInstance.GetComponent<CircleCollider2D>().radius >= bigCircle.GetComponent<CircleCollider2D>().radius;
    }

    private float Distance(Vector2 point1, Vector2 point2)
    {
        return Mathf.Sqrt(Mathf.Pow(point1.x - point2.x, 2) + Mathf.Pow(point1.y - point2.y, 2));
    }
}
