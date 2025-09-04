using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource bgmSource;
    public List<SoundData> soundDatabase; // ScriptableObject로부터 사운드 데이터베이스 가져옴
    private Dictionary<string, SoundData> soundDictionary; // 사운드를 빠르게 찾기 위한 딕셔너리
    private List<AudioSource> sfxSources = new List<AudioSource>(); // 효과음 AudioSource 리스트

    private void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 사운드 데이터 초기화
        soundDictionary = new Dictionary<string, SoundData>();
        foreach (var soundData in soundDatabase)
        {
            soundDictionary[soundData.soundName] = soundData;
        }
    }

    /// <summary> 배경음악 (BGM) 재생 </summary>
    public void PlayBGM(string soundName)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            Debug.LogWarning($"BGM '{soundName}' not found in SoundDatabase.");
            return;
        }

        AudioSource bgmSource = gameObject.AddComponent<AudioSource>();

        SoundData data = soundDictionary[soundName];
        AudioClip clip = data.clip;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.volume = data.volume;
        bgmSource.Play();
        DOTween.To(() => bgmSource.volume, x => bgmSource.volume = x, data.volume, 1f);
    }

    /// <summary> 배경음악 (BGM) 정지 </summary>
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            DOTween.To(() => bgmSource.volume, x => bgmSource.volume = x, 0f, 1.5f)
                .OnComplete(() => bgmSource.Stop());
        }

    }

    /// <summary> 효과음 (SFX) 재생 </summary>
    public void PlaySFX(string soundName)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            Debug.LogWarning($"SFX '{soundName}' not found in SoundDatabase.");
            return;
        }

        SoundData data = soundDictionary[soundName];
        AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.clip = data.clip;
        sfxSource.volume = data.volume;
        sfxSource.Play();

        sfxSources.Add(sfxSource);
        StartCoroutine(DestroyAfterPlay(sfxSource, data.clip.length));
    }

    /// <summary> 특정 시간 후에 오디오 소스를 삭제합니다. </summary>
    private IEnumerator DestroyAfterPlay(AudioSource source, float time)
    {
        yield return new WaitForSeconds(time);
        sfxSources.Remove(source);
        Destroy(source);
    }

    /// <summary> 모든 효과음 (SFX) 정지 </summary>
    public void StopAllSFX()
    {
        foreach (var source in sfxSources)
        {
            source.Stop();
            Destroy(source);
        }
        sfxSources.Clear();
    }
}