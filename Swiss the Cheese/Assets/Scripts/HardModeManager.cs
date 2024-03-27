using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardModeManager : MonoBehaviour
{
    public static bool HardMode;

    //We want the sound manager to always be preserved over the course of multiple scenes
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
