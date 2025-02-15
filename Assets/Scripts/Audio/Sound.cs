using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UIElements;
using System;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    public AudioMixerGroup mixerGroup;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 2f)]
    public float pitch = 1f;
    

    [HideInInspector]
    public AudioSource source;
}