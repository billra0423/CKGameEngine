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
    public GameObject TrnasformParticle;
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
        if (isPlayer)
        {
            SlimeOb.transform.position = PlayerOb.transform.position + new Vector3(0,1,0);
        }
        else
        {
            PlayerOb.transform.position = SlimeOb.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isPlayer)
            {

                TrnasformParticle.SetActive(true);
                TrnasformParticle.transform.position = PlayerOb.transform.position + new Vector3(0,1,0);
                TrnasSlime();
                sf.isClimbing = false;
                isPlayer = false;
            }
            else
            {
                TrnasformParticle.SetActive(true);
                TrnasformParticle.transform.position = SlimeOb.transform.position + new Vector3(0, 1, 0);
                isSlime = false;

                TransPlayer();


            }
        }
    }
    public void TransPlayer()
    {
        isPlayer = true;
        paleyrCamera.Target = PlayerTargetPos.transform;
        EnableSilme();
        animator.SetTrigger("TransF");
    }
    public void TrnasSlime()
    {
        paleyrCamera.Target = SlimeOb.transform;
        EnablePlayer();
        isSlime = true;
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
