//////////////////////////////////////////////////////////////////////////
/// @file	GameObjectActivator.cs
///
/// @author
///
/// @brief	Unity C# script template. To install copy this file into /Applications/<Unity Folder>/Unity.app>/Contents/Resources/ScriptTemplates.
///
/// @note 	Copyright 2015 Hutch Games Ltd. All rights reserved.
//////////////////////////////////////////////////////////////////////////

/************************ EXTERNAL NAMESPACES ***************************/

using UnityEngine;																// Unity 			(ref http://docs.unity3d.com/Documentation/ScriptReference/index.html)
using System;																	// String / Math 	(ref http://msdn.microsoft.com/en-us/library/system.aspx)
using System.Collections;														// Queue 			(ref http://msdn.microsoft.com/en-us/library/system.collections.aspx)
using System.Collections.Generic;												// List<> 			(ref http://msdn.microsoft.com/en-us/library/system.collections.generic.aspx)

/************************ REQUIRED COMPONENTS ***************************/

/************************** THE SCRIPT CLASS ****************************/

//////////////////////////////////////////////////////////////////////////
/// @brief	GameObjectActivator class.
//////////////////////////////////////////////////////////////////////////
public class GameObjectActivator : MonoBehaviour
{
	/****************************** CONSTANTS *******************************/
	
	/***************************** SUB-CLASSES ******************************/
	
	/***************************** GLOBAL DATA ******************************/

	/**************************** GLOBAL METHODS ****************************/
		
	/***************************** PUBLIC DATA ******************************/
    public GameObject mObjectToActivate;

    /***************************** PRIVATE DATA *****************************/

    /***************************** PROPERTIES *******************************/

    /***************************** PUBLIC METHODS ***************************/
    
    //////////////////////////////////////////////////////////////////////////
	/// @brief Activates the game object.  Doesn't check to see whether or
    /// not its already active.
	//////////////////////////////////////////////////////////////////////////
    public void ActivateObject()
    {
        if(this.mObjectToActivate != null)
        {
            this.mObjectToActivate.SetActive(true);
        }
    }

	/**************************** PRIVATE METHODS ***************************/

	
}

