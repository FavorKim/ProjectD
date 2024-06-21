using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayableCharactor : Interactor
{
    protected IInteractableObject m_interactDest;

    public void Interact(IInteractableObject obj)
    {
        InteractObject(obj);
    }

    public override abstract void Interact(Generator generator);
    public override abstract void Interact(Palete palete);
    public override abstract void Interact(JumpFence jumpFence);

    protected virtual void Update()
    {
        OnInteract();
    }

    protected void OnInteract()
    {
        if (m_interactDest != null)
            InteractObject(m_interactDest);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IInteractableObject obj))
        {
            m_interactDest = obj;
        }

    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractableObject obj))
        {
            m_interactDest = null;
        }
    }
}
