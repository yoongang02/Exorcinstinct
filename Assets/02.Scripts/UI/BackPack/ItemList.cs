using UnityEngine;

public class ItemList : BackPackItem
{
    public override void UseItem()
    {
        base.UseItem();
        UIManager.Instance.OpenUI3D(UIManager.Instance.studentListUI);
    }
}
