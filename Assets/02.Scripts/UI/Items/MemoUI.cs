using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class MemoUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _leftTextField;
    [SerializeField] private TMP_InputField _rightTextField;
    [SerializeField] private Button _leftPageButton;
    [SerializeField] private Button _rightPageButton;

    [Header("MemoNote Icon UI")]
    [Space(5)]
    [SerializeField] private Image _memoNoteIcon;
    [SerializeField] private List<Sprite> _memoNoteSprites = new List<Sprite>(); // 0 : 닫힌 메모장, 1: 열린 메모장

    private List<string> _memoPages = new List<string>();
    private int _currentPageIndex = 0;
    private bool _isOpen = false;

    public void OnOpen()
    {
        _isOpen = true;

        // 카메라 고정 및 커서 고정 해제
        CameraController.Instance.LockCamera();
        CameraController.Instance.SetCursorFree();


        // 메모장 아이콘 변경
        _memoNoteIcon.sprite = _memoNoteSprites[1];

        // 닫기 전 보고 있는 페이지로 세팅
        UpdateState();

        // 메모장 열리는 효과
        transform.GetChild(0).gameObject.SetActive(true);

        _leftTextField.onSubmit.AddListener(_ =>
        {
            SetFocus(_leftTextField);
        });

        _rightTextField.onSubmit.AddListener(_ =>
        {
            SetFocus(_rightTextField);
        });

        // 왼쪽 페이지 포커스 할당
        FocusAtEnd();
    }

    public void OnClose()
    {
        _isOpen = false;

        // 카메라 고정 해제
        if(!UIManager.Instance.IsAnyUIOpen())
            CameraController.Instance.UnLockCamera();

        // 메모장 아이콘 변경
        _memoNoteIcon.sprite = _memoNoteSprites[0];

        // 현재 페이지들 내용 저장
        SavePageContent();

        // 메모장 닫히는 효과
        transform.GetChild(0).gameObject.SetActive(false);

        _leftTextField.onSubmit.RemoveListener(_ =>
        {
            SetFocus(_leftTextField);
        });

        _rightTextField.onSubmit.RemoveListener(_ =>
        {
            SetFocus(_rightTextField);
        });
    }

    void SetFocus(TMP_InputField inputField)
    {
        // 포커스 유지
        inputField.Select();
        inputField.ActivateInputField();
        // 필요하면 커서를 끝으로
        int end = inputField.text.Length;
        inputField.caretPosition = inputField.stringPosition = end;
    }

    public void HandleKeyboardInput()
    {
        // 메모 작성 중이면, 메모장 닫기 무시
        if (_leftTextField.isFocused || _rightTextField.isFocused) return;

        if (InputRouter.Instance.ConsumeE())
        {
            this.OnClose();
        }
    }

    public void ClickLeftPageBtn()
    {
        // 사운드 연결

        // 현재 페이지 내용 저장
        SavePageContent();

        // 인덱스 감소
        _currentPageIndex -= 2;

        // UI 업데이트
        UpdateState();
    }

    public void ClickRightPageBtn()
    {
        // 사운드 연결

        // 현재 페이지 내용 저장
        SavePageContent();

        // 인덱스 증가
        _currentPageIndex += 2;

        // UI 업데이트
        UpdateState();
    }

    // 현재 페이지들 마지막 글자로 커서 및 포커스 이동하는 함수
    private void FocusAtEnd()
    {
        if(_rightTextField.text.Length > 0)
        {
            _rightTextField.Select();
            _rightTextField.ActivateInputField();
            _rightTextField.caretPosition = _rightTextField.text.Length;
        }
        else
        {
            _leftTextField.Select();
            _leftTextField.ActivateInputField();
            _leftTextField.caretPosition = _leftTextField.text.Length;
        }
    }

    // 메모 내용, 버튼 등 상태 업데이트
    private void UpdateState()
    {
        // 현재 페이지 인덱스가 0이면 왼쪽 페이지 버튼 비활성화
        if(_currentPageIndex == 0)
        {
            SetBtnState(_leftPageButton, false);
        }
        else
        {
            SetBtnState(_leftPageButton, true);
        }

        SetPageState();
        FocusAtEnd();
    }

    private void SetBtnState(Button btn, bool state)
    {
        btn.interactable = state;
    }

    private void SetPageState()
    {
        //Debug.Log($"현재 메모 개수 : {_memoPages.Count} , 현재 인덱스 : {_currentPageIndex}");
        if (_memoPages.Count < _currentPageIndex + 2)
        {
            _memoPages.Add("");
            _memoPages.Add("");
        }

        // 현재 페이지 인덱스에 맞게 좌우 페이지 내용 세팅
        _leftTextField.text = _memoPages[_currentPageIndex];
        _rightTextField.text = _memoPages[_currentPageIndex + 1];
    }

    private void SavePageContent()
    {
        _memoPages[_currentPageIndex] = _leftTextField.text;
        _memoPages[_currentPageIndex + 1] = _rightTextField.text;
    }

    public bool IsMemoOpen()
    {
        return _isOpen;
    }
}
