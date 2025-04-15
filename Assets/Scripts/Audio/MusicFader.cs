using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFader : MonoBehaviour
{
    public AudioSource normalMusic;
    public AudioSource actionMusic;

    float actionMusicTime;
    float fadeTime = 3.0f;
    float musicCombatTime = 6.0f;
    float musicVolMax = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        normalMusic.volume = musicVolMax;
        actionMusic.volume = 0.0f;
    }

    public void CombatMusicBump()
    {
        actionMusicTime = musicCombatTime;
        // TODO/note: no fade in yet, just fade out
        normalMusic.volume = 0.0f;
        actionMusic.volume = musicVolMax;
    }

    // Update is called once per frame
    void Update()
    {
        if(actionMusicTime>0.0f)
        {
            actionMusicTime -= Time.deltaTime;
            if(actionMusicTime < fadeTime)
            {
                float percShift = (fadeTime - actionMusicTime)/fadeTime;
                normalMusic.volume = musicVolMax * percShift;
                actionMusic.volume = musicVolMax * (1.0f-percShift);
            }
        }
        
    }
}
