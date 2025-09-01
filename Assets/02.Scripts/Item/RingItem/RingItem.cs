using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct DummyComposition
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}

public class RingItem : UIBase, IItemBase
{
    [Header("UI Variable")]
    [Space(5)]
    [SerializeField] private GameObject _askWindow;
    [SerializeField] private Button _escapeBtn;

    [Header("Content Variable")]
    [Space(5)]
    [SerializeField] private GameObject _ringHand;
    [SerializeField] private GhostDummy _ghostDummy;
    [SerializeField] List<DummyComposition> _compositionList = new List<DummyComposition>();

    public override void OnOpen()
    {
        base.OnOpen();

        _escapeBtn.gameObject.SetActive(false);

        // 옥반지 아이템 사용 관련하여 캔버스 및 카메라 설정

        // 정답에 따라 더미 설정
        _ghostDummy.SetDummy();

        // 랜덤 구도 설정
        SetRandomComposition();

        // 아이템 사용할지 물어보는 창 띄우기
        _askWindow.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        HideDummy();
        _askWindow.SetActive(false);
        _escapeBtn.gameObject.SetActive(false);
    }

    public void UseItem()
    {
        Debug.Log($"{this.name} 아이템 사용");
    }

    public void OnClickYesBtn()
    {
        _askWindow.SetActive(false);
        ShowDummy();
        _escapeBtn.gameObject.SetActive(true);
    }

    public void OnClickNoBtn()
    {
        UIManager.Instance.CloseTopUI();
    }

    public void OnClickEscapeBtn() {
        UIManager.Instance.CloseTopUI();
    }

    private void ShowDummy()
    {
        _ringHand.SetActive(true);
        _ghostDummy.gameObject.SetActive(true);
    }
    private void HideDummy()
    {
        _ringHand.SetActive(false);
        _ghostDummy.gameObject.SetActive(false);
    }

    private void SetRandomComposition()
    {
        int randValue = Random.Range(0, _compositionList.Count);
        DummyComposition dummyComposition = _compositionList[randValue];

        Transform dummyT = _ghostDummy.transform;
        dummyT.localPosition = dummyComposition.position;
        dummyT.localRotation = Quaternion.Euler(dummyComposition.rotation);
        dummyT.localScale = dummyComposition.scale;
    }

    public void Test()
    {
        SetRandomComposition();
    }
}
