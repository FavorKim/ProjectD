using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactor : MonoBehaviour
{
    protected void InteractObject<T>(T obj) where T : IInteractableObject
    {
        var dest = GetInteractableObject(obj);
        InteractObject(dest);
    }

    abstract public void InteractObject(Window window);
    abstract public void InteractObject(Palete palete);
    abstract public void InteractObject(Generator generator);

    IInteractableObject GetInteractableObject(IInteractableObject obj)
    {
        switch (obj)
        {
            case Window:
                return obj as Window;
            case Generator:
                return obj as Generator;
            case Palete:
                return obj as Palete;
            default:
                return null;
        }
    }
}
/*턴제에서 얻을 수 있는 이득과 리얼타임에서 얻을 수 있는 이득*/