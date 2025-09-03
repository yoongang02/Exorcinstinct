using UnityEngine;

public class ItemRing : BackPackItem
{
    public override void UseItem()
    {
        base.UseItem();
        if (UIManager.Instance.topUI != null) UIManager.Instance.CloseTopUI();
        UIManager.Instance.OpenUI(UIManager.Instance.ringItemUI);
    }
}
