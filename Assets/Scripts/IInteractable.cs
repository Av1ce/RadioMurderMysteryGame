public interface IInteractable
{
    public string InteractMessage { get; }
    public void Interact();

    public string GetName {  get; }

    public void Accuse();
}
