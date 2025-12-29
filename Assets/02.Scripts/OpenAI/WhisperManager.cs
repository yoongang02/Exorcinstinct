using Cysharp.Threading.Tasks;
using OpenAI;
using System.Security.Cryptography;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Samples.Whisper
{
    public class WhisperManager : MonoBehaviour
    {
        public static WhisperManager Instance { get; private set; }
        public bool canRecord = true;

        [Header("UI")]
        [SerializeField] private Image _progressImage;
        [SerializeField] private TextMeshProUGUI _recordingText;
        [SerializeField] private GameObject _translatorIcon;
        private CancellationTokenSource _progressCts;

        [Header("Record Settings")]
        [SerializeField] private int _durationSeconds = 5;
        [SerializeField] private int _sampleRate = 44100;

        [Header("OpenAI")]
        [SerializeField] private string _model = "gpt-4o-mini-transcribe";
        [SerializeField] private string _language = "ko";

        private OpenAIApi _openai = new OpenAIApi();
        private AudioClip _audioClip;
        private bool _isRecording = false;
        private string _outputText;
        private bool _useTranslator = false; // 번역기 아이템 사용하는지

        // 질문 응답에 대한 액션
        public UnityAction WhenAnswerO;
        public UnityAction WhenAnswerX;
        public UnityAction WhenAnswerError;
        public UnityAction WhenStartAsk;

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
            }
        }

        public void StartRecording()
        {
            if (_isRecording) return;
            if (GameManager.Instance.GetCurQuestionCnt() <= 0) return;
            _isRecording = true;
            canRecord = false;
            _recordingText.text = "";

            // #1 마이크 선택 없이, 첫번째 마이크 사용
            string device = Microphone.devices.Length > 0 ? Microphone.devices[0] : null;

            if (string.IsNullOrEmpty(device))
            {
                Debug.LogError("마이크를 찾을 수 없음.");
                _isRecording = false;
                return;
            }

            _audioClip = Microphone.Start(device, false, _durationSeconds, _sampleRate);

            // 플레이어 손 위치 초기화
            WhenStartAsk?.Invoke();

            // 녹음 진행 바 시작
            _progressCts?.Cancel();
            _progressCts = new CancellationTokenSource();
            _progressImage.fillAmount = 0f;
            FillProgress(_durationSeconds, _progressCts.Token).Forget();
            EndAfter(_durationSeconds).Forget();
        }

        private async UniTaskVoid FillProgress(int seconds, CancellationToken ct)
        {
            float t = 0f;
            while (t < seconds) {
                if(ct.IsCancellationRequested) return;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
                t += Time.unscaledDeltaTime;
                _progressImage.fillAmount = Mathf.Clamp01(t / seconds);
            }

            _progressImage.fillAmount = 1f;
        }

        private async UniTaskVoid EndAfter(int seconds)
        {
            _recordingText.text = "\"...\"";
            await UniTask.Delay(seconds * 1000);
            await EndRecording();
        }

        private async UniTask EndRecording()
        {
            Microphone.End(null);
            _isRecording = false;

            if (_audioClip == null)
            {
                Debug.LogError("녹음된 오디오 파일이 없습니다.");
                return;
            }

            byte[] wav = SaveWav.Save("output.wav", _audioClip);
            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() { Data = wav, Name = "audio.wav" },
                Model = _model,
                Language = _language,
            };

            var res = await _openai.CreateAudioTranscription(req);
            _recordingText.text = $"\"{res.Text}\"";

            _outputText = res.Text;
            
            Debug.Log($"녹음 완료. 텍스트: {_outputText}");

            // 변환한 질문 텍스트를 GptManager에 전달해서 응답 받기
            if (!string.IsNullOrWhiteSpace(res.Text))
            {
                string jundgement = await GptManager.Instance.RespondToPlayer(res.Text);
                Debug.Log($"GptManager 응답: {jundgement}");

                if (jundgement == "판단 불가")
                {
                    WhenAnswerError?.Invoke();
                }
                else
                {
                    if (jundgement == "O") WhenAnswerO?.Invoke();
                    else if(jundgement == "X") WhenAnswerX?.Invoke();
                    

                    if (_useTranslator)
                    {
                        RoundManager.Instance.isTransUsed = true;
                        string hintMessage = await HintManager.Instance.ResondToPlayer(res.Text);
                        Debug.Log(hintMessage);

                        ElevenlabsAPI.Instance.GetAudio(hintMessage);
                        SetTranslator(false);
                    }
                }
            }
        }

        /// <summary>
        /// 번역기를 사용 상태를 제어하는 함수
        /// 번역기 아이템에서 호출함.
        /// </summary>
        /// <param name="value"></param>
        public void SetTranslator(bool value)
        {
            _useTranslator = value;

            _translatorIcon.SetActive(_useTranslator);
        }

        public void EndResponse()
        {
            canRecord = true;
            _progressImage.fillAmount = 0f;
        }
    }
}
