using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCheckManager : MonoBehaviour
{
    [SerializeField] GameObject skillCheckCircle;
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
        StartCoroutine(CorSkillCheck());
    }

    private void SkillCheckStart()
    {
        RotateCircle();
        skillCheckObject.SetActive(true);
    }


    void RotateCircle()
    {
        float rot = UnityEngine.Random.Range(0f, 359.0f);
        skillCheckCircle.transform.localEulerAngles = new Vector3(0f, 0f, rot);
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
