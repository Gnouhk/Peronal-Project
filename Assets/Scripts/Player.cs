using UnityEngine;

/*===========================================================================
 * This script is created by KHUONG VUONG ANH - 11:53 GPT + 7, 12/10/2023
 * 
 * Basic player control script.
 * =========================================================================*/

public class Player : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] LayerMask groundMask;

    [Header("Player Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float grav;
    [SerializeField] float jumpHeight;
    [SerializeField] float moveSmoothTime;
    [SerializeField] Vector3 currentMoveVelocity;
    [SerializeField] Vector3 moveDampVelocity;
    [SerializeField] Vector3 currentForceVelocity;

    [Header("Player Camera")]
    [SerializeField] Transform playerCamera;
    [SerializeField] Vector2 playerSensitivity;
    private Vector2 xyRotation;

    Vector2 inputDir;
    Vector2 camAxis;
    CharacterController controller;
    Camera cam;

    private float xRotation;
    private float stepOffset;
    private float slopeLimit;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        stepOffset = controller.stepOffset;
        slopeLimit = controller.slopeLimit;
    }

    private void Update()
    {
        PlayerMovement();
        PlayerLook();
    }

    private void PlayerMovement()
    {
        Vector3 PlayerInput = new Vector3
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = 0f,
            z = Input.GetAxisRaw("Vertical")
        };

        if(PlayerInput.magnitude > 1f)
        {
            PlayerInput.Normalize();
        }

        Vector3 moveVector = transform.TransformDirection(PlayerInput);
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        currentMoveVelocity = Vector3.SmoothDamp(
            currentMoveVelocity,
            moveVector * currentSpeed,
            ref moveDampVelocity,
            moveSmoothTime
        );

        controller.Move(currentMoveVelocity * Time.deltaTime);

        //Using Raycast to check if the player is stading on the ground or not.
        Ray groundCheckRay = new Ray(transform.position, Vector3.down); 
        if(Physics.Raycast(groundCheckRay, 1.25f))
        {
            currentForceVelocity.y = -2f;

            if(Input.GetKey(KeyCode.Space))
            {
                currentForceVelocity.y = jumpHeight;
            }
        }
        else //If the groundCheckRay does not colliding with anything
        {
            currentForceVelocity.y -= grav * Time.deltaTime;
        }

        controller.Move(currentForceVelocity * Time.deltaTime);
    }

    private void PlayerLook() 
    {
        Vector2 MouseInput = new Vector2 
        {
            x = Input.GetAxis("Mouse X"),
            y = Input.GetAxis("Mouse Y")
        };

        xyRotation.x -= MouseInput.y * playerSensitivity.y;
        xyRotation.y += MouseInput.x * playerSensitivity.x;

        xyRotation.x = Mathf.Clamp(xyRotation.x, -90f, 90f);

        transform.eulerAngles = new Vector3(0f, xyRotation.y, 0f);
        playerCamera.localEulerAngles = new Vector3(xyRotation.x, 0f, 0f);
    }
}
