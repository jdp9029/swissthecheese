using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunningManager : MonoBehaviour
{
    [SerializeField] List<Sprite> sprites;
    int spriteWeAreAt = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindObjectOfType<ZoomManager>().IsZooming || GameObject.FindObjectOfType<BitingManager>().IsBiting)
        {
            return;
        }
        GameObject.FindGameObjectWithTag("Mouse").GetComponent<Image>().sprite = sprites[spriteWeAreAt % sprites.Count];
        spriteWeAreAt++;
    }
}
