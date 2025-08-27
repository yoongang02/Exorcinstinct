using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Location_NNN", menuName = "Scriptable Object/Location Data")]
public class LocationSO : ScriptableObject
{
    public string id = "Location_NNN";
    public string floor;
    public string direction;
    public string label;
    public List<CauseSO> causes;
    public bool enabled = true;
    public float weight = 1;
}
