using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public static TimelineController Instance { get; private set; }

    public PlayableDirector roundStart;
    public PlayableDirector roundFail;
    public PlayableDirector roundSuccess;

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

    
    // Timeline 재생
    public void PlayTimeline(PlayableDirector director)
    {
        if (director != null)
        {
            director.Play();
        }
    }

    // Timeline 일시정지
    public void PauseTimeline(PlayableDirector director)
    {
        if (director != null)
        {
            director.Pause();
        }
    }

    // Timeline 정지 후 처음으로
    public void StopTimeline(PlayableDirector director)
    {
        if (director != null)
        {
            director.Stop();
        }
    }
}
