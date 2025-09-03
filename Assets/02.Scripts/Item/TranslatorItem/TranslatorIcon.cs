using UnityEngine;
using UnityEngine.EventSystems;

public class TranslatorIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _toolTip;

    void OnEnable()
    {
        _toolTip?.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _toolTip?.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _toolTip?.SetActive(false);
    }
}
