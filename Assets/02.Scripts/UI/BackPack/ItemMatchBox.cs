using UnityEngine;

public class ItemMatchBox : BackPackItem
{
    public override void UseItem()
    {
        base.UseItem();
        UIManager.Instance.CloseTopUI();
        UIManager.Instance.OpenUI(UIManager.Instance.matchItemUI);
    }
}
