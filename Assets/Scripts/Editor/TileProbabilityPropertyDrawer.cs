using Structs;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(TileProbability))]
public class TileProbabilityPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        {
            float xSpace = 5f;
            float tileRectWidth = position.width * 0.8f;
            float probabilityRectWidth = position.width * 0.2f - xSpace;

            Rect tileRect = new Rect(position.x, position.y, tileRectWidth, position.height);
            Rect probabilityRect = new Rect(position.x + tileRectWidth + xSpace, position.y, probabilityRectWidth, position.height);
            EditorGUI.PropertyField(tileRect, property.FindPropertyRelative("tile"), GUIContent.none);
            EditorGUI.PropertyField(probabilityRect, property.FindPropertyRelative("highProbability"), GUIContent.none);
        }
        EditorGUI.EndProperty();
    }
}
