using UnityEngine;

[CreateAssetMenu(fileName = "Cause_NNN", menuName = "Scriptable Object/Cause Data")]
public class CauseSO : ScriptableObject
{
    public string id = "Cause_NNN";
    public string label;
    public bool enabled = true;
    public int weight = 1;
}
