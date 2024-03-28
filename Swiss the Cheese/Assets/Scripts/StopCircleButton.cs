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

        StartCoroutine(ValidClick());
    }

    private IEnumerator ValidClick()
    {
        yield return new WaitForNextFrameUnit();

        //Cut a new hole out of the big circle
        GameObject newHole = Instantiate(circlePrefab, holeManager.mouseInstance.transform.position, Quaternion.identity, bigCircle.transform);

        //Set it up as the color of the background
        newHole.GetComponent<Image>().color = background.GetComponent<Image>().color;

        holeManager.mouseInstance.transform.SetAsLastSibling();

        GameObject.FindObjectOfType<BitingManager>().StartBite(newHole);
    }
}
