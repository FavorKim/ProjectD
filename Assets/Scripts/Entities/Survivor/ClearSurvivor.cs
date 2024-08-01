using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearSurvivor : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Text Text_Result;
    [SerializeField] GameObject Outfit;
    [SerializeField] CanvasGroup Panel;
    [SerializeField] GameObject Panel_ButtonHolder;

    void Start()
    {
        anim.SetBool("isRun", true);
        anim.SetBool("isWalk", true);

        Panel.DOFade(1, 0.5f).OnComplete(() =>
        {
            Panel.DOFade(0, 2.5f);
            Outfit.SetActive(true);
            Text_Result.gameObject.SetActive(true);
            Panel_ButtonHolder.SetActive(true);

        });

    }

    public void SetResultText(GameResult result)
    {
        if (result > GameResult.Sacrificed)
        {
            Text_Result.text = "Àß¸øµÈ °á°ú";
            return;
        }
        Text_Result.text = result == GameResult.Escape ? "Å»Ãâ" : "Èñ»ýµÊ";
    }

    public void OnClick_ToMain()
    {
        MyNetworkManager.singleton.StopClient();
    }
}
