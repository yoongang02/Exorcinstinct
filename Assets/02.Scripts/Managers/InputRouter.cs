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

    public bool PressedEnter { get; private set; }
    private bool ConsumedEnter;

    public bool PressedV {  get; private set; }
    private bool ConsumedV;

    public bool PressedA { get; private set; }
    private bool ConsumedA;

    public bool PressedD { get; private set; }
    private bool ConsumedD;

    public bool PressedESC { get; private set; }
    private bool ConsumedESC;

    void Update()
    {
        PressedE = Input.GetKeyDown(KeyCode.E);
        ConsumedE = false;

        PressedBackSpace = Input.GetKeyDown(KeyCode.Backspace);
        ConsumedBackSpace = false;

        PressedEnter = Input.GetKeyDown(KeyCode.Return);
        ConsumedEnter = false;

        PressedV = Input.GetKeyDown(KeyCode.V);
        ConsumedV = false;

        PressedA = Input.GetKeyDown(KeyCode.A);
        ConsumedA = false;

        PressedD = Input.GetKeyDown(KeyCode.D);
        ConsumedD = false;

        PressedESC = Input.GetKeyDown(KeyCode.Escape);
        ConsumedESC = false;
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

    public bool ConsumeEnter()
    {
        if (PressedEnter && !ConsumedEnter)
        {
            ConsumedEnter = true;
            return true;
        }
        return false;
    }

    public bool ConsumeV()
    {
        if(PressedV && !ConsumedV)
        {
            ConsumedV=true;
            return true;
        }

        return false;
    }

    public bool ConsumeA()
    {
        if(PressedA && !ConsumedA)
        {
            ConsumedA = true;
            return true;
        }
        return false;
    }

    public bool ConsumeD()
    {
        if (PressedD && !ConsumedD)
        {
            ConsumedD = true;
            return true;
        }
        return false;
    }

    public bool ConsumeESC()
    {
        if(PressedESC && !ConsumedESC)
        {
            ConsumedESC = true;
            return true;
        }
        return false;
    }
}
