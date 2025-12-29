using UnityEngine;
using UnityEngine.EventSystems;

public class AmuletSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum AmuletSlotType
    {
        Name,
        Face,
        Location,
        Cause
    }
    [SerializeField] private AmuletSlotType _amuletSlotType;
    private AmuletUI _amuletUI;
    private ToolTip _toolTip;

    private void Awake()
    {
        _amuletUI = GetComponentInParent<AmuletUI>();
        _toolTip = GetComponent<ToolTip>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(IsBlank()) _toolTip.ShowToolTip();

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(IsBlank()) _toolTip.HideToolTip();
    }

    private bool IsBlank()
    {
        switch (_amuletSlotType)
        {
            case AmuletSlotType.Name:
                return !_amuletUI.IsStudentSet();
            case AmuletSlotType.Face:
                return !_amuletUI.IsStudentSet();
            case AmuletSlotType.Location:
                return !_amuletUI.IsLocationSet();
            case AmuletSlotType.Cause:
                return !_amuletUI.IsCauseSet();
            default:
                return false;
        }
    }
}
