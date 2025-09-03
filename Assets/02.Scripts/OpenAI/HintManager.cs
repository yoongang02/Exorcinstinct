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
        오직 이 대화에서 제공되는 [정답 데이터]와 [플레이어 질문]만 사용해 판단하라. 
        이전 메시지나 대화 맥락은 모두 무시한다.

        출력 형식(중요):
        - 한 줄의 한국어 힌트 문자열만 출력한다.
        - 힌트 외 어떤 텍스트도 쓰지 말 것.
        - 힌트는 동일 단어/구를 3회 반복(기본).
        - 반복되는 단어/구 사이에 공백을 줌. 예: 음악실 음악실 음악실, 3층동관 3층동관 3층동관
        - O/X/판단불가 같은 표기 금지. 규칙에 따라 힌트만 출력.

        범주 & 규칙:
        1) 이름
           - 이름은 모두 세글자임.
           - 질문이 완전한 이름을 맞췄다면 → 이름 반복
           - 성만 맞고 이름이 틀리면 → '내 이름 마지막 글자는 {마지막글자}'를 반복
           - 성도 틀리면 → '내 성씨는 {정답 성씨}'를 반복

        2) 특징
           - 단일 특징을 물었을 때
             - 맞음 → 맞은 특징 + 추가 다른 정답 특징 1개를 연결(예: 긴머리에 두꺼운눈썹) 후 반복
             - 이때, 다른 정답 특징은 정답 학생이 갖고 있는 특징들 중에 맞은 특징을 제외한 나머지 특징들 중 하나를 임의로 선택함.
             - 틀림 → 같은 카테고리 내 올바른 정답 특징으로 교정하여 그 특징만 반복
           - 2개 이상 특징을 물었을 때
             - 모두 맞음 → (질문에 포함된 맞은 특징들) + (정답 특징 중 질문에 없던 추가 1개)를 연결한 문자열을 반복
             - 일부만 맞음 → 틀린 특징 중 1개를 골라 같은 카테고리의 올바른 정답 특징으로 교정하여 그 특징만 반복

        3) 장소 (계층: 층 → 관 → 실)
           - 질문에 층만 포함
             - 맞음 → '층+관' 형태(예: 3층 동관)를 반복
             - 틀림 → 올바른 층만 반복
           - 질문에 관만 포함(층 언급 없음)
             - 맞음 → '층+관' 형태를 반복
             - 틀림 → 올바른 관만 반복
           - 질문에 층+관 포함
             - 둘 다 맞음 → 올바른 실만 반복
             - 일부만 맞음 → 틀린 부분만 올바르게 교정한 정보(층 또는 관 중 교정이 필요한 것)를 반복
           - 질문에 실 포함
             - 맞음 → 실만 반복
             - 틀림 → 층 또는 관 중 하나를 임의 선택하여 그 올바른 정보를 반복

        4) 사망 원인
           - 맞음 → 정답 원인의 의성어나 묘사를 반복(예: 감전사라면 따가워)
           - 틀림 → 정답 원인의 의성어나 묘사를 반복

        주의:
        - 힌트 생성은 오직 [정답 데이터]를 근거로 한다. 질문한 학생/발화자의 정보 등 외부 정보로 추론 금지.
        - 질문이 모호해도 위 규칙 중 가장 합리적인 케이스로 매핑해 힌트를 생성하라.
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
    }
}

