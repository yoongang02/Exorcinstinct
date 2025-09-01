using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
    }
}
