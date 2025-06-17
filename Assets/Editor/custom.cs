using UnityEngine;
using UnityEditor;
public class brushEditor : EditorWindow
{
    public PrefabBrush obj;
    private static brushEditor window;
    [MenuItem("Window/CustomWindow")]
    private static void Setup()
    {
        window = GetWindow<brushEditor>();
        window.titleContent = new GUIContent("CustomBrush");
    }
    private void OnGUI()
    {
        //obj = EditorGUI.TextField(new Rect(0, 160, 300, 20), "EditorGUI : ", obj);
    }
}