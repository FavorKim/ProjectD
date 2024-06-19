using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorManager : MonoBehaviour
{
    List<Generator> generators = new List<Generator>();
    [SerializeField] Lever m_lever;
    [SerializeField] int m_GeneratorsRemaining;
    [SerializeField] Text Text_GenCount;

    private void Start()
    {
        foreach(Generator g in GetComponentsInChildren<Generator>())
        {
            generators.Add(g);
            g.OnCompleteHandler += DecreasRemainingGen;
            g.OnCompleteHandler += OpenDoor;
        }
        m_GeneratorsRemaining = generators.Count;
        Text_GenCount.text = m_GeneratorsRemaining.ToString();
    }
    private void OnDisable()
    {
        foreach(Generator g in GetComponentsInChildren<Generator>())
        {
            g.OnCompleteHandler -= DecreasRemainingGen;
            g.OnCompleteHandler -= OpenDoor;
        }
    }


    void DecreasRemainingGen()//서버 작업 필
    {
        m_GeneratorsRemaining--;
        Text_GenCount.text = m_GeneratorsRemaining.ToString();
        
    }
    void OpenDoor()
    {
        if(m_GeneratorsRemaining == 0)
        {
            m_lever.IsAvailable = true;
        }
    }
}
