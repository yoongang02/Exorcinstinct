using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CurrentAmulet : MonoBehaviour
{
    [SerializeField] private float _printDelay = 0.05f;

    [Header("학생 정보")]
    [Space(5)]
    [SerializeField] private Image _portraitImage;
    [SerializeField] private Image _maskImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private List<Sprite> _maskSprite = new List<Sprite>(); // 0: 디폴트 , 1: empty

    [Header("장소 정보")]
    [Space(5)]
    [SerializeField] private TextMeshProUGUI _locationText;

    [Header("사망 원인 정보")]
    [Space(5)]
    [SerializeField] private TextMeshProUGUI _causeText;

    private void Awake()
    {
        ClearAmulet();
    }

    private void Start()
    {
        RoundManager.Instance.OnSetStudent += SetStudent;
        RoundManager.Instance.OnSetLocation += SetLocation;
        RoundManager.Instance.OnSetCause += SetCause;
        RoundManager.Instance.OnClearAmulet += ClearAmulet;
    }

    private void SetStudent(StudentSO student)
    {
        StartCoroutine(PrintOneByOne(student.label, _nameText, _printDelay));
        _maskImage.sprite = _maskSprite[0];
        _portraitImage.sprite = Resources.Load<Sprite>(student.sprite);
    }

    private void SetLocation(LocationSO location)
    {
        string str = $"{location.floor} {location.wing} {location.label}";
        StartCoroutine(PrintOneByOne(str, _locationText, _printDelay));
    }

    private void SetCause(CauseSO cause)
    {
        StartCoroutine(PrintOneByOne(cause.label, _causeText, _printDelay));
    }

    private void ClearAmulet()
    {
        _nameText.text = "";
        _maskImage.sprite = _maskSprite[1];
        _locationText.text = "";
        _causeText.text = "";
    }

    private void OnDisable()
    {
        RoundManager.Instance.OnSetStudent -= SetStudent;
        RoundManager.Instance.OnSetLocation -= SetLocation;
        RoundManager.Instance.OnSetCause -= SetCause;
        RoundManager.Instance.OnClearAmulet -= ClearAmulet;
    }

    IEnumerator PrintOneByOne(string message, TextMeshProUGUI textComponent, float delay)
    {
        //SFX
        SoundManager.Instance.PlaySFX("WriteAmulet");

        textComponent.text = "";
        foreach (var c in message)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(delay);
        }
    }
}
