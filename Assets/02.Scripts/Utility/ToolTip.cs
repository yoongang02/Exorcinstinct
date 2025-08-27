using TMPro;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private bool _isLeft = true; // true면 왼쪽, false면 오른쪽
    [SerializeField] private bool _hasTitle = true; // true면 ObjectInfoSO의 title 사용, false면 ObjectInfoSO의 description만 사용
    [SerializeField] private ObjectInfoSO _objectInfo;
    [SerializeField] private GameObject _toolTip;
    [SerializeField] private Vector2 _offset = new Vector2(0f, -100f); // 커서와 툴팁 사이의 간격

    private Canvas _toolTipCanvas;
    private RectTransform _toolTipRectTransform;

    private void Awake()
    {
        _toolTipCanvas = GetComponentInParent<Canvas>();
        _toolTipRectTransform = _toolTipCanvas.GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (_toolTip.activeSelf)
        {
            if(_isLeft)
                CalculateMousePositionLeft();
            else
                CalculateMousePositionRight();
        }
    }

    public void ShowToolTip()
    {
        SetToolTipText();
        _toolTip.SetActive(true);
    }

    public void HideToolTip()
    {
        _toolTip.SetActive(false);
    }

    private void SetToolTipText()
    {
        if (_hasTitle)
        {
            TextMeshProUGUI name = _toolTip.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI description = _toolTip.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            name.text = _objectInfo.objectName;
            description.text = _objectInfo.objectDescription;
        }
        else
        {
            TextMeshProUGUI description = _toolTip.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            description.text = _objectInfo.objectDescription;
        }
    }

    /// <summary>
    /// 마우스 커서 위치에 따라 툴팁 위치를 왼쪽으로 조정하는 함수
    /// </summary>
    private void CalculateMousePositionLeft()
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
    /// 마우스 커서 위치에 따라 툴팁 위치를 오른쪽으로 조정하는 함수
    /// </summary>
    private void CalculateMousePositionRight()
    {
        // 캔버스 좌표로 변환
        Vector2 mousePos = Input.mousePosition;
        Vector2 localPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _toolTipRectTransform, mousePos, _toolTipCanvas.worldCamera, out localPos);

        // 툴팁 RectTransform
        RectTransform rt = _toolTip.transform as RectTransform;

        // 피벗을 (0,0.5)로 설정 (왼쪽 중앙)
        rt.pivot = new Vector2(0f, 0.5f);

        // 위치 지정
        rt.anchoredPosition = localPos + _offset;
    }
}
