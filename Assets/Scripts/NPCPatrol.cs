using UnityEngine;

public class NPCPatrol : MonoBehaviour
{
    [Header("Маршрут NPC")]
    public Transform[] waypoints;
    public float speed = 2f;          // Скорость движения
    public float waitTime = 1f;       // Время ожидания на точке
    public float rotationSpeed = 5f;  // Скорость поворота

    private int currentIndex = 0;
    private Animator anim;
    private float waitTimer = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (waypoints.Length == 0)
        {
            Debug.LogWarning("Waypoints не назначены!");
            enabled = false;
            return;
        }

        // Сразу на первой точке
        transform.position = waypoints[0].position;
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        Transform target = waypoints[currentIndex];

        // Расчёт направления только по XZ
        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        float distance = direction.magnitude;

        if (distance > 0.05f)
        {
            // Поворот плавно
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);

            // Движение к точке
            transform.position += direction.normalized * speed * Time.deltaTime;

            // Анимация ходьбы
            anim.SetBool("isWalking", true);
        }
        else
        {
            // Snap к точке, чтобы не трепыхался
            transform.position = target.position;

            // Отключаем анимацию ходьбы
            anim.SetBool("isWalking", false);

            // Ожидание на точке
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                currentIndex = (currentIndex + 1) % waypoints.Length;
                waitTimer = 0f;
            }
        }
    }

    // Для визуализации маршрута в Scene
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length; i++)
        {
            Transform a = waypoints[i];
            Transform b = waypoints[(i + 1) % waypoints.Length];
            Gizmos.DrawLine(a.position, b.position);
            Gizmos.DrawSphere(a.position, 0.1f);
        }
    }
}
