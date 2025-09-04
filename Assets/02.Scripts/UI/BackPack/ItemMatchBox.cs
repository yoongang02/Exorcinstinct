using UnityEngine;

public class ItemMatchBox : BackPackItem
{
    public override void UseItem()
    {
        base.UseItem();
        if (RoundManager.Instance.isMatchUsed)
        {
            return;
        }
        UIManager.Instance.CloseTopUI();
        UIManager.Instance.OpenUI(UIManager.Instance.matchItemUI);
    }
}
