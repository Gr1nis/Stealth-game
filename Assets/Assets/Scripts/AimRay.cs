using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class AimRay : MonoBehaviour
{
    [Header("Настройки луча")]
    public float distance = 100f;
    public LayerMask targetMask;
    
    [Header("Настройки взаимодействия")]
    [SerializeField] private float interactionDistance = 3f;
    
    [Header("Визуальная обратная связь")]
    [SerializeField] private Image crosshair;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color interactableColor = Color.green;
    [SerializeField] private GameObject interactionHint; // UI текст подсказки
    
    [Header("Инвентарь")]
    [SerializeField] private GameObject keyCardUI; // Иконка ключ-карты в UI
    private bool hasKeyCard = false;
    
    void Start()
    {
        // Инициализация UI
        if (keyCardUI != null) keyCardUI.SetActive(false);
        if (interactionHint != null) interactionHint.SetActive(false);
    }
    
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        // Проверка для взаимодействия (ближний луч)
        if (Physics.Raycast(ray, out hit, interactionDistance, targetMask))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);
            Debug.Log("Можно взаимодействовать с: " + hit.collider.name);
            
            // Подсвечиваем прицел
            if (crosshair != null)
                crosshair.color = interactableColor;
            
            // Показываем подсказку
            ShowInteractionHint(hit.collider.gameObject);
            
            // Обработка нажатия кнопки взаимодействия
            if (IsInteractPressed())
            {
                // Если это ключ-карта
                if (hit.collider.CompareTag("KeyCard") && !hasKeyCard)
                {
                    hasKeyCard = true;
                    if (keyCardUI != null) keyCardUI.SetActive(true);
                    Destroy(hit.collider.gameObject);
                    Debug.Log("Ключ-карта взята!");
                }
                
                // Если это шлагбаум
                if (hit.collider.CompareTag("Barrier") && hasKeyCard)
                {
                    Debug.Log("Шлагбаум открыт!");
                    // Здесь будет код открытия шлагбаума
                    BarrierController barrierController = hit.collider.GetComponent<BarrierController>();
                    if (barrierController != null)
                    {
                        barrierController.OpenBarrier();
                    }
                }
            }
        }
        // Проверка для визуализации (дальний луч)
        else if (Physics.Raycast(ray, out hit, distance, targetMask))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.yellow);
            Debug.Log("Вижу: " + hit.collider.name);
            
            // Возвращаем обычный цвет прицела
            if (crosshair != null)
                crosshair.color = defaultColor;
            
            // Скрываем подсказку
            HideInteractionHint();
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * distance, Color.green);
            
            // Возвращаем обычный цвет прицела
            if (crosshair != null)
                crosshair.color = defaultColor;
            
            // Скрываем подсказку
            HideInteractionHint();
        }
    }
    
    private void ShowInteractionHint(GameObject target)
    {
        if (interactionHint == null) return;
        
        string hintText = "";
        
        if (target.CompareTag("KeyCard") && !hasKeyCard)
        {
            hintText = "Нажмите E чтобы взять ключ-карту";
        }
        else if (target.CompareTag("Barrier"))
        {
            if (hasKeyCard)
                hintText = "Нажмите E чтобы открыть шлагбаум";
            else
                hintText = "Требуется ключ-карта";
        }
        else if (target.CompareTag("Door"))
        {
            hintText = "Нажмите E чтобы открыть дверь";
        }
        
        // Показываем подсказку
        interactionHint.SetActive(true);
        
        // Здесь можно обновить текст подсказки
        // Например: interactionHint.GetComponent<Text>().text = hintText;
    }
    
    private void HideInteractionHint()
    {
        if (interactionHint != null)
            interactionHint.SetActive(false);
    }
    
    private bool IsInteractPressed()
    {
        // Работает и со старой и с новой системой ввода
        #if ENABLE_INPUT_SYSTEM
        // Для новой Input System
        return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
        #else
        // Для старой Input Manager
        return Input.GetKeyDown(KeyCode.E);
        #endif
    }
}