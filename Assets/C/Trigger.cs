using UnityEngine;
using static UnityEditor.Progress;

public class Trigger : MonoBehaviour
{
    public int keyCount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerBullet")
        {

          
            // AudioManager.instance.PlaySfx(AudioManager.Sfx.ChangeFrom);
        
            //ParticleManager.instance.InstantiateParticle(transform.position, "Bullet");
            
            if (other.gameObject.GetComponent<Rigidbody>() != null)
            {
                other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            AudioManager.instance.PlaySfx("HitPlayer");
            other.gameObject.transform.position = transform.position;
            other.gameObject.transform.rotation = transform.rotation;
            keyCount++;
        }
    }
}
