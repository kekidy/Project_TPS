//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UnityEditor;
using UEObject = UnityEngine.Object;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using EasyEditor.ReorderableList;

namespace EasyEditor
{
    /// <summary>
    /// Render a list in the inspector. The list UI allows to change element position, or remove them from the list in a very easy way.
    /// </summary>
    [RenderType(typeof(IList))]
    public class ReorderableListRenderer : FieldRenderer
    {
        public Action<int, SerializedProperty> OnItemInserted, OnItemBeingRemoved;

        private SerializedProperty list;
        private ReorderableListControl listControl;
        private EESerializedPropertyAdaptor listAdaptor;
       
        bool isReadOnly = false;

        public override void CreateAsset(string path)
        {
            Utils.CreateAssetFrom<ReorderableListRenderer>(this, "List_" + label, path);
        }

        public override void InitializeFromEntityInfo(EntityInfo entityInfo)
        {
            base.InitializeFromEntityInfo(entityInfo);

            isReadOnly = (AttributeHelper.GetAttribute<ReadOnlyAttribute>(entityInfo.fieldInfo) != null);

            list = FieldInfoHelper.GetSerializedPropertyFromPath(entityInfo.propertyPath, entityInfo.serializedObject);
            listControl = new ReorderableListControl();

            listControl.ItemInserted += OnItemInsertedHandler;
            listControl.ItemRemoving += OnItemRemovingHandler;

            if(isReadOnly)
            {
                listControl.Flags = ReorderableListFlags.DisableReordering 
                        | ReorderableListFlags.DisableContextMenu 
                        | ReorderableListFlags.HideAddButton
                        | ReorderableListFlags.HideRemoveButtons;
            }

			listAdaptor = new EESerializedPropertyAdaptor(serializedProperty, isReadOnly);
        }

        private void OnItemInsertedHandler(object sender, ItemInsertedEventArgs args) {
            if(OnItemInserted != null)
            {
                OnItemInserted(args.ItemIndex, list);
            }
        }
        
        private void OnItemRemovingHandler(object sender, ItemRemovingEventArgs args) {
            if(OnItemBeingRemoved != null)
            {
                OnItemBeingRemoved(args.ItemIndex, list);
            }
        }

        public override void Render(Action preRender = null)
        {
            base.Render(preRender);

            intermediarySerializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 10f * Settings.indentation);

            EditorGUILayout.BeginVertical();

            ReorderableListGUI.Title(label);

            listControl.Draw(listAdaptor);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            intermediarySerializedObject.ApplyModifiedProperties();
        }
    }
}