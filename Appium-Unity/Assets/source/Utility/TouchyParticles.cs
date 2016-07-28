//////////////////////////////////////////////////////////////////////////
/// @file	TouchyParticles.cs
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
/// @brief	TouchyParticles class.
//////////////////////////////////////////////////////////////////////////
public class TouchyParticles : MonoBehaviour
{
    /****************************** CONSTANTS *******************************/

    /***************************** SUB-CLASSES ******************************/

    /***************************** GLOBAL DATA ******************************/

    /**************************** GLOBAL METHODS ****************************/

    /***************************** PUBLIC DATA ******************************/

    /***************************** PRIVATE DATA *****************************/
    [SerializeField]
    protected GameObject m_particle;
		
	/***************************** PROPERTIES *******************************/

	/***************************** PUBLIC METHODS ***************************/

	/**************************** PRIVATE METHODS ***************************/

	//////////////////////////////////////////////////////////////////////////
	/// @brief Initialise class after construction.
	//////////////////////////////////////////////////////////////////////////
	private void Awake()
	{
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief	Everything is awake, script is about to start running.
	//////////////////////////////////////////////////////////////////////////
	private void Start()
	{
	}

	//////////////////////////////////////////////////////////////////////////
	/// @brief 	Update one time step.
	//////////////////////////////////////////////////////////////////////////
	private void Update()
	{
        for (int i = 0; i < Input.touchCount; ++i)
        {
            //if (Input.GetTouch(i).phase == TouchPhase.Moved)
            {                
                Instantiate(m_particle, Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position), transform.rotation);                
            }
        }
    }
}

