using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoleManager : MonoBehaviour
{
    //circle swinging
    [SerializeField] GameObject circularPrefab;

    //the instance of the swinging circle
    [HideInInspector] public GameObject cutterInstance;

    //center of the big circle
    [SerializeField] GameObject centerOfCircle;

    //big circle
    [SerializeField] GameObject biggerCircle;

    //color manager
    [SerializeField] ColorManager colorManager;

    //radius of the swinging circle
    [SerializeField] public float radius = 1.0f;

    //angle of the swinging circle from the center
    private float angle = 0;

    //speed of the rotating circle
    private float rotationSpeed = 4f;


    // Start is called before the first frame update
    void Start()
    {
        //find the center of the canvas
        Rect centerOfBackground = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>().rect;

        //set up the circling circle
        cutterInstance = Instantiate(circularPrefab, new Vector3(0.0f, 0 - (biggerCircle.GetComponent<RectTransform>().rect.height / 4), 0.0f),
            Quaternion.identity, biggerCircle.transform);

        //make the color white, for now
        cutterInstance.GetComponent<Image>().color = Color.white;

        //set up the radius
        radius = biggerCircle.GetComponent<RectTransform>().rect.height - (3 * centerOfCircle.GetComponent<RectTransform>().rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        //iterate the angle
        angle += Time.deltaTime * rotationSpeed;
        ReturnAngleToZero();

        //move the circle around accordingly
        cutterInstance.transform.position = new Vector3(centerOfCircle.transform.position.x + (radius * Mathf.Cos(angle)), centerOfCircle.transform.position.y + (radius * Mathf.Sin(angle)), 0.0f);
    }

    private void ReturnAngleToZero()
    {
        if(angle >= 2 * Mathf.PI)
        {
            angle -= 2 * Mathf.PI;
        }
    }
}
