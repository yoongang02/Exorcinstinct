using Samples.Whisper;
using UnityEngine;

public class TranslatorItemUI : UIBase
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
        WhisperManager.Instance.SetTranslator(true);
        UIManager.Instance.CloseTopUI();
    }

    public void OnClickNoBtn()
    {
        WhisperManager.Instance.SetTranslator(false);
        UIManager.Instance.CloseTopUI();
    }
}
