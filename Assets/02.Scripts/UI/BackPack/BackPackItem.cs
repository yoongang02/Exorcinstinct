using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackPackItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private ObjectInfoSO _objectInfo;
    [SerializeField] private GameObject _toolTip;
    [SerializeField] private Vector2 _offset = new Vector2(-12f, 0f); // 커서와 툴팁 사이의 간격

    private Outline _outline;
    private Canvas _toolTipCanvas;
    private RectTransform _toolTipRectTransform;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;

        _toolTipCanvas = GetComponentInParent<Canvas>();
        _toolTipRectTransform = _toolTipCanvas.GetComponent<RectTransform>();
    }

    protected void LateUpdate()
    {
        if(_toolTip.activeSelf)
            CalculateMousePosition();
    }

    protected void ShowToolTip()
    {
        SetToolTipText();
        _toolTip.SetActive(true);
    }

    protected void HideToolTip()
    {
        _toolTip.SetActive(false);
    }

    protected void SetToolTipText()
    {
        // 오브젝트 정보창에 오브젝트 정보 세팅
        TextMeshProUGUI name = _toolTip.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI description = _toolTip.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        name.text = _objectInfo.objectName;
        description.text = _objectInfo.objectDescription;
    }

    /// <summary>
    /// 마우스 커서 위치에 따라 툴팁 위치 조정하는 함수
    /// </summary>
    protected void CalculateMousePosition()
    {
        // 캔버스 좌표로 변환
        Vector2 mousePos = Input.mousePosition;
        Vector2 localPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _toolTipRectTransform, mousePos, _toolTipCanvas.worldCamera, out localPos);

        // 툴팁 RectTransform
        RectTransform rt = _toolTip.transform as RectTransform;

        // 피벗을 (1,0.5)로 설정 (오른쪽 중앙)
        rt.pivot = new Vector2(1f, 0.5f);

        // 위치 지정
        rt.anchoredPosition = localPos + _offset;
    }

    /// <summary>
    /// 마우스가 아이템 위에 올라갔을 때 호출되는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 아웃라인 효과 활성화
        _outline.enabled = true;
    }

    /// <summary>
    /// 마우스가 아이템에서 벗어났을 때 호출되는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        // 아웃라인 효과 비활성화
        _outline.enabled = false;
    }

    /// <summary>
    /// 마우스로 아이템을 클릭했을 때 호출되는 함수
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 각 아이템을 클릭했을 때 실행되는 함수
    /// 각 아이템마다 다르게 구현될 수 있음
    /// </summary>
    public virtual void UseItem()
    {
        Debug.Log($"{_objectInfo.objectName} 아이템 사용");
    }
}
