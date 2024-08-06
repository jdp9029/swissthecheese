using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopTabManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI bigMouseName;

    [SerializeField]
    TextMeshProUGUI bigMouseDescription;

    [SerializeField]
    List<Image> bigMouse;

    [SerializeField]
    List<Image> topAccessory;

    [SerializeField]
    List<Image> bottomAccessory;

    [HideInInspector]
    public DictItem SelectedSkin;

    [HideInInspector]
    public DictItem SelectedTopAccessory;

    [HideInInspector]
    public DictItem SelectedBottomAccessory;

    [HideInInspector]
    MouseSkinLoader msl;

    [HideInInspector]
    DictItem[] topAccessories;

    [HideInInspector]
    DictItem[] bottomAccessories;

    // Start is called before the first frame update
    void Start()
    {
        msl = FindObjectOfType<MouseSkinLoader>();
        topAccessories = msl.Accessories.Where(i => i.IsHat).ToArray();
        bottomAccessories = msl.Accessories.Where(i => !i.IsHat).ToArray();
        SelectedTopAccessory = topAccessories[0];
        SelectedBottomAccessory = bottomAccessories[0];

        for(int i = 0; i < transform.childCount; i++)
	    {
	       var obj = transform.GetChild(i);
	       obj.Find("Hitbox").GetComponent<Button>().onClick.AddListener(delegate
	       {
               obj.SetAsLastSibling();
	       });	
	    }
	    SelectSkin("Nibbles");
        transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        bigMouseName.text = SelectedSkin.Name;
        bigMouseDescription.text = SelectedSkin.Description;
        
        foreach (var mouse in bigMouse)
        {
            mouse.sprite = SelectedSkin.Sprite;
        }
        
        foreach (var img in topAccessory)
        {
            img.sprite = SelectedTopAccessory.Sprite;
            if (SelectedTopAccessory == topAccessories[0])
            {
                img.color = new Color32(255, 255, 255, 0);
            }
            else
            {
                img.color = new Color32(255, 255, 255, 255);
            }
        }
        
        foreach (var img in bottomAccessory)
        {
            img.sprite = SelectedBottomAccessory.Sprite;
            if (SelectedBottomAccessory == bottomAccessories[0])
            {
                img.color = new Color32(255,255,255, 0);
            }
            else
            {
                img.color = new Color32(255, 255, 255, 255);
            }
        }
    }

    public void SelectSkin(string name)
    {
        SelectedSkin = FindObjectOfType<MouseSkinLoader>().Skins.First(i => i.Name == name);
    }

    public void NextTopAccessory()
    {
        SelectedTopAccessory = topAccessories[(Array.IndexOf(topAccessories, SelectedTopAccessory) + 1) % topAccessories.Length];
    }
    
    public void NextBottomAccessory()
    {
        SelectedBottomAccessory = bottomAccessories[(Array.IndexOf(bottomAccessories, SelectedBottomAccessory) + 1) % bottomAccessories.Length];
    }
    
    public void PrevTopAccessory()
    {
        SelectedTopAccessory = topAccessories[(Array.IndexOf(topAccessories, SelectedTopAccessory) + topAccessories.Length - 1) % topAccessories.Length];
    }
    
    public void PrevBottomAccessory()
    {
        SelectedBottomAccessory = bottomAccessories[(Array.IndexOf(bottomAccessories, SelectedBottomAccessory) + bottomAccessories.Length - 1) % bottomAccessories.Length];
    }
}
