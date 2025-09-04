using UnityEngine;

public class UISound : MonoBehaviour
{
    public void PlayClickSound()
    {
        SoundManager.Instance.PlaySFX("Click");
    }

    public void PlayHoverSound()
    {
        SoundManager.Instance.PlaySFX("Hover");
    }
}
