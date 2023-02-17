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
    void Awake()
    {
        master = PlayerPrefs.GetFloat("MasterVolume");
        music = PlayerPrefs.GetFloat("MusicVolume");
        sfx = PlayerPrefs.GetFloat("SFXVolume");

        if (!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetFloat("MasterVolume", .5f);
            PlayerPrefs.SetFloat("MusicVolume", .5f);
            PlayerPrefs.SetFloat("SFXVolume", .5f);
        }

        main.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
        main.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        main.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume"));
        // main.SetFloat("MasterVolume", master);
        // main.SetFloat("MusicVolume", music);
        // main.SetFloat("SFXVolume", sfx);
    }
}
