using Unity.VisualScripting;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField]
    Transform point;
    void Start()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.position = point.position;
            collision.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.position = point.position;
            other.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
    }
    void Update()
    {
        
    }
}
