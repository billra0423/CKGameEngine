using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public static PlayerSkill Instance;
    public GameObject PlayerOb;
    public GameObject SlimeOb;
    public SoftBody slime;
    public Transform PlayerTargetPos;
    public bool isSlime;
    public bool isPlayer;
    public Animator animator;
    public PlayerCamera paleyrCamera;
    public SurfaceMove sf;
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }
    public void Start()
    {
        Invoke("slimeEn", 0.1f);
    }
    public void slimeEn()
    {
        isSlime = false;
        slime.ResetPhysicsVerticesToStartPositions(false);
    }
    void Update()
    {
        if (GameManager.instance.isPlay)
        {
            if (isPlayer)
            {
                SlimeOb.transform.position = PlayerOb.transform.position + new Vector3(0, 1, 0);
            }
            else
            {
                PlayerOb.transform.position = SlimeOb.transform.position;
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (isPlayer)
                {

                    TrnasSlime();

                }
                else
                {


                    TransPlayer();


                }
            }
        }
    }
    public void TransPlayer()
    {
        GameManager.instance.SkillTransUi();
        ParticleManager.instance.InstantiateParticle(SlimeOb.transform.position + new Vector3(0, 1, 0), "TransformParticle");
        isSlime = false;

        isPlayer = true;
        paleyrCamera.Target = PlayerTargetPos.transform;
        SlimeOb.GetComponent<Collider>().enabled = false;
        EnableSilme();
        animator.SetTrigger("TransF");
        AudioManager.instance.PlaySfx("Transformation");
    }
    public void TrnasSlime()
    {
        PlayerArm.Instance.OffHand();
        GameManager.instance.SkillTransUi();
        ParticleManager.instance.InstantiateParticle(PlayerOb.transform.position + new Vector3(0, 1, 0), "TransformParticle");
        sf.isClimbing = false;
        isPlayer = false;

        paleyrCamera.Target = SlimeOb.transform;
        SlimeOb.GetComponent<Collider>().enabled = true;
        EnablePlayer();
        isSlime = true;
        AudioManager.instance.PlaySfx("Transformation");
    }
    public void EnablePlayer()
    {
        PlayerOb.SetActive(false);
        slime.ResetPhysicsVerticesToStartPositions(true);
        EnableSilmeMesh(true);
    }
    public void EnableSilme()
    {
        EnableSilmeMesh(false);
        slime.ResetPhysicsVerticesToStartPositions(false);
        PlayerOb.SetActive(true);
    }
    public void EnableSilmeMesh(bool isset)
    {
        SlimeOb.GetComponent<MeshRenderer>().enabled = isset;
    }
}
