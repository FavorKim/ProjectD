
public interface IKillerVisitor
{
    IKillerInteractable KillerInteractableObject { get; set; }
    void OnKillerVisitWithJumpFence(JumpFence fence);
    void OnKillerVisitWithHanger(Hanger hanger);
    void OnKillerVisitWithPalete(Palete palete);
    void OnKillerVisitWithGenerator(Generator palete);
}
   
public interface ISurvivorVisitor
{
    ISurvivorInteractable SurvivorInteractableObject { get; set; }
    void OnSurvivorVisitWithJumpFence(JumpFence fence);
    void OnSurvivorVisitWithHanger(Hanger hanger);
    void OnSurvivorVisitWithPalete(Palete palete);
    void OnSurvivorVisitWithGenerator(Generator palete);
    void OnSurvivorVisitWithLever(Lever lever);
}
