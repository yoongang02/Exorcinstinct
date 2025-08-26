using UnityEngine;

public class InputRouter : MonoBehaviour
{
    public static InputRouter Instance { get; private set; }

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

    public bool PressedE { get; private set; }
    private bool ConsumedE;

    public bool PressedBackSpace { get; private set; }
    private bool ConsumedBackSpace;

    void Update()
    {
        PressedE = Input.GetKeyDown(KeyCode.E);
        ConsumedE = false;

        PressedBackSpace = Input.GetKeyDown(KeyCode.Backspace);
        ConsumedBackSpace = false;
    }

    public bool ConsumeE()
    {
        if (PressedE && !ConsumedE)
        {
            ConsumedE = true;
            return true;
        }
        return false;
    }

    public bool ConsumeBackSpace()
    {
        if (PressedBackSpace && !ConsumedBackSpace)
        {
            ConsumedBackSpace = true;
            return true;
        }
        return false;
    }
}
