using UnityEngine;

public class ItemMemo : BackPackItem
{
    public override void UseItem()
    {
        base.UseItem();
        UIManager.Instance.memoUI.OnOpen();
    }
}
