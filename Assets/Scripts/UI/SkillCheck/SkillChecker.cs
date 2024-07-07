using System.Collections;
using UnityEngine;
using System;
using Mirror;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.EventSystems;


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
    Image Img_Checker;


    int rotateDir = 1;
    public float GetCheckerRotateSpeed() { return CheckerRotateSpeed; }

    private void Start()
    {
        Img_Checker = GetComponentInChildren<Image>();
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
        thisRect.eulerAngles = Vector3.zero;

        if (!IsHeldSkillChecker)
            StartCoroutine(CorRotateSkillChecker());
        else
        {
            StartCoroutine(CorRotateHeldSkillChecker());
        }
    }




    IEnumerator CorRotateSkillChecker()
    {
        yield return new WaitForSeconds(0.5f);
        float curTime = 0f;
        bool isClick = false;
        float zVal = 0;
        while (curTime <= CheckerRotateSpeed)
        {
            curTime += Time.deltaTime;
            zVal = 360 * curTime / CheckerRotateSpeed;
            thisRect.eulerAngles = new Vector3(0, 0, -zVal);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnSpaceDown_Rotate(curTime);
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
            zVal = Time.deltaTime * 360.0f / CheckerRotateSpeed * rotateDir;
            thisRect.eulerAngles = new Vector3(0, 0, thisRect.eulerAngles.z - zVal);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rotateDir *= -1;
                OnSpaceDown_Held();
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

    void OnSpaceDown_Rotate(float inputTime)
    {
        switch (CheckSkillResult(inputTime))
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

    void OnSpaceDown_Held()
    {
        float thisZ = thisRect.eulerAngles.z;
        //thisZ = Mathf.Abs(thisZ);

        if (thisZ < 97 && thisZ > 79 || thisZ > 360-97 && thisZ < 360-79)
        {
            CmdOnSkillCritical();
            rotateDir *= -1;
        }
    }
    SkillCheckResult CheckSkillResult(float inputTime)
    {
        SkillCheckResult result = SkillCheckResult.Failed;

        float successTime = scM.GetTimeToSuccess();
        float resultTime = inputTime - successTime;
        if (resultTime < 0) return result;
        else if (resultTime < 0.2825f)
        {
            result = SkillCheckResult.Success;
            if (resultTime < 0.05f)
            {
                result = SkillCheckResult.Critical;
            }
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
    void RpcInvokeOnSkillSuccess() { OnSkillCheckSuccess.Invoke(); Debug.Log("성공"); }
    [ClientRpc]
    void RpcInvokeOnSkillCritical() { OnSkillCheckCritical.Invoke(); Debug.Log("대성공"); }
    [ClientRpc]
    void RpcInvokeOnSkillFailed() { OnSkillCheckFailed.Invoke(); Debug.Log("실패"); }


    public void InvokeOnSkillCheckEnd()
    {
        OnSkillCheckEnd.Invoke();
    }

    public event Action OnSkillCheckSuccess;
    public event Action OnSkillCheckCritical;
    public event Action OnSkillCheckFailed;
    public event Action OnSkillCheckEnd;
}
/*
t초에 한 바퀴(360도) 도는 객체 a와, 60와 275중 사이의 회전값 r을 갖는 객체 b가 있을 때
a의 회전 값인 p와 r이 같아지는 시간 s를 구해야 한다.


0.6초에서 1.5초 사이의 시간을 갖는 시간 s가 있다
1.5초에 한 바퀴 도는 객체 a와, 임의의 회전값 r을 갖는 객체 b가 있을 때,
a의 회전 값인 p가 s초 후 회전 값 r과 같아지려고 하려면 어떻게 해야하는가?
 
 
 */