using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{
    #region Vector Components
        private Vector3 _velocity;
        private Vector3 _playerMovementInput;
        private Vector2 _playerMouseInput;
        private float _xRotation;
    #endregion
    #region Movement Components
        [SerializeField] private Transform _playerCamera;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Transform _player;
        [SerializeField] private NavMeshAgent _agent;
    #endregion
    
    //Movement
    float Jump = 6;
    float Speed = 3;
    private float Sensitivity = 1;
    private float Gravity = 9.81f;
    
    [SerializeField] private float jumpDuration = 0.8f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {

        _playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        _playerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        MovePlayer();
        MoveCamera();
        
    }
    
    bool jumping = false;
    
    System.Collections.IEnumerator DoJump(OffMeshLinkData data)
    {
        jumping = true;
        Vector3 start = _agent.transform.position;
        Vector3 end = data.endPos;
        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            float t = elapsed / jumpDuration;
            float height = Mathf.Sin(Mathf.PI * t) * Jump;
            _agent.transform.position = Vector3.Lerp(start, end, t) + Vector3.up * height;
            elapsed += Time.deltaTime;
            yield return null;
        }

        _agent.transform.position = end;
        _agent.CompleteOffMeshLink();
        jumping = false;
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
                if (_agent.isOnOffMeshLink && !jumping)
                {
                    StartCoroutine(DoJump(_agent.currentOffMeshLinkData));
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Speed *= 3;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                Speed = 3;
            }
            _controller.Move(MoveVector * Speed * Time.deltaTime);
        }
        else
        {
            _velocity.y += Gravity * -2f * Time.deltaTime;
        }
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
