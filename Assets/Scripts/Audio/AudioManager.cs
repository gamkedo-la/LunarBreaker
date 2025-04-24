using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioClip[] gunshotClipsMac10;
    public AudioClip[] gunshotClipsRevolver;

    public static AudioManager Instance { get; private set; }

    private AudioSource ASfootsteps;


    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Cannot add duplicate static AudioManager");
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
     
    }

public void PlayGunshot(bool usingMac10, GameObject caller)
{
    AudioClip[] gunshotClips = (usingMac10 ? gunshotClipsMac10 : gunshotClipsRevolver);

    if (gunshotClips == null || gunshotClips.Length == 0)
    {
        Debug.LogWarning("Tried to play clip from empty gunshotClips array");
        return;
    }

    if (caller == null)
    {
        Debug.LogWarning("Caller GameObject is null!");
        return;
    }

    // Get or add an AudioSource to the calling object
    AudioSource callerAudio = caller.GetComponent<AudioSource>();
    if (callerAudio == null)
    {
        callerAudio = caller.AddComponent<AudioSource>();
    }

    int i = Random.Range(0, gunshotClips.Length);
    callerAudio.PlayOneShot(gunshotClips[i]); // Play sound on the caller    
}
}
