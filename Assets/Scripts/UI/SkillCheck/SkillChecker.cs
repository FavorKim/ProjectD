using System.Collections;
using UnityEngine;
using System;
using Mirror;

public class SkillChecker : NetworkBehaviour
{
    RectTransform skillCheckCircle;
    RectTransform thisRect;

    private void Start()
    {
        thisRect = GetComponent<RectTransform>();
        StartCoroutine(CorRotateSkillChecker());
    }

    public void InitCircle(RectTransform skillCheckCircle)
    {
        this.skillCheckCircle = skillCheckCircle;
    }

    private void OnEnable()
    {
        thisRect.rotation = Quaternion.identity;
        StartCoroutine(CorRotateSkillChecker());
    }


    IEnumerator CorRotateSkillChecker()
    {
        float curTime = 0f;
        bool isClick = false;
        while (curTime <= 1.0f)
        {
            curTime += Time.deltaTime;
            thisRect.localEulerAngles = new Vector3(0f, 0f, curTime * -359.0f);
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

    private void OnDisable()
    {
        StopCoroutine(CorRotateSkillChecker());
    }

    void OnSpaceDown()
    {
        float circleRot = skillCheckCircle.localEulerAngles.z;
        if (circleRot - thisRect.localEulerAngles.z > 0)
            CmdOnSkillFailed();
        else if (circleRot - thisRect.localEulerAngles.z < 15)
            CmdOnSkillCritical();
        else if (circleRot - thisRect.localEulerAngles.z < 68)
            CmdOnSkillSuccess();
        else
            CmdOnSkillFailed();
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


    void InvokeOnSkillCheckEnd()
    {
        OnSkillCheckEnd.Invoke();
    }

    public event Action OnSkillCheckSuccess;
    public event Action OnSkillCheckCritical;
    public event Action OnSkillCheckFailed;
    public event Action OnSkillCheckEnd;
}
