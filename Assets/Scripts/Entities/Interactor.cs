using Mirror;


public abstract class Interactor : NetworkBehaviour
{
    

    abstract public void Interact(JumpFence window);
    abstract public void Interact(Palete palete);
    abstract public void Interact(Generator generator);
    abstract public void Interact(Hanger hanger);
    abstract public void Interact(Lever lever);

    protected void InteractObject(IInteractableObject obj)
    {
        switch (obj)
        {
            case JumpFence:
                Interact((JumpFence)obj);
                break;
            case Generator:
                Interact((Generator)obj);
                break;
            case Palete:
                Interact((Palete)obj);
                break;
            case Hanger:
                Interact((Hanger)obj);
                break;
            case Lever:
                Interact((Lever)obj);
                break;
            default:
                break;
        }
    }
}
