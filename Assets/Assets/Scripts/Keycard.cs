using UnityEngine;

public class Keycard : MonoBehaviour, IInteractable
{
    public string GetInteractText()
    {
        return "Нажмите E чтобы взять ключ-карту";
    }

    public void Interact(AimRay player)
    {
        if (player == null) return;

        // Добавляем карту в инвентарь
        player.AddKeycard();

        // Скрываем подсказку
        if (UIInteractionPrompt.Instance != null)
            UIInteractionPrompt.Instance.Hide();

        // Убираем карту со сцены
        Destroy(gameObject);

        Debug.Log("Ключ-карта взята!");
    }
}
