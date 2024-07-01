using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCheckManager : MonoBehaviour
{
    public GameObject skillCheckCircle;
    RectTransform circleRect;
    [SerializeField] GameObject skillCheckObject;
    [SerializeField] SkillChecker skillChecker;
    public List<GameObject> CriticalArea;
    public List<GameObject> NormalArea;


    [SerializeField] float skillCheckCoolTime;

    [SerializeField] bool isSkillChecking;
    [SerializeField] bool IsRotatable;

    public bool IsSkillChecking
    {
        get { return isSkillChecking; }
        set
        {
            if (isSkillChecking != value)
            {
                isSkillChecking = value;
            }
        }
    }

    private void Start()
    {
        circleRect = skillCheckCircle.GetComponent<RectTransform>();
        skillChecker.InitCircle(this);
        StartCoroutine(CorSkillCheck());
    }

    private void OnEnable()
    {
        skillChecker.OnSkillCheckEnd += OnSkillCheckEnd_DisableObject;
    }

    private void OnDisable()
    {
        skillChecker.OnSkillCheckEnd -= OnSkillCheckEnd_DisableObject;
    }

    private void SkillCheckStart()
    {
        if (IsRotatable)
            RotateCircle();
        skillCheckObject.SetActive(true);
    }


    void RotateCircle()
    {
        float rot = UnityEngine.Random.Range(72f, 300.0f);
        circleRect.eulerAngles = new Vector3(0.0f, 0.0f, rot);
    }

    void OnSkillCheckEnd_DisableObject()
    {
        skillCheckObject.SetActive(false);
    }

    IEnumerator CorSkillCheck()
    {
        while (true)
        {
            if (!IsSkillChecking)
            {
                yield return null;
                continue;
            }
            SkillCheckStart();
            yield return new WaitForSeconds(skillCheckCoolTime);
        }
    }

    public SkillChecker GetSkillChecker()
    {
        return skillChecker;
    }

}

