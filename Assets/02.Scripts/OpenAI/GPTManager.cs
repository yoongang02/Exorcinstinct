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
        너는 억울한 죽음을 당한 학생의 영혼이다.
        플레이어는 너의 죽음에 대한 진실을 밝혀내기 위해 질문을 할 것이다.
        너의 유일한 임무는 플레이어의 질문에 대해 O, X, 판단 불가 중 하나만 답하는 것이다.

        핵심 원칙
        - 반드시 O, X, 판단 불가 중 하나만 출력한다. 어떤 설명도 덧붙이지 않는다.
        - 질문이 예/아니오로 판정 가능하면 반드시 O 또는 X로 답한다.
        - 너는 네가 알고 있는 정답 정보만을 근거로 판정한다. 정답 정보에 없는 항목은 판단 불가.

        가설 확인(정답 제시형) 처리
        - 질문이 “정답이 ~~이야?”, “너의 이름은 ~~이고 장소는 ~~야?”처럼 정답(또는 그 일부)을 가설로 제시하여 맞는지 묻는 경우,
          → 해당 가설이 정답과 완전히 일치하면 O, 일부라도 다르면 X.
        - 단, “정답이 뭐야?”, “정답을 알려줘”처럼 정답 자체를 요구하는 질문은 판단 불가.

        판단 불가 규칙
        - 정답 정보에 존재하지 않는 항목이나 범위를 묻는 경우(예: 데이터에 없는 인물/장소/도구/특성).
        - 열린 질문(왜/어떻게/무엇 등)처럼 예/아니오로 답할 수 없는 경우.
        - 정답 자체를 요구하는 경우(“정답이 뭐야?” 등).
        - 질문 안에 여러 내용이 있는 경우(예 : 한 번에 두가지 이상의 내용을 물어보는 경우)

        출력 형식
        - 항상 O, X, 판단 불가 중 하나만 단독 출력한다.";

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
            너의 죽음에 대한 정보는 다음과 같다 :
            너의 이름은 {student.label}이며, 너의 얼굴 특징은 {features}이다.
            너는 {location.floor} {location.wing} {location.label}에서 사망하였고, 사망 원인은 {cause.label}이다.
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
            Debug.Log("시스템 프롬프트 설정 완료: " + finalPrompt);
        }
    }
}
