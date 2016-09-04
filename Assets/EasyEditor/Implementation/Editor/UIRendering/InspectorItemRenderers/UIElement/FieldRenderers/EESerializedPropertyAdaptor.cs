//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UnityEditor;
using UEObject = UnityEngine.Object;
using System.Collections;
using EasyEditor.ReorderableList;

namespace EasyEditor
{
    public class EESerializedPropertyAdaptor : SerializedPropertyAdaptor, IReorderableListDropTarget
    {
        private bool isReadOnly;

        public EESerializedPropertyAdaptor(SerializedProperty arrayProperty, bool isReadOnly) : base(arrayProperty)
        {
            this.isReadOnly = isReadOnly;
        }

        private Rect DropTargetPosition 
        {
            get 
            {
                // Expand size of drop target slightly so that it is easier to drop.
                Rect dropPosition = ReorderableListGUI.CurrentListPosition;
                dropPosition.y -= 10;
                dropPosition.height += 15;
                return dropPosition;
            }
        }

        public bool CanDropInsert(int insertionIndex)
        {
            bool canDrop = true;

            canDrop &= !isReadOnly;

            if(DropTargetPosition.Contains(Event.current.mousePosition))
            {
                canDrop &= true;
            }
            else
            {
                canDrop = false;
            }

            return canDrop;
        }

        public void ProcessDropInsertion(int insertionIndex)
        {
            if (Event.current.type == EventType.DragPerform) 
            {
                for(int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    _arrayProperty.InsertArrayElementAtIndex(insertionIndex + i);
                    _arrayProperty.GetArrayElementAtIndex(insertionIndex + i).objectReferenceValue = DragAndDrop.objectReferences[i];
                }
            }
        }
    }
}