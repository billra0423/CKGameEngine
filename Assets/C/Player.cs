using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public float MaxHp = 100;
    public float currentHpPreview;
    public static float CurrentHp;
    public bool isDead;
    public GameObject JellyOb;
    public GameObject CurrentBullet;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void Start()
    {
        CurrentHp = MaxHp;
    }
    public void Update()
    {
        currentHpPreview = CurrentHp;
    }
    public void CreateJelly()
    {
        GameObject jellyOb = Instantiate(JellyOb);
        jellyOb.GetComponent<Jelly>().isSmaller = true;
        jellyOb.transform.position = transform.position  + new Vector3(0, 1f, 0);
        Rigidbody Brb = jellyOb.GetComponent<Rigidbody>();
        
        if (CurrentBullet != null && Brb != null)
        {
            Vector3 direction = (transform.position - CurrentBullet.transform.position).normalized;
            direction.y = 0.3f;
            Brb.AddForce(direction * 5f, ForceMode.Impulse);
        }
    }
    public void Damage(float damage)
    {
        CurrentHp -= damage;
        GameManager.instance.HpBar.value -= damage / 100;
        AudioManager.instance.PlaySfx("HitPlayer");
        if (PlayerSkill.Instance.isPlayer)
        {
            ParticleManager.instance.InstantiateParticle(transform.position + new Vector3(0, 1f, 0), "HitPlayerParticle");
        }
        else
        {
            CreateJelly();
            ParticleManager.instance.InstantiateParticle(transform.position + new Vector3(0, 0f, 0), "HitPlayerParticle");
        }
            Dead();
    }
    private void Dead()
    {

        if (CurrentHp <= 0)
        {
            isDead = true;
            GameManager.instance.GameOver();
            ParticleManager.instance.InstantiateParticle(transform.position + new Vector3(0, 1, 0), "DeadParticle");
            gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Success")
        {
            GameManager.instance.Success();
        }
        if (other.tag == "EnemyBullet")
        {
            CurrentBullet = other.gameObject;
            Damage(5);
            
            if(PlayerSkill.Instance.isSlime)
            Destroy(other,1f);

        }
        if(other.tag == "Jelly")
        {
            CurrentHp += 5;
            GameManager.instance.HpBar.value = CurrentHp / 100;
            ParticleManager.instance.InstantiateParticle(transform.position + new Vector3(0, 0f, 0), "EnemySlimeParticle");
            AudioManager.instance.PlaySfx("eat1");
            AudioManager.instance.PlaySfx("eat2");
            Destroy(other.gameObject);
        }
        if (PlayerSkill.Instance.isSlime)
        {
            if(other.tag == "Ground" || other.tag == "Wall")
            {
                // ParticleManager.instance.InstantiateParticle(transform.position + new Vector3(0, 0f, 0), "EnemySlimeParticle");
               // AudioManager.instance.PlaySfx("SlimeAttaching");
               // AudioManager.instance.PlaySfx("Transformation");
            }

        }
    }
}
