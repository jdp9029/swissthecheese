using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource soundFXObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AudioSource PlayRandomSoundFX(AudioClip[] audioClips, Transform spawnTransform, float volume, float clipLength)
    {
        return PlaySoundFXClip(audioClips[Random.Range(0, audioClips.Length)], spawnTransform, volume, clipLength);
    }

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
