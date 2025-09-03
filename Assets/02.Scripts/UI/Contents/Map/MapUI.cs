using UnityEngine;

public class MapUI : UIBase
{
    public override void OnOpen()
    {
        base.OnOpen();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
    }
    public void OnClickEscapeBtn()
    {
        UIManager.Instance.CloseTopUI3D();
    }
}
