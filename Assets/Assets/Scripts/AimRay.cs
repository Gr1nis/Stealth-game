using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class AimRay : MonoBehaviour
{
    [Header("Луч")]
    public float maxDistance = 100f;
    public float interactionDistance = 3f;
    public LayerMask targetMask;

    [Header("UI")]
    public Image crosshair;
    public Color defaultColor = Color.white;
    public Color interactableColor = Color.green;
    public UIInteractionPrompt uiPrompt;

    [Header("Инвентарь")]
    public Inventory playerInventory;

    private void Update()
    {
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        IInteractable interactable = null;
        bool isInteractable = false;

        if (Physics.Raycast(ray, out hit, maxDistance, targetMask))
        {
            float dist = Vector3.Distance(transform.position, hit.point);

            if (dist <= interactionDistance)
            {
                interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    isInteractable = true;
                    uiPrompt.Show(interactable.GetInteractText());

                    if (IsInteractPressed())
                        interactable.Interact(this);
                }
            }
        }

        if (!isInteractable)
            uiPrompt.Hide();

        if (crosshair != null)
            crosshair.color = isInteractable ? interactableColor : defaultColor;

        Debug.DrawRay(ray.origin, ray.direction * maxDistance, isInteractable ? Color.red : Color.green);
    }

    private bool IsInteractPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.E);
#endif
    }

    // Методы для Keycard
    // проверка для Barrier
    public bool HasKeycard(int required = 1)
    {
        if (playerInventory == null) return false;
        return playerInventory.HasEnoughKeycards(required);
    }


    public void AddKeycard() => playerInventory?.AddKeycard();
}
