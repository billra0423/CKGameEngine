using System;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class AudioInfo
{
    public string name;
    public AudioClip sfxClip;
    [Range(0f, 1f)] public float volume = 1f; // ���� ���� �߰�
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    public AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioInfo[] audioInfos;
    public float sfxVolume;
    public int channels;
    public int Objects;
    public AudioSource[] sfxPlayer;
    int ChannelIndex;
 
    public void Awake()
    {
        if (instance == null)
        { instance = this; }
        Init();
    }
    public void Start()
    {
        MasterVolume(0.5f);
       // AudioManager.instance.bgmPlayer.Play();
    }
    public void Init()
    {
        //����� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        //ȿ���� �ʱ�ȭ 

        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayer = new AudioSource[channels];

        for (int i = 0; i < sfxPlayer.Length; i++)
        {


            sfxPlayer[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayer[i].playOnAwake = false;
            sfxPlayer[i].volume = sfxVolume;

        }
        for (int i = 0; i < Objects; i++)
        {
            GameObject sfxObjects = new GameObject("ObjectSfxPlayer");
            sfxObjects.transform.parent = transform;
            sfxObjects.AddComponent<AudioSource>();
        }

    }
    // ������ ���� ���� (�� �� ���ÿ�)
    public void MasterVolume(float v)
    {
        SetBgmVolume(v);
        SetSfxVolume(v);
    }

    // BGM ���� ����
    public void SetBgmVolume(float v)
    {
        bgmVolume = v;
        float logVolume = Mathf.LinearToGammaSpace(v);
        if (bgmPlayer != null)
            bgmPlayer.volume = logVolume;
    }

    public void SetSfxVolume(float v)
    {
        sfxVolume = v;

        for (int i = 0; i < audioInfos.Length; i++)
        {
            float individualVol = audioInfos[i].volume; // ���� ����
            float finalVol = Mathf.LinearToGammaSpace(individualVol * sfxVolume); // �� �������� ���� ����

            // �ش� �̸��� ������ҽ� ã�� ���� ������Ʈ
            for (int j = 0; j < sfxPlayer.Length; j++)
            {
                if (sfxPlayer[j].clip == audioInfos[i].sfxClip)
                {
                    sfxPlayer[j].volume = finalVol;
                }
            }
        }
    }
    public void PlaySfx(string name)
    {
        for (int i = 0; i < audioInfos.Length; i++)
        {
            if (audioInfos[i].name == name)
            {
                float individualVol = audioInfos[i].volume;
                float finalVol = Mathf.LinearToGammaSpace(individualVol * sfxVolume);

                for (int j = 0; j < sfxPlayer.Length; j++)
                {
                    int loopIndex = (j + ChannelIndex) % sfxPlayer.Length;

                    if (sfxPlayer[loopIndex].isPlaying)
                        continue;

                    ChannelIndex = loopIndex;

                    sfxPlayer[loopIndex].clip = audioInfos[i].sfxClip;
                    sfxPlayer[loopIndex].volume = finalVol; // �ǽð� ���� ����
                    sfxPlayer[loopIndex].Play();

                    return;
                }

                // ���� ����
                sfxPlayer[ChannelIndex].clip = audioInfos[i].sfxClip;
                sfxPlayer[ChannelIndex].volume = Mathf.LinearToGammaSpace(audioInfos[i].volume * sfxVolume);
                sfxPlayer[ChannelIndex].Play();

                return;
            }
        }

        Debug.LogWarning("SFX not found: " + name);
    }
}
