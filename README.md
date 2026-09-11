# 귀소본능 (Exorcinstinct)
<a href="https://youtu.be/GeJOlpVHNn4" target="_blank">
  <img width="640" height="360" alt="YouTube Video" src="https://github.com/user-attachments/assets/31f7684f-b163-406d-bcea-a7696301e3cf" />
</a>
<br>[트레일러 영상]

<br>분신사바로 귀신에게 O/X 질문을 던지고, 단서를 조합해 귀신의 정체를 밝혀내는 1인칭 추리 게임입니다.

## 프로젝트 개요

| 항목 | 내용 |
| --- | --- |
| 엔진 | Unity 6 (6000.0.54f1) |
| 언어 | C# |
| 플랫폼 | PC (Windows / macOS) |
| 장르 / 분야 | 1인칭 추리 |
| 기간 | 2025.08 ~ 2025.09 |
| 인원 | 2명 (기획 1 / 개발 1) |
| 담당 역할 | 클라이언트 프로그래밍 |
| 성과 | 제1회 VARCO 3D 인디게임 공모전 대상 |

## 주요 담당 업무

- 음성 질문을 O/X/판단불가 응답으로 연결하는 AI 질의응답 흐름 구현
- 오픈소스 가중치 추첨기를 활용한 라운드 정답 조합 생성
- 학생 명단·지도·부적 UI와 추리 아이템 효과 구현
- 정답 제출, 성공·실패 분기와 컷신 연결 구현

## 주요 구현 기능

### 음성 질문과 AI 응답
<img width="550" height="300" alt="Image" src="https://github.com/user-attachments/assets/116617f2-c2b8-4fd6-b9ab-c8d7c7cdbf56" />
<br>녹음한 음성을 텍스트로 변환한 뒤, 라운드 정답을 포함한 GPT 프롬프트로 O/X/판단불가 응답을 요청했습니다. 귀신 번역기 사용 시에는 별도 힌트 생성과 ElevenLabs 음성 출력을 연결했습니다.

[관련 코드](https://github.com/yoongang02/Exorcinstinct/blob/b1e7e9d3ea089c025c56fcaad94c2a234c33e285/Assets/02.Scripts/OpenAI/WhisperManager.cs)

### 가중치 기반 정답 구성
<img width="550" height="300" alt="image" src="https://github.com/user-attachments/assets/35d527e1-712e-4a4e-9749-d4473fd61aea" />
<br>활성화된 학생·장소 데이터에서 가중치에 따라 후보를 선택하고, 선택된 장소에 연결된 사망 원인 중 하나를 추첨했습니다. Rito의 WeightedRandomPicker를 활용해 게임의 정답 조합과 프롬프트 설정을 구현했습니다.

[관련 코드](https://github.com/yoongang02/Exorcinstinct/blob/b1e7e9d3ea089c025c56fcaad94c2a234c33e285/Assets/02.Scripts/Managers/RoundManager.cs)

### 단서 UI와 아이템
<img width="550" height="300" alt="image" src="https://github.com/user-attachments/assets/c64465ca-cf0d-43a6-88ea-3d33150bf1bd" />
<br><img width="550" height="300" alt="image" src="https://github.com/user-attachments/assets/e1c4d20b-1706-465d-bf6f-cc1ade114e90" />
<br><img width="550" height="350" alt="image" src="https://github.com/user-attachments/assets/375ab963-84d0-4fea-9329-6cf0373d0f1a" />

<br>학생의 특징을 확인하는 명단과 장소·사망 원인을 표시하는 지도를 구현했습니다. 부적에 추리 결과를 선택하는 흐름과 반지·번역기·성냥의 사용 상태 및 효과를 연결했습니다.

[관련 코드](https://github.com/yoongang02/Exorcinstinct/blob/b1e7e9d3ea089c025c56fcaad94c2a234c33e285/Assets/02.Scripts/UI/Contents/StudentList/StudentListUI.cs)

### 라운드 진행과 컷신
<img width="550" height="300" alt="image" src="https://github.com/user-attachments/assets/ad65e00d-115c-4231-8b22-0512e14314b4" />
<br>라운드 초기화 시 정답·촛불·아이템 상태를 설정하고, 제출한 조합을 정답과 비교하도록 구현했습니다. 판정 결과에 따라 성공·실패 이벤트와 Timeline 연출을 연결했습니다.

[관련 코드](https://github.com/yoongang02/Exorcinstinct/blob/b1e7e9d3ea089c025c56fcaad94c2a234c33e285/Assets/02.Scripts/TimeLine/TimelineController.cs)

## 프로젝트 구조

담당 코드가 포함된 핵심 경로입니다.

```text
Assets/
├── 02.Scripts/
│   ├── OpenAI/            # 음성 인식·O/X 판정·힌트 생성
│   ├── Elevenlabs/        # 힌트 음성 출력
│   ├── Managers/          # 정답 구성·라운드 진행
│   ├── Item/              # 추리 아이템 효과
│   ├── UI/Contents/       # 학생 명단·지도·부적 UI
│   └── TimeLine/          # 컷신 연결
└── 12.ScriptableObject/DataSO/  # 학생·특징·장소·사망 원인
```

## 관련 링크

- [시연 영상](https://youtu.be/ZTDCXEcP77k)
