using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    public void OnClickStartBtn()
    {
        SceneChanger.Instance.ChangeScene("MikeCheckScene").Forget();
    }

    public void OnClickQuitBtn()
    {
        Application.Quit();
    }
}
