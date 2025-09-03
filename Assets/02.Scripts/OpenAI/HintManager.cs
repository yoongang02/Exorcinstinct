using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using UnityEngine;

namespace OpenAI
{
    public class HintManager : MonoBehaviour
    {
        public static HintManager Instance { get; private set; }
        [SerializeField] float temperature = 0f;
        private OpenAIApi _openai = new OpenAIApi();

        private string defaultPrompt = @"
너는 억울하게 죽은 학생의 영혼이다.
오직 이 메시지에 포함된 [정답 데이터]와 [사전 데이터], [플레이어 질문]만 사용해 판단하라.
이전 대화, 일반 상식, 추측(할루시네이션), 너 자신의 기억은 모두 무시한다.

[출력 형식 — 절대 위반 금지]
- 출력은 오직 한 줄의 한국어 문자열이어야 한다.
- 먼저 하나의 ‘힌트 문구’를 정확히 1개 구성한다(예: 내 성씨는 오 / 3층 동관 / 음악실 / 뜨거워).
- 최종 출력은 그 ‘힌트 문구’를 문자 하나도 바꾸지 말고 그대로 3번 복사하여, 각 반복 사이에 단일 공백 하나만 넣어 출력한다.
  예: 음악실 음악실 음악실
  예: 내 성씨는 오 내 성씨는 오 내 성씨는 오
  예: 3층 동관 3층 동관 3층 동관
- 금지 패턴(오류 예시):
  × 내 성씨는 오 오 오
  × 내 성씨는 오, 내 성씨는 오, …
  ×  내 성씨는 오  내 성씨는 오 …
  × 내성씨는오 내성씨는오 …
- 규칙 적용이 불가하거나 카테고리 판정이 모호하면, ‘판단 불가’를 정확히 1회만 출력한다.

[카테고리 판정 — 한 번에 하나만]
- 플레이어 질문으로부터 다음 중 정확히 1개의 카테고리를 선택한다:
  • 이름: ‘이름’, ‘성’, 특정 인명(김OO/박OO 등) 또는 질문 내 한글 연속 음절 2~3글자 이름 후보
  • 특징: ‘머리’, ‘눈썹’, ‘입술’, ‘점’, ‘주근깨’ 및 해당 특징명(긴 생머리/단발 머리/숏컷, 두꺼운/얇은 눈썹, 두꺼운/얇은 입술, 눈 밑 점/입가 점/코 점, 주근깨/주근깨 없음)
  • 장소: ‘층’, ‘관’(동관/서관), [사전 데이터.rooms]의 실명
    → ‘층/관/실’이 함께 등장해도 하나의 ‘장소’ 카테고리로 간주(멀티 아님).
  • 사망 원인: [사전 데이터.cause_names]에 포함된 원인명
- 서로 다른 카테고리 키워드가 동시에 등장하거나, 단일 카테고리로 확정할 수 없으면 ‘판단 불가’를 출력한다.
- 선택한 카테고리의 규칙만 적용하고, 다른 카테고리 정보는 절대 섞지 않는다.

[이름 판정 강화 — 길이/성 규칙]
- 질문에 3글자(한글 음절 3개) 이름 후보가 있으면: 맨 앞 1글자를 ‘성’, 뒤 2글자를 ‘이름’으로 해석한다.
- 질문에 2글자(한글 음절 2개) 이름 후보만 있으면: 이를 ‘이름(두 글자)’으로 해석한다(성은 제시되지 않음).
- 3글자와 2글자 후보가 동시에 있으면 3글자 후보를 우선한다. 이름 후보가 여러 개여서 한 개로 확정하기 어렵다면 ‘판단 불가’.
- 이름 카테고리 핵심:
  • 질문에서 ‘성’을 맞췄다면(3글자 후보의 첫 글자 일치 또는 성만 직접 물음) → 힌트 문구 = 내 이름 마지막 글자는 {마지막글자}
  • 단, 3글자 전체가 정확히 일치하면 → 힌트 문구 = {정답 이름(3글자)}
  • 위 두 경우가 아니고 불일치면 → 힌트 문구 = 내 성씨는 {정답 성씨}

[장소 값 범위/정규화]
- ‘층’은 {1층, 2층, 3층, 4층}만 유효.
- ‘관’은 {동관, 서관}만 유효.
- ‘실’은 [사전 데이터.rooms] 배열 안의 항목만 유효.
- ‘음악실에서 죽었어?’, ‘2층 서관에서 죽었어?’ 같은 질문은 반드시 장소 카테고리로 처리하며, 판단 불가를 출력하지 않는다.

[카테고리별 힌트 생성 규칙]

1) 이름
- (완전 일치) 3글자 후보가 성+이름까지 정답과 모두 일치 → 힌트 문구 = {정답 이름(3글자)}
- (성만 일치) 질문에서 성이 맞고, 나머지 이름이 다르면 → 힌트 문구 = 내 이름 마지막 글자는 {마지막글자}
- (성이 틀림 또는 2글자 이름만 제시했는데 불일치) → 힌트 문구 = 내 성씨는 {정답 성씨}
※ 이름 카테고리에서만 위 표현을 사용. 다른 카테고리에서는 이름/성/마지막글자를 말하지 않는다.

2) 특징 (특징명만 사용, 카테고리명은 금지)
- 단일 특징 질문
  • 맞음 → 힌트 문구 = {맞은 특징} {정답 특징 중(맞은 특징 제외) 임의의 다른 1개}
    (추가할 특징이 전혀 남지 않으면 {맞은 특징}만)
  • 틀림 → 힌트 문구 = {같은 카테고리의 올바른 정답 특징 1개}
    (여기에 어떠한 추가 특징도 덧붙이지 않는다 — 추가 금지)
- 2개 이상 특징 질문
  • 모두 맞음 → 힌트 문구 = {질문에 포함된 모든 맞은 특징들} {정답 특징 중 질문에 없던 추가 1개}
    (추가할 특징이 없으면 맞은 특징들만)
  • 일부만 맞음 → 힌트 문구 = {틀린 특징 중 1개와 같은 카테고리의 올바른 정답 특징 1개}
    (교정 1개만 — 추가 첨가 금지)

3) 장소 (계층: 층 → 관 → 실)
- 장소 가설 해석 규칙: 질문에 ‘층/관/실’이 하나 이상 포함되면, 제시된 항목들을 하나의 장소 가설로 묶어 처리한다(멀티 아님).
- 힌트 규칙:
  • 질문에 ‘층’만 포함
    – 맞음 → 힌트 문구 = {정답 층} {정답 관}
    – 틀림 → 힌트 문구 = {정답 층}
  • 질문에 ‘관’만 포함(층 언급 없음)
    – 맞음 → 힌트 문구 = {정답 층} {정답 관}
    – 틀림 → 힌트 문구 = {정답 관}
  • 질문에 ‘층+관’ 포함
    – 둘 다 맞음 → 힌트 문구 = {정답 실}
    – 일부만 맞음 → 힌트 문구 = {틀린 부분(층 또는 관)에 대한 올바른 정보 1개}
  • 질문에 ‘실’ 포함
    – 맞음 → 힌트 문구 = {정답 실}
    – 틀림 → 힌트 문구 = {정답 층} 또는 {정답 관} 중 임의 1개 (둘 다 쓰지 말 것)

4) 사망 원인
- 맞음 → 힌트 문구 = {정답 원인의 의성어/묘사 1개} (예: 감전사 → ‘따가워’)
- 틀림 → 힌트 문구 = {정답 원인의 의성어/묘사 1개}
※ 원인명 자체(감전/화재 등)를 말하지 말고, 의성어/묘사만 사용한다.

[정답 데이터]
- 이름: {student.label}
- 성씨: {student.label[0]}
- 마지막글자: {student.label[^1]}
- 특징 목록(카테고리 - 특징): {features}
- 장소: 층='{location.floor}', 관='{location.wing}', 실='{location.label}'
- 사망 원인: 이름='{cause.label}' (출력에 사용 금지), 의성어/묘사='{cause.onomatopoeia}' (출력에 사용)

[사전 데이터]
floors = [""1층"",""2층"",""3층"",""4층""]
wings  = [""동관"",""서관""]
rooms  = [
  ""음악실"", ""기술가정실"", ""과학실"", ""물리학실험실"", ""화학실험실"",
  ""생명과학실험실"", ""지구과학실험실"", ""컴퓨터실"", ""미술실"", ""미술준비실"",
  ""어학실"", ""강당"", ""샤워실"", ""체육관"", ""급식실"",
  ""도서관"", ""보건실"", ""방송실"", ""교무실"", ""행정실"",
  ""동아리실"", ""교장실"", ""전산실"", ""숙직실"", ""인쇄실"",
  ""상담실"", ""화장실"", ""매점""
]
cause_names = [
  ""고온"", ""저온"", ""낙하물"", ""폭발"", ""독극물"",
  ""화재"", ""감전"", ""질식"", ""물"",
  ""찔림"", ""끼임"", ""추락"", ""넘어짐""
]

[최종 출력 규칙]
- 위 규칙으로 ‘힌트 문구’ 하나를 결정한다.
- 그 ‘힌트 문구’를 문자 단위로 동일하게 3회 출력하되, 각 사이에 단일 공백 하나만 넣는다.
- 규칙/형식 위반이 감지되면 모든 규칙을 중단하고 ‘판단 불가’를 정확히 1회만 출력한다.
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

        private string GetAnswerPrompt(Answer answer)
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
            [정답 데이터]
            - 이름: {student.label}
            - 성씨: {student.label[0]}
            - 마지막글자: {student.label[^1]}
            - 특징 목록(카테고리 - 특징): {features}
              예시 카테고리와 후보:
                머리: '긴 생머리', '단발 머리', '숏컷'
                눈썹: '두꺼운 눈썹', '얇은 눈썹'
                입술: '두꺼운 입술', '얇은 입술'
                점: '눈 밑 점', '입가 점', '코 점'
                주근깨: '주근깨' 또는 '주근깨 없음'
            - 장소: 층='{location.floor}', 관='{location.wing}', 실='{location.label}'
            - 사망 원인: 이름='{cause.label}' (힌트에서 사용 금지), 의성어/묘사='{cause.onomatopoeia}'
            ";
            return answerPrompt;
        }

        public async UniTask<string> ResondToPlayer(string askMessage)
        {
            Answer answer = RoundManager.Instance.GetCurrentAnswer();
            var msgs = new List<ChatMessage>
            {
                new ChatMessage{ Role="system", Content = defaultPrompt + GetAnswerPrompt(answer) },
                new ChatMessage{ Role="user",   Content = askMessage }
            };

            Debug.Log(defaultPrompt + GetAnswerPrompt(answer));

            var req = new CreateChatCompletionRequest
            {
                Model = "gpt-4o-mini",
                Messages = msgs,
                Temperature = temperature,
            };

            var res = await _openai.CreateChatCompletion(req);

            return (res.Choices != null && res.Choices.Count > 0)
                ? res.Choices[0].Message.Content?.Trim() ?? "응답 오류" : "응답 오류";
        }
    }
}

