#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class MeshDebug
{
    public static void PrintMeshInfo(GameObject go)
    {
        Mesh m = null;
        var smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
        var mf = go.GetComponentInChildren<MeshFilter>();
        if (smr) m = smr.sharedMesh;
        if (!m && mf) m = mf.sharedMesh;

        if (m == null) { Debug.LogWarning("Mesh not found"); return; }

#if UNITY_EDITOR
        var path = AssetDatabase.GetAssetPath(m);
        Debug.Log($"Mesh='{m.name}', isReadable={m.isReadable}, assetPath='{path}'", go);
#else
        Debug.Log($"Mesh='{m.name}', isReadable={m.isReadable}", go);
#endif
    }
}