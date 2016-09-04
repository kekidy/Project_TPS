// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace EasyEditor
{
    /// <summary>
    /// Entity info is a wrapper for fieldInfo and methodInfo.
    /// </summary>
    public class EntityInfo  {

    	public readonly FieldInfo fieldInfo;
        public readonly string propertyPath;
        public readonly SerializedObject serializedObject;

    	public readonly MethodInfo methodInfo;
    	public readonly object caller;

    	public readonly bool isField;
    	public readonly bool isMethod;

        public EntityInfo(FieldInfo fieldInfo, SerializedObject serializedObject, string propertyPath = "")
    	{
            isField = true;
            isMethod = false;
            this.fieldInfo = fieldInfo;
            this.propertyPath = propertyPath;
            this.serializedObject = serializedObject;
    	}

        public EntityInfo(MethodInfo methodInfo, object caller, SerializedObject serializedObject)
        {
            isField = false;
            isMethod = true;
            this.methodInfo = methodInfo;
            this.caller = caller;
            this.serializedObject = serializedObject;
        }

    	override public string ToString()
    	{
    		if (isField) 
    		{
    			return fieldInfo.Name;
    		} 
    		else 
    		{
    			return methodInfo.Name;
    		}
    	}
    }
}
