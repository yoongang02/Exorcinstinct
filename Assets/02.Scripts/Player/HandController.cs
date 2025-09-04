using Samples.Whisper;
using UnityEngine;

public class HandController : MonoBehaviour
{
    private Animator _animator;
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        WhisperManager.Instance.WhenAnswerO += MoveToO;
        WhisperManager.Instance.WhenAnswerX += MoveToX;
        WhisperManager.Instance.WhenAnswerError += MoveError;
        WhisperManager.Instance.WhenStartAsk += InitHandPos;
    }

    private void MoveToO()
    {
        Debug.Log("손 O로 이동");
        _animator.SetTrigger("MoveToO");

        //SFX
        SoundManager.Instance.PlaySFX("OX");
    }

    private void MoveToX()
    {
        Debug.Log("손 X로 이동");
        _animator.SetTrigger("MoveToX");

        //SFX
        SoundManager.Instance.PlaySFX("OX");
    }

    private void MoveError()
    {
        Debug.Log("손 에러");
        _animator.SetTrigger("Error");

        //SFX
        SoundManager.Instance.PlaySFX("Error");
    }

    /// <summary>
    /// 손 위치 초기화할 때 호출하는 함수
    /// </summary>
    private void InitHandPos()
    {
        if(!_animator.GetCurrentAnimatorStateInfo(0).IsName("Default State"))
        {
            Debug.Log("손 위치 초기화");
            _animator.SetTrigger("ComeBack");
        }
    }

    /// <summary>
    /// 성공적으로 질문을 끝냈을 때, 즉 대답이 O 또는 X로 나오면
    /// 촛불 불이 꺼지고, 질문 횟수 감소
    /// </summary>
    public void WhenQuestionSuccess()
    {
        Debug.Log("성공적으로 질문이 끝나면");
        RoundManager.Instance.BlowOutCandle();
    }


    /// <summary>
    /// 대답이 판단 불가로 나온 경우, 오류 메세지 출력
    /// </summary>
    public void WhenQuestionFail()
    {
        WhisperManager.Instance.EndResponse();
    }

    private void OnDisable()
    {
        WhisperManager.Instance.WhenAnswerO -= MoveToO;
        WhisperManager.Instance.WhenAnswerX -= MoveToX;
        WhisperManager.Instance.WhenAnswerError -= MoveError;
        WhisperManager.Instance.WhenStartAsk -= InitHandPos;
    }
}
