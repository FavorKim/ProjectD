using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    public enum Icons
    {
        Healthy,
        Injured,
        Down,
        Hanged,
        Killed,
        Escaped
    }

    [SyncVar(hook =nameof(Hook_OnChangedParent))] public GameObject parent;

    [SerializeField]Slider Slider_Gauge;
    Dictionary<Icons, Image> IconDict = new Dictionary<Icons, Image>();
    GameObject curIcon;

    private void Start()
    {
        IconDict.Clear();
        var imgs = GetComponentsInChildren<Image>();
        for (int i = 0; i < 6; i++)
        {
            //if (imgs[i].transform.root.GetComponent<Slider>() != null) continue;
            IconDict.Add((Icons)i, imgs[i]);
            imgs[i].gameObject.SetActive(false);
        }
        Slider_Gauge.gameObject.SetActive(false);
        SetIcon(Icons.Healthy);
    }

    public void SetIcon(Icons state)
    {
        //if (state == (Icons)HealthStates.Held) return;


        curIcon?.gameObject.SetActive(false);
        curIcon = IconDict[state].gameObject;
        curIcon.SetActive(true);
        if((int)state == (int)Icons.Hanged)
            Slider_Gauge.gameObject.SetActive(true);

        else 
            Slider_Gauge.gameObject.SetActive(false);
    }
    public void SetIcon(HealthStates state)
    {
        if (state == HealthStates.Held) return;

        var iconEnum = (Icons)state;

        curIcon?.gameObject.SetActive(false);
        curIcon = IconDict[iconEnum].gameObject;
        curIcon.SetActive(true);

        if ((int)iconEnum == (int)Icons.Hanged)
            Slider_Gauge.gameObject.SetActive(true);
        else
            Slider_Gauge.gameObject.SetActive(false);
    }

    
    public void OnCorruptTimeChanged(float value)
    {
        Slider_Gauge.value = value;
    }

    void Hook_OnChangedParent(GameObject old,  GameObject recent)
    {
        gameObject.transform.parent = recent.transform;
        gameObject.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
    }
}
