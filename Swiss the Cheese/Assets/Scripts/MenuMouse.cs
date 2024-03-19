using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuMouse : MonoBehaviour
{
    private float timePerAngle = 0;
    public float angle;
    private float radius;
    private float speed = 100f;
    private float timeSinceLastReset = 0f;
    [SerializeField] Button playButton;

    // Start is called before the first frame update
    void Start()
    {
        angle = 0f;
        radius = speed * Time.deltaTime;
        playButton.onClick.AddListener(delegate
        {
            SceneManager.LoadScene("UpdatedGameplay");
        });
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + (radius * Mathf.Cos(angle - (Mathf.PI / 2))), transform.position.y + (radius * Mathf.Sin(angle - (Mathf.PI / 2))), 0.0f);

        timeSinceLastReset += Time.deltaTime;
        timePerAngle += Time.deltaTime;
        if(timePerAngle >= .5f)
        {
            angle = Random.Range(angle - (Mathf.PI / 2), angle + (Mathf.PI / 2));
            timePerAngle = 0;
        }
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle * 180 / Mathf.PI);
        
        if (timeSinceLastReset < 1f) { return; }

        RectTransform canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        
        if (transform.position.x < canvas.TransformPoint(canvas.rect.center).x * .05f ||
            transform.position.x > canvas.TransformPoint(canvas.rect.center).x * 2.05f ||
            transform.position.y < canvas.TransformPoint(canvas.rect.center).y * .05f ||
            transform.position.y > canvas.TransformPoint(canvas.rect.center).y * 2.05f)
        {
            timeSinceLastReset = 0;
            angle -= Mathf.PI;
        }
    }
}
