using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Style))]
public class StyleEditor : Editor
{
    private Style _myScript { get { return (Style)target; } }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(25);
        EditorGUILayout.BeginVertical("box");
        GUILayout.Space(5);

        //GUILayout.Label("Write Data");
        //GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Apply All"))
        {
            _myScript.ApplyAllStyle();
        }

        if (GUILayout.Button("Reset This"))
        {
            _myScript.ResetStyle();
        }

        if (GUILayout.Button("Apply This"))
        {
            _myScript.ApplyStyle();
        }

        

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);
        EditorGUILayout.EndVertical();
    }
}
