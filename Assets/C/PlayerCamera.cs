using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;
    private Quaternion targetRotation;

    float RotationY;
    float RotationX;
    public float Min;
    public float Max;
    public float Speed;
    public float YPosOffset;
    public Vector2 framingOffset;
    public RaycastHit hit;
    public Vector3 camera_Offset;
    public LayerMask layer;
    public GameObject LastHitOb;
    public GameObject playerOb;
    public int LastHitIndex;
    public bool isWallLook;
    public float duration = 0.5f;
    public float majnitude = 0.1f;
    public SurfaceMove sf;
    public void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void Update()
    {
        LookRotation();
    }
    private void LookRotation()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        RotationX -= Input.GetAxis("Mouse Y") * Speed;
        RotationX = Mathf.Clamp(RotationX, Min, Max);
        RotationY += Input.GetAxis("Mouse X") * Speed;

        Quaternion targetRot = Quaternion.Euler(0f, RotationY, 0f);
        playerOb.transform.rotation = Quaternion.Slerp(playerOb.transform.rotation, targetRot, Time.deltaTime * 5f);
       // Target.transform.rotation = Quaternion.Slerp(Target.transform.rotation, targetRot, Time.deltaTime * 5f);

        if(sf.isClimbing)
        Target.transform.rotation = Quaternion.Slerp(Target.transform.localRotation, targetRot, Time.deltaTime * 5f);
        targetRotation = Quaternion.Euler(RotationX, RotationY, 0);

        if (Physics.Linecast(Target.position + new Vector3(0,0.5f,0), Target.position + targetRotation * Offset, out hit, layer))
        {
            transform.position = hit.point + new Vector3(0, YPosOffset, 0);
            transform.rotation = targetRotation;
        }
        else
        {
            if(LastHitOb != null)
            {
                Renderer hitRenderer = LastHitOb.GetComponent<Renderer>();
                if(hitRenderer != null)
                {
                    hitRenderer.enabled = false;
                    LastHitOb.layer = LastHitIndex;
                }
            }
            transform.position = Target.position + targetRotation * Offset;
            transform.rotation = targetRotation;

        }


       

    }
    private void WallLook()
    {
       
    }

}
