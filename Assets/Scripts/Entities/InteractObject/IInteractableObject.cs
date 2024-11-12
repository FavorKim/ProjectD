

public interface IKillerInteractable
{
    void KillerInteract(IKillerVisitor killer);
}

public interface ISurvivorInteractable
{
    void SurvivorInteract(ISurvivorVisitor survivor);
}
