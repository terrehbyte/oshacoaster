using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignMusicPlaylist : MonoBehaviour
{
    public AudioClip[] designModeMusic;
    public AudioSource audioSource;

    public void ModeChange(bool isToggleOn)
    {
        //bool isToggleOn = _bool;

        if(isToggleOn == true)
        {
            audioSource.clip = designModeMusic[2];
            audioSource.Play();
        }
        
        if(isToggleOn == false)
        {
            audioSource.clip = GetRandomClip();
            audioSource.Play();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //audioSource = FindObjectOfType<AudioSource>();

        audioSource.loop = false;
    }

    private AudioClip GetRandomClip()
    {
        return designModeMusic[Random.Range(0, designModeMusic.Length-1)];
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = GetRandomClip();
            audioSource.Play();
        }
    }
}
