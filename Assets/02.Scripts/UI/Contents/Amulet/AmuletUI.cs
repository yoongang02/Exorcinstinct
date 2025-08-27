using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmuletUI : UIBase
{
    [Header("AmuletUI")]
    [Space(5)]
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TextMeshProUGUI _locationText;
    [SerializeField] private TextMeshProUGUI _causeText;
    [SerializeField] private Sprite _emptySprite;
    [SerializeField] Button CheckAnswerBtn;

    private void Start()
    {
        RoundManager.Instance.OnClearAmulet += ClearAll;
        RoundManager.Instance.OnSetStudent += SetStudent;
        RoundManager.Instance.OnSetLocation += SetLocation;
        RoundManager.Instance.OnSetCause += SetCause;
    }

    public override void OnOpen()
    {
        base.OnOpen();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    /// <summary>
    /// 명단, 지도 등에서 호출하는 함수. 정보 세팅을 할 수 있다.
    /// </summary>

    private void SetStudent(StudentSO student)
    {
        _nameText.text = student.label;
        _portraitImage.sprite = Resources.Load<Sprite>(student.sprite);
    }

    private void SetLocation(LocationSO location)
    {
        string str = $"{location.floor} {location.direction} {location.label}";
        _locationText.text = str;
    }

    private void SetCause(CauseSO cause)
    {
        _causeText.text = cause.label;
    }

    /// <summary>
    /// 정보를 초기화하는 함수
    /// </summary>

    private void ClearAll()
    {
        ClearStudent();
        ClearLocation();
        ClearCause();
    }

    private void ClearStudent()
    {
        _nameText.text = string.Empty;
        _portraitImage.sprite = _emptySprite;
    }

    private void ClearLocation()
    {
        _locationText.text = string.Empty;
    }

    private void ClearCause()
    {
        _causeText.text = string.Empty;
    }

    /// <summary>
    /// 현재 각 슬롯의 정보가 비어있는지 확인하는 함수.
    /// 비어있을 경우에만 마우스 호버 시 툴팁이 뜨도록 하기 위함.
    /// </summary>
    /// <returns></returns>

    public bool IsStudentSet()
    {
        return !string.IsNullOrEmpty(_nameText.text);
    }

    public bool IsLocationSet()
    {
        return !string.IsNullOrEmpty(_locationText.text);
    }

    public bool IsCauseSet()
    {
        return !string.IsNullOrEmpty(_causeText.text);
    }
}
