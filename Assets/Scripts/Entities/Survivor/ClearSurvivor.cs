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


    void Start()
    {
        Panel.DOFade(1, 0.8f).OnComplete(() =>
        {
            Panel.DOFade(0, 1.5f);
            Outfit.SetActive(true);
            Text_Result.gameObject.SetActive(true);

            anim.SetBool("isRun", true);
            anim.SetBool("isWalk", true);
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

}
