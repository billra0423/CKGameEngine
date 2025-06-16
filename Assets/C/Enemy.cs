using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Hp")]
    public float MaxHp;
    public float CurrentHp;
    public bool isDead;

    [Header("Movement")]
    public NavMeshAgent agent;
    public float DetectionRange;
    public float moveSpeed;
    public float rotationSpeed;

    [Header("Attack")]
    public GameObject Target;
    public GameObject bullet;
    public Transform ShootPos;
    public float attackRange;
    public float bulletSpeed;
    public float attackRate;

    [Header("State")]
    public bool isAttack;
    public bool isStun;
    public bool isAttached;

    [Header("Animation")]
    public Animator ani;

    private Rigidbody rb;
    public CapsuleCollider EC;

    void Start()
    {
        Target = GameObject.Find("Slime");
        Initialize();
    }

    void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        CurrentHp = MaxHp;
        agent.speed = moveSpeed;
       
        StartCoroutine(AttackLoop());
        StopMoving();
    }

    void Update()
    {
        if (isDead)
            return;

        if (isStun)
        {
            HandleStunState();
            return;
        }

        if (!GameManager.instance.isPlay)
            return;

        HandleAI();
    }

    void HandleAI()
    {
        float distanceToTarget = Vector3.Distance(Target.transform.position, transform.position);
        if (!GameManager.instance.isPlay)
        {
            OnOutOfRange();
        }
        if (distanceToTarget > DetectionRange)
        {
            OnOutOfRange();
        }
        else if (distanceToTarget > attackRange)
        {
            OnChasing();
        }
        else
        {
            OnInAttackRange();
        }
    }
    void HandleStunState()
    {
        agent.enabled = false;
        ani.SetBool("Running", false);
        ani.SetBool("isAttack", false);

    }
    void OnOutOfRange()
    {
        ani.SetBool("Running", false);
        ani.SetBool("isAttack", false);
        isAttack = false;
        StopMoving();
    }

    void OnChasing()
    {
        LookAtPlayer();
        MoveToTarget();
        isAttack = false;
    }

    void OnInAttackRange()
    {
        LookAtPlayer();
        AttackReady();
    }

    void LookAtPlayer()
    {
        Vector3 direction = Target.transform.position - transform.position;
        direction.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void MoveToTarget()
    {
        agent.SetDestination(Target.transform.position);
        ani.SetBool("Running", true);
        ani.SetBool("isAttack", false);
    }

    void StopMoving()
    {
        agent.velocity = Vector3.zero;
        agent.SetDestination(transform.position);
    }

    void AttackReady()
    {
        StopMoving();
        isAttack = true;
        ani.SetBool("Running", false);
        ani.SetBool("isAttack", true);
    }

    IEnumerator AttackLoop()
    {
        while (!isDead)
        {
            if (isAttack && !isStun && GameManager.instance.isPlay)
            {
                yield return new WaitForSeconds(1);
                ShootBullet();
                yield return new WaitForSeconds(attackRate);
            }

            yield return null;
        }
    }

    void ShootBullet()
    {
        if (isAttack)
        {
            ani.SetTrigger("Attack");

            GameObject _bullet = Instantiate(bullet, ShootPos.position, Quaternion.identity,GameManager.instance.transform);
            Rigidbody rbBullet = _bullet.GetComponent<Rigidbody>();

            if (rbBullet != null)
            {
                Vector3 direction = (Target.transform.position - ShootPos.position).normalized;
                rbBullet.AddForce(direction * bulletSpeed, ForceMode.Impulse);
            }

            ParticleManager.instance.InstantiateParticle(ShootPos.position + Vector3.up * 0.5f, "EnemyBulletParticle");
            AudioManager.instance.PlaySfx("EnemyBullet");
        }
    }
    public void ResetEnemy()
    {

        isStun = false;
        isAttached = false;
        ani.SetBool("isStun", false);
        agent.enabled = true;
        transform.tag = "Enemy";
        transform.gameObject.layer = 11;
      
    }
    public void Damage(float damage)
    {
        if (isDead) return;

        CurrentHp -= damage;
        ParticleManager.instance.InstantiateParticle(transform.position + Vector3.up, "EnemyHitParticle");
        AudioManager.instance.PlaySfx("EnemyHit");

        if (CurrentHp <= 0)
        {
            Dead();
        }
    }

    void Dead()
    {
        isDead = true;
        ani.SetBool("isAttack", false);
        ani.SetBool("isStun", false);
        ani.SetTrigger("Dead");
        rb.linearVelocity = Vector3.zero;
       
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        transform.gameObject.layer = 13;
        EC.enabled = false;

        if (agent != null)
        {
            agent.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && PlayerSkill.Instance.isSlime)
        {
            Damage(100);
        }
        if (collision.transform.CompareTag("PlayerBullet") && !isDead && !isAttached)
        {
            Damage(100);
        }

        if (isStun &&
           (collision.transform.CompareTag("Enemy") ||
            collision.transform.CompareTag("Ground") ||
            collision.transform.CompareTag("Wall")) &&
           !isDead && !isAttached)
        {
            Damage(100);
        }
    }
  
}