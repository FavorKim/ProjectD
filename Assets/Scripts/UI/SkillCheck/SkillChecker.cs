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
        while (curTime <= 1.0f)
        {
            curTime += Time.deltaTime;
            thisRect.localEulerAngles = new Vector3(0f, 0f, curTime * -359.0f);
            yield return null;
        }
        CmdOnSkillFailed();
        OnSkillCheckEnd.Invoke();
    }

    private void OnDisable()
    {
        StopCoroutine(CorRotateSkillChecker());
    }

    private void Update()
    {
        if (skillCheckCircle != null && Input.GetKeyDown(KeyCode.Space))
        {
            float circleRot = skillCheckCircle.rotation.z;
            if (circleRot - thisRect.rotation.z > 0)
                CmdOnSkillFailed();
            else if (circleRot - thisRect.rotation.z < 15)
                CmdOnSkillCritical();
            else if (circleRot - thisRect.rotation.z < 68)
                CmdOnSkillSuccess();
            else
                CmdOnSkillFailed();

            StopCoroutine(CorRotateSkillChecker());

            Invoke(nameof(InvokeOnSkillCheckEnd), 1.0f);
        }
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
