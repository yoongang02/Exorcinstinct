using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Variable")]
    [Space(5)]
    [SerializeField] private TextMeshProUGUI _remainQuestionCntText;
    [SerializeField] private TextMeshProUGUI _remainGhostCntText;

    [Header("Value Variable")]
    [Space(5)]
    [SerializeField] private int _maxGhostCnt;
    [SerializeField] private int _remainGhostCnt;
    [SerializeField] private int _maxQuestionCnt;
    [SerializeField] private int _remainQuestionCnt;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _remainGhostCnt = _maxGhostCnt;
        _remainQuestionCnt = _maxQuestionCnt;
        UpdateUI();
    }

    /// <summary>
    /// 라운드가 시작하면 질문 갯수를 최대 질문 갯수로 초기화
    /// </summary>
    public void InitQuestionCnt()
    {
        _remainQuestionCnt = _maxQuestionCnt;
    }

    /// <summary>
    /// 질문 갯수 늘릴 경우 호출하는 함수
    /// 성냥 아이템에서 사용
    /// </summary>
    public void IncreaseQuestionCnt()
    {
        _remainQuestionCnt++;
        UpdateUI();
    }

    /// <summary>
    /// 질문 갯수 감소할 때 호출하는 함수
    /// 질문을 성공적으로 했을 때, 질문 갯수를 차감함.
    /// </summary>
    public void DecreaseQuestionCnt()
    {
        _remainQuestionCnt--;
        UpdateUI();
    }

    /// <summary>
    /// 질문 갯수나, 귀신 갯수에 변동이 생겼을 때 UI에 반영해주는 함수
    /// </summary>
    private void UpdateUI()
    {
        _remainQuestionCntText.text = "남은 질문 횟수 : " + _remainQuestionCnt.ToString();
        _remainGhostCntText.text = "남은 귀신 수 : " + _remainGhostCnt.ToString();
    }

    public int GetCurQuestionCnt()
    {
        return _remainQuestionCnt;
    }

    public int GetMaxQuestionCnt()
    {
        return _maxQuestionCnt;
    }

    public void CheckGameOver()
    {

    }
}
