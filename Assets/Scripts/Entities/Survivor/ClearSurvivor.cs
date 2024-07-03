using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearSurvivor : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Text Text_Result;
    // Start is called before the first frame update
    void Start()
    {
        anim.SetBool("isRun", true);
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
