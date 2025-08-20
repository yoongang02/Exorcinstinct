using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
    private Dictionary<string, StudentSO> _students;
    public IReadOnlyDictionary<string, StudentSO> Students => _students;

    private Dictionary<string, FeatureSO> _features;
    public IReadOnlyDictionary<string, FeatureSO> Features => _features;
    
    private Dictionary<string, LocationSO> _locations;
    public IReadOnlyDictionary<string, LocationSO> Locations => _locations;
    
    private Dictionary<string, CauseSO> _causes;
    public IReadOnlyDictionary<string, CauseSO> Causes => _causes;
    
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

        InitializeDatas();
    }

    private void InitializeDatas()
    {
        // Students 초기화
        _students = new Dictionary<string, StudentSO>();
        var students = Resources.LoadAll<StudentSO>("StudentSO");
        foreach (var s in students)
        {
            if (s == null || string.IsNullOrEmpty(s.id)) continue;
            _students[s.id] = s;
        }
        
        // Features 초기화
        _features = new Dictionary<string, FeatureSO>();
        var features = Resources.LoadAll<FeatureSO>("FeatureSO");
        foreach (var s in features)
        {
            if (s == null || string.IsNullOrEmpty(s.id)) continue;
            _features[s.id] = s;
        }
        
        // Locations 초기화
        _locations = new Dictionary<string, LocationSO>();
        var locations = Resources.LoadAll<LocationSO>("LocationSO");
        foreach (var s in locations)
        {
            if (s == null || string.IsNullOrEmpty(s.id)) continue;
            _locations[s.id] = s;
        }
        
        // Causes 초기화
        _causes = new Dictionary<string, CauseSO>();
        var causes = Resources.LoadAll<CauseSO>("CauseSO");
        foreach (var s in causes)
        {
            if (s == null || string.IsNullOrEmpty(s.id)) continue;
            _causes[s.id] = s;
        }
    }
}
