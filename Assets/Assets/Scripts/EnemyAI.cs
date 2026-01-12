using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public class EnemyAI : MonoBehaviour
{
    [Header("Патруль")]
    public Transform[] waypoints;
    public float patrolSpeed = 2f;
    public float waitTime = 1f;
    public float rotationSpeed = 5f;

    [Header("Слух")]
    public float hearingRadius = 6f;
    public float investigateSpeed = 2.5f;

    [Header("Зрение")]
    public float viewDistance = 10f;
    public float viewAngle = 60f;
    public float lightVisibilityBonus = 2f; // бонус видимости под фонарём
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    [Header("Поймать игрока")]
    public float catchDistance = 1.5f; // расстояние, чтобы поймать игрока
    public float maxSightTime = 5f;    // секунд, сколько игрок в поле зрения

    private int currentIndex = 0;
    private float waitTimer = 0f;
    private Animator anim;

    private bool investigating = false;
    private Vector3 targetPosition;

    private Transform player;
    private float sightTimer = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (waypoints.Length == 0)
        {
            Debug.LogWarning("Waypoints не назначены!");
            enabled = false;
            return;
        }

        transform.position = waypoints[0].position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        if (!investigating)
        {
            CheckHearing();
            CheckSight();
            Patrol();
            sightTimer = 0f; // игрок не виден — сбрасываем таймер
        }
        else
        {
            MoveToTarget();
            CheckPlayerCaught();
        }
    }

    // ===== ПАТРУЛЬ =====
    void Patrol()
    {
        Transform target = waypoints[currentIndex];
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        float distance = direction.magnitude;

        if (distance > 0.05f)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
            transform.position += direction.normalized * patrolSpeed * Time.deltaTime;
            anim.SetBool("isWalking", true);
        }
        else
        {
            transform.position = target.position;
            anim.SetBool("isWalking", false);

            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                currentIndex = (currentIndex + 1) % waypoints.Length;
                waitTimer = 0f;
            }
        }
    }

    // ===== СЛУХ =====
    void CheckHearing()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, hearingRadius, playerMask);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log($"{name} услышал игрока!");
                targetPosition = hit.transform.position;
                investigating = true;
                anim.SetBool("isWalking", true);
                break;
            }
        }
    }

    // ===== ЗРЕНИЕ =====
    void CheckSight()
    {
        Vector3 dirToPlayer = player.position - transform.position;
        dirToPlayer.y = 0;

        float distance = dirToPlayer.magnitude;
        if (distance > viewDistance) return;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > viewAngle / 2f) return;

        // проверка препятствий
        if (!Physics.Raycast(transform.position + Vector3.up * 1.2f, dirToPlayer.normalized, out RaycastHit hit, distance, obstacleMask))
        {
            // проверка освещённости — ищем объекты с тегом "Light" рядом с игроком
            float visibilityMultiplier = 1f;
            Collider[] lights = Physics.OverlapSphere(player.position, 2f);
            foreach (var l in lights)
            {
                if (l.CompareTag("Light"))
                    visibilityMultiplier += lightVisibilityBonus;
            }

            // небольшая случайность для стелса
            if (Random.value < visibilityMultiplier / 3f)
            {
                Debug.Log($"{name} увидел игрока!");
                targetPosition = player.position;
                investigating = true;
                anim.SetBool("isWalking", true);
            }
        }
    }

    // ===== ДВИЖЕНИЕ К ЦЕЛИ =====
    void MoveToTarget()
    {
        Vector3 dir = targetPosition - transform.position;
        dir.y = 0;

        if (dir.magnitude > 0.2f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
            transform.position += dir.normalized * investigateSpeed * Time.deltaTime;
            anim.SetBool("isWalking", true);
        }
        else
        {
            investigating = false;
            anim.SetBool("isWalking", false);
        }
    }

    // ===== ПРОВЕРКА ПОИМКИ И ТАЙМЕРА ВИДИМОСТИ =====
    void CheckPlayerCaught()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        // 1️⃣ Игрок слишком близко
        if (dist <= catchDistance)
        {
            GameManager.Instance.PlayerCaught();
        }

        // 2️⃣ Игрок слишком долго в поле зрения
        Vector3 dirToPlayer = player.position - transform.position;
        dirToPlayer.y = 0;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (dirToPlayer.magnitude <= viewDistance && angle <= viewAngle / 2f)
            sightTimer += Time.deltaTime;
        else
            sightTimer = 0f;

        if (sightTimer >= maxSightTime)
            GameManager.Instance.PlayerCaught();
    }

    void OnDrawGizmosSelected()
    {
        // Радиус слуха
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);

        // Радиус зрения
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        // Патрульные точки
        if (waypoints != null && waypoints.Length > 1)
        {
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
}
