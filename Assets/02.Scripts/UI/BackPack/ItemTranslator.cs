using UnityEngine;

public class ItemTranslator : BackPackItem
{
    public override void UseItem()
    {
        base.UseItem();
        if (RoundManager.Instance.isTransUsed)
        {
            return;
        }
        UIManager.Instance.CloseTopUI();
        UIManager.Instance.OpenUI(UIManager.Instance.translatorUI);
    }
}
