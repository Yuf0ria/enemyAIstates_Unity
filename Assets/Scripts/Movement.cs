using UnityEngine;

public class Movement : MonoBehaviour
{
    #region Vector Components
        private Vector3 _velocity;
        private Vector3 _playerMovementInput;
        private Vector2 _playerMouseInput;
        private float _xRotation;
    #endregion
    
    #region Movement Components
        [Header("Movement Components")]
        [SerializeField] private Transform _playerCamera;
        [SerializeField] private CharacterController _controller;
    #endregion
    
    #region Movement Settings
        private const float Jump = 8f;  // Increased from 6 to jump higher than enemies
        private const float Speed = 3f;
        private const float Sensitivity = 1f;
        private const float Gravity = 9.81f;
    #endregion
    
    private float currentSpeed;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = Speed;
    }

    void Update()
    {
        _playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        _playerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        
        MoveCamera();
        MovePlayer();
    }
    
    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(_playerMovementInput);

        if (_controller.isGrounded)
        {
            _velocity.y = -1f;
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _velocity.y = Jump;
            }
            
            if (Input.GetKey(KeyCode.LeftShift))
                currentSpeed = Speed * 3f;
            else
                currentSpeed = Speed;
        }
        else
        {
            _velocity.y += Gravity * -2f * Time.deltaTime;
        }
        
        // Apply horizontal movement
        _controller.Move(MoveVector * currentSpeed * Time.deltaTime);
        // Apply vertical movement (jump/gravity)
        _controller.Move(_velocity * Time.deltaTime);
    }
    
    private void MoveCamera()
    {
        _xRotation -= _playerMouseInput.y * Sensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.Rotate(0f, _playerMouseInput.x * Sensitivity, 0f);
        _playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }
}