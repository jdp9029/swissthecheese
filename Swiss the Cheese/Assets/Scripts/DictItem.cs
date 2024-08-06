using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DictItem
{
    [SerializeField]
    public string Name;
    
    [SerializeField]
    public Sprite Sprite;

    [SerializeField]
    public int ItemCost;

    [HideInInspector]
    public bool IsUnlocked;

    [SerializeField]
    public string Description;

    [SerializeField]
    public bool IsHat;

    public void UnlockItem()
    {
        GameObject.FindObjectOfType<MouseSkinLoader>().UnlockItem(this);
    }
}