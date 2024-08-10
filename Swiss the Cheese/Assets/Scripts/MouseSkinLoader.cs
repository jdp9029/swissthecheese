using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSkinLoader : MonoBehaviour
{
    private int coins;

    [SerializeField]
    public List<DictItem> Skins;
    
    [SerializeField]
    public List<DictItem> Accessories;

    [HideInInspector]
    public int Coins
    {
        get
        {
            return coins;
        }
        set
        {
            PlayerPrefs.SetInt("Coins", value);
            coins = value;
        }
    }

    [SerializeField]
    public DictItem EquippedSkin;

    [SerializeField]
    public DictItem EquippedTopAccessory;

    [SerializeField]
    public DictItem EquippedBottomAccessory;

    //We want the sound manager to always be preserved over the course of multiple scenes
    public void Load()
    {
        DontDestroyOnLoad(gameObject);
        
        coins = PlayerPrefs.GetInt("Coins", 0);

        if (!PlayerPrefs.HasKey("EquippedSkin"))
        {
            UnlockItem("Nibbles");
            UnlockItem("No Top");
            UnlockItem("No Bottom");

            EquippedSkin = Skins[0];
            EquippedTopAccessory = Accessories[0];
            EquippedBottomAccessory = Accessories[1];
            PlayerPrefs.SetString("EquippedSkin", "Nibbles");
            PlayerPrefs.SetString("EquippedTopAccessory", "No Top");
            PlayerPrefs.SetString("EquippedBottomAccessory", "No Bottom");
            Debug.Log("Loaded skins");
        }
        else
        {
            string[] uSkins = PlayerPrefs.GetString("Skins", string.Empty).Split(',');
            string[] uAccessories = PlayerPrefs.GetString("Accessories", string.Empty).Split(',');
            EquippedSkin = Skins.First(i => i.Name == PlayerPrefs.GetString("EquippedSkin"));
            EquippedTopAccessory = Accessories.First(i => i.Name == PlayerPrefs.GetString("EquippedTopAccessory"));
            EquippedBottomAccessory = Accessories.First(i => i.Name == PlayerPrefs.GetString("EquippedBottomAccessory"));
            foreach (var skin in Skins)
            {
                skin.IsUnlocked = uSkins.Contains(skin.Name);
            }
            foreach (var a in Accessories)
            {
                a.IsUnlocked = uAccessories.Contains(a.Name);
            }
        }

    }
    
    public void UnlockItem(DictItem item)
    {
        item.IsUnlocked = true;
        Coins -= item.ItemCost;
	    PlayerPrefs.SetString("Skins", String.Join(',', Skins.Where(i => i.IsUnlocked).Select(i => i.Name).ToArray()));
        PlayerPrefs.SetString("Accessories", String.Join(',', Accessories.Where(i => i.IsUnlocked).Select(i => i.Name).ToArray()));
    }

    public void UnlockItem(string name)
    {
        if (Skins.Where(i => i.Name == name).Any())
        {
            UnlockItem(Skins.First(i => i.Name == name));
            return;
        }
        UnlockItem(Accessories.First(i => i.Name == name));
    }
}