using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXPlay : MonoBehaviour
{
    FMOD.Studio.EventInstance PlayMoneySound;
    FMOD.Studio.PLAYBACK_STATE playbackState;
    //public Button buyButton;

    //void Awake()
    //{
    //    PlayMoneySound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/BoughtItem");

    //}

    void start()
    {
        PlayMoneySound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/BoughtItem");
        //Button btn = buyButton.GetComponent<Button>();
        //btn.onClick.AddListener(taskToClick);
        //btn.onClick.AddListener(playSound);
        //PlayMoneySound.start();

    }

    public void playSound()
    {
        PlayMoneySound.getPlaybackState(out playbackState);
        //PlayMoneySound.start();
        Debug.Log(PlayMoneySound);
        Debug.Log(playbackState);
        //Debug.Log(PlayMoneySound);
        //if (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED)
        //{
        //    PlayMoneySound.start();

        //}
        //Debug.Log(playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED);
        
    }


    //void playSound()
    //{
    //    PlayMoneySound.getPlaybackState(out playbackState);
    //    Debug.Log(playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED);
    //    PlayMoneySound.start();


    //    //layMoneySound.start();
    //    //Debug.Log("Clicked");
    //}

    // Update is called once per frame
    //void Update()
    //{
    //    Button btn = buyButton.GetComponent<Button>();
    //    btn.onClick.AddListener(playSound);
    //}
}

