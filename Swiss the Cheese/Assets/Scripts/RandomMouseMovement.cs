using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMouseMovement : MonoBehaviour
{
    private float timePerAngle = 0;
    private float angleChange;
    // Start is called before the first frame update
    void Start()
    {
        angleChange = Random.Range(-10f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(Mathf.Cos((transform.rotation.z - 90) * Mathf.PI / 180), Mathf.Sin((transform.rotation.z - 90) * Mathf.PI / 180), 0f) * 50 * Time.deltaTime;
        timePerAngle += Time.deltaTime;
        if(timePerAngle >= 10)
        {
            angleChange = Random.Range(-10f, 10f);
            timePerAngle = 0;
        }
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + angleChange);
    }
}
