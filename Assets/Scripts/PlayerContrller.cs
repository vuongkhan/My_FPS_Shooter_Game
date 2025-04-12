using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }
    public float MouseSensitivity = 100.0f;
    public float MoveSpeed = 5.0f;
    public Transform CameraPosition;

    private CharacterController characterController;
    private float verticalAngle, horizontalAngle;
    private Vector3 playerVelocity;
    private float gravity = -9.81f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        horizontalAngle = transform.localEulerAngles.y;
    }

    private void Update()
    {
        if (!GameSystem.START_GAME || Controller.Instance.LockControl)
            return;

        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float turnPlayer = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        horizontalAngle = (horizontalAngle + turnPlayer) % 360;

        Vector3 currentAngles = transform.localEulerAngles;
        currentAngles.y = horizontalAngle;
        transform.localEulerAngles = currentAngles;

        float turnCam = -Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
        verticalAngle = Mathf.Clamp(verticalAngle + turnCam, -89.0f, 89.0f);

        currentAngles = CameraPosition.localEulerAngles;
        currentAngles.x = verticalAngle;
        CameraPosition.localEulerAngles = currentAngles;
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical"); 

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(moveDirection * MoveSpeed * Time.deltaTime);

        if (characterController.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }
    
}
