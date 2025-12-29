using UnityEngine;

public class ItemMap : BackPackItem
{
    public override void UseItem()
    {
        base.UseItem();
        UIManager.Instance.OpenUI3D(UIManager.Instance.mapUI);
    }
}
