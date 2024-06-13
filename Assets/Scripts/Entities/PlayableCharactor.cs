using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharactor : Interactor
{
    //public override void InteractObject(IInteractableObject obj)
    //{
    //}
    //IInteractableObject GetInteractable(IInteractableObject obj)
    //{
    //    switch(obj)
    //    {
    //        case Window:
    //            break;
    //        case Palete:
    //            break;
    //        case Generator:
    //            break;
    //    }
    //}
    public void Interact(IInteractableObject obj)
    {
        InteractObject(obj);
    }

    public override void InteractObject(Generator generator)
    {
        
    }
    public override void InteractObject(Palete palete)
    {
        
    }
    public override void InteractObject(Window window)
    {
        
    }
}
