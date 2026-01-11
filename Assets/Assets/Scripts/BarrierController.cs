using UnityEngine;

public class BarrierController : MonoBehaviour
{
    [Header("Настройки анимации")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private Transform barrierArm; // Двигающаяся часть
    
    [Header("Визуальные эффекты")]
    [SerializeField] private Light indicatorLight;
    [SerializeField] private Color closedColor = Color.red;
    [SerializeField] private Color openColor = Color.green;
    
    [Header("Аудио")]
    [SerializeField] private AudioClip openSound;
    
    private bool isOpen = false;
    private AudioSource audioSource;
    
    void Start()
    {
        // Назначаем тег автоматически
        gameObject.tag = "Barrier";
        
        // Инициализируем аудио источник
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Устанавливаем начальный цвет индикатора
        if (indicatorLight != null)
            indicatorLight.color = closedColor;
    }
    
    public void OpenBarrier()
    {
        if (isOpen) return;
        
        isOpen = true;
        
        // Запускаем анимацию открытия
        StartCoroutine(OpenBarrierAnimation());
        
        // Меняем цвет индикатора
        if (indicatorLight != null)
            indicatorLight.color = openColor;
        
        // Воспроизводим звук
        if (openSound != null && audioSource != null)
            audioSource.PlayOneShot(openSound);
    }
    
    private System.Collections.IEnumerator OpenBarrierAnimation()
    {
        if (barrierArm == null) yield break;
        
        Quaternion startRotation = barrierArm.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, openAngle, 0);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * openSpeed;
            barrierArm.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime);
            yield return null;
        }
    }
}