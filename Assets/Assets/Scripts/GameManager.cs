using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  // Singleton
    public GameObject loseMenu;

    void Awake()
    {
        Instance = this; // Назначаем текущий объект
    }

    public void PlayerCaught()
    {
        Debug.Log("Игрок пойман!");
        if (loseMenu != null)
            loseMenu.SetActive(true);

        Time.timeScale = 0f;                     // Пауза игры
        Cursor.lockState = CursorLockMode.None;  // Разблокировать курсор
        Cursor.visible = true;                   // Сделать видимым
    }

    public void Retry()
    {
        Time.timeScale = 1f; // Снимаем паузу
        StartCoroutine(RestartScene());
    }

    private IEnumerator RestartScene()
    {
        yield return null; // Ждём один кадр
        Cursor.lockState = CursorLockMode.Locked; // Блокируем курсор
        Cursor.visible = false;                   // Скрываем курсор
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
