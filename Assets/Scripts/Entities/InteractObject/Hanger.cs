using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hanger : NetworkBehaviour, IKillerInteractable, ISurvivorInteractable
{
    [SerializeField] Transform Pos_HangedPos;
    [SerializeField] GameObject Shackle;
    [SerializeField] GameObject Xray_silhouette;
    [SerializeField] GameObject Xray_Light;

    [SerializeField] float Duration_Xray;

    Survivor hangedSurvivor;
    public bool IsAvailable() { return Shackle.activeSelf; }
    public Survivor HangedSurvivor
    {
        get { return hangedSurvivor; }
        set
        {
            if (hangedSurvivor != value)
            {
                if (hangedSurvivor != null)
                {
                    hangedSurvivor.OnSacrificed -= RemoveHangedSurvivor;
                    hangedSurvivor.OnSacrificed -= DropShackle;
                }

                CmdOnChangedHangedSurvivor(value);

                if (hangedSurvivor != null)
                {
                    hangedSurvivor.OnSacrificed += DropShackle;
                    hangedSurvivor.OnSacrificed += RemoveHangedSurvivor;
                }
            }
        }
    }
    public Transform GetHangedPos() { return Pos_HangedPos; }

    public void SurvivorInteract(ISurvivorVisitor survivor)
    {
        HangedSurvivor.CmdOnResqued();
        CmdOnXrayOn();
        survivor.OnSurvivorVisitWithHanger(this);
    }

    public void KillerInteract(IKillerVisitor killer)
    {
        CmdOnXrayOn();
        killer.OnKillerVisitWithHanger(this);
    }

    public void DropShackle()
    {
        Shackle.SetActive(false);
    }

    void RemoveHangedSurvivor()
    {
        HangedSurvivor = null;
    }

    void TurnOnXray()
    {
        Xray_Light.SetActive(true);
        Xray_silhouette.SetActive(true);
    }
    void TurnOffXray()
    {
        Xray_Light.SetActive(false);
        Xray_silhouette.SetActive(false);
    }

    public void TurnSilhouette(bool onOff)
    {
        Xray_silhouette.SetActive(onOff);
    }
    [Command(requiresAuthority = false)]
    void CmdOnXrayOn()
    {
        RpcOnXrayOn();
    }
    [Command(requiresAuthority = false)]
    void CmdOnChangedHangedSurvivor(Survivor value)
    {
        RpcOnChangedHangedSurvivor(value);
    }



    [ClientRpc]
    void RpcOnXrayOn()
    {
        StartCoroutine(CorFlashXRay());
    }
    [ClientRpc]
    void RpcOnChangedHangedSurvivor(Survivor value)
    {
        hangedSurvivor = value;
    }
    IEnumerator CorFlashXRay()
    {
        TurnOnXray();
        yield return new WaitForSeconds(Duration_Xray);
        TurnOffXray();
    }
}
