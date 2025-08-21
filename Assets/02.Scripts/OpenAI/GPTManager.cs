using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace OpenAI
{
    public class GptManager : MonoBehaviour
    {
        public static GptManager Instance { get; private set; }

        [SerializeField] float temperature;
        private OpenAIApi _openai = new OpenAIApi();
        private List<ChatMessage> _contexts = new List<ChatMessage>(); // 대화 맥락

        private string defatulPrompt = @"
        너는 추리 게임 속의 심판이다.
        너의 유일한 임무는 플레이어의 질문에 대해 O, X, 판단 불가 중 하나만 답하는 것이다.  

        규칙:  
        1. 반드시 O, X, 판단 불가 중 하나만 출력한다. 다른 텍스트는 출력하지 않는다.  
        2. 플레이어의 질문은 O 또는 X로 대답할 수 있는 질문이어야 한다.  
           - 만약 질문이 그 외의 형식이라면, 판단 불가로 답한다.  
        3. 질문에 여러 가지 내용이 포함되어 있어도 된다.  
           - 모든 내용이 O 또는 X로 판별 가능할 때만 유효하다.  
           - 내용 중 하나라도 판별 불가능하면 전체 답변은 판단 불가다.  
           - 질문이 AND 형식일 경우: 모든 질문이 O일 때만 O, 하나라도 X면 X.  
           - 질문이 OR 형식일 경우: 모든 질문이 X일 때만 X, 하나라도 O면 O.  
        4. 정답을 직접적으로 묻는 질문은 판단 불가로 답한다.  
        5. 네가 알고 있는 정보에 존재하지 않는 질문 또한 판단 불가로 답한다.  

        너는 반드시 O, X, 판단 불가 중 하나만 답한다.";

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

        public void ResetConversation() => _contexts.Clear();

        // 시스템 프롬프트 설정
        public void SetSystemPrompt(string systemPrompt)
        {
            _contexts.Clear();
            _contexts.Add(new ChatMessage { Role = "system", Content = systemPrompt });
        }

        // 플레이어의 질문에 응답하는 함수
        public async UniTask<string> RespondToPlayer(string askMessage)
        {
            var PlayerQuestion = new ChatMessage
            {
                Role = "user",
                Content = askMessage
            };

            _contexts.Add(PlayerQuestion);

            var req = new CreateChatCompletionRequest
            {
                Model = "gpt-4o-mini",
                Messages = _contexts,
                Temperature = temperature,
            };

            var res = await _openai.CreateChatCompletion(req);

            return (res.Choices != null && res.Choices.Count > 0)
                ? res.Choices[0].Message.Content?.Trim() ?? "응답 오류" : "응답 오류";
        }

        public string GetAnswerPrompt(Answer answer)
        {
            StudentSO student = answer.studentSO;
            LocationSO location = answer.locationSO;
            CauseSO cause = answer.causeSO;

            if (student == null || location == null)
            {
                Debug.LogError("정답 정보가 올바르지 않습니다. 학생 또는 위치 정보가 누락되었습니다.");
                return "";
            }

            string features = "";
            foreach (var feature in student.features)
            {
                features += $"{feature.label}, ";
            }
            features = features.TrimEnd(',', ' ');


            string answerPrompt = $@"
            이번 라운드의 정답은 다음과 같다:
            - 이름 : {student.label}
            - 특징 : {features}
            - 사망 장소 : {location.label}
            - 사망 원인 : {cause.label}
            ";

            return answerPrompt;
        }

        // 라운드 시작 시, GPT 세팅
        public void RoundStartSetting(Answer answer)
        {
            // 정답 프롬프프 생성
            string answerPrompt = GetAnswerPrompt(answer);

            // 최종 시스템 프롬프트 생성
            string finalPrompt = $"{defatulPrompt}\n\n{answerPrompt}";

            // 시스템 프롬프트 설정 -> 여기서 context 초기화도 이루어짐
            SetSystemPrompt(finalPrompt);
        }
    }
}
