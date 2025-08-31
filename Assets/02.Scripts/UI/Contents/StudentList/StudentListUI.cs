using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StudentListUI : MonoBehaviour
{
    const int pageCapacity = 8;
    [SerializeField] int _currentChapterIndex;
    [SerializeField] int _currentOffset;

    [Header("Slot Variable")]
    [Space(5)]
    [SerializeField] List<StudentSlot> _leftSlots = new List<StudentSlot>();
    [SerializeField] List<StudentSlot> _rightSlots = new List<StudentSlot>();

    [Header("Button Variable")]
    [Space(5)]
    [SerializeField] private Button _prevBtn;
    [SerializeField] private Button _nextBtn;
    [SerializeField] private List<Button> _chapters = new List<Button>();
    [SerializeField] private Vector3 _selected;
    [SerializeField] private Vector3 _disSelected;

    private List<StudentSO> _students = new List<StudentSO>();
    private List<int> _chapterList = new List<int>();
    private Dictionary<int, List<StudentSO>> _studentsDictionary;

    private List<FeatureSO> _curSelectedFeatures = new List<FeatureSO>();

    private void Start()
    {
        
    }

    public void ClickTest()
    {
        Initialize();
        UpdatePages();
    }

    private void Initialize()
    {
        _currentChapterIndex = 0;
        _currentOffset = 0;

        foreach (var student in DataManager.Instance.Students)
        {
            _students.Add(student.Value);
        }

        _studentsDictionary = _students.GroupBy(student => student.chapterIndex)
            .ToDictionary(group => group.Key, group => group.OrderBy(student => student.label).ToList());
    
        _chapterList = _studentsDictionary.Keys.OrderBy(key => key).ToList();
    }

    private void ClearAll()
    {
        for(int i=0; i<4; i++)
        {
            _leftSlots[i].gameObject.SetActive(false);
            _rightSlots[i].gameObject.SetActive(false);
        }
    }

    private void UpdatePages()
    {
        var list = GetFilteredList(_currentChapterIndex);

        // 필터된 리스트에 따라 offset 도 조정
        if(_currentOffset >= list.Count)
        {
            _currentOffset = Mathf.Max(0, ((list.Count - 1) / pageCapacity) * pageCapacity);
        }

        if(list.Count == 0)
        {
            // 다 비어보이게
            ClearAll();
            return;
        }

        for(int i=0; i< pageCapacity; i++)
        {
            int index = _currentOffset + i;
            bool hasData = index < list.Count;

            if(i < 4)
            {
                // 왼쪽 페이지에 생성
                GenerateSlot(_leftSlots[i], hasData ? list[index] : null);
            }
            else
            {
                int rightIndex = i - 4;
                GenerateSlot(_rightSlots[rightIndex], hasData ? list[index] : null);
            }
        }

        SetChapterPosition(_currentChapterIndex);
        UpdateBtnState();
    }

    private void GenerateSlot(StudentSlot slot, StudentSO data)
    {
        if(data == null)
        {
            slot.gameObject.SetActive(false);
        }
        else
        {
            slot.gameObject.SetActive(true);
            slot.SetStudentInfo(data);
        }
    }

    public void OnClickPrevBtn()
    {
        var list = GetFilteredList(_currentChapterIndex);

        // 같은 챕터를 가진 학생들이 앞에 더 있다면
        if (_currentOffset - pageCapacity >= 0)
        {
            _currentOffset -= pageCapacity;
        }
        else
        {
            // 같은 챕터를 가진 학생이 없다면
            if(_currentChapterIndex == _chapterList.First())
            {
                // 첫번째 챕터, 첫번째 페이지라면 prev 버튼 비활성화
                _prevBtn.interactable = false;
                return;
            }
            else
            {
                // 이전 챕터로 이동
                int curChapterIndex = _chapterList.IndexOf(_currentChapterIndex);
                _currentChapterIndex = _chapterList[curChapterIndex - 1];

                // 이전 챕터 학생이 몇 페이지 나오는지 계산해서 offset 결정해야 함.
                // 이전 챕터의 총 학생 수 계산
                var prevList = GetFilteredList(_currentChapterIndex);
                int studentCount = prevList.Count;
                _currentOffset = Mathf.Max(0, ((studentCount - 1) / pageCapacity) * pageCapacity);
            }
        }
        UpdatePages();
    }

    public void OnClickNextBtn()
    {
        var list = GetFilteredList (_currentChapterIndex);

        // 같은 챕터를 가진 학생들이 더 남아있는지 체크
        if (_currentOffset + pageCapacity < list.Count)
        {
            _currentOffset += pageCapacity;
        }
        else
        {
            // 마지막 챕터, 마지막 페이지라면 버튼 비활성화
            if(_currentChapterIndex == _chapterList.Last())
            {
                _nextBtn.interactable = false;
                return;
            }
            else
            {
                // 마지막 챕터가 아니라면, 다음 챕터로 이동
                int curChapterIndex = _chapterList.IndexOf(_currentChapterIndex);
                _currentChapterIndex = _chapterList[curChapterIndex + 1];
                _currentOffset = 0;
            }
        }
        UpdatePages();
    }

    public void OnClickChapter(int chapterIndex)
    {
        if (!_chapterList.Contains(chapterIndex)) return;
        _currentChapterIndex = chapterIndex;
        _currentOffset = 0;

        SetChapterPosition(chapterIndex);
        UpdatePages();
    }

    private void SetChapterPosition(int chapterIndex)
    {
        for (int i = 0; i < _chapters.Count; i++)
        {
            if (i == chapterIndex)
            {
                OnChapterSelected(i);
            }
            else
            {
                OnChapterDisSelected(i);
            }
        }
    }

    private void OnChapterSelected(int chapterIndex)
    {
        Transform t = _chapters[chapterIndex].GetComponent<Transform>();
        Vector3 newPos = new Vector3(_selected.x, t.localPosition.y, t.localPosition.z);
        t.localPosition = newPos;
    }
    
    private void OnChapterDisSelected(int chapterIndex)
    {
        Transform t = _chapters[chapterIndex].GetComponent<Transform>();
        Vector3 newPos = new Vector3(_disSelected.x, t.localPosition.y, t.localPosition.z);
        t.localPosition = newPos;
    }

    public void UnSelectSlot(StudentSlot data)
    {
        foreach(var slot in _leftSlots)
        {
            if (slot == data) continue;
            else slot.UnSelect();
        }

        foreach (var slot in _rightSlots)
        {
            if (slot == data) continue;
            else slot.UnSelect();
        }
    }

    private void UpdateBtnState()
    {
        if (_currentOffset - pageCapacity < 0 && _currentChapterIndex == _chapterList.First())
        {
            // 첫번째 챕터, 첫번째 페이지라면 prev 버튼 비활성화
            _prevBtn.interactable = false;
        }
        else
        {
            _prevBtn.interactable = true;
        }

        var list = GetFilteredList(_currentChapterIndex);
        if (_currentOffset + pageCapacity >= list.Count && _currentChapterIndex == _chapterList.Last())
        {
            _nextBtn.interactable = false;
        }
        else
        {
            _nextBtn.interactable = true;
        }
    }

    private readonly HashSet<FeatureSO> _studentFeatures = new HashSet<FeatureSO>();
    private bool HasAllSelectedFeatures(StudentSO studentSO)
    {
        if (_curSelectedFeatures == null || _curSelectedFeatures.Count == 0) return true;

        var set = _studentFeatures;
        set.Clear();

        // 학생이 갖고 있는 특징들을 모두 저장
        foreach(var feature in studentSO.features)
        {
            set.Add(feature);
        }

        // 학생이 갖고 있는 특징과 현재 특징 페이지에서 선택된 특징들 비교
        // 하나라도 다르면 false 리턴. 즉, and 조건임
        foreach (var need in _curSelectedFeatures) {
            if (!set.Contains(need)) return false;
        }

        return true;
    }

    private readonly List<StudentSO> _filteredBuffer = new List<StudentSO>();
    private List<StudentSO> GetFilteredList(int chapterIndex)
    {
        if (!_studentsDictionary.TryGetValue(chapterIndex, out var list) || list == null)
            return new List<StudentSO>();

        _filteredBuffer.Clear();
        foreach (var student in list)
        {
            if (HasAllSelectedFeatures(student)) _filteredBuffer.Add(student);
        }

        return _filteredBuffer;
    }
}
