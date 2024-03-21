using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource soundFXObject;
    [SerializeField] AudioSource musicObject;

    [SerializeField] public AudioMixer audioMixer;

    //We want the sound manager to always be preserved over the course of multiple scenes
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instantiate(musicObject, transform);
    }

    //Play a random sound effect from a list
    public AudioSource PlayRandomSoundFX(AudioClip[] audioClips, Transform spawnTransform, float volume, float clipLength)
    {
        return PlaySoundFXClip(audioClips[Random.Range(0, audioClips.Length)], spawnTransform, volume, clipLength);
    }

    //Play a specific sound effect clip
    public AudioSource PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume, float clipLength)
    {
        //spawn in gameobject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audio clip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        return audioSource;
    }
}
