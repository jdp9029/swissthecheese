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
        /*Vector2 bigCircleCenter = new Vector2(bigCircle.transform.position.x + (.5f * bigCircle.GetComponent<RectTransform>().rect.width), 
            bigCircle.transform.position.y + (.5f * bigCircle.GetComponent <RectTransform>().rect.height));
        Vector2 rotatingCircleCenter = new Vector2(holeManager.cutterInstance.transform.position.x, holeManager.cutterInstance.transform.position.y);
        Vector2 centerCircleCenter = bigCircleCenter;
        float bigCircleRadius = bigCircle.GetComponent<RectTransform>().rect.width / 2;
        float rotatingCircleRadius = holeManager.cutterInstance.GetComponent<RectTransform>().rect.width / 2;
        float centerCircleRadius = centerCircle.GetComponent<RectTransform>().rect.width / 2;

        bool radiusTooLarge = Distance(bigCircleCenter, rotatingCircleCenter) + rotatingCircleRadius + 10 >= bigCircleRadius;
        bool radiusTooSmall = Distance(centerCircleCenter, rotatingCircleCenter) <= centerCircleRadius + rotatingCircleRadius + 10;

        Debug.Log("radius too small: " + radiusTooSmall);
        Debug.Log("radius too large: " + radiusTooLarge);*/

        if(mouseBeingHeld)
        {
            int multiplier = Increase ? 1 : -1;
            holeManager.radius += Time.deltaTime * 250 * multiplier;
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

    private float Distance(Vector2 point1, Vector2 point2)
    {
        return Mathf.Sqrt(Mathf.Pow(point1.x - point2.x, 2) + Mathf.Pow(point1.y - point2.y, 2));
    }
}
