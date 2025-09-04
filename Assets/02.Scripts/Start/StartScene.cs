using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayBGM("BGM1");
    }
    public void OnClickStartBtn()
    {
        SoundManager.Instance.PlaySFX("Click");
        SceneChanger.Instance.ChangeScene("MikeCheckScene").Forget();
    }

    public void OnClickQuitBtn()
    {
        SoundManager.Instance.PlaySFX("Click");
        Application.Quit();
    }
}
