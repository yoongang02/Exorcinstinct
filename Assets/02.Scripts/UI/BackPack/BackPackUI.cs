using UnityEngine;

public class BackPackUI : UIBase
{
    public override void OnOpen()
    {
        base.OnOpen();

        // 카메라 고정 및 커서 고정 해제
        CameraController.Instance.LockCamera();
        CameraController.Instance.SetCursorFree();

        // SFX
        SoundManager.Instance.PlaySFX("OpenBackpack");

        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();

        // 카메라 고정 해제
        if(!UIManager.Instance.IsAnyUIOpen())
            CameraController.Instance.UnLockCamera();

        // SFX
        SoundManager.Instance.PlaySFX("CloseBackpack");

        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();

        if (InputRouter.Instance.ConsumeA())
        {
            UIManager.Instance.CloseTopUI();
        }

    }
}
