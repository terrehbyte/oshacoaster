using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignMusicPlaylist : MonoBehaviour
{
    public AudioClip[] designModeMusic;
    public AudioClip[] playModeMusic;
    public AudioSource audioSource;
    public bool isMode;

    public void ModeChange(bool isToggleOn)
    {
        //bool isToggleOn = _bool;
        isMode = isToggleOn;
        if(isToggleOn == true)
        {
            audioSource.clip = GetRandomPlay();
            audioSource.Play();
        }
        
        if(isToggleOn == false)
        {
            audioSource.clip = GetRandomDesign();
            audioSource.Play();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //audioSource = FindObjectOfType<AudioSource>();

        audioSource.loop = false;
    }

    private AudioClip GetRandomPlay()
    {
        return playModeMusic[Random.Range(0, playModeMusic.Length - 1)];
    }

    private AudioClip GetRandomDesign()
    {
        return designModeMusic[Random.Range(0, designModeMusic.Length-1)];
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying && isMode == false)
        {
            audioSource.clip = GetRandomDesign();
            audioSource.Play();
        }

        if (!audioSource.isPlaying && isMode == true)
        {
            audioSource.clip = GetRandomPlay();
            audioSource.Play();
        }

    }
}
