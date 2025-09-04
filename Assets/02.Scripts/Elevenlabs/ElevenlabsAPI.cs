using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Audio; // (선택) 믹서 쓰면 필요

public class ElevenlabsAPI : MonoBehaviour
{
    public static ElevenlabsAPI Instance { get; private set; }
    private string _voiceId = "piTKgcLEGmPE4e6mEKli";
    private string _apiKey = "sk_1cddacce7145bc4f0d699d0cef3add9a9ee48bd3fa21dd71"; // ⚠️ 배포 금지(테스트용)
    private string _apiUrl = "https://api.elevenlabs.io";

    [Header("Options")]
    public bool Streaming = false;
    [Range(0, 4)] public int LatencyOptimization = 0;

    [Header("SFX")]
    [Tooltip("TTS 앞에 재생할 효과음")]
    public AudioClip sfxBefore;
    [Range(0f, 1f)] public float sfxBeforeVolume = 1f;

    [Tooltip("TTS 뒤에 재생할 효과음")]
    public AudioClip sfxAfter;
    [Range(0f, 1f)] public float sfxAfterVolume = 1f;

    [Tooltip("TTS 본편 볼륨")]
    [Range(0f, 1f)] public float ttsVolume = 1f;

    [Tooltip("3D로 들리게 할지(1=완전 3D)")]
    [Range(0f, 1f)] public float spatialBlend = 0f;

    [Tooltip("(선택) 출력 믹서 그룹")]
    public AudioMixerGroup outputMixer;

    private AudioSource _source; // 재생 전용 소스

    [System.Serializable]
    public class AudioClipEvent : UnityEngine.Events.UnityEvent<AudioClip> { }
    public AudioClipEvent AudioReceived;

    private void Awake()
    {
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

        // 재생용 오디오소스 준비
        _source = gameObject.AddComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.loop = false;
        _source.spatialBlend = spatialBlend;
        _source.outputAudioMixerGroup = outputMixer;

        // TTS 수신 시 재생
        AudioReceived.AddListener(PlayClip);
    }

    [Serializable]
    public class TextToSpeechRequest
    {
        public string text;
        public string model_id;
        public VoiceSettings voice_settings;
    }

    [Serializable]
    public class VoiceSettings
    {
        public float stability;          // 0..1
        public float similarity_boost;   // 0..1
        public float style;              // 0..1
        public bool use_speaker_boost;
    }

    public void GetAudio(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            Debug.LogWarning("TTS text is empty.");
            return;
        }
        StartCoroutine(DoRequest(text));
    }

    private IEnumerator DoRequest(string message)
    {
        var postData = new TextToSpeechRequest
        {
            text = message,
            model_id = "eleven_multilingual_v2",
            voice_settings = new VoiceSettings
            {
                stability = 0.3f,
                similarity_boost = 0.8f,
                style = 0.2f,
                use_speaker_boost = true
            }
        };

        var json = JsonConvert.SerializeObject(postData);
        var bytes = Encoding.UTF8.GetBytes(json);

        var streamPath = Streaming ? "/stream" : "";
        var query = Streaming ? $"?optimize_streaming_latency={LatencyOptimization}" : "";
        var url = $"{_apiUrl}/v1/text-to-speech/{_voiceId}{streamPath}{query}";

        using (var request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bytes);
            var dl = new DownloadHandlerAudioClip(url, AudioType.MPEG);
            if (Streaming) dl.streamAudio = true;
            request.downloadHandler = dl;

            request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
            request.SetRequestHeader("xi-api-key", _apiKey);
            request.SetRequestHeader("Accept", "audio/mpeg");

            yield return request.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                string body = null;
                try { body = request.downloadHandler?.text; } catch { }
                Debug.LogError($"TTS Error: {request.error}\n{body}");
                yield break;
            }

            var clip = DownloadHandlerAudioClip.GetContent(request);
            if (clip == null)
            {
                Debug.LogError("AudioClip parse failed.");
                yield break;
            }

            AudioReceived?.Invoke(clip);
        }
    }

    public void PlayClip(AudioClip clip)
    {
        // 직렬 재생 코루틴으로 연결
        StartCoroutine(PlayWithSfx(clip));
    }

    private IEnumerator PlayWithSfx(AudioClip ttsClip)
    {
        if (_source == null)
        {
            Debug.LogWarning("AudioSource not initialized.");
            yield break;
        }

        // (선택) 공간감 업데이트: 카메라 위치로 이동
        if (Camera.main != null)
        {
            transform.position = Camera.main.transform.position;
        }
        _source.spatialBlend = spatialBlend;

        // 1) SFX Before
        if (sfxBefore != null)
        {
            _source.clip = sfxBefore;
            _source.volume = sfxBeforeVolume;
            _source.Play();
            yield return new WaitForSeconds(sfxBefore.length);
        }

        // 2) TTS 본편
        if (ttsClip != null)
        {
            _source.clip = ttsClip;
            _source.volume = ttsVolume;
            _source.Play();
            // 길이 신뢰가 낮으면 isPlaying 폴백도 고려
            yield return new WaitForSeconds(ttsClip.length);
        }

        // 3) SFX After
        if (sfxAfter != null)
        {
            _source.clip = sfxAfter;
            _source.volume = sfxAfterVolume;
            _source.Play();
            yield return new WaitForSeconds(sfxAfter.length);
        }
    }
}
