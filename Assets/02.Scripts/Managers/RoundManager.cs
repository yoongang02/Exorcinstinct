using OpenAI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct Answer
{
    public StudentSO studentSO;
    public LocationSO locationSO;
    public CauseSO causeSO;
}
public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }
    private Answer _currentAnswer;

    public UnityAction OnClearAmulet;
    public UnityAction<StudentSO> OnSetStudent;
    public UnityAction<LocationSO> OnSetLocation;
    public UnityAction<CauseSO> OnSetCause;

    private StudentSO _curStudentSO;
    private LocationSO _curLocationSO;
    private CauseSO _curCauseSO;

    [Header("Candle Variable")]
    [Space(5)]
    [SerializeField] private List<CandleController> _candles = new List<CandleController>();


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

    public void RoundStart()
    {
        // 부적 UI 초기화
        OnClearAmulet?.Invoke();

        // 라운드 시작 시, 정답을 랜덤으로 생성
        _currentAnswer = PickRandomAnswer();

        // GPT 매니저에 정답 전달 및 시스템 프롬프트 설정
        GptManager.Instance.RoundStartSetting(_currentAnswer);
    }

    // 라운드 시작마다, 정답을 랜덤으로 생성하는 함수
    private Answer PickRandomAnswer()
    {
        List<StudentSO> studentOptions = new List<StudentSO>();
        List<LocationSO> locationOptions = new List<LocationSO>();
        List<CauseSO> causeOptions = new List<CauseSO>();

        // #1 enabled = true 인 항목만 선택지에 포함하기
        foreach (var student in DataManager.Instance.Students)
        {
            StudentSO so = student.Value;
            if (so == null) continue;
            if (!so.enabled) continue;   
            
            studentOptions.Add(so);
        }
        
        foreach (var location in DataManager.Instance.Locations)
        {
            LocationSO so = location.Value;
            if (so == null) continue;
            if (!so.enabled) continue;   
            
            locationOptions.Add(so);
        }
        
        // #2 WeightedRandomPicker 생성
        var studentPicker = new Rito.WeightedRandomPicker<StudentSO>();
        var locationPicker = new Rito.WeightedRandomPicker<LocationSO>();
        var causePicker = new Rito.WeightedRandomPicker<CauseSO>();

        // #3 WeightedRandomPicker에 후보지 전달하기
        foreach (var option in studentOptions)
        {
            studentPicker.Add(option, option.weight);
        }
        foreach (var option in locationOptions)
        {
            locationPicker.Add(option, option.weight);
        }
        
        // #4 Pick 하기
        StudentSO studentPick = studentPicker.GetRandomPick();
        LocationSO locationPick = locationPicker.GetRandomPick();


        // #5 뽑힌 LocationSO가 갖고 있는 CauseSO를 확인하고, CauseSO를 뽑아내기
        foreach (var cause in locationPick.causes)
        {
            if (cause == null) continue;
            if (!cause.enabled) continue; // CauseSO도 enabled 체크
            
            causePicker.Add(cause, cause.weight);
        }
        CauseSO causePick = causePicker.GetRandomPick();

        // #5 Pick 값 확인하기
        Debug.Log($"Student Pick : {studentPick.label}({studentPick.id})");
        Debug.Log($"Location Pick : {locationPick.label}({locationPick.id})");
        Debug.Log($"Cause Pick : {causePick.label}({causePick.id})");

        Answer answer = new Answer
        {
            studentSO = studentPick,
            locationSO = locationPick,
            causeSO = causePick,
        };

        return answer;
    }

    /// <summary>
    /// 현재 부적에 세팅되어 있는 학생, 장소, 사망 원인 정보를 리턴 받을 수 있다.
    /// </summary>
    /// <returns></returns>
    public StudentSO GetStudent()
    {
        return _curStudentSO;
    }

    public LocationSO GetLocation()
    {
        return _curLocationSO;
    }

    public CauseSO GetCause()
    {
        return _curCauseSO;
    }

    public void SetStudentSO(StudentSO studentSO)
    {
        _curStudentSO = studentSO;
    }
    public void SetLocationSO(LocationSO locationSO)
    {
        _curLocationSO = locationSO;
    }
    public void SetCauseSO(CauseSO causeSO)
    {
        _curCauseSO = causeSO;
    }

    public Answer GetCurrentAnswer()
    {
        return _currentAnswer;
    }
}
