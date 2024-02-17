using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StopCircleButton : MonoBehaviour
{
    //button getting clicked
    private Button stopButton;

    //prefab to spawn the circle
    [SerializeField] private GameObject circlePrefab;

    //background image reference
    [SerializeField] private GameObject background;

    //bigCircle reference
    [SerializeField] private GameObject bigCircle;

    //hole manager
    [SerializeField] HoleManager holeManager;

    // Start is called before the first frame update
    void Start()
    {
        stopButton = GetComponent<Button>();
        stopButton.onClick.AddListener(OnClick);
    }

    //Occurs when the button is clicked
    private void OnClick()
    {
        //Cut a new hole out of the big circle
        GameObject newHole = Instantiate(circlePrefab, holeManager.cutterInstance.transform.position, Quaternion.identity, bigCircle.transform);

        //Set it up as the color of the background
        newHole.GetComponent<Image>().color = background.GetComponent<Image>().color;

        //check it against other holes cut
        holeManager.CheckIntersections(newHole);

        //Make the rotating circle the last sibling
        holeManager.cutterInstance.transform.SetAsLastSibling();
    }
}
