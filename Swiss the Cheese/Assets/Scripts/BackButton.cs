using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //If we are in options
        if(SceneManager.GetActiveScene().name == "Options")
        {
            //Load all of the sound changes that you can make in options
            AudioMixer audioMixer = GameObject.FindObjectOfType<SoundManager>().audioMixer;
            Slider masterSlider = GameObject.FindGameObjectWithTag("MasterVolSlider").GetComponent<Slider>();
            Slider fxSlider = GameObject.FindGameObjectWithTag("FXVolSlider").GetComponent<Slider>();
            Slider musicSlider = GameObject.FindGameObjectWithTag("MusicVolSlider").GetComponent<Slider>();

            audioMixer.GetFloat("masterVolume", out float masterValue);
            audioMixer.GetFloat("soundFXVolume", out float soundFXValue);
            audioMixer.GetFloat("musicVolume", out float musicValue);

            masterSlider.value = Mathf.Pow(10, masterValue / 20);
            fxSlider.value = Mathf.Pow(10, soundFXValue / 20);
            musicSlider.value = Mathf.Pow(10, musicValue / 20);

            masterSlider.onValueChanged.AddListener(delegate
            {
                audioMixer.SetFloat("masterVolume", 20f * Mathf.Log10(masterSlider.value));
                PlayerPrefs.SetFloat("masterVolume", 20f * Mathf.Log10(masterSlider.value));
            });
            fxSlider.onValueChanged.AddListener(delegate
            {
                audioMixer.SetFloat("soundFXVolume", 20f * Mathf.Log10(fxSlider.value));
                PlayerPrefs.SetFloat("soundFXVolume", 20f * Mathf.Log10(fxSlider.value));
            });
            musicSlider.onValueChanged.AddListener(delegate
            {
                audioMixer.SetFloat("musicVolume", 20f * Mathf.Log10(musicSlider.value));
                PlayerPrefs.SetFloat("musicVolume", 20f * Mathf.Log10(musicSlider.value));
            });
        }

        //Regardless of whether or not we're in options
        //Add the listener to return to menu
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(delegate
        {
            SceneManager.LoadScene("Menu");
        });
    }
}
