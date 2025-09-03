using Samples.Whisper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {  get; private set; }
    public UIBase topUI;

    private Stack<UIBase> _uiStack = new Stack<UIBase>();

    [Header("UI 목록")]
    [Space(5)]
    public MemoUI memoUI;
    public UIBase backPackUI;
    public UIBase amuletUI;
    public UIBase mapUI;
    public UIBase studentListUI;

    [Header("3D 콘텐츠 UI 세팅")]
    [Space(5)]
    [SerializeField] private GameObject _canvasUI2D;
    [SerializeField] private Camera _mainCamera;
    private bool _isUI3DOpen = false;

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
    }

    private void Update()
    {
        if (InputRouter.Instance.ConsumeE())
        {
            memoUI.OnOpen();
        }

        if (memoUI.IsMemoOpen() && InputRouter.Instance.ConsumeESC())
        {
            memoUI.OnClose();
        }

        if (!IsAnyUIOpen())
        {
            if (!memoUI.IsMemoOpen() && InputRouter.Instance.ConsumeA())
            {
                OpenUI(backPackUI);
            }

            if (!memoUI.IsMemoOpen() && InputRouter.Instance.ConsumeD())
            {
                OpenUI3D(amuletUI);
            }

            // ToDo : 메모장이랑 가방이 열려있는 것은 가능. 예외처리 진행해야 함.
            // ToDo : 번역기를 사용하는 경우도 체크해서 분기 나눠야 함.
            if (InputRouter.Instance.ConsumeV() && WhisperManager.Instance.canRecord)
            {
                WhisperManager.Instance.StartRecording();
            }
        }
    }

    public void OpenUI(UIBase ui)
    {
        if (ui == null) return;

        _uiStack.Push(ui);
        topUI = ui;
        ui.OnOpen();
    }


    public void OpenUI3D(UIBase ui)
    {
        if(ui == null) return;

        if (topUI != null) CloseTopUI();

        _uiStack.Push(ui);
        topUI = ui;

        // 카메라 및 캔버스 세팅
        CameraController.Instance.LockCamera();
        CameraController.Instance.SetCursorFree();
        SetMainCameraPriority(-3);
        _canvasUI2D.SetActive(false);

        ui.OnOpen();
    }


    public void CloseTopUI()
    {
        if (_uiStack.Count == 0) return;

        UIBase topUI = _uiStack.Pop();

        topUI.OnClose();

        if (_uiStack.Count > 0)
        {
            this.topUI = _uiStack.Peek();
        }
        else
        {
            this.topUI = null;
        }
    }

    /// <summary>
    /// 3d UI를 닫을 때 호출하는 함수
    /// 캔버스 및 카메라 설정을 담당함.
    /// </summary>
    public void CloseTopUI3D()
    {
        if (_uiStack.Count == 0) return;

        UIBase topUI = _uiStack.Pop();

        // 카메라 및 캔버스 세팅
        CameraController.Instance.UnLockCamera();
        SetMainCameraPriority(-1);
        _canvasUI2D.SetActive(true);

        topUI.OnClose();

        if (_uiStack.Count > 0)
        {
            this.topUI = _uiStack.Peek();
        }
        else
        {
            this.topUI = null;
        }
    }

    public void CloseAllUI()
    {
        while (_uiStack.Count > 0)
        {
            CloseTopUI();
        }
    }

    public UIBase GetTopUI()
    {
        if (_uiStack.Count == 0) return null;
        return _uiStack.Peek();
    }

    public bool IsAnyUIOpen()
    {
        return _uiStack.Count > 0;
    }

    public bool IsUIOpen(UIBase ui)
    {
        return _uiStack.Contains(ui);
    }

    private void SetMainCameraPriority(int value)
    {
        _mainCamera.depth = value;
    }
}
