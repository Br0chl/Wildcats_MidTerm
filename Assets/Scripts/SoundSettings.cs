using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] AudioMixer main;
    public float master;
    public float music;
    public float sfx;


    // Start is called before the first frame update
    void Start()
    {
        master = PlayerPrefs.GetFloat("MasterVolume");
        music = PlayerPrefs.GetFloat("MusicVolume");
        sfx = PlayerPrefs.GetFloat("SFXVolume");

        main.SetFloat("MasterVolume", master);
        main.SetFloat("MusicVolume", music);
        main.SetFloat("SFXVolume", sfx);
    }
}
