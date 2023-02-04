using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace TeamShrimp.GGJ23
{
    public class SoundControl : MonoBehaviour
    {
        public AudioMixer mixer;

        public void SetLevel(float sliderVal)
        {
            mixer.SetFloat("musicVol", Mathf.Log10(sliderVal)*20);
        }
    }
}
