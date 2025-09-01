using UnityEngine;

public class MatchItem : UIBase, IItemBase
{
    [SerializeField] private GameObject _askWindow;
    public override void OnOpen()
    {
        base.OnOpen();
        _askWindow.SetActive(true);
    }

    public override void OnClose()
    { 
        base.OnClose(); 
    }

    public void UseItem()
    {
        
    }

    public void OnClickYesBtn()
    {

    }

    public void OnClickNoBtn()
    {

    }
}
