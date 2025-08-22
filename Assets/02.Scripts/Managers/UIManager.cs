using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {  get; private set; }
    public UIBase topUI;

    private Stack<UIBase> _uiStack = new Stack<UIBase>();


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

    public void OpenUI(UIBase ui)
    {
        if (ui == null) return;

        _uiStack.Push(ui);
        topUI = ui;
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

}
