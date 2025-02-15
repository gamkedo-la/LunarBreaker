using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            
            s.source.volume = 1;
            s.source.pitch = s.pitch;

            if (s.mixerGroup != null)
            {
                s.source.outputAudioMixerGroup = s.mixerGroup;
            }                    

            s.source.playOnAwake = false;
        }        
    }

    public void PlaySound (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s.source.isPlaying)
        {
            s.source.Stop();
        }        
        s.source.Play();
    }
}
