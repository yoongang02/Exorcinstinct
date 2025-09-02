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

        private const string HintSystemPrompt = @"
        너의 출력은 반드시 단일 JSON 객체 한 개여야 한다.
        텍스트 설명, 코드펜스, 주석을 절대 포함하지 마라.
        키는 version, category, rule_id, params (선택: confidence, reason) 만 포함한다.
        힌트 한국어 문구는 절대 생성하지 마라. (문구 생성은 클라이언트가 한다)
        판정이 어렵다면 rule_id는 ""UNSURE""로 설정하라.
        version은 반드시 ""1.0""으로 설정하라.

        category ∈ { ""name"", ""feature"", ""location"", ""cause"", ""unsure"" }
        rule_id ∈ {
          ""NAME_FULL_MATCH"",
          ""NAME_SURNAME_ONLY_MATCH"",
          ""NAME_ALL_WRONG"",
          ""FEAT_SINGLE_CORRECT_PLUS_ONE"",
          ""FEAT_SINGLE_INCORRECT_CORRECTION"",
          ""FEAT_MULTI_ALL_CORRECT_PLUS_ONE"",
          ""FEAT_MULTI_PARTIAL_CORRECTION_ONE"",
          ""FEAT_MULTI_ALL_KNOWN_REPEAT"",
          ""LOC_FLOOR_MATCH_HINT_WING"",
          ""LOC_FLOOR_INCORRECT_CORRECTION"",
          ""LOC_WING_MATCH_HINT_FLOOR"",
          ""LOC_WING_INCORRECT_CORRECTION"",
          ""LOC_FLOOR_WING_MATCH_HINT_ROOM"",
          ""LOC_FLOOR_WING_PARTIAL_CORRECTION"",
          ""LOC_ROOM_CORRECT_REPEAT"",
          ""LOC_ROOM_INCORRECT_HINT_UPPER_RANDOM"",
          ""CAUSE_CORRECT_ONOMATOPOEIA"",
          ""CAUSE_INCORRECT_SHOW_TRUE_ONOMATOPOEIA"",
          ""UNSURE""
        }

        params 필수 키 (규칙별):
        - NAME_FULL_MATCH:            { ""name"" }
        - NAME_SURNAME_ONLY_MATCH:    { ""last_char"" }
        - NAME_ALL_WRONG:             { ""surname"" }
        - FEAT_SINGLE_CORRECT_PLUS_ONE:      { ""correct_features"": [.. 최소 2개 ..] }
        - FEAT_SINGLE_INCORRECT_CORRECTION:  { ""category"", ""correct_feature"" }
        - FEAT_MULTI_ALL_CORRECT_PLUS_ONE:   { ""correct_features"": [..] }
        - FEAT_MULTI_PARTIAL_CORRECTION_ONE: { ""category"", ""correct_feature"" }
        - FEAT_MULTI_ALL_KNOWN_REPEAT:       { ""repeat_features"": [..] }
        - LOC_FLOOR_MATCH_HINT_WING:         { ""floor"", ""wing"" }
        - LOC_FLOOR_INCORRECT_CORRECTION:    { ""floor"" }
        - LOC_WING_MATCH_HINT_FLOOR:         { ""floor"", ""wing"" }
        - LOC_WING_INCORRECT_CORRECTION:     { ""wing"" }
        - LOC_FLOOR_WING_MATCH_HINT_ROOM:    { ""room"" }
        - LOC_FLOOR_WING_PARTIAL_CORRECTION: { ""floor"" 또는 ""wing"" 중 최소 1개 }
        - LOC_ROOM_CORRECT_REPEAT:           { ""room"" }
        - LOC_ROOM_INCORRECT_HINT_UPPER_RANDOM: { ""floor"", ""wing"", ""random_pool"": [""floor""|""wing""] }
        - CAUSE_CORRECT_ONOMATOPOEIA:        { ""onomatopoeia"" }
        - CAUSE_INCORRECT_SHOW_TRUE_ONOMATOPOEIA: { ""onomatopoeia"" }
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

        /// <summary>
        /// 정답 데이터 + 질문을 함께 전달하는 유저 프롬프트
        /// </summary>
        /// <param name="answer">현재 라운드의 정답</param>
        /// <param name="askMessage">음성인식을 통해 인식한 유저의 질문</param>
        private string BuildHintUserPrompt(Answer answer, string askMessage)
        {
            var featurePairs = new List<string>();

            // 정답 학생의 특징들을 카테고리 : 특징명 형태로 구성
            if(answer.studentSO?.features != null)
            {
                foreach(var feature in answer.studentSO.features)
                {
                    if(!string.IsNullOrWhiteSpace(feature.category) && !string.IsNullOrWhiteSpace(feature.label))
                    {
                        featurePairs.Add($"{feature.category} : \"{feature.label}\"");
                    }
                }
            }

            var name = answer.studentSO?.label ?? string.Empty;
            var surname = name[0];
            var endname = name[^1];
            var floor = answer.locationSO?.floor ?? string.Empty;
            var wing = answer.locationSO?.wing ?? string.Empty;
            var room = answer.locationSO?.label ?? string.Empty;
            var cause = answer.causeSO?.label ?? string.Empty;
            var onoma = answer.causeSO?.onomatopoeia ?? string.Empty;

            // 프롬프트 메세지 변수 채우기
            return $@"
            [정답 데이터]
            - 이름: {name}
            - 성씨: {surname}
            - 이름 마지막 글자 : {endname}
            - 특징: {{ {string.Join(", ", featurePairs)} }}
            - 장소: {{ floor: ""{floor}"", wing: ""{wing}"", room: ""{room}"" }}
            - 사망 원인: {{ type: ""{cause}"", onomatopoeia: ""{onoma}"" }}

            [플레이어 질문]
            ""{askMessage}""

            [판정 규칙 요약]
            1) 이름:
              - 다 맞음 → NAME_FULL_MATCH
              - 성만 맞음 → NAME_SURNAME_ONLY_MATCH
              - 다 틀림 → NAME_ALL_WRONG
            2) 특징:
              - 단일 맞음 → FEAT_SINGLE_CORRECT_PLUS_ONE
              - 단일 틀림(같은 카테고리로 교정) → FEAT_SINGLE_INCORRECT_CORRECTION
              - 다중 모두 맞음 → FEAT_MULTI_ALL_CORRECT_PLUS_ONE
              - 다중 일부만 맞음(틀린 것 중 1개 교정) → FEAT_MULTI_PARTIAL_CORRECTION_ONE
              - 모든 특징 이미 파악되어 반복만 필요 → FEAT_MULTI_ALL_KNOWN_REPEAT
            3) 장소(층→관→실):
              - 층만 포함: 맞음→LOC_FLOOR_MATCH_HINT_WING / 틀림→LOC_FLOOR_INCORRECT_CORRECTION
              - 관만 포함(층 언급 없음): 맞음→LOC_WING_MATCH_HINT_FLOOR / 틀림→LOC_WING_INCORRECT_CORRECTION
              - 층+관 포함: 둘 다 맞음→LOC_FLOOR_WING_MATCH_HINT_ROOM / 일부만 맞음→LOC_FLOOR_WING_PARTIAL_CORRECTION
              - 실 포함: 맞음→LOC_ROOM_CORRECT_REPEAT / 틀림→LOC_ROOM_INCORRECT_HINT_UPPER_RANDOM
            4) 사망 원인:
              - 맞음→CAUSE_CORRECT_ONOMATOPOEIA
              - 틀림→CAUSE_INCORRECT_SHOW_TRUE_ONOMATOPOEIA

            [출력 형식]
            version, category, rule_id, params만 포함된 단일 JSON 객체만 출력하라.
            ";
        }

        public async UniTask<HintRuleResponse> RespondToPlayer(Answer answer, string askMessage)
        {
            var contexts = new List<ChatMessage>
            {
                new ChatMessage { Role = "system", Content = HintSystemPrompt },
                new ChatMessage { Role = "user", Content = BuildHintUserPrompt(answer,askMessage) }
            };
            Debug.Log(BuildHintUserPrompt(answer, askMessage));
            var req = new CreateChatCompletionRequest
            {
                Model = "gpt-4o-mini",
                Messages = contexts,
                Temperature = temperature,
            };

            var res = await _openai.CreateChatCompletion(req);
            var raw = (res.Choices != null && res.Choices.Count > 0) ? res.Choices[0].Message.Content : null;

            if (string.IsNullOrEmpty(raw))
            {
                throw new System.Exception("빈 응답");
            }

            var json = ExtractJsonObject(raw);
            var parsed = JsonConvert.DeserializeObject<HintRuleResponse>(json);
            if(parsed == null || parsed.Params == null)
            {
                throw new Exception($"파싱 실패\nRAW:\n{raw}\nJSON:\n{json}");
            }

            return parsed;
        }

        // 백틱/앞뒤 텍스트 제거
        private static string ExtractJsonObject(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            s = Regex.Replace(s, "^```(?:json)?\\s*|\\s*```$", "", RegexOptions.Singleline);
            int start = s.IndexOf('{'); int last = s.LastIndexOf('}');
            if (start >= 0 && last > start) return s.Substring(start, last - start + 1).Trim();
            return s.Trim();
        }

        // 힌트 문구 출력
        public string ComposeHintText(HintRuleResponse r)
        {
            Debug.Log(r.RuleId);
            string one = r.RuleId switch
            {
                "NAME_FULL_MATCH" => r.Params.Name,
                "NAME_SURNAME_ONLY_MATCH" => $"내 이름 마지막글자는 {r.Params.LastChar}",
                "NAME_ALL_WRONG" => $"내 성씨는 {r.Params.Surname}",

                "FEAT_SINGLE_CORRECT_PLUS_ONE" => string.Join("에", r.Params.CorrectFeatures ?? new[] { "특징" }),
                "FEAT_SINGLE_INCORRECT_CORRECTION" => r.Params.CorrectFeature,
                "FEAT_MULTI_ALL_CORRECT_PLUS_ONE" => string.Join("에", r.Params.CorrectFeatures ?? new[] { "특징" }),
                "FEAT_MULTI_PARTIAL_CORRECTION_ONE" => r.Params.CorrectFeature,
                "FEAT_MULTI_ALL_KNOWN_REPEAT" => string.Join("", r.Params.RepeatFeatures ?? new[] { "특징" }),

                "LOC_FLOOR_MATCH_HINT_WING" => $"{r.Params.Floor}{r.Params.Wing}",
                "LOC_FLOOR_INCORRECT_CORRECTION" => r.Params.Floor,
                "LOC_WING_MATCH_HINT_FLOOR" => $"{r.Params.Floor}{r.Params.Wing}",
                "LOC_WING_INCORRECT_CORRECTION" => r.Params.Wing,
                "LOC_FLOOR_WING_MATCH_HINT_ROOM" => r.Params.Room,
                "LOC_FLOOR_WING_PARTIAL_CORRECTION" => $"{r.Params.Floor}{r.Params.Wing}".Trim(),
                "LOC_ROOM_CORRECT_REPEAT" => r.Params.Room,
                "LOC_ROOM_INCORRECT_HINT_UPPER_RANDOM" => (r.Params.RandomPool != null && r.Params.RandomPool.Length > 0 &&
                                                            r.Params.RandomPool[UnityEngine.Random.Range(0, r.Params.RandomPool.Length)] == "wing")
                                                            ? r.Params.Wing : r.Params.Floor,

                "CAUSE_CORRECT_ONOMATOPOEIA" => r.Params.Onomatopoeia,
                "CAUSE_INCORRECT_SHOW_TRUE_ONOMATOPOEIA" => r.Params.Onomatopoeia,

                _ => "..."
            };

            return string.Concat(one, one, one);
        }
    }
}

