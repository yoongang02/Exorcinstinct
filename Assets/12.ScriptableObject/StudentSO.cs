using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Student_NNN", menuName = "Scriptable Object/Student Data")]
public class StudentSO : ScriptableObject
{
    public string id = "Student_NNN";
    public string label;
    public List<FeatureSO> features;
    public bool enabled = true;
    public float weight = 1;
    public string sprite = "Portrait/Student_NNN";
}
