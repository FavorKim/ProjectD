using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ConfigManager : SingletonMono<ConfigManager>
{

    private enum Resolution
    {
        FHD,
        HD,
        NHD
    }

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] GameObject SettingPopup;

    private bool isWindowed;
    private bool IsWindowed
    {
        get
        {
            return isWindowed;
        }
        set
        {
            if (isWindowed != value)
            {
                isWindowed = value;
                SetScreenConfig();
            }
        }
    }


    private Resolution currentResolution;
    private Resolution CurrentResolution
    {
        get { return currentResolution; }
        set
        {
            if(currentResolution != value)
            {
                currentResolution = value;
                SetScreenConfig();
            }
        }
    }

    
    

    private void Start()
    {
        if (instance != null && instance.gameObject != gameObject)
        {
            DestroyImmediate(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SettingPopup.SetActive(!SettingPopup.activeSelf);
        }
    }


    public void OnChangedValue_MasterSound(Single value)
    {
        audioMixer.SetFloat("Master", value * 80 - 80);
    }

    public void OnChangedValue_SFXSound(Single value)
    {
        audioMixer.SetFloat("SFX", value * 80 - 80);
    }

    public void OnValueChanged_SetResolution(Int32 value)
    {
        CurrentResolution = (Resolution)value;
    }

    public void OnClick_SetFullScreenMode()
    {
        IsWindowed = !isWindowed;
    }
    

    private void SetScreenConfig()
    {
        switch (CurrentResolution) 
        {
            case Resolution.FHD:
                Screen.SetResolution(1920, 1080, IsWindowed);
                break;

            case Resolution.HD:
                Screen.SetResolution(1280, 720, IsWindowed);
                break;

            case Resolution.NHD:
                Screen.SetResolution(640, 360, IsWindowed);
                break;
        }
    }
}
