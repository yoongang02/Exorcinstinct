using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StudentListUI : MonoBehaviour
{
    [SerializeField] int _currentChapterIndex = 0;
    [SerializeField] int _studentIndex = 0;
    [SerializeField] Transform _leftPageParent;
    [SerializeField] Transform _rightPageParent;
    [SerializeField] GameObject _slotPrefab;

    private Dictionary<int, StudentSO> _students;

    private void Start()
    {
        _currentChapterIndex = 0;
        _studentIndex = 0;
        Initialize();
    }

    private void Initialize()
    {
        int start = 0;
        foreach (var student in DataManager.Instance.Students)
        {
            _students.Add(start, student.Value);
            start++;
        }

        UpdatePages();
    }

    private void UpdatePages()
    {
        // #1 페이지 내의 슬롯을 모두 제거
        ClearPage(_leftPageParent);
        ClearPage(_rightPageParent);

        // #2 현재 챕터 인덱스와 동일한 인덱스를 갖고 있는 학생들을 최대 4명까지 넣는다.
        // 만약 4명까지 존재하지 않는다면 오른쪽 페이지를 체크하지 않고 바로 리턴한다.
        // 4명까지 존재한다면 리턴하지 않고, 오른쪽 페이지도 체크한다.
        int left;
        for (left = 0; left < 4; left++)
        {
            if (_students[_studentIndex+left].chapterIndex == _currentChapterIndex)
            {
                GenerateSlot(_leftPageParent, _students[_studentIndex+left]);
            }
            else
            {
                left--;
                break;
            }
        }

        _studentIndex += left;

        // 왼쪽 페이지에 4개가 모두 채워졌다면
        if (left != 3) return;

        int right;
        for (right = 0; right < 4; right++)
        {
            if (_students[_studentIndex + right].chapterIndex == _currentChapterIndex)
            {
                GenerateSlot(_rightPageParent, _students[_studentIndex + right]);
            }
            else
            {
                right--;
                break;
            }
        }

        _studentIndex += right;
    }

    private void GenerateSlot(Transform parent, StudentSO student)
    {
        GameObject instance = Instantiate(_slotPrefab, parent.transform);
        instance.GetComponent<StudentSlot>().SetStudentInfo(student);
    }

    private void ClearPage(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClickLeftPageBtn()
    {

    }

    private void ClickRightPageBtn()
    {

    }
}
