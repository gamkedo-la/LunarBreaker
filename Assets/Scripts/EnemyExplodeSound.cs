using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplodeSound : MonoBehaviour
{
    private static AudioManager soundMaker;

    void Start()
    {
        if (soundMaker == null)
        {
            soundMaker = Camera.main.GetComponent<AudioManager>();
        }
        soundMaker.PlayRandSound(AudioManager.SoundType.enemyExplode, gameObject);
    }
}
