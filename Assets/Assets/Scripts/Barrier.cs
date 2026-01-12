using UnityEngine;

public class Barrier : MonoBehaviour, IInteractable
{
    public Transform arrow; // ссылка на дочерний объект стрелы
    public Vector3 openPositionOffset = new Vector3(0, 3, 0); // на сколько поднимать стрелу
    public float openSpeed = 2f; // скорость поднятия стрелы
    public int requiredKeycards = 2; // сколько ключ-карт нужно

    private bool isOpening = false;
    private Vector3 closedPosition;
    private Vector3 targetPosition;

    void Start()
    {
        if (arrow != null)
            closedPosition = arrow.localPosition; // запоминаем исходную позицию
    }

    void Update()
    {
        if (isOpening && arrow != null)
        {
            // плавно двигаем стрелу к цели
            arrow.localPosition = Vector3.Lerp(arrow.localPosition, targetPosition, Time.deltaTime * openSpeed);

            // если почти дошли, фиксируем точно
            if (Vector3.Distance(arrow.localPosition, targetPosition) < 0.01f)
            {
                arrow.localPosition = targetPosition;
                isOpening = false;
            }
        }
    }

    public string GetInteractText()
    {
        return $"Нажмите E чтобы открыть шлагбаум ({requiredKeycards} ключ-карты)";
    }

    public void Interact(AimRay player)
    {
        if (player.playerInventory.HasEnoughKeycards(requiredKeycards))
        {
            if (arrow != null)
            {
                targetPosition = closedPosition + openPositionOffset;
                isOpening = true;
            }

            UIInteractionPrompt.Instance?.Hide();
            Debug.Log("Шлагбаум открыт!");
        }
        else
        {
            int remaining = requiredKeycards - player.playerInventory.KeycardCount();
            UIInteractionPrompt.Instance?.Show($"Нужно ещё {remaining} ключ-карта(ы)");
        }
    }
}
