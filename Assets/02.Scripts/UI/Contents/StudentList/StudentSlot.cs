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

    void Awake()
    {
        _outline.enabled = false;
        _hoverBackground.enabled = false;
        ChangeOpacity(0f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _outline.enabled = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeOpacity(_alphaValue);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeOpacity(0);
    }

    public void SetStudentInfo(StudentSO studentSO)
    {
        _portraitImage.sprite = Resources.Load<Sprite>(studentSO.sprite);
        _nameText.text = studentSO.label;

        string features = "";
        foreach (var feature in studentSO.features)
        {
            features += $"{feature.label}, ";
        }
        features = features.TrimEnd(',', ' ');
        _descriptionText.text = features;
    }

    private void ChangeOpacity(float value)
    {
        float newValue = value / 255f;
        Color color = _hoverBackground.color;
        color.a = newValue;
        _hoverBackground.color = color;
    }
}
