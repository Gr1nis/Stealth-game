using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float speed = 4f;
    public float mouseSensitivity = 100f;
    public Transform cameraPivot;
    public Animator animator;

    public float gravity = -9.81f;   // ← добавили гравитацию

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation;

    private float verticalVelocity;  // ← вертикальная скорость

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    void Update()
    {
        // ===== ГОРИЗОНТАЛЬНОЕ ДВИЖЕНИЕ =====
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // ===== ПРОВЕРКА ЗЕМЛИ =====
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = -2f; // прижимаем к земле
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // ===== ИТОГОВОЕ ДВИЖЕНИЕ =====
        Vector3 velocity = move * speed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        // ===== ВРАЩЕНИЕ КАМЕРЫ =====
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }
}
