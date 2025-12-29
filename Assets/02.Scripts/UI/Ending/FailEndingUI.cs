using UnityEngine;

public class FailEndingUI : UIBase
{
    public override void OnOpen()
    {
        base.OnOpen();
        RoundManager.Instance.AllCandleLightOn();
        CameraController.Instance.LockCamera();
        CameraController.Instance.SetCursorFree();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public void OnClickMainBtn()
    {
        UIManager.Instance.CloseTopUI();
        SceneChanger.Instance.ChangeScene("StartScene").Forget();
        SoundManager.Instance.StopBGM();
    }

    public void OnClickExitBtn()
    {
        Application.Quit();
    }
}
