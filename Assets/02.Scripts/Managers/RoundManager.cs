using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

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
        PickRandomAnswer();
    }
    
    // 라운드 시작마다, 정답을 랜덤으로 생성하는 함수
    private void PickRandomAnswer()
    {
        List<StudentSO> studentOptions = new List<StudentSO>();
        List<LocationSO> locationOptions = new List<LocationSO>();
        
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
        
        // #5 Pick 값 확인하기
        Debug.Log($"Student Pick : {studentPick.label}({studentPick.id})");
        Debug.Log($"Location Pick : {locationPick.label}({locationPick.id})");
    }
}
