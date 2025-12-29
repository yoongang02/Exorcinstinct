using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }
    public CanvasGroup _fadeImg;
    float fadeDuration = 1.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _fadeImg = transform.GetComponentInChildren<CanvasGroup>();
        _fadeImg.DOFade(0, fadeDuration)
            .OnComplete(() =>
            {
                _fadeImg.blocksRaycasts = false;
            });
    }

    public async UniTaskVoid ChangeScene(string sceneName)
    {
        _fadeImg = transform.GetComponentInChildren<CanvasGroup>();
        await _fadeImg.DOFade(1, fadeDuration)
            .OnStart(() => {
                _fadeImg.blocksRaycasts = true;
                //SoundManager.Instance.StopBGM();
            })
            .AsyncWaitForCompletion();

        await LoadScene(sceneName);
    }

    private async UniTask LoadScene(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        while (!async.isDone)
        {
            await UniTask.Yield();
        }
    }

    public async UniTask FadeIn()
    {
        _fadeImg = transform.GetComponentInChildren<CanvasGroup>();
        _fadeImg.alpha = 1; // 시작은 가려진 상태
        _fadeImg.blocksRaycasts = true;

        await _fadeImg.DOFade(0, fadeDuration)
            .OnComplete(() => { _fadeImg.blocksRaycasts = false; })
            .AsyncWaitForCompletion();
    }

    public async UniTask FadeOut()
    {
        _fadeImg = transform.GetComponentInChildren<CanvasGroup>();
        _fadeImg.alpha = 0; // 시작은 보이는 상태
        _fadeImg.blocksRaycasts = false;

        await _fadeImg.DOFade(1, fadeDuration)
            .OnStart(() => { _fadeImg.blocksRaycasts = true; })
            .AsyncWaitForCompletion();
    }

    public void DoFadeIn()
    {
        FadeIn().Forget();
    }

    public void DoFadeOut()
    {
        FadeOut().Forget();
    }

    public void MoveToSuccess()
    {
        ChangeScene("SuccessEnding").Forget();
    }
}
