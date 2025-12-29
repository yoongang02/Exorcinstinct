using UnityEngine;

public class UIBase : MonoBehaviour
{
    private void Update()
    {
        if (IsTopUI())
        {
            HandleKeyboardInput();
            HandleMouseInput();
        }
    }
    public virtual void OnOpen()
    {
        Debug.Log($"#{gameObject.name}이(가) 열렸습니다.");
    }

    public virtual void OnClose()
    {
        Debug.Log($"#{gameObject.name}이(가) 닫혔습니다.");
    }

    public virtual void HandleKeyboardInput() { }
    public virtual void HandleMouseInput() { }

    public bool IsTopUI()
    {
        return UIManager.Instance.GetTopUI() == this;
    }

    public void PlayClickSound()
    {
        SoundManager.Instance.PlaySFX("Click");
    }

    public void PlayHoverSound()
    {
        SoundManager.Instance.PlaySFX("Hover");
    }
}
