using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SourceMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float airAcceleration = 2.5f;
    public float maxAirSpeed = 8f;
    
    [Header("Mouse Settings")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;
    public float maxLookAngle = 90f;
    public bool canMove = true;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;

    private Vector3 moveInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!canMove)
            return;
        GroundCheck();
        LookAround();
        HandleMovement();
        ApplyGravity();
        controller.Move(velocity * Time.deltaTime);
    }


    void GroundCheck()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // small downward force to stick to ground
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
{
    float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
    Vector3 forward = transform.forward;
    Vector3 right = transform.right;

    float moveX = Input.GetAxis("Horizontal");
    float moveZ = Input.GetAxis("Vertical");

    Vector3 wishDir = (forward * moveZ + right * moveX).normalized;

    if (isGrounded)
    {
        if (wishDir.magnitude > 0)
        {
            velocity.x = wishDir.x * speed;
            velocity.z = wishDir.z * speed;
        }
        else
        {
            // stop immediately when no input
            velocity.x = 0f;
            velocity.z = 0f;
        }

        if (Input.GetButtonDown("Jump"))
            velocity.y = jumpForce;
    }
    else
    {
        // Air control like Source engine
        AirAccelerate(wishDir, speed, airAcceleration);
    }
}


    void AirAccelerate(Vector3 wishDir, float wishSpeed, float accel)
    {
        Vector3 velocityXZ = new Vector3(velocity.x, 0, velocity.z);
        float currentSpeed = Vector3.Dot(velocityXZ, wishDir);

        float addSpeed = wishSpeed - currentSpeed;
        if (addSpeed <= 0)
            return;

        float accelSpeed = accel * Time.deltaTime * wishSpeed;
        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        velocity.x += accelSpeed * wishDir.x;
        velocity.z += accelSpeed * wishDir.z;

        // Clamp max air speed
        Vector3 horizontalVel = new Vector3(velocity.x, 0, velocity.z);
        if (horizontalVel.magnitude > maxAirSpeed)
        {
            horizontalVel = horizontalVel.normalized * maxAirSpeed;
            velocity.x = horizontalVel.x;
            velocity.z = horizontalVel.z;
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }
}
