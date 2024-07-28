using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopTabManager : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
	{
	   var obj = transform.GetChild(i);
	   obj.Find("Hitbox").GetComponent<Button>().onClick.AddListener(delegate
	   {
		obj.SetAsLastSibling();
	   });	
	} 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
