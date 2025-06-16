using UnityEngine;

public class SurfaceMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpVelocity = 10f;
    public float acceleration = 10f;

    [Header("Ground & Wall Settings")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public Transform referenceFrame;

    private Rigidbody rb;
    private SphereCollider coll;

    [Header("States")]
    public bool isGround;
    public bool isWall;
    public bool isClimbing;

    private Vector3 _groundNormal = Vector3.up;
    private Vector3 _lastSurfacePoint;
    private Vector3 inputDirection;
    private Vector3 inputDirectionRay;
    private Vector3 lastInputDirection;
    private float surfaceOffset = 0.1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
      
        if (rb != null)
        {
            rb.useGravity = false; 
           
        }
    }

    void Update()
    {
        HandleInput();
        GroundCheck();
        WallCheck();
        HandleJump();
        if (isClimbing)
            RotateModelToSurface();
    }

    void FixedUpdate()
    {
        ApplyPhysicsMovement();
    }

    void HandleInput()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        inputDirection = (transform.right * hor + transform.forward * ver).normalized;
        inputDirectionRay = new Vector3( hor ,0, ver);
        if (inputDirection != Vector3.zero)
            lastInputDirection = inputDirection;

        if (Input.GetKeyDown(KeyCode.V) && (isGround || isWall))
        {
            isClimbing = !isClimbing;

            if (isClimbing)
            {
                AudioManager.instance.PlaySfx("SlimeAttaching");
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isClimbing = false;
        }

        UpdateMovement();
    }

    void ApplyPhysicsMovement()
    {
        if (!isClimbing && inputDirection != Vector3.zero && referenceFrame != null)
        {
            Vector3 force = referenceFrame.TransformDirection(inputDirectionRay) * acceleration;
            force.y = 0;
            rb.AddForce(force, ForceMode.Acceleration);
        }
    }

    void UpdateMovement()
    {
        if (!isClimbing || inputDirection == Vector3.zero) return;

        Vector3 move = GetProjectedMovement(inputDirection * moveSpeed) * Time.fixedDeltaTime;
        transform.position += move;

        Vector3 offsetPosition = _lastSurfacePoint + _groundNormal * surfaceOffset;
        transform.position = Vector3.Lerp(transform.position, offsetPosition, 0.1f);
    }
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (isGround || isWall))
        {
          
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
            AudioManager.instance.PlaySfx("Jump");
        }
    }
    public Vector3 GetProjectedMovement(Vector3 movement)
    {
        return Vector3.ProjectOnPlane(movement, _groundNormal);
    }

    void GroundCheck()
    {
        isGround = false;

        if (Physics.Raycast(transform.position, -_groundNormal, out RaycastHit hit, 1f, groundLayer))
        {
            UpdateGroundNormal(hit.normal, hit.point);
            isGround = true;
        }
    }

    void WallCheck()
    {
        isWall = false;

        float rayLength = 0.3f;
        float offset = coll ? coll.radius : 0.2f;

        Vector3 center = transform.position;

        if (Physics.Raycast(center, lastInputDirection, out RaycastHit frontHit, 0.3f, wallLayer))
        {
            UpdateGroundNormal(frontHit.normal, frontHit.point);
            isWall = true;
            return;
        }

        Vector3 direction = new Vector3(inputDirectionRay.x, -1, inputDirectionRay.z).normalized;
        Vector3 worldDir = transform.TransformDirection(direction);
        Vector3 outerPoint = center + worldDir * offset;
        Vector3 target = center + transform.TransformDirection(Vector3.down) * offset;
        Vector3 rayDir = (target - outerPoint).normalized;

        if (Physics.Raycast(outerPoint, rayDir, out RaycastHit hit, rayLength, wallLayer))
        {
            UpdateGroundNormal(hit.normal, hit.point);
            isWall = true;
        }
    }

    void UpdateGroundNormal(Vector3 newNormal, Vector3 hitPoint)
    {
        _groundNormal = newNormal;
        _lastSurfacePoint = hitPoint;
    }

    void RotateModelToSurface()
    {
        if (isGround || isWall)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, _groundNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        float rayLength = 0.3f;
        float offset = coll ? coll.radius : 0.2f;

        Vector3 direction = new Vector3(inputDirectionRay.x, -1, inputDirectionRay.z).normalized;
        Vector3 worldDir = transform.TransformDirection(direction);
        Vector3 outerPoint = transform.position + worldDir * offset;
        Vector3 target = transform.position + transform.TransformDirection(Vector3.down) * offset;
        Vector3 rayDir = (target - outerPoint).normalized;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(outerPoint, outerPoint + rayDir * rayLength);
        Gizmos.DrawSphere(outerPoint, 0.02f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - _groundNormal);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + lastInputDirection * 0.7f);
    }
}