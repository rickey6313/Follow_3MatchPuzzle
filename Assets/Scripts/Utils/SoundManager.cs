using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Clip
{
    Chomp = 0,
    BlockClear = 1
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private AudioSource[] sfx;

    // Start is called before the first frame update
    void Start()
    {
        instance = GetComponent<SoundManager>();
        sfx = GetComponents<AudioSource>();
    }

    public void PlayOneShot(Clip audioClip)
    {
        sfx[(int)audioClip].Play();
    }

    public void PlayOneShot(Clip audioClip, float volumeScale)
    {
        AudioSource source = sfx[(int)audioClip];
        source.PlayOneShot(source.clip, volumeScale);
    }
}
