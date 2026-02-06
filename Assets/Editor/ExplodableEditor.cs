#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Explodable))]
public class ExplodableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Explodable e = (Explodable)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Fragments (Editor)"))
        {
            e.fragmentInEditor();
        }

        if (GUILayout.Button("Delete Fragments"))
        {
            e.deleteFragments();
        }
    }
}
#endif
