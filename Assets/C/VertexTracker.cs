using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class VertexTracker : MonoBehaviour
{
    public Rigidbody rigid;

    public Vector3 Tensor = Vector3.one * 0.1f;

    public float PositionProportional = 150;
    public float PositionDerivative = 20f;

    public float RotationProportional = 1f;
    public float RotationDerivative = 0.1f;

    public float maxDepenetrationVelocity = 10f;

    private PDC positionPD;

    public Transform TransTarget;

    public Vector3 Force = Vector3.zero;

    public void Start()
    {
        positionPD = new PDC();
        rigid = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        if (PlayerSkill.Instance.isSlime)
        {
            if (!rigid)
                rigid = GetComponent<Rigidbody>();

            UpdateTracker();
        }
    }

    public void UpdateTracker()
    {
        rigid.inertiaTensor = Tensor;
        rigid.maxDepenetrationVelocity = maxDepenetrationVelocity;

        Vector3 PositionError;

        PositionError = TransTarget.position - transform.position;

        Force = positionPD.CalculatePD(PositionError, PositionProportional, PositionDerivative);

        rigid.AddForce(Force * rigid.mass);

    }

}
public class PDC 
{
    private Vector3 PD;

    private Vector3 LastE;
    private Vector3 P;
    private Vector3 D;

    public Vector3 CalculatePD(Vector3 e,float pk,float dk)
    {
        P = e * pk;
        D = ((e - LastE) / Time.deltaTime) * dk;

        PD = P + D;

        LastE = e;

        return PD;
    }

}

