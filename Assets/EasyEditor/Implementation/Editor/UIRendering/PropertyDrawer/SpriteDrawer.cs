using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UEObject = UnityEngine.Object;

namespace EasyEditor
{
    /// <summary>
    /// Sprite Drawer. Allows to display a sprite as a thumbnail in the editor (Only from Unity 5).
    /// </summary>
    [CustomPropertyDrawer(typeof(SpriteAttribute))]
    public class SpriteDrawer : PropertyDrawer 
    {
    	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    	{
            if(fieldInfo.FieldType.IsAssignableFrom(typeof(Sprite)))
            {
                property.objectReferenceValue = EditorGUILayout.ObjectField(property.name, property.objectReferenceValue, typeof(Sprite), true);
            }
            else
            {
                EditorGUILayout.HelpBox("Only field of type Sprite can have the attribute [Sprite].", UnityEditor.MessageType.Error);
            }
    	}
    }
}