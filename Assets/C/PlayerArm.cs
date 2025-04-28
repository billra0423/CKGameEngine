using UnityEngine;

public class PlayerArm : MonoBehaviour
{
    public GameObject armL;
    public GameObject armR;

    public Transform TargetPointL;
    public Transform TargetPointR;
    public GameObject TargetObL;
    public GameObject TargetObR;

    public RaycastHit hit;
    public Ray ray;
    public GameObject HitObL;
    public GameObject HitObR;
    public float RayLength;
    public float rotationSmoothTime = 0.1f;

    public LayerMask layer;
    public bool isMoving;
    public bool isGrapping;
    public bool isAttachingL;
    public bool isAttachingR;
    public bool isArmTarGetL;
    public bool isArmTarGetR;
    public bool isHit;
    [Header("Grappling")]
    public SpringJoint jointL;
    public SpringJoint jointR;
    public LineRenderer lrL;
    public LineRenderer lrR;
    public float disL;
    public float disR;
    public bool isStartGrapL;
    public bool isStartGrapR;
    public Transform GrapplingPosL;
    public Transform GrapplingPosR;
    public Animator animator;
    [Header("Shoot")]
    public float ShootSpeed;

    private void Update()
    {
        AttachArm();
        if (Input.GetMouseButtonUp(0))
        {
            if (isAttachingL)
            {
                Invoke("StopGrapL", 0f);

                animator.SetBool("LeftAttack", false);
            }

        }
        if (Input.GetMouseButtonUp(1))
        {
            if (isAttachingR)
            {
                Invoke("StopGrapR",0f);

                animator.SetBool("RightAttack", false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ShootOb(HitObL, true);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShootOb(HitObR, false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isAttachingL && !isStartGrapL)
            {
                isStartGrapL = true;
                StartGrapple(ref jointL, ref armL, ref GrapplingPosL);
            }
            if (isAttachingR && !isStartGrapR)
            {
                isStartGrapR = true;
                StartGrapple(ref jointR, ref armR, ref GrapplingPosR);
            }
        }
        Grap();
    }
    private void LateUpdate()
    {
        DrawRope();
    }
    private void DrawRope()
    {
        if (isAttachingL)
        {
            lrL.SetPosition(0, armL.transform.position);
            lrL.SetPosition(1, GrapplingPosL.position);
          
        }
        if (isAttachingR)
        {
            lrR.SetPosition(0, armR.transform.position);
            lrR.SetPosition(1, GrapplingPosR.position);
           
        }
    }
    public void AttachArm()
    {
        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out hit, RayLength))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Invoke("StartGrapL", 0f);

            }
            if (Input.GetMouseButtonDown(1))
            {
                Invoke("StartGrapR", 0f);

            }
        }
    }
    public void StartGrapL()
    {
        animator.SetBool("LeftAttack", true);
        animator.SetTrigger("LeftT");
        HitObL = hit.transform.gameObject;
        isGrapping = true;
        isAttachingL = true;

        RayGrap(ref lrL, ref GrapplingPosL, ref isAttachingL);
      
    }
    public void StartGrapR()
    {
        animator.SetBool("RightAttack", true);
        animator.SetTrigger("RightT");
        HitObR = hit.transform.gameObject;
        isGrapping = true;

        isAttachingR = true;
        RayGrap(ref lrR, ref GrapplingPosR, ref isAttachingR);
      
    }
    public void StopGrapL()
    {
        isStartGrapL = false;
        StopGrapple(ref lrL, ref jointL, ref isAttachingL);

    }
    public void StopGrapR()
    {
        isStartGrapR = false;
        StopGrapple(ref lrR, ref jointR, ref isAttachingR);
    }
    public void RayGrap(ref LineRenderer lr, ref Transform GrapplingPos, ref bool isAttaching)
    {
        isAttaching = true;
        GrapplingPos.position = hit.point;
        GrapplingPos.transform.SetParent(hit.transform);
        lr.positionCount = 2;
    }
    public void StartGrapple(ref SpringJoint joint, ref GameObject arm, ref Transform GrapplingPos)
    {
        if (hit.transform.GetComponent<Rigidbody>() == null)
        {
            isGrapping = true;
            joint = transform.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = GrapplingPos.position;

            float Distance = Vector3.Distance(transform.position + new Vector3(0,1,0), GrapplingPos.position);

            joint.maxDistance = Distance * 0.3f;
            joint.minDistance = Distance * 0.25f;

            joint.spring =10f;
            joint.damper = 7;
            joint.massScale = 4.5f;
        }
    }
    public void StopGrapple(ref LineRenderer lr, ref SpringJoint joint, ref bool isAttaching)
    {
        isGrapping = false;
        isHit = false;
        isAttaching = false;
        isMoving = false;

        lr.positionCount = 0;
        Destroy(joint);
    }
    public float stopDistance;
    public void Grap()
    {
        if (!isAttachingL && !isAttachingR) return;
        if (isAttachingL)
        {
            if (HitObL.transform != null && HitObL.transform.GetComponent<Rigidbody>() != null)
            {
                Rigidbody hitRigidbody = HitObL.transform.GetComponent<Rigidbody>();

                Vector3 targetPosition = armL.transform.position;
                Vector3 dir = (targetPosition - hitRigidbody.position);
                float distance = dir.magnitude;

                if (distance > stopDistance)
                {
                    dir.Normalize();
                    float forceMagnitude = 300 * Mathf.Clamp01(distance); 
                    hitRigidbody.AddForce(dir * forceMagnitude, ForceMode.Force); 

                  
                    hitRigidbody.linearDamping = 5;
                }
                else
                {
                    hitRigidbody.linearVelocity = Vector3.zero;
                    hitRigidbody.angularVelocity = Vector3.zero;
                    hitRigidbody.linearDamping = 0f; 
                }
            }
        }
        if (isAttachingR)
        {
            if (HitObR.transform != null && HitObR.transform.GetComponent<Rigidbody>() != null)
            {


                Rigidbody hitRigidbody = HitObR.transform.GetComponent<Rigidbody>();

                Vector3 targetPosition = armR.transform.position;
                Vector3 dir = (targetPosition - hitRigidbody.position);
                float distance = dir.magnitude;

                if (distance > stopDistance)
                {
                    dir.Normalize();
                    float forceMagnitude = 300 * Mathf.Clamp01(distance); 
                    hitRigidbody.AddForce(dir * forceMagnitude, ForceMode.Force); 

                  
                    hitRigidbody.linearDamping = 5;
                }
                else
                {
                    hitRigidbody.linearVelocity = Vector3.zero;
                    hitRigidbody.angularVelocity = Vector3.zero;
                    hitRigidbody.linearDamping = 0f;
                }

            }
        }
    }

    public void ShootOb(GameObject HitOb, bool isLR)
    {
        if (HitOb.GetComponent<Rigidbody>() != null)
        {
            HitOb.transform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            HitOb.transform.GetComponent<Rigidbody>().linearDamping = 0;
            HitOb.transform.GetComponent<Rigidbody>().AddForce(ray.direction * ShootSpeed, ForceMode.Impulse);
            if (isLR)
            {
                StopGrapple(ref lrL, ref jointL, ref isAttachingL);
                animator.SetBool("LeftAttack", false);
              
            }
            else
            {
                StopGrapple(ref lrR, ref jointR, ref isAttachingR);
                animator.SetBool("RightAttack", false);
              
            }
        }
    }

}
