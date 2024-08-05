using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSkinLoader : MonoBehaviour
{
    [SerializeField]
    public List<DictItem> Skins;
    
    [SerializeField]
    public List<DictItem> Accessories;

    [HideInInspector]
    int coins;

    //We want the sound manager to always be preserved over the course of multiple scenes
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        string[] uSkins = PlayerPrefs.GetString("Skins").Split(',');
        string[] uAccessories = PlayerPrefs.GetString("Accessories").Split(',');
        coins = PlayerPrefs.GetInt("Coins", 0);
        
        foreach (var skin in Skins)
        {
            skin.IsUnlocked = uSkins.Contains(skin.Name);
        }
        foreach (var a in Accessories)
        {
            a.IsUnlocked = uAccessories.Contains(a.Name);
        }
    }
    
    public void UnlockItem(DictItem item)
    {
        item.IsUnlocked = true;
        coins -= item.ItemCost;
        PlayerPrefs.SetInt("Coins", coins);
	PlayerPrefs.SetString("Skins", String.Join(',', Skins.Where(i => i.IsUnlocked).Select(i => i.Name).ToArray()));
        PlayerPrefs.SetString("Accessories", String.Join(',', Accessories.Where(i => i.IsUnlocked).Select(i => i.Name).ToArray()));
    }

    public void UnlockItem(string name)
    {
        if (Skins.Select(i => i.Name == name).Any())
        {
            UnlockItem(Skins.First(i => i.Name == name));
        }
        UnlockItem(Accessories.First(i => i.Name == name));
    }
}