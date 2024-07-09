using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MySoundManager : SingletonMono<MySoundManager>
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] GameObject SettingPopup;

    private void Start()
    {
        if(instance!=null && instance.gameObject != gameObject)
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
        audioMixer.SetFloat("Master", value * 100 - 80);
    }

    public void OnChangedValue_SFXSound(Single value)
    {
        audioMixer.SetFloat("SFX", value * 100 - 80);
    }
}
