using UnityEngine;

public class Movement : MonoBehaviour
{
    //vectors for movement
    private Vector3 Velocity;
    private Vector3 PlayerMovementInput;
    private Vector2 PlayerMouseInput;
    private float xRotation;

    [Header("Components Needed")]
    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private CharacterController Controller;
    [SerializeField] private Transform Player;
    //Movement
    // float Jump = 3;
    float Speed = 3;
    private float Sensetivity = 1;
    private float Gravity = 9.81f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {

        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer();
        MoveCamera();
    }
    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput);

        if (Controller.isGrounded)
        {
            Velocity.y = -1f;

            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     Velocity.y = Jump;
            // }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Speed *= 3;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                Speed = 3;
            }
            Controller.Move(MoveVector * Speed * Time.deltaTime);
        }
        else
        {
            Velocity.y += Gravity * -2f * Time.deltaTime;
        }
        
        Controller.Move(Velocity * Time.deltaTime);

    }
    private void MoveCamera()
    {
        xRotation -= PlayerMouseInput.y * Sensetivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.Rotate(0f, PlayerMouseInput.x * Sensetivity, 0f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
