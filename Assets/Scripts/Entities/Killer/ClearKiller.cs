using UnityEngine.UI;
using UnityEngine;

public class ClearKiller : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Text Text_Result;

    void Start()
    {
        anim.SetBool("isMoving", true);
    }

    public void SetResultText(GameResult result)
    {
        if (result < GameResult.ALLKILL)
        {
            Text_Result.text = "�߸��� ���";
            return;
        }
        switch (result)
        {
            case GameResult.ALLKILL:
                Text_Result.text = "���ں��� ������";
                break;
            case GameResult.SOSO:
                Text_Result.text = "��Ȥ�� ������";
                break;
            case GameResult.NoKill:
                Text_Result.text = "��ƼƼ �����...";
                break;
        }
    }

}
