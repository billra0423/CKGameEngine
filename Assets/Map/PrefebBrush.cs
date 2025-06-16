using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "Prefab brush", menuName = "Brushes/Prefab brush")]
[CustomGridBrush(false, true, false, "Prefab Brush")]
public class PrefabBrush : GameObjectBrush
{
}