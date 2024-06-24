using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorManager : NetworkBehaviour
{
    List<Generator> generators = new List<Generator>();
    [SerializeField] Lever m_lever;
    //[SerializeField] int m_GeneratorsRemaining;
    public int GeneratorRemaining
    {
        get
        {
            int count = 0;
            foreach(Generator g in generators)
            {
                if (!g.IsCompleted)
                    count++;
            }
            return count;
        }
    }
    [SerializeField] Text Text_GenCount;

    private void Start()
    {
        foreach(Generator g in GetComponentsInChildren<Generator>())
        {
            generators.Add(g);
            g.OnCompleteHandler += DecreasRemainingGen;
            g.OnCompleteHandler += OpenDoor;
        }
        //m_GeneratorsRemaining = generators.Count;
        Text_GenCount.text = GeneratorRemaining.ToString();
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
        //m_GeneratorsRemaining--;
        Text_GenCount.text = GeneratorRemaining.ToString();
    }


    void OpenDoor()
    {
        if(GeneratorRemaining == 0)
        {
            m_lever.IsAvailable = true;
        }
    }
}
