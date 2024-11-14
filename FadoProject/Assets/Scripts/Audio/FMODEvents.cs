using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents Instance { get; private set; }

    [field: Header("SFX")]
    [field: SerializeField] public EventReference CardDrew { get; private set; }
    [field: SerializeField] public EventReference Clap { get; private set; }
    [field: SerializeField] public EventReference Snap { get; private set; }
    [field: SerializeField] public EventReference PoisonVapor { get; private set; }
    [field: SerializeField] public EventReference Slide { get; private set; }
    [field: SerializeField] public EventReference Attack { get; private set; }
    [field: SerializeField] public EventReference Error { get; private set; }
    [field: SerializeField] public EventReference Moving { get; private set; }
    [field: SerializeField] public EventReference Positioning { get; private set; }
    [field: SerializeField] public EventReference Typewriter { get; private set; }
    [field: SerializeField] public EventReference Suzu { get; private set; }
    [field: SerializeField] public EventReference Slide2 { get; private set; }
    [field: SerializeField] public EventReference Magic { get; private set; }
    [field: SerializeField] public EventReference Boom { get; private set; }


    [field: Header("Ambience")]
    [field: SerializeField] public EventReference Ambience { get; private set; }


    [field: Header("Music")]
    [field: SerializeField] public EventReference Music { get; private set; }

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
    }


}
