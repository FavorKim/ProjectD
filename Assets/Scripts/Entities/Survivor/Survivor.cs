using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Survivor : PlayableCharactor
{
    CharacterController m_CharacterController;
    [SerializeField] Animator Animator;

    Vector3 m_MoveDir;

    [SerializeField] float m_moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        m_CharacterController.Move(m_MoveDir);

    }

    public void OnMove(InputValue val)
    {
        Vector2 dir = val.Get<Vector2>();
        m_MoveDir = new Vector3(dir.x, 0, dir.y);
        m_MoveDir *= Time.deltaTime * m_moveSpeed;
        //if (!val.isPressed) m_MoveDir = Vector3.zero;

    }


    public override void InteractObject(Generator generator)
    {
        // 발전기 작동 애니메이션 출력
    }
    public override void InteractObject(Palete palete)
    {
        // 발전기 내리기 혹은 발전기 넘기
        if (palete.isUsed)
        {
            // 발전기 넘기
        }
        else
        {
            // 발전기 내리기
        }
    }

    public override void InteractObject(Window window)
    {
        // 창틀 뛰어넘기
    }
}
