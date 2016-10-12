using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UEObject = UnityEngine.Object;

namespace EasyEditor
{
    /// <summary>
    /// Texture Drawer. Allows to display a texture or a sprite as a thumbnail in the editor.
    /// </summary>
    [CustomPropertyDrawer(typeof(TextureAttribute))]
    public class TextureDrawer : PropertyDrawer 
    {
    	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    	{
            if(fieldInfo.FieldType.IsAssignableFrom(typeof(Texture2D)))
            {
                property.objectReferenceValue = EditorGUILayout.ObjectField(property.name, property.objectReferenceValue, typeof(Texture), true);
            }
            else
            {
                EditorGUILayout.HelpBox("Only texture can have the attribute [Texture].", UnityEditor.MessageType.Error);
            }
    	}
    }
}