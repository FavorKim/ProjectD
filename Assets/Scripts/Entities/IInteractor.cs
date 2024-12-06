
public interface IKillerVisitor
{
    IKillerInteractable KillerInteractableObject { get; set; }
    void OnKillerInteract(IKillerInteractable obj);
    
    void OnKillerVisitWith(JumpFence fence);
    void OnKillerVisitWith(Hanger hanger);
    void OnKillerVisitWith(Palete palete);
    void OnKillerVisitWith(Generator palete);
}
   
public interface ISurvivorVisitor
{
    ISurvivorInteractable SurvivorInteractableObject { get; set; }
    void OnSurvivorInteract(ISurvivorInteractable obj);
    
    void OnSurvivorVisitWith(JumpFence fence);
    void OnSurvivorVisitWith(Hanger hanger);
    void OnSurvivorVisitWith(Palete palete);
    void OnSurvivorVisitWith(Generator palete);
    void OnSurvivorVisitWith(Lever lever);
}
