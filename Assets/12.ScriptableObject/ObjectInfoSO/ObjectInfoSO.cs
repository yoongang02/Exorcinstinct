using UnityEngine;

[CreateAssetMenu(fileName = "ObjectInfo", menuName = "Scriptable Object/ObjectInfo Data")]
public class ObjectInfoSO : ScriptableObject
{
    public string objectName;
    public string objectDescription;
}
