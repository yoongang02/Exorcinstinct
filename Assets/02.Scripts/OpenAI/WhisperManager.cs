using OpenAI;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Samples.Whisper
{
    public class WhisperManager : MonoBehaviour
    {
        public static WhisperManager Instance { get; private set; }

        [Header("UI")]
        [SerializeField] private Button _recordButton;
        [SerializeField] private Image _progressImage;
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
            _isRecording = true;

            // #1 마이크 선택 없이, 첫번째 마이크 사용
            string device = Microphone.devices.Length > 0 ? Microphone.devices[0] : null;

            if (string.IsNullOrEmpty(device))
            {
                Debug.LogError("마이크를 찾을 수 없음.");
                _isRecording = false;
                return;
            }

            _recordButton.interactable = false;
            _audioClip = Microphone.Start(device, false, _durationSeconds, _sampleRate);

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

        public void StartRecordingTranslator()
        {
            if (_isRecording) return;
            _isRecording = true;

            // #1 마이크 선택 없이, 첫번째 마이크 사용
            string device = Microphone.devices.Length > 0 ? Microphone.devices[0] : null;

            if (string.IsNullOrEmpty(device))
            {
                Debug.LogError("마이크를 찾을 수 없음.");
                _isRecording = false;
                return;
            }

            _recordButton.interactable = false;
            _audioClip = Microphone.Start(device, false, _durationSeconds, _sampleRate);

            EndAfterTranslator(_durationSeconds).Forget();
        }

        private async UniTaskVoid EndAfter(int seconds)
        {
            await UniTask.Delay(seconds * 1000);
            await EndRecording();
        }

        private async UniTaskVoid EndAfterTranslator(int seconds)
        {
            await UniTask.Delay(seconds * 1000);
            await EndRecordingTranslator();
        }

        private async UniTask EndRecording()
        {
            Microphone.End(null);
            _isRecording = false;

            if (_audioClip == null)
            {
                Debug.LogError("녹음된 오디오 파일이 없습니다.");
                _recordButton.interactable = true;
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

            _outputText = res.Text;

            Debug.Log($"녹음 완료. 텍스트: {_outputText}");

            // 변환한 질문 텍스트를 GptManager에 전달해서 응답 받기
            if (!string.IsNullOrWhiteSpace(res.Text))
            {
                string jundgement = await GptManager.Instance.RespondToPlayer(res.Text);
                Debug.Log($"GptManager 응답: {jundgement}");

                if (jundgement == "판단 불가")
                {
                    
                }
                else
                {
                    // 0, X 판단 가능
                }
                    _recordButton.interactable = true;
            }
        }

        private async UniTask EndRecordingTranslator()
        {
            Microphone.End(null);
            _isRecording = false;

            if (_audioClip == null)
            {
                Debug.LogError("녹음된 오디오 파일이 없습니다.");
                _recordButton.interactable = true;
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

            _outputText = res.Text;

            Debug.Log($"녹음 완료. 텍스트: {_outputText}");

            // 변환한 질문 텍스트를 HintManager에 전달해서 응답 받기
            if (!string.IsNullOrWhiteSpace(res.Text))
            {
                string jundgement = await GptManager.Instance.RespondToPlayer(res.Text);
                Debug.Log($"GPTManager 응답: {jundgement}");
                if (jundgement != "판단 불가")
                {
                    string hintMessage = await HintManager.Instance.ResondToPlayer(res.Text);
                    Debug.Log(hintMessage);

                    ElevenlabsAPI.Instance.GetAudio(hintMessage);
                }
                else
                {
                    
                }
                _recordButton.interactable = true;
            }
        }
    }
}
