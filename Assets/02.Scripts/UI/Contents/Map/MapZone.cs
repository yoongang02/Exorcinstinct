using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Map Zone Info")]
    [Space(5)]
    [SerializeField] private LocationSO _locationSO;

    [Header("Map Zone UI")]
    [Space(5)]
    private Image _mapBackground;
    [SerializeField] private float _colorAlphaValue = 0.5f;

    private ToolTip _toolTip;

    private void Awake()
    {
        _mapBackground = GetComponent<Image>();
        _toolTip = GetComponent<ToolTip>();
    }

    private void Start()
    {
        Initialize();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHighlight(_colorAlphaValue);
        _toolTip.ShowToolTip(_locationSO);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHighlight(0f);
        _toolTip.HideToolTip();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    /// <summary>
    /// 맵 존 정보 초기화
    /// </summary>
    private void Initialize()
    {
        SetHighlight(0f);
    }

    private void SetHighlight(float value)
    {
        Color color = _mapBackground.color;
        color.a = value;
        _mapBackground.color = color;
    }
}
