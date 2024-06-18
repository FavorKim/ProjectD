using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorManager : MonoBehaviour
{
    List<Generator> generators = new List<Generator>();
    [SerializeField] int m_GeneratorsRemaining;
    [SerializeField] Text Text_GenCount;

    private void Start()
    {
        foreach(Generator g in GetComponentsInChildren<Generator>())
        {
            generators.Add(g);
            g.OnCompleteHandler += OnGeneratorComplete;
        }
        m_GeneratorsRemaining = generators.Count;
        Text_GenCount.text = m_GeneratorsRemaining.ToString();
    }

    void OnGeneratorComplete()//서버 작업 필
    {
        m_GeneratorsRemaining--;
        Text_GenCount.text = m_GeneratorsRemaining.ToString();
    }
}
