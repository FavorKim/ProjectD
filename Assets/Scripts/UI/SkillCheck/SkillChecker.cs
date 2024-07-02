using System.Collections;
using UnityEngine;
using System;
using Mirror;
using System.Collections.Generic;
using Unity.VisualScripting;


public enum SkillCheckResult
{
    Failed, Success, Critical
}

public class SkillChecker : NetworkBehaviour
{
    RectTransform skillCheckCircle;
    RectTransform thisRect;
    [SerializeField] float CheckerRotateSpeed;
    SkillCheckManager scM;
    [SerializeField] bool IsHeldSkillChecker;


    int rotateDir = 1;


    private void Start()
    {
        thisRect = GetComponent<RectTransform>();
        if (!IsHeldSkillChecker)
            StartCoroutine(CorRotateSkillChecker());
        else
        {
            OnSkillCheckCritical += OnHeldCheckerSkillSuccess;
            OnSkillCheckSuccess += OnHeldCheckerSkillSuccess;
            StartCoroutine(CorRotateHeldSkillChecker());
        }
    }

    public void InitCircle(SkillCheckManager SCM)
    {
        this.scM = SCM;
    }


    private void OnEnable()
    {
        if (!IsHeldSkillChecker)
            StartCoroutine(CorRotateSkillChecker());
        else
        {
            StartCoroutine(CorRotateHeldSkillChecker());
        }
    }




    IEnumerator CorRotateSkillChecker()
    {
        
        float curTime = 0f;
        bool isClick = false;
        float zVal = 0;
        while (curTime <= CheckerRotateSpeed)
        {
            curTime += Time.deltaTime;
            zVal += Time.deltaTime * 359.0f / CheckerRotateSpeed;
            thisRect.eulerAngles = new Vector3(0f, 179, 359 - zVal);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnSpaceDown();
                isClick = true;
                break;
            }
            yield return null;
        }
        if (!isClick)
            CmdOnSkillFailed();
        Invoke(nameof(InvokeOnSkillCheckEnd), 1.0f);
    }

    IEnumerator CorRotateHeldSkillChecker()
    {
        float zVal = 0;
        while (true)
        {
            zVal = Time.deltaTime * 359.0f / CheckerRotateSpeed * rotateDir;
            thisRect.rotation = Quaternion.Euler(new Vector3(0f, 180, thisRect.rotation.z + zVal));
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnSpaceDown();
            }
            yield return null;

        }
    }

    void OnHeldCheckerSkillSuccess()
    {
        rotateDir *= -1;
    }

    private void OnDisable()
    {
        if (!IsHeldSkillChecker)
            StopCoroutine(CorRotateSkillChecker());
        else
        {
            StopCoroutine(CorRotateHeldSkillChecker());
        }
    }

    private void OnApplicationQuit()
    {
        OnSkillCheckSuccess -= OnHeldCheckerSkillSuccess;
        OnSkillCheckCritical -= OnHeldCheckerSkillSuccess;

        OnSkillCheckCritical = null;
        OnSkillCheckEnd = null;
        OnSkillCheckFailed = null;
        OnSkillCheckSuccess = null;
            
    }

    void OnSpaceDown()
    {
        switch (CheckSkillResult())
        {
            case SkillCheckResult.Success:
                CmdOnSkillSuccess();
                break;
            case SkillCheckResult.Critical:
                CmdOnSkillCritical();
                break;
            case SkillCheckResult.Failed:
                CmdOnSkillFailed();
                break;
        }
    }
    SkillCheckResult CheckSkillResult()
    {
        SkillCheckResult result = SkillCheckResult.Failed;

        foreach (GameObject val in scM.NormalArea)
        {
            if (val.transform.eulerAngles.z - thisRect.eulerAngles.z < 69 && val.transform.eulerAngles.z - thisRect.eulerAngles.z > 0)
                result = SkillCheckResult.Success;
        }

        foreach (GameObject val in scM.CriticalArea)
        {
            if (val.transform.eulerAngles.z - thisRect.eulerAngles.z < 14 && val.transform.eulerAngles.z - thisRect.eulerAngles.z > 0)
                result = SkillCheckResult.Critical;
        }
        return result;
    }

    [Command(requiresAuthority = false)]
    void CmdOnSkillSuccess() { RpcInvokeOnSkillSuccess(); }
    [Command(requiresAuthority = false)]
    void CmdOnSkillCritical() { RpcInvokeOnSkillCritical(); }
    [Command(requiresAuthority = false)]
    void CmdOnSkillFailed() { RpcInvokeOnSkillFailed(); }

    [ClientRpc]
    void RpcInvokeOnSkillSuccess() { OnSkillCheckSuccess.Invoke(); }
    [ClientRpc]
    void RpcInvokeOnSkillCritical() { OnSkillCheckCritical.Invoke(); }
    [ClientRpc]
    void RpcInvokeOnSkillFailed() { OnSkillCheckFailed.Invoke(); }


    public void InvokeOnSkillCheckEnd()
    {
        OnSkillCheckEnd.Invoke();
    }

    public event Action OnSkillCheckSuccess;
    public event Action OnSkillCheckCritical;
    public event Action OnSkillCheckFailed;
    public event Action OnSkillCheckEnd;
}
