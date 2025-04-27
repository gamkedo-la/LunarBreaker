using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioClip[] gunshotClipsMac10;
    public AudioClip[] gunshotClipsRevolver;
    public AudioClip[] shellBounceClips;
    public AudioClip[] emptyGunClips;
    public AudioClip[] playerDamagedClips;
    public AudioClip[] playerJumpClips;
    public AudioClip[] playerLandClips;
    public AudioClip[] enemyExplodeClips;

    public enum SoundType { mac10, revolver, shells, emptyGun, playerDamaged, playerJump, playerLand, enemyExplode };

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

public void PlayRandSound(SoundType whichSound, GameObject caller)
{
        AudioClip[] gunshotClips;
        switch(whichSound)
        {
            case SoundType.mac10:
                gunshotClips = gunshotClipsMac10;
                break;
            case SoundType.revolver:
                gunshotClips = gunshotClipsRevolver;
                break;
            case SoundType.shells:
                gunshotClips = shellBounceClips;
                break;
            case SoundType.emptyGun:
                gunshotClips = emptyGunClips;
                break;
            case SoundType.playerJump:
                gunshotClips = playerJumpClips;
                break;
            case SoundType.playerLand:
                gunshotClips = playerLandClips;
                break;
            case SoundType.enemyExplode:
                gunshotClips = enemyExplodeClips;
                break;
            case SoundType.playerDamaged:
            default:
                gunshotClips = playerDamagedClips;
                break;
        }

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
    callerAudio.spatialBlend = 0.5f;

        int i = Random.Range(0, gunshotClips.Length);
    callerAudio.PlayOneShot(gunshotClips[i]); // Play sound on the caller
}
}
