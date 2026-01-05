using UnityEngine;

public class AimRay : MonoBehaviour
{
    public float distance = 100f;
    public LayerMask targetMask;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance, targetMask))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            // Проверка: во что целимся
            Debug.Log("Навёлся на: " + hit.collider.name);
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * distance, Color.green);
        }
    }
}
