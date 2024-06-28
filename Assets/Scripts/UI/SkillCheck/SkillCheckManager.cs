using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCheckManager : MonoBehaviour
{
    [SerializeField] GameObject skillCheckCircle;
    RectTransform circleRect;
    [SerializeField] GameObject skillCheckObject;
    [SerializeField] SkillChecker skillChecker;

    [SerializeField] float skillCheckCoolTime;

    [SerializeField] bool isSkillChecking;
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
        skillChecker.InitCircle(circleRect);
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
        RotateCircle();
        skillCheckObject.SetActive(true);
    }


    void RotateCircle()
    {
        float rot = UnityEngine.Random.Range(-60f, -280.0f);
        circleRect.localEulerAngles = new Vector3(0.0f, 0.0f, rot);
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
            yield return new WaitForSeconds(skillCheckCoolTime);
            SkillCheckStart();
        }
    }

    public SkillChecker GetSkillChecker()
    {
        return skillChecker;
    }

}

