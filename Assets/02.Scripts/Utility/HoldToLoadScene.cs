using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HoldToLoadScene : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float holdDuration = 3f; 
    [SerializeField] private string targetSceneName;   

    [Header("UI")]
    [SerializeField] private Image progressImage;    

    private float holdTime = 0f;
    private bool _isComplete = false;

    private void Awake()
    {
        _isComplete = false;
    }

    void Update()
    {
        if (_isComplete) return;

        if (Input.GetKey(KeyCode.Space))
        {
            holdTime += Time.deltaTime;
            progressImage.fillAmount = Mathf.Clamp01(holdTime / holdDuration);

            if (progressImage.fillAmount >= 1f)
            {
                _isComplete = true;
                SceneChanger.Instance.ChangeScene(targetSceneName).Forget();
            }
        }
        else
        {
            holdTime = 0f;
            progressImage.fillAmount = 0f;
        }
    }
}
