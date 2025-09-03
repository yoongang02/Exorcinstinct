using UnityEngine;

public class ItemTranslator : BackPackItem
{
    public override void UseItem()
    {
        base.UseItem();
        UIManager.Instance.CloseTopUI();
        UIManager.Instance.OpenUI(UIManager.Instance.translatorUI);
    }
}
