using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;


public class ClearKiller : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] TMP_Text Text_Result;
    [SerializeField] CanvasGroup Panel_Fade;
    [SerializeField] GameObject OutFit;
    [SerializeField] GameObject Panel_ButtonHolder;

    void Start()
    {
        anim.SetBool("isMoving", true);
        anim.SetFloat("inputY", 1);

        Panel_Fade.DOFade(1, 0.5f).OnComplete(() =>
        {
            Panel_Fade.DOFade(0, 1.5f);
            Text_Result.gameObject.SetActive(true);
            OutFit.gameObject.SetActive(true);
            Panel_ButtonHolder.SetActive(true);
        });

    }

    public void SetResultText(GameResult result)
    {
        if (result < GameResult.ALLKILL)
        {
            Text_Result.text = "잘못된 결과";
            return;
        }
        switch (result)
        {
            case GameResult.ALLKILL:
                Text_Result.text = "무자비한 살인자";
                break;
            case GameResult.SOSO:
                Text_Result.text = "잔혹한 살인자";
                break;
            case GameResult.NoKill:
                Text_Result.text = "엔티티 배고파...";
                break;
        }
    }

    public void OnClick_ToMain()
    {
        MyNetworkManager.singleton.StopClient();
    }
}
