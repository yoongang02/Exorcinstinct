using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image _mapBackground;
    [SerializeField] private float _colorAlphaValue = 0.5f;

    private void Awake()
    {
        _mapBackground = GetComponent<Image>();
        SetHighlight(0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHighlight(_colorAlphaValue);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHighlight(0f);
    }

    public void SetHighlight(float value)
    {
        Color color = _mapBackground.color;
        color.a = value;
        _mapBackground.color = color;
    }
}
