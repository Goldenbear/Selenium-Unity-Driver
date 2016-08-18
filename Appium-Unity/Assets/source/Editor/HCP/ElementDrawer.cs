//////////////////////////////////////////////////////////////////////////
/// @file	ElementDrawer.cs
///
/// @author Colin Nickerson
///
/// @brief	Custom rendering of the UniqueId component
///
/// @note 	Copyright 2016 Hutch Games Ltd. All rights reserved.
//////////////////////////////////////////////////////////////////////////

/************************ EXTERNAL NAMESPACES ***************************/

using UnityEditor;																// Unity 			(ref )
using UnityEngine;																// Unity 			(ref http://docs.unity3d.com/Documentation/ScriptReference/index.html)
using System;																	// String / Math 	(ref http://msdn.microsoft.com/en-us/library/system.aspx)

/************************ REQUIRED COMPONENTS ***************************/

/************************** THE SCRIPT CLASS ****************************/

//////////////////////////////////////////////////////////////////////////
/// @brief	UniqueIdDrawer class.  For custom rendering of
/// UniqueId component.  
//////////////////////////////////////////////////////////////////////////
[CustomPropertyDrawer (typeof(HCP.ElementAttribute))]
public class ElementDrawer : PropertyDrawer
{
	/***************************** PUBLIC METHODS ***************************/
    public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
    {
        // Place a label so it can't be edited by accident
        Rect textFieldPosition = position;
        textFieldPosition.height = 16;
        DrawLabelField (textFieldPosition, prop, label);
    }

	/**************************** PRIVATE METHODS ***************************/
    void DrawLabelField (Rect position, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.LabelField(position, label, new GUIContent (prop.stringValue));
    }
}

