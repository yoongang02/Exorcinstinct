using System.Collections;
using UnityEngine;

public class CandleController : MonoBehaviour
{
    [SerializeField] private float _smokeSeconds = 3f;
    [SerializeField] private GameObject _flame;
    [SerializeField] private GameObject _light;
    [SerializeField] private GameObject _smoke;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void LightOn()
    {
        _smoke.SetActive(false);
        _animator.SetTrigger("LightOn");

        // SFX
        SoundManager.Instance.PlaySFX("LightOn");
    }

    public void LightOff()
    {
        _animator.SetTrigger("LightOff");

        // SFX
        SoundManager.Instance.PlaySFX("LightOff");
    }

    public void SmokeOn()
    {
        _flame.SetActive(false);
        _light.SetActive(false);
        _smoke.SetActive(true);
        //StartCoroutine(SmokeOff());
    }

    IEnumerator SmokeOff()
    {
        yield return new WaitForSeconds(_smokeSeconds);
        _smoke.SetActive(false);
    }
}
