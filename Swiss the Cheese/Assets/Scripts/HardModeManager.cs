using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardModeManager : MonoBehaviour
{
    public static Modes Mode;

    //We want the sound manager to always be preserved over the course of multiple scenes
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public enum Modes
    {
        NORMAL, HARD, TWICEMICE
    }
}
