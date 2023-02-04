using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource AnimationAudioSource;

    public void PlayAnimationAudio()
    {
        AnimationAudioSource.Play();
    }
}
