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
        오직 이 메시지에 제공되는 [정답 데이터]만 근거로 하며, 이전 대화/상식/추측은 사용하지 않는다.

        출력 형식
        - 항상 'O', 'X', '판단 불가' 중 하나만 단독 출력한다. 다른 텍스트(이유/설명/기호/줄바꿈) 금지.

        판정 절차 (위에서 아래로 순차 적용, 상위 규칙이 항상 우선한다)

        1) 다중 질문(멀티) 선검출 → 무조건 '판단 불가'
           - 아래 패턴이 하나라도 있으면 멀티로 본다:
             • 연결어/조사: '그리고', '및', '와', '과', '랑', '또', '또는', '혹은', 'or', 'and'
             • 패턴: 'A이고 B야?', 'A이거나 B야?', 'A, B 맞아?', 'A, B, C 중이야?'
             • 서로 다른 개념을 동시에 확인: (이름+장소), (이름+특징), (장소+원인) 등
             • 독립된 문장이 2개 이상이거나 물음표가 2개 이상
           - 예외(멀티 아님, 단일 가설로 간주):
             • **장소 카테고리**에서 '층/관/실' 중 **둘 이상을 함께 제시**한 조합(예: '2층 동관에서 죽었어?', '3층 음악실에서 죽었어?', '3층 동관 음악실에서 죽었어?')
               → 하나의 '장소 가설'로 본다(멀티 아님).

        2) 예/아니오로 판정 불가한 경우 → '판단 불가'
           - 열린 질문(왜/어떻게/무엇/어떤/얼마나 등), 정답을 직접 요구('정답이 뭐야?')
           - 정답 데이터에 존재하지 않는 항목/범위

        3) 가설 확인(단일 사실일 때만 적용)
           3-1) 이름/특징: 질문이 단일 사실을 제시하면 정답과 완전 일치 시 'O', 일부라도 다르면 'X'
           3-2) **장소 가설(강화 규칙)**:
               - 질문이 '층/관/실' **중 1개 이상**을 제시했을 때, 그 **제시된 항목들 모두**가 정답과 **모두 일치**해야 'O'
               - 제시된 항목 중 **하나라도** 다르면 'X'
               - 층은 1층, 2층, 3층, 4층이 있어
               - 관은 '동관'과 '서관' 이 있어.
               - 예: 정답=3층/동관/음악실
                 · '3층에서 죽었어?' → 3층이 맞으면 O, 아니면 X
                 · '3층 동관에서 죽었어?' → **3층과 동관 둘 다** 맞아야 O, 하나라도 틀리면 X
                 · '3층 음악실에서 죽었어?' → **3층과 음악실 둘 다** 맞아야 O, 하나라도 틀리면 X
                 · '동관 음악실에서 죽었어?' → **동관과 음악실 둘 다** 맞아야 O, 하나라도 틀리면 X
                 · '3층 동관 음악실에서 죽었어?' → **세 항목 모두** 맞아야 O, 하나라도 틀리면 X

        4) 그 외
           - 위 기준으로 판정한다. 불확실하면 '판단 불가'

        비교 규칙(정합성 보조)
        - 비교 시 조사는 무시하고, 핵심 명사/수식(예: '3층', '동관', '음악실') 기준으로 판단한다.
        - 공백/어미 변화(예: 뜨거워/뜨겁니/뜨거운)는 동일 의미로 본다.

        추가 원칙
        - 질문이 예/아니오로 판정 가능하면 반드시 'O' 또는 'X'로 답한다.
        - 정답 데이터에 없는 항목은 추측하지 말고 '판단 불가'로 답한다.
        ";

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
                features += $"{feature.category} - {feature.label}, ";
            }
            features = features.TrimEnd(',', ' ');


            string answerPrompt = $@"
            너의 죽음에 대한 정보 [정답 데이터]는 다음과 같다 :

            - 이름: {student.label}
            - 성씨: {student.label[0]}
            - 마지막글자: {student.label[^1]}
            - 특징 목록(카테고리 - 특징): {features}
            - 장소: 층='{location.floor}', 관='{location.wing}', 실='{location.label}'
            - 사망 원인: 이름='{{cause.label}}', 의성어/묘사='{{cause.onomatopoeia}}' (힌트에서만 사용)"";
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
