public interface IInteractable
{
    string GetInteractText();
    void Interact(AimRay player); // Теперь метод принимает AimRay
}