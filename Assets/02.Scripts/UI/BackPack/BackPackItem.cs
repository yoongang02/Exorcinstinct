using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackPackItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private ObjectInfoSO _objectInfo;
    [SerializeField] private GameObject _infoWindowPrefab;
    private Outline _outline;
    private GameObject _infoWindowInstance;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(gameObject.name + " 호버");
        // 호버 시 OutLine 활성화
        _outline.enabled = true;

        // 마우스 커서 왼쪽 하단에 오브젝트 정보창 표시
        SetInfoText();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    private void SetInfoText()
    {
        // 오브젝트 정보창에 오브젝트 정보 세팅
        TextMeshProUGUI name = _infoWindowPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI description = _infoWindowPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        name.text = _objectInfo.objectName;
        description.text = _objectInfo.objectDescription;
    }
}
