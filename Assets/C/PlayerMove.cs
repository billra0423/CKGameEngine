using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Rigidbody rigid;
    public GameObject CameraOb;
    public GameObject PlayerCenter;

    [Header("Movement")]
    public float moveSpeed;
    public float runSpeed;
    public float walkSpeed;
    public float groundDrag;
    public float TransSpeed;
    public float RotationSpeed;
    public bool isMove;
    public bool isRun;
    public bool isSlope;
    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool isJump;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode RunKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask GroundMask;
    public bool isGround;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Animation")]
    public Animator animator;

    float horizontalInput;
    float verticalInput;
    public Vector3 offset;
    public Vector3 moveDirection;
    public Transform orientation;

    RaycastHit hit;
    Ray rya;

    public MovementState state;

    public enum MovementState
    {
        walking,
        Run,
        air
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        TransSpeed = moveSpeed;
        readyToJump = true;
    }

    private void Update()
    {
        GroundCheck();
        MyInput();
        SpeedControl();
        StateHendler();
        Run();
        isSlope = OnSlope();
        animator.SetFloat("vInput", verticalInput);
        animator.SetFloat("hzInput", -(horizontalInput));
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (Input.GetKey(jumpKey) && readyToJump && isGround)
        {
            readyToJump = false;
            animator.SetBool("Falling", true);
            isJump = true;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

    }
    private void StateHendler()
    {
        if (isGround && Input.GetKey(RunKey))
        {
            animator.SetBool("Running", true);
            state = MovementState.Run;
            moveSpeed = runSpeed;
        }
        else if (isGround)
        { 
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {

            state = MovementState.air;
        }


    }
    private void MovePlayer()
    {
        Vector3 inputDirection = new Vector3(verticalInput, 0, horizontalInput).normalized;

        if (inputDirection.magnitude > 0)
        {
            animator.SetBool("Walking", true);
            isMove = true;
        }
        else
        {
            animator.SetBool("Walking", false);
            isMove = false;
        }
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;


        if (OnSlope() && !exitingSlope)
        {
            rigid.AddForce(GetSlopeMoveDirection() * moveSpeed * 10f, ForceMode.Force);
            if (rigid.linearVelocity.y > 0)
            {
                if (isRun)
                {

                    rigid.AddForce(Vector3.down * 80f, ForceMode.Force);
                }
                else
                {
                    rigid.AddForce(Vector3.down * 30f, ForceMode.Force);
                }
            }
        }
        else if (isGround)
        {
            if(isMove)
            rigid.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            if (isMove)
                rigid.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }



        rigid.useGravity = !OnSlope();
    }
    public void GroundCheck()
    {
        isGround = Physics.Raycast(PlayerCenter.transform.position, Vector3.down, playerHeight * 0.5f + 1f, GroundMask);
        if (isGround && readyToJump)
        {
            isJump = false;
            animator.SetBool("Falling", false);

        }
    }
    public void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
        }
        else
        {
            animator.SetBool("Running", false);
            isRun = false;
        }
    }
    private void Jump()
    {
        exitingSlope = true;

        rigid.linearVelocity = new Vector3(rigid.linearVelocity.x, 0f, rigid.linearVelocity.z);
        rigid.AddForce(transform.up * jumpForce, ForceMode.Force);

    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }
    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rigid.linearVelocity.magnitude > moveSpeed)
                rigid.linearVelocity = rigid.linearVelocity.normalized * moveSpeed;
        }
        else
        {



            Vector3 flatVel = new Vector3(rigid.linearVelocity.x, 0f, rigid.linearVelocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * TransSpeed;
                rigid.linearVelocity = new Vector3(limitedVel.x, rigid.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(PlayerCenter.transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 1))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
    private void OnDrawGizmosSelected()
    {
      
        Vector3 start = transform.position;

        
        Vector3 slopeDirection = GetSlopeMoveDirection();

       
        Gizmos.color = Color.blue;

       
        Gizmos.DrawRay(start, slopeDirection * 2f);
    }
}
