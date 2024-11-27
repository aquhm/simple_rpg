using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")] [SerializeField]
    private CharacterController characterController;

    [SerializeField] private Animator animator;

    [SerializeField] private Transform cameraTransform;

    [Header("Movement Settings")] [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField] private float runSpeedMultiplier = 2f;

    [SerializeField] private float rotationSmoothTime = 0.1f;

    [SerializeField] private float movementThreshold = 0.1f;

    [Header("Jump Settings")] [SerializeField]
    private float gravity = -19.62f;

    [SerializeField] private float jumpPower = 4.3f;

    [Header("Collision Settings")] [SerializeField]
    private Transform detector;

    [SerializeField] private LayerMask floorMask;

    [SerializeField] private bool isGrounded;

    [SerializeField] private bool _isJumpingAnimation;


    [Header("Sword")] [SerializeField] private GameObject swordBack;

    [SerializeField] private GameObject swordHand;


    [Header("Magic")] [SerializeField] private GameObject[] magics;

    [SerializeField] private Transform[] magicFirePosition;

    [SerializeField] private int magicId;

    [SerializeField] private bool wait;

    private bool _attacking;
    private float _currentRotationVelocity;
    private bool _defensing;

    private Vector3 _forceY;
    private bool _isJumping;
    private bool _isMoving;
    private bool _isRunning;

    private Vector3 _moveDirection;
    private bool _readySword;
    private Vector3 _velocity;


    private void Update()
    {
        _forceY.y += gravity * Time.deltaTime;
        characterController.Move(_forceY * Time.deltaTime);

        GetMovementInput();
        HandleMovement();
        HandleAction();
        UpdateAnimations();
        //Debug.Log($"_forceY = {_forceY}");
    }

    private void FixedUpdate()
    {
        if (detector != default)
        {
            isGrounded = Physics.CheckSphere(detector.position, 0.3f, floorMask);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
#endif

    private void GetMovementInput()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var verticalInput = Input.GetAxisRaw("Vertical");
        _moveDirection = new Vector3(horizontalInput, 0f, verticalInput);


        _isMoving = _moveDirection.magnitude >= movementThreshold;
        _isRunning = Input.GetKey(KeyCode.LeftShift) && _isMoving;
    }

    private void HandleMovement()
    {
        // 중력 적용
        if (characterController.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // 약간의 음수값을 주어 확실히 지면에 붙도록 함
        }

        _velocity.y += gravity * Time.deltaTime;

        if (_isMoving)
        {
            var targetAngle = CalculateTargetAngle();
            var smoothedAngle = CalculateSmoothedAngle(targetAngle);
            var moveVector = CalculateMoveVector(targetAngle);

            // 캐릭터 회전 적용
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            // 이동 속도 계산 및 적용
            var currentMoveSpeed = _isRunning ? moveSpeed * runSpeedMultiplier : moveSpeed;
            // 수평 이동 속도 설정
            _velocity.x = moveVector.x * currentMoveSpeed;
            _velocity.z = moveVector.z * currentMoveSpeed;

            //Debug.Log($"GetMovementInput _moveDirection = {_moveDirection}  moveVector = {moveVector} targetAngle = {targetAngle} smoothedAngle = {smoothedAngle} moveVector = {moveVector} _velocity = {_velocity}");
        }
        else
        {
            // 이동하지 않을 때는 수평 속도를 0으로
            _velocity.x = 0f;
            _velocity.z = 0f;
        }

        characterController.Move(_velocity * Time.deltaTime);
    }

    public void SetIsJumpingAnimation(bool doing)
    {
        _isJumpingAnimation = doing;
    }

    private void HandleAction()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded && _isJumpingAnimation == false /*characterController.isGrounded*/
           )
        {
            _isJumping = true;
            _forceY.y = Mathf.Sqrt(jumpPower * -2 * gravity);
        }
        else
        {
            _isJumping = false;
        }
        // else if (characterController.isGrounded)
        // {
        //     _isJumping = false;
        // }

        // ATTACK
        if (Input.GetButtonDown("Fire1") && _defensing)
        {
            if (_readySword)
            {
                _readySword = false;

                if (swordHand.activeSelf)
                {
                    animator.SetTrigger("sword");
                }
            }

            if (wait == false && _defensing == false)
            {
                wait = true;
                animator.SetTrigger("magic");
                animator.SetInteger("magicId", magicId);
            }
        }


        // DEFENSE
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Fire2 = true");

            animator.SetLayerWeight(2, 1);
            _defensing = true;
            animator.SetBool("shield", _defensing);
        }

        // DEFENSE
        if (Input.GetButtonUp("Fire2"))
        {
            Debug.Log("Fire2 = false");
            _defensing = false;

            animator.SetTrigger("saveSword");
            animator.SetLayerWeight(2, 0);

            animator.SetBool("shield", _defensing);
        }
    }

    public void SwordActive()
    {
        Debug.Log("SwordActive");
        swordBack?.SetActive(false);
        swordHand?.SetActive(true);
        _readySword = true;
    }

    public void SwordDeactive()
    {
        swordBack?.SetActive(true);
        swordHand?.SetActive(false);
    }

    public void StartMagic()
    {
        //wait = false;
        var temp = Instantiate(magics[magicId], magicFirePosition[magicId].position,
                magicFirePosition[magicId].rotation);
        Destroy(temp, 3f);
    }

    public void StopMagic()
    {
        wait = false;
    }


    private float CalculateTargetAngle()
    {
        var targetAngle = Mathf.Atan2(_moveDirection.x, _moveDirection.z) * Mathf.Rad2Deg;

        return targetAngle + cameraTransform.eulerAngles.y;
    }

    private float CalculateSmoothedAngle(float targetAngle)
    {
        return Mathf.SmoothDampAngle(transform.eulerAngles.y,
                targetAngle,
                ref _currentRotationVelocity,
                rotationSmoothTime);
    }

    private Vector3 CalculateMoveVector(float targetAngle)
    {
        return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
    }

    private void UpdateAnimations()
    {
        //Debug.Log($"_isMoving = {_isMoving} _isRunning = {_isRunning} _isJumping = {_isJumping}");
        if (_isMoving)
        {
            if (_isRunning)
            {
                animator.SetBool("running", true);
            }
            else
            {
                animator.SetBool("walking", true);
            }
        }
        else
        {
            // 이동하지 않을 때는 모두 false
            animator.SetBool("walking", false);
            animator.SetBool("running", false);
        }

        animator.SetBool("jump", _isJumping);
    }
}
