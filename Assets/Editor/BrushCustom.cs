using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabBrush))]
public class MyScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PrefabBrush myScript = (PrefabBrush)target;
        // 기본 인스펙터 표시

        // 추가 버튼 생성
        if (GUILayout.Button("바닥 버튼"))
        {
            myScript.SetOffset(Vector3Int.zero, Vector3.zero);
            myScript.SetOrientation(Vector3Int.zero,quaternion.identity);
            myScript.SetGameObject(Vector3Int.zero,myScript.obj);
        }
        if (GUILayout.Button("벽 버튼"))
        {
            myScript.SetOffset(Vector3Int.zero, Vector3.zero);
            myScript.SetOrientation(Vector3Int.zero,quaternion.identity);
            myScript.SetGameObject(Vector3Int.zero,myScript.obj1);
        }
        
        if (GUILayout.Button("벽 버튼"))
        {
            myScript.SetOffset(Vector3Int.zero, Vector3.zero);
            myScript.SetOrientation(Vector3Int.zero,quaternion.identity);
            myScript.SetGameObject(Vector3Int.zero,myScript.obj2);
        }
        if (GUILayout.Button("검정 버튼"))
        {
            myScript.SetOffset(Vector3Int.zero, Vector3.zero);
            myScript.SetOrientation(Vector3Int.zero,quaternion.identity);
            myScript.SetGameObject(Vector3Int.zero,myScript.obj3);
        }
        if (GUILayout.Button("지붕 버튼"))
        {
            myScript.SetOffset(Vector3Int.zero, Vector3.zero);
            myScript.SetOrientation(Vector3Int.zero,quaternion.identity);
            myScript.SetGameObject(Vector3Int.zero,myScript.obj4);
        }
        if (GUILayout.Button("적 버튼"))
        {
            myScript.SetOffset(Vector3Int.zero, Vector3.zero);
            myScript.SetOrientation(Vector3Int.zero,quaternion.identity);
            myScript.SetGameObject(Vector3Int.zero,myScript.obj5);
        }
        if (GUILayout.Button("상자 버튼"))
        {
            myScript.SetOffset(Vector3Int.zero, Vector3.zero);
            myScript.SetOrientation(Vector3Int.zero,quaternion.identity);
            myScript.SetGameObject(Vector3Int.zero,myScript.obj6);
        }
        if (GUILayout.Button("배럴 버튼"))
        {
            myScript.SetOffset(Vector3Int.zero, Vector3.zero);
            myScript.SetOrientation(Vector3Int.zero,quaternion.identity);
            myScript.SetGameObject(Vector3Int.zero,myScript.obj7);
        }
        if (GUILayout.Button("젤리 버튼"))
        {
            myScript.SetOffset(Vector3Int.zero, Vector3.zero);
            myScript.SetOrientation(Vector3Int.zero,quaternion.identity);
            myScript.SetGameObject(Vector3Int.zero,myScript.obj8);
        }
        if (GUILayout.Button("10 버튼"))
        {
            myScript.SetOffset(Vector3Int.zero, Vector3.zero);
            myScript.SetOrientation(Vector3Int.zero,quaternion.identity);
            myScript.SetGameObject(Vector3Int.zero,myScript.obj9);
        }
        // 변경 사항 적용
        serializedObject.ApplyModifiedProperties();
            
        DrawDefaultInspector();
    }
}