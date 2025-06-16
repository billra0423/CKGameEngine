using System;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class AudioInfo
{
    public string name;
    public AudioClip sfxClip;
    [Range(0f, 1f)] public float volume = 1f; // 개별 볼륨 추가
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
        //배경음 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        //효과음 초기화 

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
    // 마스터 볼륨 조절 (둘 다 동시에)
    public void MasterVolume(float v)
    {
        SetBgmVolume(v);
        SetSfxVolume(v);
    }

    // BGM 볼륨 설정
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
            float individualVol = audioInfos[i].volume; // 개별 볼륨
            float finalVol = Mathf.LinearToGammaSpace(individualVol * sfxVolume); // 곱 연산으로 비율 적용

            // 해당 이름의 오디오소스 찾고 볼륨 업데이트
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
                    sfxPlayer[loopIndex].volume = finalVol; // 실시간 볼륨 적용
                    sfxPlayer[loopIndex].Play();

                    return;
                }

                // 강제 재사용
                sfxPlayer[ChannelIndex].clip = audioInfos[i].sfxClip;
                sfxPlayer[ChannelIndex].volume = Mathf.LinearToGammaSpace(audioInfos[i].volume * sfxVolume);
                sfxPlayer[ChannelIndex].Play();

                return;
            }
        }

        Debug.LogWarning("SFX not found: " + name);
    }
}
