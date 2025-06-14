using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

public class PlayerArm : MonoBehaviour
{
    public static PlayerArm Instance;
    [Header("Arm References")]
    public GameObject armL, armR;
    public Transform StartHandPointL, StartHandPointR;
    public GameObject LHandPos, RHandPos;
    public GameObject LArmPos, RArmPos;
    public Animator animator;

    [Header("Grappling Setup")]
    public LineRenderer lrL, lrR;
    private SpringJoint jointL, jointR;
    public Transform GrapplingPosL, GrapplingPosR;
    public float RayLength = 10f;
    public LayerMask layer;

    [Header("States")]
    public bool isGrapping;
    public bool isMovingL, isMovingR;
    public bool isAttachingL, isAttachingR;
    public bool isStartGrapL, isStartGrapR;
    public bool isShootL, isShootR;

    [Header("Objects & Aim")]
    public GameObject HitObL, HitObR, AimPoint;

    private RaycastHit hit, hitL, hitR;
    private Ray ray;
    private Coroutine leftGrappleCoroutine, rightGrappleCoroutine;
    private Coroutine leftMoveCoroutine, rightMoveCoroutine;
    public float stopDistance = 0.2f;
    public float ShootSpeed = 10f;
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }
    private void Start()
    {
        LArmPos.gameObject.SetActive(false);
        RArmPos.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.instance.isPlay)
        {
            HandleInput();
            AttachArm();
        }
    }

    private void LateUpdate()
    {
        if (GameManager.instance.isPlay)
        {
            RotateArms();
            UpdateHandAndGrapplePositions();
        }
    }
    public void OffHand()
    {
        ResetLeftArm();
        ResetRightArm();
    }
    private void HandleInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            ResetLeftArm();
        }

        if (Input.GetMouseButtonUp(1))
        {
            ResetRightArm();
        }

        if (Input.GetKeyDown(KeyCode.Q) && isAttachingL) ShootOb(HitObL, true);
        if (Input.GetKeyDown(KeyCode.E) && isAttachingR) ShootOb(HitObR, false);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isAttachingL && !isStartGrapL && !isMovingL)
            {
                isStartGrapL = true;
                StartGrapple(HitObL, ref jointL, ref armL, ref GrapplingPosL);
            }
            if (isAttachingR && !isStartGrapR && !isMovingR)
            {
                isStartGrapR = true;
                StartGrapple(HitObR,ref jointR, ref armR, ref GrapplingPosR);
            }
        }

        Grap();
    }
    private void ResetLeftArm()
    {
        if (leftGrappleCoroutine != null)
            StopCoroutine(leftGrappleCoroutine);
        if (leftMoveCoroutine != null)
            StopCoroutine(leftMoveCoroutine);

        animator.SetBool("LeftAttack", false);
        LArmPos.gameObject.SetActive(false);
        if (HitObL.GetComponent<Rigidbody>())
        {
            if (HitObL.transform.gameObject.layer != 13)
                HitObL.transform.gameObject.layer = 11;
        }
        if (HitObL?.GetComponent<Enemy>() != null && !isShootL)
            HitObL.GetComponent<Enemy>().ResetEnemy();

        if (isAttachingL)
            StartCoroutine(StopGrapL());
    }

    private void ResetRightArm()
    {
        if (rightGrappleCoroutine != null)
            StopCoroutine(rightGrappleCoroutine);
        if (rightMoveCoroutine != null)
            StopCoroutine(rightMoveCoroutine);

        animator.SetBool("RightAttack", false);
        RArmPos.gameObject.SetActive(false);
        if (HitObR.GetComponent<Rigidbody>())
        {
            if(HitObR.transform.gameObject.layer != 13)
                HitObR.transform.gameObject.layer = 11;
        }
      
        if (HitObR?.GetComponent<Enemy>() != null && !isShootR)
            HitObR.GetComponent<Enemy>().ResetEnemy();

        if (isAttachingR)
            StartCoroutine(StopGrapR());
    }
    private void AttachArm()
    {
        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out hit, RayLength, layer))
        {
            AimPoint.SetActive(true);

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetBool("LeftAttack", true);
                animator.SetTrigger("LeftT");
                LArmPos.SetActive(true);
                hitL = hit;
                leftGrappleCoroutine = StartCoroutine(StartGrapLAfterDelay(0.3f));
                AudioManager.instance.PlaySfx("Attack");
            }

            if (Input.GetMouseButtonDown(1))
            {
                animator.SetBool("RightAttack", true);
                animator.SetTrigger("RightT");
                RArmPos.SetActive(true);
                hitR = hit;
                rightGrappleCoroutine = StartCoroutine(StartGrapRAfterDelay(0.3f));
                AudioManager.instance.PlaySfx("Attack");
            }
        }
        else
        {
            AimPoint.SetActive(false);
        }
    }

    IEnumerator StartGrapLAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (hitL.collider == null) yield break;

        HitObL = hitL.transform.gameObject;
        if (HitObL.transform.GetComponent<Enemy>())
        {


            HitObL.transform.GetComponent<Enemy>().ani.SetBool("isStun",true);
        }
        isGrapping = true;
        RayGrap(ref lrL, ref GrapplingPosL, ref isAttachingL, hitL);

        if (leftMoveCoroutine != null)
        {
            StopCoroutine(leftMoveCoroutine);
            leftMoveCoroutine = null;
        }

        isMovingL = true;
        leftMoveCoroutine = StartCoroutine(MoveArm(LHandPos, GrapplingPosL, StartHandPointL, isAttachingL));
    }

    IEnumerator StartGrapRAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (hitR.collider == null) yield break;
        HitObR = hitR.transform.gameObject;

        if (HitObR.transform.GetComponent<Enemy>())
        {


            HitObR.transform.GetComponent<Enemy>().ani.SetBool("isStun", true);
        }
        isGrapping = true;
        RayGrap(ref lrR, ref GrapplingPosR, ref isAttachingR, hitR);

        if (rightMoveCoroutine != null)
        {
            StopCoroutine(rightMoveCoroutine);
            rightMoveCoroutine = null;
        }

        isMovingR = true;
        rightMoveCoroutine = StartCoroutine(MoveArm(RHandPos, GrapplingPosR, StartHandPointR, isAttachingR));
    }

    IEnumerator StopGrapL()
    {
        isStartGrapL = false;
        StopGrapple(ref lrL, ref jointL, ref isAttachingL);
        isAttachingL = false;

        if (leftMoveCoroutine != null)
        {
            StopCoroutine(leftMoveCoroutine);
            leftMoveCoroutine = null;
        }

        yield return StartCoroutine(MoveArm(LHandPos, GrapplingPosL, StartHandPointL, isAttachingL));
    }

    IEnumerator StopGrapR()
    {
        isStartGrapR = false;
        StopGrapple(ref lrR, ref jointR, ref isAttachingR);
        isAttachingR = false;

        if (rightMoveCoroutine != null)
        {
            StopCoroutine(rightMoveCoroutine);
            rightMoveCoroutine = null;
        }

        yield return StartCoroutine(MoveArm(RHandPos, GrapplingPosR, StartHandPointR, isAttachingR));
    }

    public void RayGrap(ref LineRenderer lr, ref Transform GrapplingPos, ref bool isAttaching, RaycastHit hitData)
    {
        isShootL = false;
        isShootR = false;
        isAttaching = true;
        if (hitData.transform.GetComponent<Rigidbody>())
        {
            if (hitData.transform.tag == "Enemy")
            {
                GrapplingPos.position = hitData.transform.position + new Vector3(0,1.5f,0);
                GrapplingPos.SetParent(hitData.transform);
            }
            else
            {
                GrapplingPos.position = hitData.transform.position;
                GrapplingPos.SetParent(hitData.transform);
            }
            
        }
        else
        {
            GrapplingPos.position = hitData.point;
            GrapplingPos.SetParent(hitData.transform);
        }
       
        lr.positionCount = 2;
    }

    public void StartGrapple(GameObject hitob, ref SpringJoint joint, ref GameObject arm, ref Transform GrapplingPos)
    {
        if (hitob?.GetComponent<Rigidbody>() == null )
        {
            isGrapping = true;
            joint = gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = GrapplingPos.position;
            float distance = Vector3.Distance(transform.position + Vector3.up, GrapplingPos.position);
            AudioManager.instance.PlaySfx("Grappling"); 
            joint.maxDistance = distance * 0.3f;
            joint.minDistance = distance * 0.25f;
            joint.spring = 10f;
            joint.damper = 7;
            joint.massScale = 4.5f;
        }
    }

    public void StopGrapple(ref LineRenderer lr, ref SpringJoint joint, ref bool isAttaching)
    {
        isGrapping = false;
        isAttaching = false;
        lr.positionCount = 0;
        Destroy(joint);
    }

    
    public void Grap()
    {
        if (!isAttachingL && !isAttachingR && !isMovingL && !isMovingR) return;

        ApplyPullForce(HitObL, armL, isAttachingL, ref isMovingL);
        ApplyPullForce(HitObR, armR, isAttachingR, ref isMovingR);
        // 왼팔 물체 -> 왼손 쪽으로 회전
        if (isAttachingL && HitObL != null)
        {
            LookAtHand(HitObL, armL.transform.position, GrapplingPosL);
        }

        // 오른팔 물체 -> 오른손 쪽으로 회전
        if (isAttachingR && HitObR != null)
        {
            LookAtHand(HitObR, armR.transform.position, GrapplingPosR);
        }
    }

    private void ApplyPullForce(GameObject targetObj, GameObject arm, bool isAttached, ref bool isMoving)
    {
        if (!isAttached || isMoving || targetObj == null) return;

        Rigidbody rb = targetObj.GetComponent<Rigidbody>();
        if (rb == null) return;
        targetObj.transform.tag = "PlayerBullet";
        targetObj.transform.gameObject.layer = 12;

        if (targetObj.GetComponent<Enemy>())
        {
            targetObj.GetComponent<Enemy>().isStun = true;
            targetObj.GetComponent<Enemy>().isAttached = true;
            transform.gameObject.layer = 11;
        }

        Vector3 dir = (arm.transform.position - rb.position);
        float distance = dir.magnitude;

        if (distance > stopDistance)
        {
            dir.Normalize();
            float forceMagnitude = 20 * Mathf.Clamp01(distance);
            rb.AddForce(dir * forceMagnitude, ForceMode.Force);
            rb.linearDamping = 1;
        }
        else
        {
            rb.linearVelocity = new Vector3(0,0,0);
            rb.angularVelocity = new Vector3(0, 0, 0);
            rb.linearDamping = 0f;
        }
    }

    private void LookAtHand(GameObject targetObj, Vector3 handPosition, Transform aTransform)
    {
        if (targetObj == null || targetObj.GetComponent<Rigidbody>() == null)
            return;

        Vector3 directionToHand = (handPosition - targetObj.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToHand);

        targetObj.transform.rotation = Quaternion.Slerp(
            targetObj.transform.rotation,
            targetRotation,
            Time.deltaTime * 1
        );

    }
    public void ShootOb(GameObject obj, bool isLeft)
    {
        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(ray.direction * ShootSpeed, ForceMode.Impulse);
            obj.transform.gameObject.layer = 11;
            if (obj.GetComponent<Enemy>())
            {
               
                obj.GetComponent<Enemy>().isAttached = false;
            }
            if (isLeft)
            {
                isShootL = true;
                StopGrapple(ref lrL, ref jointL, ref isAttachingL);
                animator.SetBool("LeftAttack", false);
                AudioManager.instance.PlaySfx("Throwing");
            }
            else
            {
                isShootR = true;
                StopGrapple(ref lrR, ref jointR, ref isAttachingR);
                animator.SetBool("RightAttack", false);
                AudioManager.instance.PlaySfx("Throwing");
            }
        }
    }

    IEnumerator MoveArm(GameObject Arm, Transform GrapplingPos, Transform StartHandPoint, bool isAttaching)
    {
        float duration = 0.3f;
        float elapsedTime = 0f;

        Transform target = isAttaching ? GrapplingPos : StartHandPoint;
        Vector3 startPosition = Arm.transform.position;
        Vector3 endPosition = target.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Arm.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        Arm.transform.position = endPosition;
        if(isAttaching)
        AudioManager.instance.PlaySfx("Attaching");
        if (Arm == LHandPos) isMovingL = false;
        else if (Arm == RHandPos) isMovingR = false;
    }

    private void RotateArms()
    {
        RotateArm(LArmPos, GrapplingPosL);
        RotateArm(RArmPos, GrapplingPosR);
    }

    private void RotateArm(GameObject arm, Transform target)
    {
        Vector3 direction = (target.position - arm.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
        arm.transform.rotation = targetRotation;
    }

    private void UpdateHandAndGrapplePositions()
    {
        if (isAttachingL && !isMovingL) LHandPos.transform.position = GrapplingPosL.position;
        if (isAttachingR && !isMovingR) RHandPos.transform.position = GrapplingPosR.position;

        if (!isAttachingL)
        {
            GrapplingPosL.position = StartHandPointL.position;
            LHandPos.transform.position = StartHandPointL.position;
            GrapplingPosL.parent = null;
        }

        if (!isAttachingR)
        {
            GrapplingPosR.position = StartHandPointR.position;
            RHandPos.transform.position = StartHandPointR.position;
            GrapplingPosR.parent = null;
        }

      
    }

  
}