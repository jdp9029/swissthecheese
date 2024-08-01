using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSkinLoader : MonoBehaviour
{
    [SerializeField]
    List<DictItem> Skins;
    
    [SerializeField]
    List<DictItem> Accessories;

    //We want the sound manager to always be preserved over the course of multiple scenes
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}

[Serializable]
public class DictItem
{
    [SerializeField]
    public string Name;
    
    [SerializeField]
    public Sprite Sprite;

    [SerializeField]
    public Vector2 Anchors;

    [HideInInspector]
    public bool IsUnlocked;
}