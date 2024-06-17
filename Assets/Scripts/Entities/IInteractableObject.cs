using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableObject
{
    void Interact();
}

public class Window : IInteractableObject
{
    public void Interact() { }
}

public class Palete : IInteractableObject
{
    public bool isUsed;
    public void Interact() 
    {
        if(isUsed)
        {
            //�μ�����
        }
    }
}

public class Generator : IInteractableObject
{
    float curGauge;
    float maxGauge;
    public void Interact() 
    {
        // ���������� �ø���
    }
}
