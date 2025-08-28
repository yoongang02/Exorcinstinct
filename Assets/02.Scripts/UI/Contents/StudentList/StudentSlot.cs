using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StudentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Student Info")]
    [Space(5)]
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [Header("Pointer Effect")]
    [Space(5)]
    [SerializeField] private float _alphaValue = 65f;
    [SerializeField] private Image _outline;
    [SerializeField] private Image _hoverBackground;

    private StudentListUI _studentListUI;
    private StudentSO _studentSO;

    void Awake()
    {
        _outline.enabled = false;
        ChangeOpacity(0f);
    }

    void Start()
    {
        _studentListUI = GetComponentInParent<StudentListUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _outline.enabled = true;

        // 본인 슬롯 제외하고 나머지 슬롯 선택 아웃라인 비활성화
        _studentListUI.UnSelectSlot(this);

        // 선택한 슬롯 정보 부적에 반영하기
        RoundManager.Instance.OnSetStudent?.Invoke(_studentSO);
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeOpacity(_alphaValue);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!_outline.enabled) ChangeOpacity(0);
    }

    public void SetStudentInfo(StudentSO studentSO)
    {
        _studentSO = studentSO;

        _portraitImage.sprite = Resources.Load<Sprite>(studentSO.sprite);
        _nameText.text = studentSO.label;

        string features = "";
        foreach (var feature in studentSO.features)
        {
            features += $"{feature.label}, ";
        }
        features = features.TrimEnd(',', ' ');
        _descriptionText.text = features;

        // 첫 설정이니까, 선택 효과 초기화
        _outline.enabled = false;
        ChangeOpacity(0);
    }

    private void ChangeOpacity(float value)
    {
        float newValue = value / 255f;
        Color color = _hoverBackground.color;
        color.a = newValue;
        _hoverBackground.color = color;
    }

    public void UnSelect()
    {
        _outline.enabled = false;
        ChangeOpacity(0);
    }
}
