using UnityEngine;

public class SlimeController : MonoBehaviour
{
    public Transform referenceFrame;
    public float acceleration = 100;
    public float jumpPower = 10;

    [Range(0, 1)]
    public float airControl = 0.3f;

    Vector3 direction;
    GameObject slime;
    bool onGround = false;

    private void Start()
    {
        slime = gameObject;
    }

    private void Update()
    {
        if(referenceFrame != null)
        {
            direction = Vector3.zero;

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

            direction.y = 0;

           
            float effectiveAcceleration = acceleration;

            if (!onGround)
                effectiveAcceleration *= airControl;

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
        slime.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Acceleration);
    }
}
