using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType
{
    Spikes,
    MovingPlatform,
    FallingPlatform,
    ProjectileLauncher,
    RotatingSpike,
    Laser
}

public class Obstacle : MonoBehaviour
{
    public ObstacleType obstacleType;
    public GameObject Spikes;
    public Vector3 movementDirection;
    public float speed;
    private bool isFalling = false;
    private Vector3 startPosition;
    public GameObject projectilePrefab;
    public GameObject LaserOb;
    public float launchInterval = 2f;
    public float rotationSpeed = 50f;
    public float launchSpeed;
    public float dist;
    public GameObject Target;
    public Vector3 LastPoint;
    private void Start()
    {
        startPosition = transform.position; // 초기 위치 저장
        Target = Player.Instance.transform.gameObject;
        TriggerEffect();
    }
    public void Update()
    {
        TriggerEffect1();


    }
    public void TriggerEffect1()
    {
        switch (obstacleType)
        {
            case ObstacleType.Spikes:
                //Spikes.SetActive(true);
                break;

            case ObstacleType.MovingPlatform:
                transform.position = startPosition + movementDirection * Mathf.Sin(Time.time * speed);
                break;

            case ObstacleType.FallingPlatform:
                //  StartCoroutine(Fall());
                break;

            case ObstacleType.ProjectileLauncher:
                // StartCoroutine(LaunchProjectiles());
                break;

            case ObstacleType.RotatingSpike:
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                break;

            case ObstacleType.Laser:
                LaserOb.transform.Rotate(new Vector3(0, 1, 0), 5);
                //   StartCoroutine(LaserRoutine());
                break;
        }
    }
    public void Falll()
    {
        // 발판이 떨어지게 하는 로직
        if (!isFalling)
        {
            isFalling = true;
            StartCoroutine(Fall());
        }
    }
    public void TriggerEffect()
    {
        switch (obstacleType)
        {
            case ObstacleType.Spikes:
                StartCoroutine(SpikesRoutine());
                break;

            case ObstacleType.MovingPlatform:
                transform.position = startPosition + movementDirection * Mathf.Sin(Time.time * speed);
                break;

            case ObstacleType.FallingPlatform:
                //StartCoroutine(Fall());
                LastPoint = transform.position;
                break;

            case ObstacleType.ProjectileLauncher:
                StartCoroutine(LaunchProjectiles());
                break;

            case ObstacleType.RotatingSpike:
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                break;

            case ObstacleType.Laser:
                StartCoroutine(LaserRoutine());
                break;
        }
    }
    private IEnumerator SpikesRoutine()
    {
        while (true)
        {
            float ran = Random.Range(0.5f, 2);
            yield return new WaitForSeconds(ran);
            // 레이저 비활성화
            Spikes.SetActive(false);
            yield return new WaitForSeconds(ran);
            // 레이저 활성화
            Spikes.SetActive(true);
        }
    }
    private IEnumerator LaserRoutine()
    {
        while (true)
        {
            float ran = Random.Range(0.5f, 2);
            yield return new WaitForSeconds(ran);
            // 레이저 비활성화
            LaserOb.SetActive(false);
            yield return new WaitForSeconds(ran);
            // 레이저 활성화
            LaserOb.SetActive(true);
        }
    }
    private IEnumerator LaunchProjectiles()
    {
        while (true)
        {

            yield return new WaitForSeconds(launchInterval - 1);
            float Dis = Vector3.Distance(Target.transform.position, transform.position);
            Vector3 dir = Target.transform.position - this.transform.position;
            yield return new WaitForSeconds(1);
            if (Dis < dist)
            {

               // AudioManager.instance.PlaySfx(AudioManager.Sfx.EnemyBullet);
                GameObject m = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
               // MainCamera.instance.StartShake();
                m.GetComponent<Rigidbody>().AddForce(dir * launchSpeed, ForceMode.Impulse);
            }
        }
    }
    public void ResetFallOb()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        transform.position = LastPoint;
    }
    private IEnumerator Fall()
    {
        // 잠시 후에 발판을 떨어뜨림
        yield return new WaitForSeconds(1.5f);
        GetComponent<Rigidbody>().isKinematic = false; // 물리적 충돌 허용
        yield return new WaitForSeconds(4f);
        ResetFallOb();
        // Destroy(gameObject); // 일정 시간 후 발판 삭제
    }
    public void OnCollisionEnter(Collision collision)
    {

        switch (obstacleType)
        {
            case ObstacleType.Spikes:

                break;

            case ObstacleType.MovingPlatform:
                if (collision.transform.tag == "Player")
                {
                    collision.transform.parent = this.transform;
                }
                break;

            case ObstacleType.FallingPlatform:
                if (collision.transform.tag == "Player")
                {
                    Falll();
                }
                break;

            case ObstacleType.ProjectileLauncher:
                if (collision.transform.tag == "PlayerBullet")
                {

                    //AudioManager.instance.PlaySfx(AudioManager.Sfx.EnemyHit);
                    ParticleManager.instance.InstantiateParticle(transform.position, "Boom");
                    gameObject.SetActive(false);
                    //MainCamera.instance.StartShake();
                }
                break;

            case ObstacleType.RotatingSpike:
                if (collision.transform.tag == "Player")
                {
                    collision.transform.parent = this.transform;
                }
                break;

            case ObstacleType.Laser:

                break;
        }

    }
    public void OnCollisionExit(Collision collision)
    {
        switch (obstacleType)
        {
            case ObstacleType.Spikes:

                break;

            case ObstacleType.MovingPlatform:
                if (collision.transform.tag == "Player")
                {
                    collision.transform.parent = null;
                }
                break;

            case ObstacleType.FallingPlatform:
                if (collision.transform.tag == "Player")
                {
                    // Falll();
                }
                break;

            case ObstacleType.ProjectileLauncher:

                break;

            case ObstacleType.RotatingSpike:
                if (collision.transform.tag == "Player")
                {
                    collision.transform.parent = null;
                }
                break;

            case ObstacleType.Laser:

                break;
        }
    }
}
