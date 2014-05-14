using UnityEngine;
using System.Collections;

public class SoundFxManager : MonoBehaviour 
{
    internal static SoundFxManager instance;

    public AudioSource wrongTapSound;
    public AudioSource objectPickSound;
    public AudioSource whopSound;
    public AudioSource tickSound;
    public AudioSource electricZapSound;
    public AudioSource wrongMoveSound;
    public AudioSource columnFallSound;

    public AudioSource []objectFall;

    void Awake()
    {
        instance = this;
    }

    bool playFallingSound = false;

    void Start()
    {
        InvokeRepeating("CheckForFallingSound", .1f, .5f);
    }

    void CheckForFallingSound()
    {
        if (playFallingSound)
        {
            objectFall[Random.Range(0, 3)].Play();
        }
    }

    internal void PlayLevelAdvanceSound()
    {
       
    }

    internal void PlayFallingSound()
    {
        playFallingSound = true;
    }

    internal void StopFallingSound()
    {
        playFallingSound = false;
    }

    internal void PlayLevelClearSound()
    {
       
    }

    internal void PlayLevelFailSound()
    {
       
    }
   
}
