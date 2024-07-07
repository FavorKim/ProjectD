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

    [SerializeField]float timeToSuccess;
    public float GetTimeToSuccess() { return timeToSuccess; }

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

    public void SkillCheckStart()
    {
        if (IsRotatable)
            RotateCircle();
        skillCheckObject.SetActive(true);
    }
    public void SkillCheckStop()
    {
        skillCheckObject.SetActive(false);
    }


    void RotateCircle()
    {
        float checkTime = skillChecker.GetCheckerRotateSpeed();


        timeToSuccess = UnityEngine.Random.Range(checkTime * 0.1667f, checkTime * 0.8f);
        float rot = timeToSuccess / checkTime * 360;
        circleRect.eulerAngles = new Vector3(0, 0, -rot);
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

    public void RegisterOnSkillSuccess(Action onSkillSuccess)
    {
        skillChecker.OnSkillCheckSuccess += onSkillSuccess;
    }
    public void RegisterOnSkillCritical(Action onSkillCritical)
    {
        skillChecker.OnSkillCheckCritical += onSkillCritical;
    }
    public void RegisterOnSkillFailed(Action onSkillFailed)
    {
        skillChecker.OnSkillCheckFailed += onSkillFailed;
    }

    public void UnRegisterOnSkillSuccess(Action onSkillSuccess)
    {
        skillChecker.OnSkillCheckSuccess -= onSkillSuccess;
    }
    public void UnRegisterOnSkillCritical(Action onSkillCritical)
    {
        skillChecker.OnSkillCheckCritical -= onSkillCritical;
    }
    public void UnRegisterOnSkillFailed(Action onSkillFailed)
    {
        skillChecker.OnSkillCheckFailed -= onSkillFailed;
    }
}

