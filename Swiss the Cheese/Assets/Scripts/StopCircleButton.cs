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

    //holes cut out of the cheese
    private List<GameObject> holesMade = new List<GameObject>();

    //hole manager
    [SerializeField] HoleManager holeManager;

    // Start is called before the first frame update
    void Start()
    {
        stopButton = GetComponent<Button>();
        stopButton.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnClick()
    {
        GameObject newHole = Instantiate(circlePrefab, holeManager.cutterInstance.transform.position, Quaternion.identity, bigCircle.transform);
        newHole.GetComponent<Image>().color = background.GetComponent<Image>().color;
        holesMade.Add(newHole);

        holeManager.cutterInstance.transform.SetAsLastSibling();
    }
}
