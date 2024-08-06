using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRecorder : MonoBehaviour
{
    int count = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //ScreenCapture.CaptureScreenshot($"C:\\Users\\james\\Downloads\\screenshot-{count++}.png");
            
        }
    }
}
