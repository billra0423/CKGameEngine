using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("State")]
    public bool isMain;
    public bool isPlay;
    public bool isSettting;
    public bool isSuccess;
    public bool isGameOver;
    [Header("UI")]
    public GameObject MainUi;
    public GameObject PlayerUi;
    public GameObject SettingUi;
    public GameObject GameOverUi;
    public GameObject SuccessUi;
    public GameObject SlimeUi;
    public GameObject HumanUi;
    public Slider HpBar;
    [Header("Button")]
    public GameObject StartButton;
    public GameObject SettingButton;
    public GameObject RestartButton;
    [Header("Player")]
    public GameObject player;
    public Transform StartPoint;
    public void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public void Start()
    {

    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Setting();
        }
    }
    public void ResetState()
    {
        MainUi.SetActive(false);
        PlayerUi.SetActive(false);
        SettingUi.SetActive(false);
        GameOverUi.SetActive(false);
        SuccessUi.SetActive(false);

        isMain = false;
        isPlay = false;
        isSettting = false;
        isSuccess = false;
        isGameOver = false;
    }
    public void ResetPlayer()
    {
        Player.CurrentHp = Player.Instance.MaxHp;
        Player.Instance.gameObject.transform.position = StartPoint.position;
        PlayerSkill.Instance.TransPlayer();
    }
    public void Restart()
    {
        AudioManager.instance.PlaySfx("ButtonSound");
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

    }
    public void Main()
    {
        ResetState();
        ResetPlayer();
        isMain = true;
        MainUi.SetActive(isMain);
        PlayerUi.SetActive(isPlay);
        SettingUi.SetActive(isSettting);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        AudioManager.instance.PlaySfx("ButtonSound");
    }
    public void Setting()
    {
        isSettting = !isSettting;

        if (isSettting)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (isPlay)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        AudioManager.instance.PlaySfx("ButtonSound");
        SettingUi.SetActive(isSettting);
    }
    public void GameStart()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ResetState();
        isPlay = true;
        MainUi.SetActive(isMain);
        PlayerUi.SetActive(isPlay);
        AudioManager.instance.PlaySfx("ButtonSound");
    }

    public void Success()
    {
        ResetState();
        isSuccess = true;
        SuccessUi.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        AudioManager.instance.PlaySfx("Success");
    }
    public void GameOver()
    {
        ResetState();
        isGameOver = true;
        GameOverUi.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        AudioManager.instance.PlaySfx("GameOver");
    }
    public void SkillTransUi()
    {
        if (PlayerSkill.Instance.isPlayer)
        {
            HumanUi.SetActive(true);
            SlimeUi.SetActive(false);
        }
        else
        {
            HumanUi.SetActive(false);
            SlimeUi.SetActive(true);
        }

    }
}
