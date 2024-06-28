using System.Collections;
using UnityEngine;
using System;
using Mirror;

public class SkillChecker : NetworkBehaviour
{
    private void OnEnable()
    {
        transform.localEulerAngles = new Vector3(0f, 0f, 180f);
        StartCoroutine(CorRotateSkillChecker());
    }


    IEnumerator CorRotateSkillChecker()
    {
        float curTime = 0f;
        while (curTime <= 1.0f)
        {
            yield return null;
            curTime += Time.deltaTime;
            transform.localEulerAngles = new Vector3(0f, 0f, 180 + curTime * 360.0f);
        }
        CmdOnSkillFailed();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var tag = collision.gameObject.tag;
            switch (tag)
            {
                case "NormalSkillCheck":
                    CmdOnSkillSuccess();
                    break;
                case "CriticalSkillCheck":
                    CmdOnSkillCritical();
                    break;
                case "SkillCheckFail":
                    CmdOnSkillFailed();
                    break;
            }
            StopCoroutine(CorRotateSkillChecker());
        }
    }
    [Command(requiresAuthority = false)]
    void CmdOnSkillSuccess() { RpcInvokeOnSkillSuccess(); }
    [Command(requiresAuthority = false)]
    void CmdOnSkillCritical() {  RpcInvokeOnSkillCritical(); }
    [Command(requiresAuthority = false)]
    void CmdOnSkillFailed() {  RpcInvokeOnSkillFailed(); }
    
    [ClientRpc]
    void RpcInvokeOnSkillSuccess() { OnSkillCheckSuccess.Invoke(); }
    [ClientRpc]
    void RpcInvokeOnSkillCritical() {  OnSkillCheckCritical.Invoke(); }
    [ClientRpc]
    void RpcInvokeOnSkillFailed() {  OnSkillCheckFailed.Invoke(); }

    public event Action OnSkillCheckSuccess;
    public event Action OnSkillCheckCritical;
    public event Action OnSkillCheckFailed;
}
