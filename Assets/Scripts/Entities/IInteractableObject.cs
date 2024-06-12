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
    public void Interact() { }
}

public class Generator : IInteractableObject
{
    public void Interact() { }
}
