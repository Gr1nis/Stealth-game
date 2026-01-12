using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 4f;
    public float runMultiplier = 1.6f;
    public float gravity = -9.81f;

    [Header("Look")]
    public float mouseSensitivity = 100f;
    public Transform cameraPivot;

    [Header("Animation")]
    public Animator animator;

    private CharacterController controller;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float xRotation;
    private float verticalVelocity;

    private bool isRunning;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ===== INPUT =====
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        isRunning = ctx.ReadValueAsButton();

    }

    void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleLook();
        UpdateAnimator();
    }

    // ===== MOVEMENT =====
    void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        float finalSpeed = speed;
        if (isRunning)
            finalSpeed *= runMultiplier;

        controller.Move(move * finalSpeed * Time.deltaTime);
    }

    // ===== GRAVITY =====
    void HandleGravity()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 gravityMove = Vector3.up * verticalVelocity;
        controller.Move(gravityMove * Time.deltaTime);
    }

    // ===== LOOK =====
    void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // ===== ANIMATION =====
    void UpdateAnimator()
    {
        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0f;

        float animSpeed = horizontalVelocity.magnitude;
        animator.SetFloat("Speed", animSpeed);
    }
}
