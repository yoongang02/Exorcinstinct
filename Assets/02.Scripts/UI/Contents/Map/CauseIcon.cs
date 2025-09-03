using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CauseIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private CauseSO _causeSO;
    [SerializeField] private Image _iconImage;
    private MapZone _mapZone;
    private ToolTip _toolTip;
    private Image _outline;

    void Awake()
    {
        _outline = GetComponent<Image>();
        _iconImage.sprite = Resources.Load<Sprite>(_causeSO.iconPath);
    }
    void Start()
    {
        _mapZone = GetComponentInParent<MapZone>();
        _toolTip = GetComponentInParent<ToolTip>();
        SetOutline(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetOutline(true);
        RoundManager.Instance.OnSetLocation?.Invoke(_mapZone.GetLocationSO());
        RoundManager.Instance.OnSetCause?.Invoke(_causeSO);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetOutline(true);
        _toolTip.ShowToolTip(_mapZone.GetLocationSO(), _causeSO);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetOutline(false);
        _toolTip.HideToolTip();
    }

    void SetOutline(bool value)
    {
        _outline.enabled = value;
    }
}
