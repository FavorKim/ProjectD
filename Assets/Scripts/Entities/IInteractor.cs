using Mirror;


public interface IKillerInteractor
{
    IKillerInteractable KillerInteractableObject { get; set; }
    void OnKillerInteract(IKillerInteractable obj);
   
}
public interface ISurvivorInteractor
{
    ISurvivorInteractable SurvivorInteractableObject { get; set; }
    void OnSurvivorInteract(ISurvivorInteractable obj);
}
