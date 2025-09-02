using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ElevenlabsAPI : MonoBehaviour
{
    public static ElevenlabsAPI Instance { get; private set; }
    private string _voiceId = "piTKgcLEGmPE4e6mEKli";
    private string _apiKey = "sk_1cddacce7145bc4f0d699d0cef3add9a9ee48bd3fa21dd71"; // ⚠️ 배포 금지(테스트용)
    private string _apiUrl = "https://api.elevenlabs.io";

    [Header("Options")]
    public bool Streaming = false; // 우선 false로 안정화 후 확장
    [Range(0, 4)] public int LatencyOptimization = 0;

    [System.Serializable]
    public class AudioClipEvent : UnityEngine.Events.UnityEvent<AudioClip> { }
    public AudioClipEvent AudioReceived;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

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
        public float style;              // 0..1 (문서상 범위는 모델별 상이)
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
                // 본문까지 출력해주면 디버깅 쉬움
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
        // 메인 카메라 위치에서 3D one-shot로 재생 (임시 AudioSource를 내부적으로 생성했다가 clip 길이 후 파괴)
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }
}
