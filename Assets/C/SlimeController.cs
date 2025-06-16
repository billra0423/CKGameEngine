using UnityEngine;

public class SlimeController : MonoBehaviour
{
    public Transform referenceFrame;
    public float acceleration = 100;
    public float jumpPower = 10;

    [Range(0, 1)]
    public float airControl = 0.3f;

    Vector3 direction;
    Vector3 RayDirection;
    GameObject slime;
    bool onGround = false;

    private void Start()
    {
        slime = gameObject;
        rb = GetComponent<Rigidbody>();
    }
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float wallCheckDistance = 1f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private LayerMask climbableLayer;
    [SerializeField] private Transform sphereVisual; // 시각적인 공 모델
    [SerializeField] private float sphereOffset = 0.5f;
    private Vector3 inputDirection;
    public bool isClimbing = false;
    private Rigidbody rb;
    private bool CheckWall(out RaycastHit hit)
    {
        return Physics.Raycast(transform.position, RayDirection, out hit, wallCheckDistance, climbableLayer);
    }

    private void EnterClimbMode()
    {
        isClimbing = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    private void ExitClimbMode()
    {
        isClimbing = false;
        rb.useGravity = true;
    }

    private void Climb(RaycastHit hit)
    {
        // 벽에 밀착
        Vector3 targetPos = hit.point + hit.normal * sphereOffset;
        transform.position = Vector3.Lerp(transform.position, transform.TransformDirection(Vector3.down), Time.deltaTime * climbSpeed);

        // 회전 보정
        Vector3 wallNormal = hit.normal;
        Vector3 wallForward = Vector3.Cross(transform.right, wallNormal); // 또는 벽의 이동 방향
        Quaternion targetRotation = Quaternion.LookRotation(wallForward, wallNormal);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 현재 공의 로컬 기준 방향
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        Vector3 forward = transform.forward;

        // 입력 기반 방향 설정 (x: 좌우, y: 상하)
        inputDirection = new Vector3(h, 0, v).normalized;
        if (CheckWall(out RaycastHit hit))
        {
            if (!isClimbing)
            {
                EnterClimbMode();
            }

            Climb(hit);
        }
        else
        {
            if (isClimbing)
            {
                //    ExitClimbMode();
            }
        }
        if (isClimbing)
        {
            //Climb(hit);
        }
        if (referenceFrame != null)
        {
            direction = Vector3.zero;
            if (!isClimbing)
            {
               

                if (Input.GetKey(KeyCode.W))
                {
                    direction += referenceFrame.forward * acceleration;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    direction += -referenceFrame.right * acceleration;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    direction += -referenceFrame.forward * acceleration;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    direction += referenceFrame.right * acceleration;
                }
            }
            else
            {

                if (Input.GetKey(KeyCode.W))
                {
                    direction += referenceFrame.forward;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    direction -= referenceFrame.forward;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    direction -= referenceFrame.right;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    direction += referenceFrame.right;
                }

            }

            direction.y = 0;


            float effectiveAcceleration = acceleration;

            if (!onGround)
                effectiveAcceleration *= airControl;

            RayDirection = transform.TransformDirection(Vector3.down);//direction.normalized;
            direction = direction.normalized * effectiveAcceleration;
            

            if (Input.GetKeyDown(KeyCode.Space))
            {
                onGround = false;
                slime.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            }
        }
    }
    private void FixedUpdate()
    {
        if (!isClimbing)
        {
            slime.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Acceleration);
        }
        else
        {
            if (direction != Vector3.zero)
            {
                direction = direction.normalized;
                transform.position += direction * 10 * Time.deltaTime;
            }
        }
           
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = transform.forward;

        if (Physics.Raycast(rayOrigin, RayDirection, out RaycastHit hit, wallCheckDistance, climbableLayer))
        {
            Gizmos.DrawRay(rayOrigin, RayDirection * hit.distance);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.05f);
        }
        else
        {
            Gizmos.DrawRay(rayOrigin, RayDirection * wallCheckDistance);
        }
    }
}
