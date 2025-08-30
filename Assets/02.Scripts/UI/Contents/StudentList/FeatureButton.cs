using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;

public class FeatureButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private bool _isDefault = false;
    [SerializeField] private bool _isNone = false;
    [SerializeField] private FeatureSO _featureSO;
    [SerializeField] private GameObject _featureObject;
    [SerializeField] private List<GameObject> _otherFeatures = new List<GameObject>();

    [Header("Default UI Variable")]
    [Space(5)]
    [SerializeField] private float _alphaValue = 65f;
    [SerializeField] private Image _outline;
    [SerializeField] private Image _hoverBackground;
    [SerializeField] private TextMeshProUGUI _featureLabelText;

    [Header("Freckles Variable")]
    [Space(5)]
    [SerializeField] private bool _isFreckles = false;
    [SerializeField] private MeshRenderer _faceMeshRenderer;
    [SerializeField] private List<Material> _faceMaterials = new List<Material>(); // 0 : 디폴트 상태 , 1: 주근깨 상태

    void Awake()
    {
        if (_featureSO == null)
        {
            _featureLabelText.text = "없음";
        }
        else
        {
            _featureLabelText.text = _featureSO.label;
        }

        if (_isDefault)
        {
            Select();
        }
        else
        {
            Unselect();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeOpacity(_alphaValue);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_outline.enabled) ChangeOpacity(0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }

    private void ChangeOpacity(float value)
    {
        float newValue = value / 255f;
        Color color = _hoverBackground.color;
        color.a = newValue;
        _hoverBackground.color = color;
    }

    private void ActiveFeature()
    {
        if (_featureObject != null)
        {
            _featureObject.SetActive(true);
            return;
        }

        if (_isFreckles)
        {
            if (_isNone)
            {
                _faceMeshRenderer.material = _faceMaterials[0];
            }
            else
            {
                _faceMeshRenderer.material = _faceMaterials[1];
            }
        }
    }

    private void ClearFeature()
    {
        if (_featureObject != null)
        {
            _featureObject.SetActive(false);
            return;
        }

        if (_isFreckles)
        {
            _faceMeshRenderer.material = _faceMaterials[0];
        }
    }

    public void Unselect()
    {
        _outline.enabled = false;
        ChangeOpacity(0);
        ClearFeature();
    }

    private void Select()
    {
        _outline.enabled = true;
        ChangeOpacity(_alphaValue);
        DecideResult();
    }

    private void DecideResult()
    {
        foreach (var feature in _otherFeatures)
        {
            if (feature == null) continue;
            feature.GetComponent<FeatureButton>().Unselect();
        }
        ActiveFeature();
    }
}
