using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [Header("ToolTip Default Settings")]
    [Space(5)]
    [SerializeField] private bool _isLeft = true; // true면 왼쪽, false면 오른쪽
    [SerializeField] private bool _hasTitle = true; // true면 ObjectInfoSO의 title 사용, false면 ObjectInfoSO의 description만 사용

    [Header("ToolTip Map Settings")]
    [Space(5)]
    [SerializeField] private bool _isMapInfo = false; // true면 MapInfoSO, false면 ObjectInfoSO
    [SerializeField] private GameObject _causeSlotPrefab; // MapInfoSO의 툴팁에 원인 슬롯이 있을 때 사용할 프리팹

    [Header("ToolTip Info")]
    [Space(5)]
    [SerializeField] private ObjectInfoSO _objectInfo;

    [Header("ToolTip UI")]
    [Space(5)]
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

    public void ShowToolTip(LocationSO location)
    {
        SetToolTipText(location);
        _toolTip.SetActive(true);
    }

    public void ShowToolTip(LocationSO location, CauseSO cause)
    {
        SetToolTipText(location, cause);
        _toolTip.SetActive(true);
    }

    public void HideToolTip()
    {
        _toolTip.SetActive(false);
    }

    private void SetToolTipText()
    {
        // ObjectInfoSO의 정보를 툴팁에 설정
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

    public void SetToolTipText(LocationSO location)
    {
        // location의 label 정보를 툴팁 타이틀로 설정
        TextMeshProUGUI name = _toolTip.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        name.text = location.label;

        // location의 causes 내의 causeSO 개수만큼 툴팁에 슬롯 생성
        Transform slotParent = _toolTip.transform.GetChild(1);

        // 기존 슬롯 제거
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        // 신규 슬롯 생성
        foreach (var causeSO in location.causes)
        {
            GameObject slot = Instantiate(_causeSlotPrefab, slotParent);

            // cause 이름 설정
            TextMeshProUGUI slotText = slot.GetComponentInChildren<TextMeshProUGUI>();
            slotText.text = causeSO.label;

            // cause 아이콘 설정
            GameObject causeIcon = slot.transform.GetChild(0).gameObject;
            Image iconImage = causeIcon.GetComponent<Image>();

            // 아웃라인 비활성화
            iconImage.enabled = false;

            Image innerIcon = causeIcon.GetComponentInChildren<Image>();
            innerIcon.sprite = Resources.Load<Sprite>(causeSO.iconPath);
        }
    }

        public void SetToolTipText(LocationSO location, CauseSO cause)
    {
        // location의 label 정보를 툴팁 타이틀로 설정
        TextMeshProUGUI name = _toolTip.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        name.text = location.label;

        // location의 causes 내의 causeSO 개수만큼 툴팁에 슬롯 생성
        Transform slotParent = _toolTip.transform.GetChild(1);

        // 기존 슬롯 제거
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        // 신규 슬롯 생성
        foreach (var causeSO in location.causes)
        {
            GameObject slot = Instantiate(_causeSlotPrefab, slotParent);

            // cause 이름 설정
            TextMeshProUGUI slotText = slot.GetComponentInChildren<TextMeshProUGUI>();
            slotText.text = causeSO.label;

            // cause 아이콘 설정, 아웃라인 활성화
            GameObject causeIcon = slot.transform.GetChild(0).gameObject;
            Image iconImage = causeIcon.GetComponent<Image>();


            if (cause == causeSO)
            {
                // 아웃라인 활성화
                iconImage.enabled = true;
            }
            else
            {
                // 아웃라인 비활성화
                iconImage.enabled = false;
            }
            Image innerIcon = causeIcon.GetComponentInChildren<Image>();
            innerIcon.sprite = Resources.Load<Sprite>(causeSO.iconPath);
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
