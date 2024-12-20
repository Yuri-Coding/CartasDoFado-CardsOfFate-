﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private EventInstance backgroundMusic;
    private EventInstance ambientSound;

    private List<EventInstance> eventInstances;
    private EventInstance musicEventInstance;

    private Musics playingNow;

    //Configura uma Instância para tornar acessível por todo projeto.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        eventInstances = new List<EventInstance>();
    }

    private void Start()
    {
        InitializeMusic(FMODEvents.Instance.Music);
    }
    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
        //musicEventInstance.setParameterByName("track", 1f);


    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
    
    public void SetMusic(Musics music)
    {
        //Debug.Log($"SetMusic() called to {music}");

        if (musicEventInstance.isValid())
        {
            FMOD.RESULT result = musicEventInstance.setParameterByName("track", (float)music);
            if (result != FMOD.RESULT.OK)
            {
                Debug.LogError($"Failed to set parameter 'track': {result}");
            }
        }
        else
        {
            Debug.LogError("musicEventInstance is not valid when trying to set 'track' parameter.");
        }
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    // Controle de volume geral
    public void SetVolume(float volume)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Volume", volume);
    }

    public void ChangeMusicByTensionIndicator(int tensionIndicator)
    {
        Debug.Log(tensionIndicator);
        Musics toPlay = Musics.Respiracao;
        switch (tensionIndicator)
        {
            case 0: case 1:  toPlay = Musics.CantoDaVila; break;
            case 2: case 3:  toPlay = Musics.Respiracao;  break;
            case 4: case 5:  toPlay = Musics.Duvida;      break;
            case 6: case 7:  toPlay = Musics.AQueCusto;   break;
            case 8: case 9:  toPlay = Musics.Postuma;     break;
        }
        if (tensionIndicator >= 10) toPlay = Musics.Postuma; // Alterar para o Track Aterrorizante

        if (toPlay != playingNow)
        {
            SetMusic(toPlay);
        }
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}