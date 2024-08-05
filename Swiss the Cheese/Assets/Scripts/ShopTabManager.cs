using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
	SelectSkin("Nibbles");
	NextTopAccessory();
	NextBottomAccessory();
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
        }
        
        foreach (var img in bottomAccessory)
        {
            img.sprite = SelectedBottomAccessory.Sprite;
        }
    }

    public void SelectSkin(string name)
    {
        SelectedSkin = FindObjectOfType<MouseSkinLoader>().Skins.First(i => i.Name == name);
    }

    public void NextTopAccessory()
    {
        var msl = FindObjectOfType<MouseSkinLoader>();
	Debug.Log(msl.Accessories.Count);
        
        if (SelectedTopAccessory == null)
        {
            SelectedTopAccessory = msl.Accessories.First(i => i.IsHat);
        }
        else
        {
            var topAccessories = msl.Accessories.Where(i => i.IsHat).ToArray();
            for (int i = 0; i < topAccessories.Length; i++)
            {
                if (SelectedTopAccessory == topAccessories[i])
                {
                    SelectedTopAccessory = topAccessories[(i + 1) % topAccessories.Length];
                    return;
                }
            }
        }
    }
    
    public void NextBottomAccessory()
    {
        var msl = FindObjectOfType<MouseSkinLoader>();
        
        if (SelectedBottomAccessory == null)
        {
            SelectedBottomAccessory = msl.Accessories.First(i => !i.IsHat);
        }
        else
        {
            var bottomAccessories = msl.Accessories.Where(i => !i.IsHat).ToArray();
            for (int i = 0; i < bottomAccessories.Length; i++)
            {
                if (SelectedBottomAccessory == bottomAccessories[i])
                {
                    SelectedBottomAccessory = bottomAccessories[(i + 1) % bottomAccessories.Length];
                    return;
                }
            }
        }
    }


    
    public void PrevTopAccessory()
    {
        var msl = FindObjectOfType<MouseSkinLoader>();
        
        if (SelectedTopAccessory == null)
        {
            SelectedTopAccessory = msl.Accessories.First(i => i.IsHat);
        }
        else
        {
            var topAccessories = msl.Accessories.Where(i => i.IsHat).ToArray();
            for (int i = 0; i < topAccessories.Length; i++)
            {
                if (SelectedTopAccessory == topAccessories[i])
                {
                    SelectedTopAccessory = topAccessories[(i + topAccessories.Length - 1) % topAccessories.Length];
                    return;
                }
            }
        }
    }
    
    public void PrevBottomAccessory()
    {
        var msl = FindObjectOfType<MouseSkinLoader>();
        
        if (SelectedBottomAccessory == null)
        {
            SelectedBottomAccessory = msl.Accessories.First(i => !i.IsHat);
        }
        else
        {
            var bottomAccessories = msl.Accessories.Where(i => !i.IsHat).ToArray();
            for (int i = 0; i < bottomAccessories.Length; i++)
            {
                if (SelectedBottomAccessory == bottomAccessories[i])
                {
                    SelectedBottomAccessory = bottomAccessories[(i + bottomAccessories.Length - 1) % bottomAccessories.Length];
                    return;
                }
            }
        }
    }
}
