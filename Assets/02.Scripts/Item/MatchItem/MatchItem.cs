using UnityEngine;

public class MatchItem : UIBase
{
    [SerializeField] private GameObject _askWindow;
    public override void OnOpen()
    {
        base.OnOpen();
        CameraController.Instance.LockCamera();
        CameraController.Instance.SetCursorFree();
        _askWindow.SetActive(true);
    }

    public override void OnClose()
    { 
        base.OnClose();
        CameraController.Instance.UnLockCamera();
        _askWindow.SetActive(false);
    }

    public void OnClickYesBtn()
    {
        RoundManager.Instance.LightCandle();
    }

    public void OnClickNoBtn()
    {
        UIManager.Instance.CloseTopUI();
    }
}
