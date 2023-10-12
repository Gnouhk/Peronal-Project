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

    [Header("Sentivity")]
    [SerializeField] 
    [Range(0f, 100f)] // Adjust the range of player sentivity.
    float playerSensitivity = 100f;

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
}
