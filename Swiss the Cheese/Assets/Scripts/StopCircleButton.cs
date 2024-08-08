using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StopCircleButton : MonoBehaviour
{
    //prefab to spawn the circle
    [SerializeField] private GameObject circlePrefab;

    //background image reference
    [SerializeField] private GameObject background;

    //bigCircle reference
    [SerializeField] private GameObject bigCircle;

    //hole manager
    [SerializeField] HoleManager holeManager;
    
    //Occurs when the button is clicked
    public void OnClick()
    {
        //only work if we're not in the middle of zooming
        if(GameObject.FindObjectOfType<ZoomManager>().IsZooming || GameObject.FindObjectOfType<BitingManager>().IsBiting)
        {
            return;
        }

        //Cut a new hole out of the big circle
        GameObject newHole = Instantiate(circlePrefab, holeManager.mouseInstance.transform.position, Quaternion.identity, bigCircle.transform);
        newHole.GetComponent<RectTransform>().localScale = Vector3.zero;

        //Set it up as the color of the background
        newHole.GetComponent<Image>().color = background.GetComponent<Image>().color;

        holeManager.mouseInstance.transform.SetAsLastSibling();

        GameObject.FindObjectOfType<BitingManager>().StartBite(newHole);

        if (HardModeManager.Mode == HardModeManager.Modes.TWICEMICE)
        {
            GameObject otherNewHole = Instantiate(circlePrefab, holeManager.mouseInstance2.transform.position, Quaternion.identity, bigCircle.transform);
            otherNewHole.GetComponent<RectTransform>().localScale = Vector3.zero;
            otherNewHole.GetComponent<Image>().color = background.GetComponent<Image>().color;

            holeManager.mouseInstance2.transform.SetAsLastSibling();

            GameObject.FindObjectOfType<BitingManager>().StartBite(otherNewHole, false);
        }
    }
}
